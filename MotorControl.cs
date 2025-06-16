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
namespace MotorControl6h39
{
    public partial class MotorControl : Form
    {
        #region COMPort and Graph objects
        ControlSignals control = new ControlSignals();
        GraphPane myPane = new GraphPane(); // Khai báo sửa dụng Graph loại GraphPane;
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
        #endregion
        private void MotorControl_Load(object sender, EventArgs e)
        {
            try
            {
                cbCom.SelectedIndex = 0;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            cbRate.SelectedIndex = 3; // 9600
            cbBits.SelectedIndex = 2; // 8
            cbParity.SelectedIndex = 0; // None
            cbBit.SelectedIndex = 0; // None
            btdisc.Enabled = false;
            P.Close();

            // 初始化图表
            myPane = zedGraphControl1.GraphPane;
            myPane.Title.Text = "电机控制";
            myPane.XAxis.Title.Text = "时间(s)";
            myPane.YAxis.Title.Text = "速度(rpm)";
            
            // 设置坐标轴范围
            myPane.XAxis.Scale.Min = 0;
            myPane.XAxis.Scale.Max = 10;  // 显示最近10秒的数据
            myPane.YAxis.Scale.Min = 0;
            myPane.YAxis.Scale.Max = 1000;  // 根据实际数据范围调整
            
            // 初始化数据点列表
            listPointsOne = new PointPairList();
            myCurveOne = myPane.AddCurve("速度", listPointsOne, Color.Red, SymbolType.Circle);
            myCurveOne.Line.Width = 2;
            myCurveOne.Symbol.Size = 5;
            
            // 初始化时间戳
            tickStart = Environment.TickCount;
            
            zedGraphControl1.AxisChange();
            timer1.Interval = 10;

            // 确保在窗体加载时调整图表大小
            this.BeginInvoke(new Action(() =>
            {
                MotorControl_Resize(this, EventArgs.Empty);
            }));

            // 启动数据处理线程
            processThread = new Thread(ProcessDataThread);
            processThread.IsBackground = true;
            processThread.Start();
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
            try
            {
                // 读取所有可用数据
                byte[] buffer = new byte[P.BytesToRead];
                P.Read(buffer, 0, buffer.Length);
                
                // 将数据添加到缓存队列
                lock (bufferLock)
                {
                    foreach (byte b in buffer)
                    {
                        dataBuffer.Enqueue(b);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DataReceive: {ex.Message}");
            }
        }

        private void ProcessDataThread()
        {
            while (!shouldStop)
            {
                try
                {
                    // 检查是否有足够的数据进行处理
                    if (dataBuffer.Count >= 8)
                    {
                        ProcessBuffer();
                    }
                    Thread.Sleep(1); // 给其他线程一些执行时间
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in ProcessDataThread: {ex.Message}");
                }
            }
        }

        private void ProcessBuffer()
        {
            lock (bufferLock)
            {
                try
                {
                    // 创建一个临时列表来存储待处理的数据包
                    List<byte[]> packetsToProcess = new List<byte[]>();
                    
                    // 查找所有完整的数据包
                    while (dataBuffer.Count >= 8)
                    {
                        // 复制前8个字节进行检查
                        byte[] tempBuffer = dataBuffer.Take(8).ToArray();
                        
                        // 检查是否是完整的数据包（最后两个字节是0D 0A）
                        if (tempBuffer[6] == 0x0D && tempBuffer[7] == 0x0A)
                        {
                            // 找到完整数据包，添加到待处理列表
                            packetsToProcess.Add(tempBuffer);
                            
                            // 从缓存中移除这8个字节
                            for (int i = 0; i < 8; i++)
                            {
                                dataBuffer.Dequeue();
                            }
                        }
                        else
                        {
                            // 如果不是完整的数据包，保留在缓存中
                            break;
                        }
                    }

                    // 处理所有找到的完整数据包
                    if (packetsToProcess.Count > 0)
                    {
                        this.Invoke(new Action(() =>
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine($"找到 {packetsToProcess.Count} 个完整数据包");
                            
                            // 处理所有数据包
                            foreach (byte[] packet in packetsToProcess)
                            {
                                try
                                {
                                    ProcessPacket(packet, sb);
                                }
                                catch (Exception ex)
                                {
                                    sb.AppendLine($"处理数据包时出错: {ex.Message}");
                                }
                            }
                            
                            // 显示当前缓存状态
                            sb.AppendLine($"\n当前缓存状态:");
                            sb.AppendLine($"缓存数据长度: {dataBuffer.Count} 字节");
                            if (dataBuffer.Count > 0)
                            {
                                sb.AppendLine("缓存中的数据:");
                                foreach (byte b in dataBuffer.Take(16)) // 只显示前16个字节
                                {
                                    sb.Append($"{b:X2} ");
                                }
                                if (dataBuffer.Count > 16)
                                {
                                    sb.AppendLine("...");
                                }
                            }
                            
                            // 更新显示处理后的数据
                            richTextBox1.Text = sb.ToString();
                        }));
                    }
                    else
                    {
                        // 如果没有找到完整的数据包，显示当前缓存状态
                        this.Invoke(new Action(() =>
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("当前缓存状态:");
                            sb.AppendLine($"缓存数据长度: {dataBuffer.Count} 字节");
                            if (dataBuffer.Count > 0)
                            {
                                sb.AppendLine("缓存中的数据:");
                                foreach (byte b in dataBuffer.Take(16)) // 只显示前16个字节
                                {
                                    sb.Append($"{b:X2} ");
                                }
                                if (dataBuffer.Count > 16)
                                {
                                    sb.AppendLine("...");
                                }
                            }
                            richTextBox1.Text = sb.ToString();
                        }));
                    }
                }
                catch (Exception ex)
                {
                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.Text = $"处理缓存时出错: {ex.Message}";
                    }));
                }
            }
        }

        private void ProcessPacket(byte[] packet, StringBuilder sb)
        {
            if (packet.Length == 8)
            {
                try
                {
                    // 显示原始数据包内容
                    string hexString = BitConverter.ToString(packet).Replace("-", " ");
                    sb.AppendLine($"\n数据包内容: {hexString}");
                    
                    // 提取x值（第1字节为高字节，第2字节为低字节）
                    short x = (short)((packet[0] << 8) | packet[1]);
                    // 提取y值（第3字节为高字节，第4字节为低字节）
                    short y = (short)((packet[2] << 8) | packet[3]);

                    // 更新显示
                    sb.AppendLine($"X值: {x} (0x{packet[0]:X2} {packet[1]:X2})");
                    sb.AppendLine($"Y值: {y} (0x{packet[2]:X2} {packet[3]:X2})");
                    sb.AppendLine("------------------------");

                    // 更新图表
                    draw(y);
                }
                catch (Exception ex)
                {
                    sb.AppendLine($"处理数据包时出错: {ex.Message}");
                }
            }
        }

        private void draw(double ypoint)
        {
            try
            {
                // 计算时间（毫秒转换为秒）
                double time = (Environment.TickCount - tickStart) / 1000.0;
                
                // 添加新的数据点
                listPointsOne.Add(time, ypoint);
                
                // 如果数据点超过显示范围，移除旧的数据点
                while (listPointsOne.Count > 0 && listPointsOne[0].X < time - 10)
                {
                    listPointsOne.RemoveAt(0);
                }
                
                // 更新X轴范围
                myPane.XAxis.Scale.Min = time - 10;
                myPane.XAxis.Scale.Max = time;
                
                // 更新Y轴范围（考虑负数）
                double minY = listPointsOne.Count > 0 ? listPointsOne.Min(p => p.Y) : -1000;
                double maxY = listPointsOne.Count > 0 ? listPointsOne.Max(p => p.Y) : 1000;
                double range = maxY - minY;
                myPane.YAxis.Scale.Min = minY - range * 0.1;
                myPane.YAxis.Scale.Max = maxY + range * 0.1;
                
                // 刷新显示
                zedGraphControl1.AxisChange();
                zedGraphControl1.Invalidate();
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText($"\n更新图表时出错: {ex.Message}");
            }
        }

        private void cbCom_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (P.IsOpen)
            {
                // Nếu đang mở Port thì phải đóng lại. Vì không thể đang chạy mà thay đổi Port được
                P.Close();
            }
            string selectedPort = cbCom.SelectedItem?.ToString();
            P.PortName = selectedPort; // Assign PortName by selected COM port!
            try
            {
                P.Open();
            }
            catch (Exception exception)
            {
                Console.Write(exception);
                MessageBox.Show($"Error opening port {selectedPort}: {exception.Message}", "Error");
            }
        }

        private void cbRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (P.IsOpen)
            {
                P.Close();
            }
            P.BaudRate = Convert.ToInt32(cbRate.Text);
            try
            {
                P.Open();
            }
            catch (Exception exception)
            {
                Console.Write(exception);
            }
        }

        private void cbBits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (P.IsOpen)
            {
                P.Close();
            }
            P.DataBits = Convert.ToInt32(cbBits.Text);
            try
            {
                P.Open();
            }
            catch (Exception exception)
            {
                Console.Write(exception);
            }
        }

        private void cbParity_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (P.IsOpen)
            {
                P.Close();
            }
            // Với thằng Parity hơn lằng nhằng. Nhưng cũng OK thôi. ^_^
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
            try
            {
                P.Open();
            }
            catch (Exception exception)
            {
                Console.Write(exception);
            }
        }

        private void cbBit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (P.IsOpen)
            {
                P.Close();
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
            try
            {
                P.Open();
            }
            catch (Exception exception)
            {
                Console.Write(exception);
            }
        }
        private void btconnect_Click(object sender, EventArgs e)
        {
            if (!P.IsOpen)
            {
                try
                {
                    P.Open();
                    //User cannot click in the btconnect button anymore
                    btconnect.Enabled = false;
                    //User now can click in the btdisc button
                    btdisc.Enabled = true;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    MessageBox.Show("Incorrect COM Port");
                }
            }

        }

        private void btdisc_Click(object sender, EventArgs e)
        {
            P.Close();
            btconnect.Enabled = true;
            btdisc.Enabled = false;
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
        private void checkpos_CheckedChanged_1(object sender, EventArgs e)
        {

        }
        private void btpositive_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void btnegative_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void MotorControl_Resize(object sender, EventArgs e)
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

        // 添加 DropDown 事件处理
        private void cbCom_DropDown(object sender, EventArgs e)
        {
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

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            shouldStop = true;
            if (processThread != null && processThread.IsAlive)
            {
                processThread.Join(1000); // 等待线程结束
            }
            base.OnFormClosing(e);
        }
    }
}