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
        // private Queue<byte> dataBuffer = new Queue<byte>();
        // private readonly object bufferLock = new object();
        // private bool isProcessing = false;
        // private Thread processThread;
        // private bool shouldStop = false;
        #endregion
        // 在类级别添加更多的数据点列表和曲线
        private PointPairList listPointsGoal = new PointPairList();    // 目标位置
        private PointPairList listPointsMotor = new PointPairList();   // 电机位置
        private PointPairList listPointsError = new PointPairList();   // 位置误差
        private LineItem myCurveGoal;
        private LineItem myCurveMotor;
        private LineItem myCurveError;
        private bool legendClickRegistered = false;
        
        // 添加二维数组来存储目标位置和电机位置数据
        private List<float[]> positionData = new List<float[]>();  // 使用List来动态存储数据
        private readonly object dataLock = new object();  // 用于线程安全的数据访问
        private bool isRecordingData = false;  // 控制数据记录开始的标志
        
        // 添加动画效果相关变量
        private System.Windows.Forms.Timer animationTimer;  // 用于button11动画效果
        private bool isButtonBlinking = false;  // 控制按钮闪烁状态
        
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
                    return new float[0, 3];
                
                float[,] array = new float[positionData.Count, 3];
                for (int i = 0; i < positionData.Count; i++)
                {
                    array[i, 0] = positionData[i][0]; // 目标位置
                    array[i, 1] = positionData[i][1]; // 电机位置
                    array[i, 2] = positionData[i][2]; // 当前时间
                }
                return array;
            }
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
                        listPointsError = new PointPairList();
                        
                        Console.WriteLine("数据点列表初始化完成");
                        
                        // 添加三条曲线，使用不同颜色
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
            
            // 添加测试数据点
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    float testGoal = i * 5.0f;
                    float testMotor = i * 4.0f;
                    float testError = testGoal - testMotor;
                    draw(testGoal, testMotor, testError);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"测试数据点添加错误: {ex.Message}");
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
                
                
                // 使用 Invoke 确保在 UI 线程中更新控件
                if (this != null && !this.IsDisposed)
                {
                    this.Invoke(new Action(() =>
                    {
                        try
                        {
                            // 确保图表已初始化
                            if (myPane == null && zedGraphControl1 != null)
                            {
                                Console.WriteLine("重新初始化图表...");
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
                                    
                                    if (myCurveError == null && listPointsError != null)
                                    {
                                        myCurveError = AddCurveWithSettings("位置误差", listPointsError, Color.Green, SymbolType.Triangle);
                                    }
                                    
                                    Console.WriteLine("图表重新初始化完成");
                                    
                                    // 确保Legend设置一致性
                                    EnsureLegendConsistency();
                                }
                            }
                            
                            if (richTextBox1 != null)
                            {
                                richTextBox1.AppendText($"接收时间: {DateTime.Now:HH:mm:ss.fff}\n");
                                richTextBox1.AppendText($"接收字节数: {bytesRead}\n");
                            }
                            
                            // 如果数据长度是20字节（4个float + 4字节尾标识）
                            if (bytesRead == 20)
                            {
                                try
                                {
                                    // 解析4个float数据
                                    float goalPos = BitConverter.ToSingle(buffer, 0);      // 目标位置
                                    float motorPos = BitConverter.ToSingle(buffer, 4);     // 电机位置
                                    float pidOutput = BitConverter.ToSingle(buffer, 8);    // PID输出
                                    float positionError = BitConverter.ToSingle(buffer, 12); // 位置误差
                                    
                                    // 检查尾标识
                                    bool validTail = (buffer[16] == 0x00 && buffer[17] == 0x00 && 
                                                    buffer[18] == 0x80 && buffer[19] == 0x7f);
                                    
                                    if (richTextBox1 != null)
                                    {
                                        richTextBox1.AppendText($"目标位置: {goalPos:F3}\n");
                                        richTextBox1.AppendText($"电机位置: {motorPos:F3}\n");
                                        richTextBox1.AppendText($"PID输出: {pidOutput:F3}\n");
                                        richTextBox1.AppendText($"位置误差: {positionError:F3}\n");
                                        richTextBox1.AppendText($"尾标识有效: {(validTail ? "是" : "否")}\n");
                                    }
                                    
                                    // 更新图表，传入三个参数
                                    draw(goalPos, motorPos, positionError);
                                }
                                catch (Exception parseEx)
                                {
                                    if (richTextBox1 != null)
                                    {
                                        richTextBox1.AppendText($"数据解析错误: {parseEx.Message}\n");
                                    }
                                }
                            }
                            else
                            {
                                // 显示原始字节数据
                                string hexString = BitConverter.ToString(buffer, 0, bytesRead).Replace("-", " ");
                                if (richTextBox1 != null)
                                {
                                    richTextBox1.AppendText($"原始数据: {hexString}\n");
                                }
                            }
                            
                            if (richTextBox1 != null)
                            {
                                richTextBox1.AppendText("------------------------\n");
                                // 自动滚动到底部
                                richTextBox1.ScrollToCaret();
                            }
                        }
                        catch (Exception invokeEx)
                        {
                            Console.WriteLine($"UI更新错误: {invokeEx.Message}");
                        }
                    }));
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

        private void draw(float goalPos, float motorPos, float positionError)
        {
            try
            {
                // 确保在UI线程中执行
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => draw(goalPos, motorPos, positionError)));
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
                if (myPane == null || listPointsGoal == null || listPointsMotor == null || listPointsError == null)
                {
                    Console.WriteLine("图表对象未初始化");
                    Console.WriteLine($"myPane: {myPane != null}, listPointsGoal: {listPointsGoal != null}, listPointsMotor: {listPointsMotor != null}, listPointsError: {listPointsError != null}");
                    return;
                }

                // 计算时间（毫秒转换为秒）
                double time = (Environment.TickCount - tickStart) / 1000.0;
                
                // 添加新的数据点到三条曲线
                listPointsGoal.Add(time, goalPos);
                listPointsMotor.Add(time, motorPos);
                listPointsError.Add(time, positionError);
                
                // 只有在开始记录后才将数据存储到二维数组中（线程安全）
                if (isRecordingData)
                {
                    lock (dataLock)
                    {
                        // 计算当前时间（秒）
                        double currentTime = (Environment.TickCount - tickStart) / 1000.0;
                        positionData.Add(new float[] { goalPos, motorPos, (float)currentTime });
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
                
                if (listPointsGoal.Count > 0 || listPointsMotor.Count > 0 || listPointsError.Count > 0)
                {
                    minY = Math.Min(
                        listPointsGoal.Count > 0 ? listPointsGoal.Min(p => p.Y) : double.MaxValue,
                        Math.Min(
                            listPointsMotor.Count > 0 ? listPointsMotor.Min(p => p.Y) : double.MaxValue,
                            listPointsError.Count > 0 ? listPointsError.Min(p => p.Y) : double.MaxValue
                        )
                    );
                    maxY = Math.Max(
                        listPointsGoal.Count > 0 ? listPointsGoal.Max(p => p.Y) : double.MinValue,
                        Math.Max(
                            listPointsMotor.Count > 0 ? listPointsMotor.Max(p => p.Y) : double.MinValue,
                            listPointsError.Count > 0 ? listPointsError.Max(p => p.Y) : double.MinValue
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
                    
                    if (richTextBox1 != null)
                    {
                        richTextBox1.AppendText($"串口 {P.PortName} 连接成功\n");
                        richTextBox1.AppendText($"波特率: {P.BaudRate}\n");
                        richTextBox1.AppendText($"数据位: {P.DataBits}\n");
                        richTextBox1.AppendText($"校验位: {P.Parity}\n");
                        richTextBox1.AppendText($"停止位: {P.StopBits}\n");
                        richTextBox1.AppendText("------------------------\n");
                        
                        // 测试图表功能 - 添加一些测试数据点
                        for (int i = 0; i < 5; i++)
                        {
                            float testGoal = i * 10.0f;
                            float testMotor = i * 8.0f;
                            float testError = testGoal - testMotor;
                            draw(testGoal, testMotor, testError);
                            Thread.Sleep(100); // 短暂延迟
                        }
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
            // 移除串口事件处理程序
            P.DataReceived -= new SerialDataReceivedEventHandler(DataReceive);
            
            // 关闭串口
            P.Close();
            btconnect.Enabled = true;
            btdisc.Enabled = false;
            
            // 只清空串口输出显示，不清空图表数据
            if (richTextBox1 != null)
            {
                richTextBox1.AppendText("串口已断开连接\n");
                richTextBox1.AppendText("------------------------\n");
            }
            
            Console.WriteLine("串口已断开连接");
        }

        private void btexit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you want to exit?",
                "Nhom 9", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                MessageBox.Show("Thanks!", "Nhom 9");
                this.Close();
            }
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

        private void button8_Click(object sender, EventArgs e)
        {
            // 手动测试图表功能
            try
            {
                // 清空现有数据
                if (listPointsGoal != null)
                {
                    listPointsGoal.Clear();
                }
                if (listPointsMotor != null)
                {
                    listPointsMotor.Clear();
                }
                if (listPointsError != null)
                {
                    listPointsError.Clear();
                }
                
                // 重置时间戳
                tickStart = Environment.TickCount;
                
                // 添加测试数据点
                for (int i = 0; i < 10; i++)
                {
                    float testGoal = i * 2.0f;
                    float testMotor = i * 1.8f;
                    float testError = testGoal - testMotor;
                    
                    draw(testGoal, testMotor, testError);
                    
                    // 短暂延迟
                    Thread.Sleep(50);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"图表测试错误: {ex.Message}");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // 简单图表测试
            try
            {
                Console.WriteLine("开始简单图表测试...");
                
                // 检查图表对象
                Console.WriteLine($"zedGraphControl1: {zedGraphControl1 != null}");
                Console.WriteLine($"myPane: {myPane != null}");
                Console.WriteLine($"listPointsGoal: {listPointsGoal != null}");
                Console.WriteLine($"myCurveGoal: {myCurveGoal != null}");
                
                // 添加一个简单的测试点
                if (listPointsGoal != null && listPointsMotor != null && listPointsError != null)
                {
                    double testTime = 1.0;
                    float testGoal = 10.0f;
                    float testMotor = 8.0f;
                    float testError = 2.0f;
                    
                    listPointsGoal.Add(testTime, testGoal);
                    listPointsMotor.Add(testTime, testMotor);
                    listPointsError.Add(testTime, testError);
                    
                    Console.WriteLine($"测试点添加成功: 时间={testTime}, 目标={testGoal}, 电机={testMotor}, 误差={testError}");
                    
                    // 强制刷新图表
                    if (zedGraphControl1 != null)
                    {
                        zedGraphControl1.AxisChange();
                        zedGraphControl1.Invalidate();
                        Console.WriteLine("图表刷新完成");
                    }
                }
                else
                {
                    Console.WriteLine("数据点列表为空！");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"图表测试错误: {ex.Message}");
            }
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
                            writer.WriteLine("序号,目标位置,电机位置,时间(秒)");
                            
                            // 写入数据
                            for (int i = 0; i < currentData.Count; i++)
                            {
                                writer.WriteLine($"{i + 1},{currentData[i][0]:F6},{currentData[i][1]:F6},{currentData[i][2]:F6}");
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
            
            base.OnFormClosing(e);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // 测试方法已移除
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
    }
}