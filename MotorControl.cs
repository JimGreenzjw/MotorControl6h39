using System;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;
using System.Linq;
using DOTHI_HIENCLUBVN;
using ZedGraph;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
namespace MotorControl6h39
{
    public partial class MotorControl : Form
    {
        #region COMPort and Graph objects
        ControlSignals control = new ControlSignals();
        GraphPane myPane; // 移除初始化，在Load事件中正确初始化
        //Create a new Serial Port object
        SerialPort P = new System.IO.Ports.SerialPort();
        //Declare a buffer for storing data
        private string InputData = String.Empty;
        PointPairList listPointsOne = new PointPairList();
        LineItem myCurveOne;
        //declare an event inside a class
        delegate void SetTextCallback(string text);

        // 添加数据缓存队列和锁对象
        private Queue<byte> dataBuffer = new Queue<byte>();
        private readonly object bufferLock = new object();
        private bool isProcessing = false;
        private Thread processThread;
        private bool shouldStop = false;
        
        // 添加数据包处理相关变量
        private const int PACKET_SIZE = 20; // 4个float + 4字节尾标识
        private const byte TAIL_BYTE1 = 0x00;
        private const byte TAIL_BYTE2 = 0x00;
        private const byte TAIL_BYTE3 = 0x80;
        private const byte TAIL_BYTE4 = 0x7f;
        
        // 添加统计信息
        private int totalPacketsReceived = 0;
        private int validPacketsProcessed = 0;
        private int invalidPacketsDropped = 0;
        private int bufferOverflowCount = 0;
        private const int MAX_BUFFER_SIZE = 10000; // 最大缓冲区大小
        
        // 添加UI更新节流机制
        private System.Windows.Forms.Timer uiUpdateTimer;
        private const int UI_UPDATE_INTERVAL = 100; // UI更新间隔(毫秒)
        private Queue<Action> uiUpdateQueue = new Queue<Action>();
        private readonly object uiQueueLock = new object();
        private bool isUiUpdateScheduled = false;
        
        // 可配置的UI更新频率
        private int uiUpdateFrequency = 10; // 每10个数据包更新一次UI
        private int chartUpdateFrequency = 5; // 每5个数据包更新一次图表
        private int statusUpdateFrequency = 50; // 每50个数据包显示一次状态
        #endregion
        // 在类级别添加更多的数据点列表和曲线
        private PointPairList listPointsGoal = new PointPairList();    // 目标位置
        private PointPairList listPointsMotor = new PointPairList();   // 电机位置
        private PointPairList listPointsPid = new PointPairList();     // PID输出
        private PointPairList listPointsError = new PointPairList();   // 位置误差
        private LineItem myCurveGoal;
        private LineItem myCurveMotor;
        private LineItem myCurvePid;
        private LineItem myCurveError;
        private bool legendClickRegistered = false;

        // 添加二维数组来存储目标位置和电机位置数据
        private List<float[]> positionData = new List<float[]>();  // 使用List来动态存储数据
        private readonly object dataLock = new object();  // 用于线程安全的数据访问
        private bool isRecordingData = false;  // 控制数据记录开始的标志

        // 添加动画效果相关变量
        private System.Windows.Forms.Timer animationTimer;  // 用于button11动画效果
        private bool isButtonBlinking = false;  // 控制按钮闪烁状态

        // 添加UDP接收器相关变量
        private UdpClient udpClient;
        private Thread udpReceiveThread;
        private bool isUdpReceiving = false;
        private int udpPort = 8080;
        private int udpPacketCount = 0;
        private DateTime udpStartTime;

        // 添加清空数据的方法
        private void ClearPositionData()
        {
            lock (dataLock)
            {
                positionData.Clear();
            }
            Console.WriteLine("位置数据已清空");
        }

        // 获取当前数据数量的方法
        private int GetPositionDataCount()
        {
            lock (dataLock)
            {
                return positionData.Count;
            }
        }

        // 获取位置数据为二维数组的方法
        public float[,] GetPositionDataArray()
        {
            lock (dataLock)
            {
                if (positionData.Count == 0)
                    return new float[0, 4];

                float[,] array = new float[positionData.Count, 4];
                for (int i = 0; i < positionData.Count; i++)
                {
                    array[i, 0] = positionData[i][0]; // 目标位置
                    array[i, 1] = positionData[i][1]; // 电机位置
                    array[i, 2] = positionData[i][2]; // 当前时间
                    array[i, 3] = positionData[i][3]; // PID输出
                }
                return array;
            }
        }

        // FIFO数据处理核心方法
        private void StartDataProcessing()
        {
            if (processThread != null && processThread.IsAlive)
                return;

            shouldStop = false;
            isProcessing = true;
            processThread = new Thread(DataProcessingLoop);
            processThread.IsBackground = true;
            processThread.Priority = ThreadPriority.AboveNormal; // 提高处理优先级
            processThread.Start();
            Console.WriteLine("FIFO数据处理线程已启动");
        }

        private void StopDataProcessing()
        {
            shouldStop = true;
            isProcessing = false;

            if (processThread != null && processThread.IsAlive)
            {
                processThread.Join(1000); // 等待1秒
                if (processThread.IsAlive)
                {
                    processThread.Abort(); // 强制终止
                }
            }
            Console.WriteLine("FIFO数据处理线程已停止");
        }

        private void DataProcessingLoop()
        {
            while (!shouldStop)
            {
                try
                {
                    byte[] packet = GetNextPacket();
                    if (packet != null)
                    {
                        ProcessPacket(packet);
                    }
                    else
                    {
                        // 没有完整数据包，短暂休眠
                        Thread.Sleep(1);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"数据处理循环错误: {ex.Message}");
                    Thread.Sleep(10); // 出错时稍长休眠
                }
            }
        }

        private byte[] GetNextPacket()
        {
            lock (bufferLock)
            {
                // 检查是否有足够的数据构成一个完整的数据包
                if (dataBuffer.Count < PACKET_SIZE)
                    return null;

                // 查找数据包边界（尾标识）
                byte[] bufferArray = dataBuffer.ToArray();
                int packetStart = -1;

                // 从缓冲区开始查找尾标识
                for (int i = 0; i <= bufferArray.Length - PACKET_SIZE; i++)
                {
                    if (IsValidTail(bufferArray, i + PACKET_SIZE - 4))
                    {
                        packetStart = i;
                        break;
                    }
                }

                if (packetStart == -1)
                {
                    // 没有找到完整的数据包，保留最后3个字节（可能是不完整的尾标识）
                    int bytesToKeep = Math.Min(3, bufferArray.Length);
                    dataBuffer.Clear();
                    for (int i = bufferArray.Length - bytesToKeep; i < bufferArray.Length; i++)
                    {
                        dataBuffer.Enqueue(bufferArray[i]);
                    }
                    return null;
                }

                // 提取完整的数据包
                byte[] packet = new byte[PACKET_SIZE];
                for (int i = 0; i < PACKET_SIZE; i++)
                {
                    packet[i] = bufferArray[packetStart + i];
                }

                // 移除已处理的数据（包括数据包之前的所有数据）
                for (int i = 0; i < packetStart + PACKET_SIZE; i++)
                {
                    dataBuffer.Dequeue();
                }

                return packet;
            }
        }

        private bool IsValidTail(byte[] buffer, int startIndex)
        {
            if (startIndex + 3 >= buffer.Length)
                return false;

            return buffer[startIndex] == TAIL_BYTE1 &&
                   buffer[startIndex + 1] == TAIL_BYTE2 &&
                   buffer[startIndex + 2] == TAIL_BYTE3 &&
                   buffer[startIndex + 3] == TAIL_BYTE4;
        }

        private void ProcessPacket(byte[] packet)
        {
            try
            {
                totalPacketsReceived++;

                // 验证数据包完整性
                if (packet.Length != PACKET_SIZE)
                {
                    invalidPacketsDropped++;
                    Console.WriteLine($"数据包大小错误: {packet.Length} != {PACKET_SIZE}");
                    return;
                }

                // 验证尾标识
                if (!IsValidTail(packet, PACKET_SIZE - 4))
                {
                    invalidPacketsDropped++;
                    Console.WriteLine($"数据包尾标识无效: {packet[PACKET_SIZE - 4]:X2} {packet[PACKET_SIZE - 3]:X2} {packet[PACKET_SIZE - 2]:X2} {packet[PACKET_SIZE - 1]:X2}");
                    return;
                }

                // 解析数据
                float goalPos = BitConverter.ToSingle(packet, 0);
                float motorPos = BitConverter.ToSingle(packet, 4);
                float pidOutput = BitConverter.ToSingle(packet, 8);
                float positionError = BitConverter.ToSingle(packet, 12);

                validPacketsProcessed++;

                // 在UI线程中更新显示和图表
                if (this != null && !this.IsDisposed && this.IsHandleCreated)
                {
                    try
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            UpdateUIWithPacket(goalPos, motorPos, pidOutput, positionError);
                        }));
                    }
                    catch (ObjectDisposedException)
                    {
                        // 窗体已关闭，忽略异常
                    }
                }
            }
            catch (Exception ex)
            {
                invalidPacketsDropped++;
                Console.WriteLine($"处理数据包时出错: {ex.Message}");
            }
        }

        private void UpdateUIWithPacket(float goalPos, float motorPos, float pidOutput, float positionError)
        {
            try
            {
                // 将UI更新操作加入队列，使用节流机制
                lock (uiQueueLock)
                {
                    uiUpdateQueue.Enqueue(() =>
                    {
                        try
                        {
                            // 更新richTextBox1显示（减少显示频率）
                            if (richTextBox1 != null && !richTextBox1.IsDisposed)
                            {
                                // 只在每uiUpdateFrequency个数据包显示一次详细信息
                                if (validPacketsProcessed % uiUpdateFrequency == 0)
                                {
                                    richTextBox1.AppendText($"接收时间: {DateTime.Now:HH:mm:ss.fff}\n");
                                    richTextBox1.AppendText($"目标位置: {goalPos:F3}\n");
                                    richTextBox1.AppendText($"电机位置: {motorPos:F3}\n");
                                    richTextBox1.AppendText($"PID输出: {pidOutput:F3}\n");
                                    richTextBox1.AppendText($"位置误差: {positionError:F3}\n");
                                    richTextBox1.AppendText($"统计: 总包数={totalPacketsReceived}, 有效={validPacketsProcessed}, 丢弃={invalidPacketsDropped}, 溢出={bufferOverflowCount}\n");
                                    richTextBox1.AppendText("------------------------\n");
                                    richTextBox1.ScrollToCaret();
                                }
                                else if (validPacketsProcessed % statusUpdateFrequency == 0)
                                {
                                    // 每statusUpdateFrequency个数据包显示一次简单状态
                                    richTextBox1.AppendText($"数据接收中... 有效包数: {validPacketsProcessed}\n");
                                    richTextBox1.ScrollToCaret();
                                }
                            }

                            // 更新图表（每个数据包都更新）
                            draw(goalPos, motorPos, pidOutput, positionError);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"UI更新错误: {ex.Message}");
                        }
                    });

                    // 如果还没有安排UI更新，则安排一个
                    if (!isUiUpdateScheduled)
                    {
                        ScheduleUiUpdate();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UI更新错误: {ex.Message}");
            }
        }

        private void ScheduleUiUpdate()
        {
            if (this != null && !this.IsDisposed && this.IsHandleCreated)
            {
                try
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        lock (uiQueueLock)
                        {
                            isUiUpdateScheduled = false;
                        }
                        ProcessUiUpdateQueue();
                    }));
                }
                catch (ObjectDisposedException)
                {
                    // 窗体已关闭，忽略异常
                }
            }
        }

        private void ProcessUiUpdateQueue()
        {
            try
            {
                lock (uiQueueLock)
                {
                    // 处理队列中的所有UI更新操作
                    while (uiUpdateQueue.Count > 0)
                    {
                        Action updateAction = uiUpdateQueue.Dequeue();
                        try
                        {
                            updateAction();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"处理UI更新操作时出错: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"处理UI更新队列时出错: {ex.Message}");
            }
        }

        private void UiUpdateTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                // 定期处理UI更新队列
                ProcessUiUpdateQueue();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UI更新定时器错误: {ex.Message}");
            }
        }

        // 获取统计信息的方法
        public string GetStatistics()
        {
            return $"总接收包数: {totalPacketsReceived}, 有效包数: {validPacketsProcessed}, 丢弃包数: {invalidPacketsDropped}, 缓冲区溢出: {bufferOverflowCount}";
        }

        // 重置统计信息的方法
        public void ResetStatistics()
        {
            totalPacketsReceived = 0;
            validPacketsProcessed = 0;
            invalidPacketsDropped = 0;
            bufferOverflowCount = 0;
        }

        // 配置UI更新频率的方法
        public void SetUiUpdateFrequency(int uiFreq, int chartFreq, int statusFreq)
        {
            uiUpdateFrequency = Math.Max(1, uiFreq);
            chartUpdateFrequency = Math.Max(1, chartFreq);
            statusUpdateFrequency = Math.Max(1, statusFreq);
            
            Console.WriteLine($"UI更新频率已调整: UI={uiUpdateFrequency}, 图表={chartUpdateFrequency}, 状态={statusUpdateFrequency}");
        }

        // 获取当前UI更新频率的方法
        public string GetUiUpdateFrequency()
        {
            return $"UI更新频率: UI={uiUpdateFrequency}, 图表={chartUpdateFrequency}, 状态={statusUpdateFrequency}";
        }

        // 确保Legend设置一致性的方法
        private void EnsureLegendConsistency()
        {
            if (myPane?.Legend == null) return;

            // 设置Legend基本属性
            myPane.Legend.IsVisible = true;
            myPane.Legend.IsHStack = false; // 保持垂直排列
            myPane.Legend.IsReverse = false; // 保持正常顺序

            // 确保所有Legend项都可见
            foreach (CurveItem curve in myPane.CurveList)
            {
                if (curve.Label != null)
                {
                    curve.Label.IsVisible = true;
                }
            }
        }

        // 添加曲线的辅助方法
        private LineItem AddCurveWithSettings(string label, PointPairList points, Color color, SymbolType symbolType)
        {
            LineItem curve = myPane.AddCurve(label, points, color, symbolType);
            if (curve != null)
            {
                curve.Line.Width = 2;
                curve.Symbol.Size = 4;
                curve.IsVisible = true;
                if (curve.Label != null)
                {
                    curve.Label.IsVisible = true;
                }
            }
            return curve;
        }
        private void MotorControl_Load(object sender, EventArgs e)
        {
            try
            {
                // 检查控件是否存在
                if (cbCom != null && cbCom.Items.Count > 0)
                {
                    cbCom.SelectedIndex = 0;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"ComboBox初始化错误: {exception.Message}");
            }

            try
            {
                if (cbRate != null && cbRate.Items.Count > 3)
                {
                    cbRate.SelectedIndex = 3; // 9600
                }
                if (cbBits != null && cbBits.Items.Count > 2)
                {
                    cbBits.SelectedIndex = 2; // 8
                }
                if (cbParity != null && cbParity.Items.Count > 0)
                {
                    cbParity.SelectedIndex = 0; // None
                }
                if (cbBit != null && cbBit.Items.Count > 0)
                {
                    cbBit.SelectedIndex = 0; // None
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"控件初始化错误: {ex.Message}");
            }

            if (btdisc != null)
            {
                btdisc.Enabled = false;
            }

            if (P != null)
            {
                P.Close();
            }

            // 初始化图表
            try
            {
                Console.WriteLine("开始初始化图表...");

                if (zedGraphControl1 != null)
                {
                    Console.WriteLine("zedGraphControl1 存在");
                    myPane = zedGraphControl1.GraphPane;
                    if (myPane != null)
                    {
                        Console.WriteLine("myPane 初始化成功");
                        myPane.Title.Text = "电机控制";
                        myPane.XAxis.Title.Text = "时间(s)";
                        myPane.YAxis.Title.Text = "位置(mm)";

                        // 设置坐标轴范围
                        myPane.XAxis.Scale.Min = 0;
                        myPane.XAxis.Scale.Max = 10;  // 显示最近10秒的数据
                        myPane.YAxis.Scale.Min = -100;
                        myPane.YAxis.Scale.Max = 100;  // 根据实际数据范围调整

                        // 初始化数据点列表和曲线
                        listPointsGoal = new PointPairList();
                        listPointsMotor = new PointPairList();
                        listPointsPid = new PointPairList();
                        listPointsError = new PointPairList();

                        Console.WriteLine("数据点列表初始化完成");

                        // 添加四条曲线，使用不同颜色
                        myCurveGoal = AddCurveWithSettings("目标位置", listPointsGoal, Color.Blue, SymbolType.Circle);
                        if (myCurveGoal != null)
                        {
                            Console.WriteLine("目标曲线添加成功");
                        }
                        else
                        {
                            Console.WriteLine("目标曲线添加失败！");
                        }

                        myCurveMotor = AddCurveWithSettings("电机位置", listPointsMotor, Color.Red, SymbolType.Square);
                        if (myCurveMotor != null)
                        {
                            Console.WriteLine("电机曲线添加成功");
                        }
                        else
                        {
                            Console.WriteLine("电机曲线添加失败！");
                        }

                        myCurvePid = AddCurveWithSettings("PID输出", listPointsPid, Color.Orange, SymbolType.Diamond);
                        if (myCurvePid != null)
                        {
                            Console.WriteLine("PID曲线添加成功");
                        }
                        else
                        {
                            Console.WriteLine("PID曲线添加失败！");
                        }

                        myCurveError = AddCurveWithSettings("位置误差", listPointsError, Color.Green, SymbolType.Triangle);
                        if (myCurveError != null)
                        {
                            Console.WriteLine("误差曲线添加成功");
                        }
                        else
                        {
                            Console.WriteLine("误差曲线添加失败！");
                        }

                        // 确保图例可见
                        myPane.Legend.IsVisible = true;
                        myPane.Legend.Position = LegendPos.Top; // 设置图例位置在顶部
                        myPane.Legend.FontSpec.Size = 10; // 设置图例字体大小
                        myPane.Legend.FontSpec.IsBold = false; // 设置图例字体为普通
                        myPane.Legend.Border.IsVisible = true; // 显示图例边框
                        myPane.Legend.Border.Color = Color.Black; // 设置边框颜色
                        myPane.Legend.Fill.IsVisible = true; // 显示图例背景
                        myPane.Legend.Fill.Color = Color.White; // 设置背景颜色
                        myPane.Legend.IsHStack = false; // 确保Legend项垂直排列
                        myPane.Legend.IsReverse = false; // 正常顺序显示
                        Console.WriteLine("图例设置为可见");

                        // 确保Legend设置一致性
                        EnsureLegendConsistency();

                        // 初始化时间戳
                        tickStart = Environment.TickCount;

                        // 强制刷新图表
                        Console.WriteLine("开始初始图表刷新...");
                        zedGraphControl1.AxisChange();
                        zedGraphControl1.Invalidate();
                        Console.WriteLine("初始图表刷新完成");

                        // 添加图表点击事件处理
                        zedGraphControl1.MouseClick += new MouseEventHandler(zedGraphControl1_MouseClick);
                        Console.WriteLine("图表点击事件已添加");
                    }
                    else
                    {
                        Console.WriteLine("myPane 初始化失败！");
                    }
                }
                else
                {
                    Console.WriteLine("zedGraphControl1 为空！");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"图表初始化错误: {ex.Message}");
                Console.WriteLine($"错误堆栈: {ex.StackTrace}");
            }

            if (timer1 != null)
            {
                timer1.Interval = 10;
            }

            // 确保在窗体加载时调整图表大小
            try
            {
                this.BeginInvoke(new Action(() =>
                {
                    MotorControl_Resize(this, EventArgs.Empty);
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"窗体大小调整错误: {ex.Message}");
            }

            // 初始化伯德图控件
            try
            {
                if (zedGraphControlBode != null)
                {
                    Console.WriteLine("初始化伯德图控件...");
                    GraphPane bodePane = zedGraphControlBode.GraphPane;
                    if (bodePane != null)
                    {
                        bodePane.Title.Text = "伯德图 - 幅频特性和相频特性";
                        bodePane.XAxis.Title.Text = "频率 (Hz)";
                        bodePane.YAxis.Title.Text = "幅值 (dB)";
                        bodePane.Y2Axis.Title.Text = "相位 (度)";
                        bodePane.Y2Axis.IsVisible = true;

                        // 设置坐标轴范围
                        bodePane.XAxis.Type = AxisType.Log;
                        bodePane.XAxis.Scale.Min = 0.1;
                        bodePane.XAxis.Scale.Max = 10.0;
                        bodePane.YAxis.Scale.Min = -60;
                        bodePane.YAxis.Scale.Max = 20;
                        bodePane.Y2Axis.Scale.Min = -180;
                        bodePane.Y2Axis.Scale.Max = 180;

                        // 添加网格
                        bodePane.XAxis.MajorGrid.IsVisible = true;
                        bodePane.YAxis.MajorGrid.IsVisible = true;
                        bodePane.Y2Axis.MajorGrid.IsVisible = true;

                        // 设置图例
                        bodePane.Legend.IsVisible = true;
                        bodePane.Legend.Position = LegendPos.Top;

                        Console.WriteLine("伯德图控件初始化完成");
                    }
                    else
                    {
                        Console.WriteLine("伯德图面板初始化失败！");
                    }
                }
                else
                {
                    Console.WriteLine("zedGraphControlBode 为空！");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"伯德图控件初始化错误: {ex.Message}");
                Console.WriteLine($"错误堆栈: {ex.StackTrace}");
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {

        }
        public MotorControl()
        {
            InitializeComponent();
            this.Resize += new System.EventHandler(this.MotorControl_Resize);

            //ports variable to store all the available COM ports in Device List
            string[] ports = SerialPort.GetPortNames();
            cbCom.Items.AddRange(ports);
            cbCom.SelectedIndexChanged += new EventHandler(cbCom_SelectedIndexChanged);
            P.ReadTimeout = 1000;
            P.DataReceived += new SerialDataReceivedEventHandler(DataReceive);

            // Cài đặt cho BaudRate
            string[] BaudRate =
            {
                    "1200", "2400", "4800", "9600", "19200",
                    "38400", "57600", "115200"
                };
            cbRate.Items.AddRange(BaudRate);
            // Cài đặt cho DataBits
            string[] Databits = { "6", "7", "8" };
            cbBits.Items.AddRange(Databits);
            //Cho Parity
            string[] Parity = { "None", "Odd", "Even" };
            cbParity.Items.AddRange(Parity);
            //Cho Stop bit
            string[] stopbit = { "1", "1.5", "2" };
            cbBit.Items.AddRange(stopbit);

            // Set minimum form size to prevent controls from overlapping
            this.MinimumSize = new System.Drawing.Size(1024, 768);

            // 注册checkBox2的CheckedChanged事件
            checkBox2.CheckedChanged += new EventHandler(checkBox2_CheckedChanged);
        }
        private void btrefesh_Click(object sender, EventArgs e)
        {
            cbCom.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            cbCom.Items.AddRange(ports);
        }
        private void DataReceive(object obj, SerialDataReceivedEventArgs e)
        {
            // 检查串口是否还开着，如果关了就直接返回
            if (P == null || !P.IsOpen)
            {
                return;
            }

            try
            {
                // 检查是否有数据可读
                if (P.BytesToRead <= 0)
                {
                    return;
                }

                // 读取所有可用数据
                byte[] buffer = new byte[P.BytesToRead];
                int bytesRead = P.Read(buffer, 0, buffer.Length);

                // 检查是否收到SWEEP_DONE信号
                string receivedText = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                if (receivedText.Contains("SWEEP_DONE"))
                {
                    isRecordingData = false;
                    AutoSaveSweepData();
                    if (richTextBox1 != null)
                    {
                        richTextBox1.AppendText("收到下位机扫频完成信号，已保存数据。\n");
                        richTextBox1.ScrollToCaret();
                    }
                    return; // 不再处理后续数据
                }

                // 将数据添加到FIFO队列
                lock (bufferLock)
                {
                    // 检查缓冲区大小，防止内存溢出
                    if (dataBuffer.Count + bytesRead > MAX_BUFFER_SIZE)
                    {
                        bufferOverflowCount++;
                        Console.WriteLine($"缓冲区溢出，丢弃 {bytesRead} 字节数据");
                        
                        // 丢弃最旧的数据，为新数据腾出空间
                        int bytesToRemove = dataBuffer.Count + bytesRead - MAX_BUFFER_SIZE + 1000;
                        for (int i = 0; i < bytesToRemove && dataBuffer.Count > 0; i++)
                        {
                            dataBuffer.Dequeue();
                        }
                    }

                    // 将新数据添加到队列
                    for (int i = 0; i < bytesRead; i++)
                    {
                        dataBuffer.Enqueue(buffer[i]);
                    }
                }

                // 如果数据处理线程未启动，则启动它
                if (!isProcessing)
                {
                    StartDataProcessing();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DataReceive: {ex.Message}");

                // 在 richTextBox1 中显示错误信息
                if (this != null && !this.IsDisposed)
                {
                    try
                    {
                        this.Invoke(new Action(() =>
                        {
                            if (richTextBox1 != null)
                            {
                                richTextBox1.AppendText($"接收错误: {ex.Message}\n");
                                richTextBox1.ScrollToCaret();
                            }
                        }));
                    }
                    catch (Exception invokeEx)
                    {
                        Console.WriteLine($"错误信息显示失败: {invokeEx.Message}");
                    }
                }
            }
        }

        private void draw(float goalPos, float motorPos, float pidOutput, float positionError)
        {
            try
            {
                // 确保在UI线程中执行
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => draw(goalPos, motorPos, pidOutput, positionError)));
                    return;
                }

                // 首次调用时注册Legend点击事件
                if (!legendClickRegistered)
                {
                    zedGraphControl1.MouseClick -= zedGraphControl1_MouseClick; // 防止重复注册
                    zedGraphControl1.MouseClick += zedGraphControl1_MouseClick;
                    legendClickRegistered = true;
                }

                // 检查必要的对象是否存在，如果myPane为null则重新初始化
                if (myPane == null)
                {
                    Console.WriteLine("myPane为空，尝试重新初始化...");
                    if (zedGraphControl1 != null)
                    {
                        myPane = zedGraphControl1.GraphPane;
                        if (myPane != null)
                        {
                            myPane.Title.Text = "电机控制";
                            myPane.XAxis.Title.Text = "时间(s)";
                            myPane.YAxis.Title.Text = "位置(mm)";

                            // 设置坐标轴范围
                            myPane.XAxis.Scale.Min = 0;
                            myPane.XAxis.Scale.Max = 10;
                            myPane.YAxis.Scale.Min = -100;
                            myPane.YAxis.Scale.Max = 100;

                            // 确保曲线已添加
                            if (myCurveGoal == null && listPointsGoal != null)
                            {
                                myCurveGoal = AddCurveWithSettings("目标位置", listPointsGoal, Color.Blue, SymbolType.Circle);
                            }

                            if (myCurveMotor == null && listPointsMotor != null)
                            {
                                myCurveMotor = AddCurveWithSettings("电机位置", listPointsMotor, Color.Red, SymbolType.Square);
                            }

                            if (myCurvePid == null && listPointsPid != null)
                            {
                                myCurvePid = AddCurveWithSettings("PID输出", listPointsPid, Color.Orange, SymbolType.Diamond);
                            }

                            if (myCurveError == null && listPointsError != null)
                            {
                                myCurveError = AddCurveWithSettings("位置误差", listPointsError, Color.Green, SymbolType.Triangle);
                            }

                            Console.WriteLine("图表重新初始化完成");

                            // 确保Legend设置一致性
                            EnsureLegendConsistency();
                        }
                    }
                }

                // 再次检查必要的对象是否存在
                if (myPane == null || listPointsGoal == null || listPointsMotor == null || listPointsPid == null || listPointsError == null)
                {
                    Console.WriteLine("图表对象未初始化");
                    Console.WriteLine($"myPane: {myPane != null}, listPointsGoal: {listPointsGoal != null}, listPointsMotor: {listPointsMotor != null}, listPointsPid: {listPointsPid != null}, listPointsError: {listPointsError != null}");
                    return;
                }

                // 计算时间（毫秒转换为秒）
                double time = (Environment.TickCount - tickStart) / 1000.0;

                // 添加新的数据点到四条曲线
                listPointsGoal.Add(time, goalPos);
                listPointsMotor.Add(time, motorPos);
                listPointsPid.Add(time, pidOutput);
                listPointsError.Add(time, positionError);

                // 只有在开始记录后才将数据存储到二维数组中（线程安全）
                if (isRecordingData)
                {
                    lock (dataLock)
                    {
                        // 计算当前时间（秒）
                        double currentTime = (Environment.TickCount - tickStart) / 1000.0;
                        positionData.Add(new float[] { goalPos, motorPos, (float)currentTime, pidOutput });
                    }
                }

                // 如果数据点超过显示范围，移除旧的数据点
                while (listPointsGoal.Count > 0 && listPointsGoal[0].X < time - 10)
                {
                    listPointsGoal.RemoveAt(0);
                }
                while (listPointsMotor.Count > 0 && listPointsMotor[0].X < time - 10)
                {
                    listPointsMotor.RemoveAt(0);
                }
                while (listPointsPid.Count > 0 && listPointsPid[0].X < time - 10)
                {
                    listPointsPid.RemoveAt(0);
                }
                while (listPointsError.Count > 0 && listPointsError[0].X < time - 10)
                {
                    listPointsError.RemoveAt(0);
                }

                // 更新X轴范围
                if (myPane.XAxis != null && myPane.XAxis.Scale != null)
                {
                    myPane.XAxis.Scale.Min = time - 10;
                    myPane.XAxis.Scale.Max = time;
                }

                // 更新Y轴范围（考虑所有数据）
                double minY = 0, maxY = 0;

                if (listPointsGoal.Count > 0 || listPointsMotor.Count > 0 || listPointsPid.Count > 0 || listPointsError.Count > 0)
                {
                    minY = Math.Min(
                        listPointsGoal.Count > 0 ? listPointsGoal.Min(p => p.Y) : double.MaxValue,
                        Math.Min(
                            listPointsMotor.Count > 0 ? listPointsMotor.Min(p => p.Y) : double.MaxValue,
                            Math.Min(
                                listPointsPid.Count > 0 ? listPointsPid.Min(p => p.Y) : double.MaxValue,
                                listPointsError.Count > 0 ? listPointsError.Min(p => p.Y) : double.MaxValue
                            )
                        )
                    );
                    maxY = Math.Max(
                        listPointsGoal.Count > 0 ? listPointsGoal.Max(p => p.Y) : double.MinValue,
                        Math.Max(
                            listPointsMotor.Count > 0 ? listPointsMotor.Max(p => p.Y) : double.MinValue,
                            Math.Max(
                                listPointsPid.Count > 0 ? listPointsPid.Max(p => p.Y) : double.MinValue,
                                listPointsError.Count > 0 ? listPointsError.Max(p => p.Y) : double.MinValue
                            )
                        )
                    );
                }
                else
                {
                    // 如果没有数据点，使用默认范围
                    minY = -10;
                    maxY = 10;
                }

                double range = maxY - minY;
                if (range < 1) range = 1; // 避免范围太小

                if (myPane.YAxis != null && myPane.YAxis.Scale != null)
                {
                    myPane.YAxis.Scale.Min = minY - range * 0.1;
                    myPane.YAxis.Scale.Max = maxY + range * 0.1;
                }

                // 节流图表刷新 - 只在每chartUpdateFrequency个数据包刷新一次
                if (validPacketsProcessed % chartUpdateFrequency == 0)
                {
                    // 刷新显示
                    if (zedGraphControl1 != null)
                    {
                        // 确保Legend设置保持不变
                        EnsureLegendConsistency();

                        zedGraphControl1.AxisChange();
                        zedGraphControl1.Invalidate();
                        zedGraphControl1.Refresh();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新图表时出错: {ex.Message}");
                Console.WriteLine($"错误堆栈: {ex.StackTrace}");
            }
        }

        private void cbCom_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (P.IsOpen)
            {
                P.Close();
                // 更新按钮状态
                btconnect.Enabled = true;
                btdisc.Enabled = false;
            }
            string selectedPort = cbCom.SelectedItem?.ToString();
            P.PortName = selectedPort;
        }

        private void cbRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (P.IsOpen)
            {
                P.Close();
                btconnect.Enabled = true;
                btdisc.Enabled = false;
            }
            P.BaudRate = Convert.ToInt32(cbRate.Text);
        }

        private void cbBits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (P.IsOpen)
            {
                P.Close();
                btconnect.Enabled = true;
                btdisc.Enabled = false;
            }
            P.DataBits = Convert.ToInt32(cbBits.Text);
        }

        private void cbParity_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (P.IsOpen)
            {
                P.Close();
                btconnect.Enabled = true;
                btdisc.Enabled = false;
            }
            switch (cbParity.SelectedItem.ToString())
            {
                case "Odd":
                    P.Parity = Parity.Odd;
                    break;
                case "None":
                    P.Parity = Parity.None;
                    break;
                case "Even":
                    P.Parity = Parity.Even;
                    break;
            }
        }

        private void cbBit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (P.IsOpen)
            {
                P.Close();
                btconnect.Enabled = true;
                btdisc.Enabled = false;
            }
            switch (cbBit.SelectedItem.ToString())
            {
                case "1":
                    P.StopBits = StopBits.One;
                    break;
                case "1.5":
                    P.StopBits = StopBits.OnePointFive;
                    break;
                case "2":
                    P.StopBits = StopBits.Two;
                    break;
            }
        }
        private void btconnect_Click(object sender, EventArgs e)
        {
            if (P == null || !P.IsOpen)
            {
                try
                {
                    // 确保串口配置正确
                    if (string.IsNullOrEmpty(P?.PortName))
                    {
                        MessageBox.Show("请先选择串口");
                        return;
                    }

                    // 设置串口参数
                    if (cbRate != null && !string.IsNullOrEmpty(cbRate.Text))
                    {
                        P.BaudRate = Convert.ToInt32(cbRate.Text);
                    }
                    if (cbBits != null && !string.IsNullOrEmpty(cbBits.Text))
                    {
                        P.DataBits = Convert.ToInt32(cbBits.Text);
                    }

                    if (cbParity != null && cbParity.SelectedItem != null)
                    {
                        switch (cbParity.SelectedItem.ToString())
                        {
                            case "Odd":
                                P.Parity = Parity.Odd;
                                break;
                            case "None":
                                P.Parity = Parity.None;
                                break;
                            case "Even":
                                P.Parity = Parity.Even;
                                break;
                        }
                    }

                    if (cbBit != null && cbBit.SelectedItem != null)
                    {
                        switch (cbBit.SelectedItem.ToString())
                        {
                            case "1":
                                P.StopBits = StopBits.One;
                                break;
                            case "1.5":
                                P.StopBits = StopBits.OnePointFive;
                                break;
                            case "2":
                                P.StopBits = StopBits.Two;
                                break;
                        }
                    }

                    P.Open();
                    if (btconnect != null)
                    {
                        btconnect.Enabled = false;
                    }
                    if (btdisc != null)
                    {
                        btdisc.Enabled = true;
                    }

                    // 重新添加串口事件处理程序
                    P.DataReceived += new SerialDataReceivedEventHandler(DataReceive);

                    // 清空FIFO缓冲区并重置统计信息
                    lock (bufferLock)
                    {
                        dataBuffer.Clear();
                    }
                    ResetStatistics();

                    // 启动FIFO数据处理线程
                    StartDataProcessing();

                    // 初始化UI更新定时器
                    if (uiUpdateTimer == null)
                    {
                        uiUpdateTimer = new System.Windows.Forms.Timer();
                        uiUpdateTimer.Interval = UI_UPDATE_INTERVAL;
                        uiUpdateTimer.Tick += UiUpdateTimer_Tick;
                    }
                    uiUpdateTimer.Start();

                    if (richTextBox1 != null)
                    {
                        richTextBox1.AppendText($"串口 {P.PortName} 连接成功\n");
                        richTextBox1.AppendText($"波特率: {P.BaudRate}\n");
                        richTextBox1.AppendText($"数据位: {P.DataBits}\n");
                        richTextBox1.AppendText($"校验位: {P.Parity}\n");
                        richTextBox1.AppendText($"停止位: {P.StopBits}\n");
                        richTextBox1.AppendText("FIFO数据处理已启动\n");
                        richTextBox1.AppendText("UI更新节流已启用\n");
                        richTextBox1.AppendText("------------------------\n");
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"连接错误: {exception.Message}");
                    MessageBox.Show($"连接失败: {exception.Message}");
                }
            }
        }

        private void btdisc_Click(object sender, EventArgs e)
        {
            // 停止FIFO数据处理线程
            StopDataProcessing();

            // 停止UI更新定时器
            if (uiUpdateTimer != null)
            {
                uiUpdateTimer.Stop();
            }

            // 移除串口事件处理程序
            P.DataReceived -= new SerialDataReceivedEventHandler(DataReceive);

            // 关闭串口
            P.Close();
            btconnect.Enabled = true;
            btdisc.Enabled = false;

            // 清空FIFO缓冲区和UI更新队列
            lock (bufferLock)
            {
                dataBuffer.Clear();
            }
            lock (uiQueueLock)
            {
                uiUpdateQueue.Clear();
            }

            // 显示统计信息
            string stats = GetStatistics();
            if (richTextBox1 != null)
            {
                richTextBox1.AppendText("串口已断开连接\n");
                richTextBox1.AppendText($"数据处理统计: {stats}\n");
                richTextBox1.AppendText("------------------------\n");
            }

            Console.WriteLine("串口已断开连接");
            Console.WriteLine($"数据处理统计: {stats}");
        }


        //CONTROL TAB

        #region control tab fields
        double Kp, Ki, Kd, POS, SP;
        private int tickStart = 0;
        bool checkki = true, checkkp = true, checkkd = true;
        //Checking the number requirement and show message
        private bool spcheck, pocheck;
        #endregion

        //Take the control string send to STM32F4. Plot the graph
        public void COMhandler(string data)
        {

        }
        public string sendstring;
        private void txtsp_TextChanged(object sender, EventArgs e)
        {

        }
        private void timercheck_Tick(object sender, EventArgs e)
        {

        }
        private void txtsetspeed_TextChanged(object sender, EventArgs e)
        {
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void txtsetposition_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btsendcontrol_Click(object sender, EventArgs e)
        {
            //Xu li tin hieu trong Text Box. Kp Ki Kd

        }

        private void button7_Click(object sender, EventArgs e)
        {

        }


        private void btstop_Click(object sender, EventArgs e)
        {
            //Send command to stop the motor. 29 times "#"
        }
        private void checkspeed_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void button12_Click(object sender, EventArgs e)
        {
            try
            {
                // 线程安全地获取数据
                List<float[]> currentData;
                lock (dataLock)
                {
                    currentData = new List<float[]>(positionData);
                }

                if (currentData.Count == 0)
                {
                    return; // 没有数据，直接返回
                }

                // 创建保存文件对话框
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "CSV文件 (*.csv)|*.csv|文本文件 (*.txt)|*.txt";
                saveDialog.Title = "保存位置数据";
                saveDialog.FileName = $"位置数据_{DateTime.Now:yyyyMMdd_HHmmss}";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (StreamWriter writer = new StreamWriter(saveDialog.FileName))
                        {
                            // 写入表头
                            writer.WriteLine("序号,目标位置,电机位置,时间(秒),PID输出");

                            // 写入数据
                            for (int i = 0; i < currentData.Count; i++)
                            {
                                writer.WriteLine($"{i + 1},{currentData[i][0]:F6},{currentData[i][1]:F6},{currentData[i][2]:F6},{currentData[i][3]:F6}");
                            }
                        }

                        // 保存成功后清空数组
                        ClearPositionData();
                        isRecordingData = false; // 停止记录
                        StopButtonAnimation(); // 停止按钮动画

                        Console.WriteLine($"数据已成功保存到: {saveDialog.FileName}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"保存文件时出错: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"处理数据时出错: {ex.Message}");
            }
        }

        private void checkpos_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void button11_Click(object sender, EventArgs e)
        {
            try
            {
                // 线程安全地获取数据
                List<float[]> currentData;
                lock (dataLock)
                {
                    currentData = new List<float[]>(positionData);
                }

                if (currentData.Count == 0)
                {
                    // 开始记录数据
                    isRecordingData = true;
                    tickStart = Environment.TickCount;  // 重置时间戳
                    StartButtonAnimation(); // 开始按钮动画
                    return;
                }

                // 直接清空数据并重新开始记录
                ClearPositionData();
                tickStart = Environment.TickCount;  // 重置时间戳
                isRecordingData = true;  // 重新开始记录
                StartButtonAnimation(); // 开始按钮动画
            }
            catch (Exception ex)
            {
                Console.WriteLine($"处理位置数据时出错: {ex.Message}");
            }
        }

        private void MotorControl_Resize(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsHandleCreated || this.IsDisposed) return;

                // First adjust tabControl1 size to fill the form
                if (tabControl1 != null)
                {
                    const int formMargin = 14;  // Margin from form edges
                    tabControl1.Size = new Size(
                        this.ClientSize.Width - (formMargin * 2),
                        this.ClientSize.Height - (formMargin * 2)
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"窗体大小调整错误: {ex.Message}");
            }
        }

        // 添加 DropDown 事件处理
        private void cbCom_DropDown(object sender, EventArgs e)
        {
            try
            {
                if (cbCom == null) return;

                // 保存当前选中的项
                string currentSelection = cbCom.SelectedItem?.ToString();

                // 清空并更新列表
                cbCom.Items.Clear();
                string[] ports = SerialPort.GetPortNames();
                cbCom.Items.AddRange(ports);

                // 尝试恢复之前的选择
                if (!string.IsNullOrEmpty(currentSelection) && cbCom.Items.Contains(currentSelection))
                {
                    cbCom.SelectedItem = currentSelection;
                }
                else if (cbCom.Items.Count > 0)
                {
                    cbCom.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ComboBox下拉列表更新错误: {ex.Message}");
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // 停止并释放动画定时器
            if (animationTimer != null)
            {
                animationTimer.Stop();
                animationTimer.Dispose();
                animationTimer = null;
            }

            // 停止并释放UI更新定时器
            if (uiUpdateTimer != null)
            {
                uiUpdateTimer.Stop();
                uiUpdateTimer.Dispose();
                uiUpdateTimer = null;
            }

            // 停止并释放UDP接收器
            StopUdpReceiver();

            // 停止FIFO数据处理线程
            StopDataProcessing();

            // 关闭串口连接
            if (P != null && P.IsOpen)
            {
                try
                {
                    P.DataReceived -= new SerialDataReceivedEventHandler(DataReceive);
                    P.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"关闭串口时出错: {ex.Message}");
                }
            }

            base.OnFormClosing(e);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (P == null || !P.IsOpen)
                {
                    return;
                }

                // 获取textBox1的输入值
                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    Console.WriteLine("请输入目标位置");
                    return;
                }

                // 尝试将输入转换为数字
                if (float.TryParse(textBox1.Text, out float targetPosition))
                {
                    // 构造移动命令
                    string moveCommand = $"move to {targetPosition}";

                    // 发送命令
                    byte[] commandBytes = Encoding.UTF8.GetBytes(moveCommand + "\r\n");
                    P.Write(commandBytes, 0, commandBytes.Length);

                    Console.WriteLine($"发送移动命令: {moveCommand}");
                }
                else
                {
                    Console.WriteLine("输入格式错误，请输入有效的数字");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发送移动命令时出错: {ex.Message}");
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            // UDP接收器控制
            if (!isUdpReceiving)
            {
                // 启动UDP接收器
                StartUdpReceiver();
                button10.Text = "停止UDP";
                Console.WriteLine("UDP接收器已启动");
            }
            else
            {
                // 停止UDP接收器
                StopUdpReceiver();
                button10.Text = "启动UDP";
                Console.WriteLine("UDP接收器已停止");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (P == null || !P.IsOpen)
                {
                    MessageBox.Show("请先连接串口");
                    return;
                }

                // 获取四个输入值（需要添加第4个输入框）
                if (string.IsNullOrEmpty(textBox13.Text) || string.IsNullOrEmpty(textBox14.Text) ||
                    string.IsNullOrEmpty(textBox16.Text) || string.IsNullOrEmpty(textBox15.Text))
                {
                    MessageBox.Show("请填写所有扫频参数");
                    return;
                }

                // 尝试将输入转换为数字
                if (float.TryParse(textBox15.Text, out float param1) &&
                    float.TryParse(textBox13.Text, out float param2) &&
                    float.TryParse(textBox14.Text, out float param3) &&
                    float.TryParse(textBox16.Text, out float param4))
                {
                    // 清空之前的数据并开始记录
                    ClearPositionData();
                    tickStart = Environment.TickCount;  // 重置时间戳
                    isRecordingData = true;  // 开始记录数据

                    // 在richTextBox1中显示扫频开始信息
                    if (richTextBox1 != null)
                    {
                        richTextBox1.AppendText($"开始扫频测试 - {DateTime.Now:HH:mm:ss}\n");
                        richTextBox1.AppendText($"振幅: {param2:F3}, 起始频率: {param3:F3}Hz, 结束频率: {param4:F3}Hz, 中心点: {param1:F3}\n");
                        richTextBox1.AppendText("开始记录数据...\n");
                        richTextBox1.AppendText("------------------------\n");
                        richTextBox1.ScrollToCaret();
                    }

                    // 构造频率移动命令 - 添加中心点参数
                    // 格式：freq move amplitude start_freq end_freq center_point
                    string freqMoveCommand = $"freq move {param2:F3} {param3:F3} {param4:F3} {param1:F3}";

                    // 发送命令
                    byte[] commandBytes = Encoding.UTF8.GetBytes(freqMoveCommand + "\r\n");
                    P.Write(commandBytes, 0, commandBytes.Length);

                    // 清空串口缓冲区，确保命令完整发送
                    P.DiscardInBuffer();
                    P.DiscardOutBuffer();
                }
                else
                {
                    MessageBox.Show("输入格式错误，请输入有效的数字");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发送频率移动命令时出错: {ex.Message}");
                MessageBox.Show($"发送扫频命令失败: {ex.Message}");
            }
        }

        private void zedGraphControl1_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (myPane?.Legend == null || !myPane.Legend.IsVisible) return;

                // 获取Legend区域
                RectangleF legendRect = myPane.Legend.Rect;
                if (legendRect.Width <= 0 || legendRect.Height <= 0)
                {
                    // 强制刷新获取有效的Legend区域
                    zedGraphControl1.AxisChange();
                    zedGraphControl1.Invalidate();
                    legendRect = myPane.Legend.Rect;
                    if (legendRect.Width <= 0 || legendRect.Height <= 0) return;
                }

                // 检查是否点击在Legend区域内（包含容差）
                const float tolerance = 10.0f;
                if (e.X < legendRect.X - tolerance || e.X > legendRect.X + legendRect.Width + tolerance ||
                    e.Y < legendRect.Y - tolerance || e.Y > legendRect.Y + legendRect.Height + tolerance)
                {
                    return; // 点击不在Legend区域内
                }

                // 计算点击的Legend项索引
                int itemCount = myPane.CurveList.Count;
                if (itemCount <= 0) return;

                float clickRatio = (e.Y - legendRect.Y) / legendRect.Height;
                int clickedIndex = Math.Max(0, Math.Min((int)(clickRatio * itemCount), itemCount - 1));

                // 切换对应曲线的可见性
                CurveItem clickedCurve = myPane.CurveList[clickedIndex];
                if (clickedCurve != null)
                {
                    clickedCurve.IsVisible = !clickedCurve.IsVisible;

                    // 确保Legend设置一致性并刷新图表
                    EnsureLegendConsistency();
                    zedGraphControl1.AxisChange();
                    zedGraphControl1.Invalidate();
                    zedGraphControl1.Refresh();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Legend点击处理错误: {ex.Message}");
            }
        }

        // 开始按钮动画效果
        private void StartButtonAnimation()
        {
            if (animationTimer == null)
            {
                animationTimer = new System.Windows.Forms.Timer();
                animationTimer.Interval = 500; // 500毫秒闪烁间隔
                animationTimer.Tick += AnimationTimer_Tick;
            }

            if (!animationTimer.Enabled)
            {
                animationTimer.Start();
                isButtonBlinking = true;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                // 停止UDP接收器
                if (isUdpReceiving)
                {
                    StopUdpReceiver();
                    Console.WriteLine("UDP接收器已停止");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"关闭网络连接时出错: {ex.Message}");
            }
        }

        // 停止按钮动画效果
        private void StopButtonAnimation()
        {
            if (animationTimer != null && animationTimer.Enabled)
            {
                animationTimer.Stop();
                isButtonBlinking = false;

                // 恢复按钮原始颜色
                if (button11 != null)
                {
                    button11.BackColor = SystemColors.Control;
                    button11.ForeColor = SystemColors.ControlText;
                }
            }
        }

        // 动画定时器事件
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (button11 != null && isButtonBlinking)
            {
                // 切换按钮颜色实现闪烁效果
                if (button11.BackColor == Color.Red)
                {
                    button11.BackColor = SystemColors.Control;
                    button11.ForeColor = SystemColors.ControlText;
                }
                else
                {
                    button11.BackColor = Color.Red;
                    button11.ForeColor = Color.White;
                }
            }
        }

        private void btnegative_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (P != null && P.IsOpen)
                {
                    string debugCommand = checkBox2.Checked ? "set debug 1" : "set debug 0";

                    // 发送调试命令
                    byte[] commandBytes = Encoding.UTF8.GetBytes(debugCommand + "\r\n");
                    P.Write(commandBytes, 0, commandBytes.Length);

                    Console.WriteLine($"发送调试命令: {debugCommand}");
                }
                else
                {
                    Console.WriteLine("串口未连接，无法发送调试命令");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发送调试命令时出错: {ex.Message}");
            }
        }

        // UDP接收器方法
        private void StartUdpReceiver()
        {
            try
            {
                if (isUdpReceiving) return;

                udpClient = new UdpClient(udpPort);
                udpClient.Client.ReceiveTimeout = 1000; // 1秒超时
                isUdpReceiving = true;
                udpPacketCount = 0;
                udpStartTime = DateTime.Now;

                udpReceiveThread = new Thread(UdpReceiveLoop);
                udpReceiveThread.IsBackground = true;
                udpReceiveThread.Start();

                // 在richTextBox1中显示启动信息
                if (richTextBox1 != null)
                {
                    richTextBox1.AppendText($"UDP 接收测试\n");
                    richTextBox1.AppendText($"监听端口 {udpPort}...\n");
                    richTextBox1.AppendText($"期望数据格式: 4个float值 + 4字节尾标识\n");
                    richTextBox1.AppendText("------------------------\n");
                    richTextBox1.ScrollToCaret();
                }

                Console.WriteLine($"UDP 接收测试");
                Console.WriteLine($"监听端口 {udpPort}...");
                Console.WriteLine($"期望数据格式: 4个float值 + 4字节尾标识");
            }
            catch (Exception ex)
            {
                string errorMsg = $"启动UDP接收器失败: {ex.Message}";
                Console.WriteLine(errorMsg);
                if (richTextBox1 != null)
                {
                    richTextBox1.AppendText($"{errorMsg}\n");
                    richTextBox1.ScrollToCaret();
                }
            }
        }

        private void StopUdpReceiver()
        {
            try
            {
                isUdpReceiving = false;

                if (udpReceiveThread != null && udpReceiveThread.IsAlive)
                {
                    udpReceiveThread.Join(1000); // 等待1秒
                }

                if (udpClient != null)
                {
                    udpClient.Close();
                    udpClient = null;
                }

                string stopMsg = $"UDP接收器已停止，共收到 {udpPacketCount} 个数据包";
                Console.WriteLine(stopMsg);
                if (richTextBox1 != null)
                {
                    richTextBox1.AppendText($"{stopMsg}\n");
                    richTextBox1.AppendText("------------------------\n");
                    richTextBox1.ScrollToCaret();
                }
            }
            catch (Exception ex)
            {
                string errorMsg = $"停止UDP接收器失败: {ex.Message}";
                Console.WriteLine(errorMsg);
                if (richTextBox1 != null)
                {
                    richTextBox1.AppendText($"{errorMsg}\n");
                    richTextBox1.ScrollToCaret();
                }
            }
        }

        private void UdpReceiveLoop()
        {
            try
            {
                while (isUdpReceiving && udpClient != null)
                {
                    try
                    {
                        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                        byte[] data = udpClient.Receive(ref remoteEndPoint);
                        udpPacketCount++;

                        // 检查数据长度 (4个float + 4字节尾标识 = 20字节)
                        if (data != null && data.Length == 20)
                        {
                            // 解析4个float值
                            float goalPos = BitConverter.ToSingle(data, 0);
                            float currentPos = BitConverter.ToSingle(data, 4);
                            float pidOutput = BitConverter.ToSingle(data, 8);
                            float positionError = BitConverter.ToSingle(data, 12);

                            // 检查尾标识
                            byte[] tail = new byte[4];
                            Array.Copy(data, 16, tail, 0, 4);
                            bool tailValid = (tail[0] == 0x00 && tail[1] == 0x00 && tail[2] == 0x80 && tail[3] == 0x7f);

                            // 在UI线程中更新richTextBox1
                            if (this != null && !this.IsDisposed && this.IsHandleCreated)
                            {
                                try
                                {
                                    this.Invoke(new Action(() =>
                                    {
                                        if (richTextBox1 != null && !richTextBox1.IsDisposed)
                                        {
                                            richTextBox1.AppendText($"[{udpPacketCount}] 收到来自 {remoteEndPoint}:\n");
                                            richTextBox1.AppendText($"    目标位置: {goalPos:F6}\n");
                                            richTextBox1.AppendText($"    当前位置: {currentPos:F6}\n");
                                            richTextBox1.AppendText($"    PID输出: {pidOutput:F6}\n");
                                            richTextBox1.AppendText($"    位置误差: {positionError:F6}\n");
                                            richTextBox1.AppendText($"    尾标识: {BitConverter.ToString(tail).Replace("-", " ")} {(tailValid ? "✓" : "✗")}\n");
                                            richTextBox1.AppendText("------------------------\n");
                                            richTextBox1.ScrollToCaret();
                                        }
                                    }));
                                }
                                catch (ObjectDisposedException)
                                {
                                    // 窗体已关闭，停止接收
                                    break;
                                }
                            }

                            // 只有尾标识有效时才更新图表
                            if (tailValid)
                            {
                                // 更新图表（在UI线程中）
                                if (this != null && !this.IsDisposed && this.IsHandleCreated)
                                {
                                    try
                                    {
                                        this.Invoke(new Action(() =>
                                        {
                                            draw(goalPos, currentPos, pidOutput, positionError);
                                        }));
                                    }
                                    catch (ObjectDisposedException)
                                    {
                                        // 窗体已关闭，停止接收
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                // 尾标识无效，记录错误
                                Console.WriteLine($"UDP数据包 {udpPacketCount} 尾标识无效: {BitConverter.ToString(tail).Replace("-", " ")}");
                            }
                        }
                    }
                    catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
                    {
                        // 超时，继续等待
                        continue;
                    }
                    catch (Exception ex)
                    {
                        string errorMsg = $"UDP接收错误: {ex.Message}";

                        // 在UI线程中更新错误信息
                        if (this != null && !this.IsDisposed && this.IsHandleCreated)
                        {
                            try
                            {
                                this.Invoke(new Action(() =>
                                {
                                    if (richTextBox1 != null && !richTextBox1.IsDisposed)
                                    {
                                        richTextBox1.AppendText($"{errorMsg}\n");
                                        richTextBox1.ScrollToCaret();
                                    }
                                }));
                            }
                            catch (ObjectDisposedException)
                            {
                                // 窗体已关闭，停止接收
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                string errorMsg = $"UDP接收循环错误: {ex.Message}";

                // 在UI线程中更新错误信息
                if (this != null && !this.IsDisposed && this.IsHandleCreated)
                {
                    try
                    {
                        this.Invoke(new Action(() =>
                        {
                            if (richTextBox1 != null && !richTextBox1.IsDisposed)
                            {
                                richTextBox1.AppendText($"{errorMsg}\n");
                                richTextBox1.ScrollToCaret();
                            }
                        }));
                    }
                    catch (ObjectDisposedException)
                    {
                        // 窗体已关闭，忽略异常
                    }
                }
            }
        }

        // 自动保存扫频数据的方法
        private void AutoSaveSweepData()
        {
            try
            {
                // 线程安全地获取数据
                List<float[]> currentData;
                lock (dataLock)
                {
                    currentData = new List<float[]>(positionData);
                }

                if (currentData.Count == 0)
                {
                    Console.WriteLine("没有数据可保存");
                    return;
                }

                // 生成默认文件名
                string defaultFileName = $"扫频数据_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string fullPath = Path.Combine(documentsPath, "MotorControl", defaultFileName);

                // 确保目录存在
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                using (StreamWriter writer = new StreamWriter(fullPath))
                {
                    // 写入表头
                    writer.WriteLine("序号,目标位置,电机位置,时间(秒),PID输出");

                    // 写入数据
                    for (int i = 0; i < currentData.Count; i++)
                    {
                        writer.WriteLine($"{i + 1},{currentData[i][0]:F6},{currentData[i][1]:F6},{currentData[i][2]:F6},{currentData[i][3]:F6}");
                    }
                }

                // 弹出对话框提示保存位置
                string message = $"扫频测试数据保存成功！\n\n" +
                               $"保存位置：{fullPath}\n" +
                               $"数据点数：{currentData.Count}\n" +
                               $"文件大小：{new FileInfo(fullPath).Length / 1024.0:F1} KB\n\n" +
                               $"是否立即生成伯德图？";

                DialogResult result = MessageBox.Show(message, "数据保存成功",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                {
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(() =>
                        {
                            List<double> frequencies, magnitudes, phases;
                            GenerateBodePlotWithReturn(currentData, out frequencies, out magnitudes, out phases);
                            double freqAtMinus3dB = FindClosestFrequency(frequencies, magnitudes, -3);
                            double freqAtMinus45Deg = FindClosestFrequency(frequencies, phases, -45);
                            if (tabControl1 != null && tabPage3 != null)
                                tabControl1.SelectedTab = tabPage3;
                            if (richTextBoxBode != null)
                            {
                                richTextBoxBode.Clear();
                                richTextBoxBode.AppendText($"-3dB带宽频率: {freqAtMinus3dB:F3} Hz\n");
                                richTextBoxBode.AppendText($"-45°相位滞后频率: {freqAtMinus45Deg:F3} Hz\n");
                            }
                        }));
                    }
                    else
                    {
                        List<double> frequencies, magnitudes, phases;
                        GenerateBodePlotWithReturn(currentData, out frequencies, out magnitudes, out phases);
                        double freqAtMinus3dB = FindClosestFrequency(frequencies, magnitudes, -3);
                        double freqAtMinus45Deg = FindClosestFrequency(frequencies, phases, -45);
                        if (tabControl1 != null && tabPage3 != null)
                            tabControl1.SelectedTab = tabPage3;
                        if (richTextBoxBode != null)
                        {
                            richTextBoxBode.Clear();
                            richTextBoxBode.AppendText($"-3dB带宽频率: {freqAtMinus3dB:F3} Hz\n");
                            richTextBoxBode.AppendText($"-45°相位滞后频率: {freqAtMinus45Deg:F3} Hz\n");
                        }
                    }
                }

                // 清空数据
                ClearPositionData();

                Console.WriteLine($"扫频数据已自动保存到: {fullPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"自动保存数据时出错: {ex.Message}");
                MessageBox.Show($"自动保存数据时出错：{ex.Message}",
                    "保存错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 生成伯德图按钮事件处理程序
        private void buttonGenerateBode_Click(object sender, EventArgs e)
        {
            try
            {
                // 线程安全地获取数据
                List<float[]> currentData;
                lock (dataLock)
                {
                    currentData = new List<float[]>(positionData);
                }

                if (currentData.Count == 0)
                {
                    MessageBox.Show("没有数据可处理，请先记录数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 生成伯德图并获取数据
                List<double> frequencies, magnitudes, phases;
                GenerateBodePlotWithReturn(currentData, out frequencies, out magnitudes, out phases);

                // 查找-3dB点
                double freqAtMinus3dB = FindClosestFrequency(frequencies, magnitudes, -3);
                // 查找-45度点
                double freqAtMinus45Deg = FindClosestFrequency(frequencies, phases, -45);

                // 切换到伯德图tab
                tabControl1.SelectedTab = tabPage3;

                if (richTextBoxBode != null)
                {
                    richTextBoxBode.Clear();
                    richTextBoxBode.AppendText($"-3dB带宽频率: {freqAtMinus3dB:F3} Hz\n");
                    richTextBoxBode.AppendText($"-45°相位滞后频率: {freqAtMinus45Deg:F3} Hz\n");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"生成伯德图时出错：{ex.Message}",
                    "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 查找最接近目标值的频率
        private double FindClosestFrequency(List<double> freqs, List<double> values, double target)
        {
            double minDiff = double.MaxValue;
            double freqResult = 0;
            for (int i = 0; i < freqs.Count; i++)
            {
                double diff = Math.Abs(values[i] - target);
                if (diff < minDiff)
                {
                    minDiff = diff;
                    freqResult = freqs[i];
                }
            }
            return freqResult;
        }

        // 生成伯德图并返回数据
        private void GenerateBodePlotWithReturn(List<float[]> data, out List<double> frequencies, out List<double> magnitudes, out List<double> phases)
        {
            frequencies = new List<double>();
            magnitudes = new List<double>();
            phases = new List<double>();
            try
            {
                if (zedGraphControlBode == null)
                {
                    MessageBox.Show("伯德图控件未初始化", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                GraphPane bodePane = zedGraphControlBode.GraphPane;
                if (bodePane == null)
                {
                    MessageBox.Show("伯德图面板未初始化", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                bodePane.CurveList.Clear();
                bodePane.Title.Text = "伯德图 - 幅频特性和相频特性";
                bodePane.XAxis.Title.Text = "频率 (Hz)";
                bodePane.YAxis.Title.Text = "幅值 (dB)";
                bodePane.Y2Axis.Title.Text = "相位 (度)";
                bodePane.Y2Axis.IsVisible = true;
                bodePane.Y2Axis.Scale.Min = -180;
                bodePane.Y2Axis.Scale.Max = 180;
                double startFreq = 0.1;
                double endFreq = 5.0;
                if (textBox14 != null && float.TryParse(textBox14.Text, out float startFreqValue))
                    startFreq = startFreqValue;
                if (textBox16 != null && float.TryParse(textBox16.Text, out float endFreqValue))
                    endFreq = endFreqValue;
                double totalTime = data[data.Count - 1][2] - data[0][2];
                int windowSize = Math.Min(100, data.Count / 10);
                if (windowSize < 10) windowSize = 10;
                for (int i = windowSize; i < data.Count - windowSize; i += windowSize / 2)
                {
                    double timeRatio = data[i][2] / totalTime;
                    double currentFreq = startFreq + timeRatio * (endFreq - startFreq);
                    double sumInput = 0, sumOutput = 0;
                    double sumInputSquared = 0, sumOutputSquared = 0;
                    double sumCross = 0;
                    for (int j = i - windowSize / 2; j < i + windowSize / 2 && j < data.Count; j++)
                    {
                        if (j >= 0)
                        {
                            double input = data[j][0];
                            double output = data[j][1];
                            sumInput += input;
                            sumOutput += output;
                            sumInputSquared += input * input;
                            sumOutputSquared += output * output;
                            sumCross += input * output;
                        }
                    }
                    int actualWindowSize = Math.Min(windowSize, data.Count - (i - windowSize / 2));
                    if (actualWindowSize > 0)
                    {
                        double avgInput = sumInput / actualWindowSize;
                        double avgOutput = sumOutput / actualWindowSize;
                        double avgInputSquared = sumInputSquared / actualWindowSize;
                        double avgOutputSquared = sumOutputSquared / actualWindowSize;
                        double avgCross = sumCross / actualWindowSize;
                        double inputVariance = avgInputSquared - avgInput * avgInput;
                        double outputVariance = avgOutputSquared - avgOutput * avgOutput;
                        double crossCovariance = avgCross - avgInput * avgOutput;
                        if (inputVariance > 0)
                        {
                            double magnitude = Math.Sqrt(outputVariance / inputVariance);
                            if (magnitude > 0)
                                magnitude = 20 * Math.Log10(magnitude);
                            else
                                magnitude = -60;
                            double phase = Math.Atan2(crossCovariance, inputVariance) * 180 / Math.PI;
                            frequencies.Add(currentFreq);
                            magnitudes.Add(magnitude);
                            phases.Add(phase);
                        }
                    }
                }
                if (frequencies.Count == 0)
                {
                    MessageBox.Show("无法从数据中提取有效的频率响应信息", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                PointPairList magnitudePoints = new PointPairList();
                PointPairList phasePoints = new PointPairList();
                for (int i = 0; i < frequencies.Count; i++)
                {
                    magnitudePoints.Add(frequencies[i], magnitudes[i]);
                    phasePoints.Add(frequencies[i], phases[i]);
                }
                LineItem magnitudeCurve = bodePane.AddCurve("幅频特性", magnitudePoints, Color.Blue, SymbolType.Circle);
                magnitudeCurve.Line.Width = 2;
                magnitudeCurve.Symbol.Size = 4;
                LineItem phaseCurve = bodePane.AddCurve("相频特性", phasePoints, Color.Red, SymbolType.Square);
                phaseCurve.Line.Width = 2;
                phaseCurve.Symbol.Size = 4;
                phaseCurve.IsY2Axis = true;
                bodePane.XAxis.Type = AxisType.Log;
                bodePane.XAxis.Scale.Min = startFreq;
                bodePane.XAxis.Scale.Max = endFreq;
                double minMag = magnitudes.Min();
                double maxMag = magnitudes.Max();
                bodePane.YAxis.Scale.Min = Math.Floor(minMag / 10) * 10;
                bodePane.YAxis.Scale.Max = Math.Ceiling(maxMag / 10) * 10;
                bodePane.XAxis.MajorGrid.IsVisible = true;
                bodePane.YAxis.MajorGrid.IsVisible = true;
                bodePane.Y2Axis.MajorGrid.IsVisible = true;
                bodePane.Legend.IsVisible = true;
                bodePane.Legend.Position = LegendPos.Top;
                zedGraphControlBode.AxisChange();
                zedGraphControlBode.Invalidate();
                zedGraphControlBode.Refresh();
                Console.WriteLine("伯德图生成完成");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"生成伯德图时出错: {ex.Message}");
                throw;
            }
        }
    }
}