using System;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;
using DOTHI_HIENCLUBVN;
using ZedGraph;
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
            // set your pane
            myPane = zedGraphControl1.GraphPane;
            // set a title
            myPane.Title.Text = "Điều khiển động cơ";
            // set X and Y axis titles
            myPane.XAxis.Title.Text = "time";
            myPane.YAxis.Title.Text = "Speed(rpm)";
            zedGraphControl1.AxisChange();
            timer1.Interval = 10;

            // Khởi động timer về vị trí ban đầu 
            tickStart = Environment.TickCount;
            //enable button
            btsendcontrol.Enabled = false;
            txtsetposition.Enabled = false;
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
                /* Thêm toàn bộ các COM đã tìm được vào combox cbCom
                 * Sử dụng AddRange thay vì dùng foreach
                */
                cbCom.Items.AddRange(ports);
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
                string[] Databits = {"6", "7", "8"};
                cbBits.Items.AddRange(Databits);
                //Cho Parity
                string[] Parity = {"None", "Odd", "Even"};
                cbParity.Items.AddRange(Parity);
                //Cho Stop bit
                string[] stopbit = {"1", "1.5", "2"};
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
            InputData = P.ReadExisting();
            if (InputData != String.Empty)
            {
                /* txtIn.Text = InputData; // Ko dùng đc như thế này vì khác
                threads. */
                SetText(InputData); /* Chính vì vậy phải sử dụng ủy quyền tại
            đây.Gọi delegate đã khai báo trước đó. */
            }
        }

        //Hàm của em nó là ở đây.Đừng hỏi vì sao lại thế.
        private void SetText(string text)
        {

        }

        private void cbCom_SelectedIndexChanged(object sender, EventArgs e)
            {
                if (P.IsOpen)
                {
                    // Nếu đang mở Port thì phải đóng lại. Vì không thể đang chạy mà thay đổi Port được
                    P.Close();
                }
                P.PortName = cbCom.SelectedItem.ToString(); // Assign PortName by selected COM port!
                try
                {
                    P.Open();
                }
                catch (Exception exception)
                {
                    Console.Write(exception);
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
        private bool spcheck,pocheck;
        #endregion

        //Take the control string send to STM32F4. Plot the graph
        public void COMhandler(string data)
        {
            try
            {
                double fbspeed, fbposition;
                bool checkfbspeed, checkfbposition;
                checkfbspeed = Double.TryParse(data.Substring(0, 7), out fbspeed);
                checkfbposition = Double.TryParse(data.Substring(7, 5), out fbposition);
                if (checkfbspeed)
                {
                    //Hien thi len sptext. Dong thoi ve do thi
                    txtsp.Text = Convert.ToString(fbspeed);
                   
                }
                if (checkfbposition )
                {
                    //Hien thi len sptext. Dong thoi ve do thi
                    txtpo.Text = Convert.ToString(fbposition);
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public void draw(double ypoint) 
        {
            zedGraphControl1.GraphPane.CurveList.Clear();
            // Time được tính bằng ms
            double time = (Environment.TickCount - tickStart) / 1000.0;
            //add point to Zed Graph
            listPointsOne.Add(time, ypoint);
            myCurveOne = myPane.AddCurve(null, listPointsOne, Color.Red, SymbolType.Default);
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
        }
        private void btcleargraph_Click(object sender, EventArgs e)
        {
            tickStart = 0;
            listPointsOne.Clear();
            zedGraphControl1.GraphPane.CurveList.Clear();
            zedGraphControl1.Invalidate();
            zedGraphControl1.Refresh();
        }
        public string sendstring;
        private void txtsp_TextChanged(object sender, EventArgs e)
        {

        }
        private void timercheck_Tick(object sender, EventArgs e)
        {
            if (checkki && checkkp && checkkd && (spcheck || pocheck))
            {
                btsendcontrol.Enabled = true;
            }
            else
            {
                btsendcontrol.Enabled = false;
            }
        }
        private void txtsetspeed_TextChanged(object sender, EventArgs e)
        {
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void txtsetposition_TextChanged(object sender, EventArgs e)
        {
            try
            {
                POS = Convert.ToDouble(txtsetposition.Text);
                if (POS >= 0 && SP <= 360)
                {
                    POS = Math.Round(POS, 3);
                    pocheck = true;
                }
                else
                {
                    pocheck = false;
                }
            }
            catch (Exception exception)
            {
                pocheck = false;
                Console.WriteLine(exception);
                //throw;

            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btsendcontrol_Click(object sender, EventArgs e)
        {
            //Xu li tin hieu trong Text Box. Kp Ki Kd
   
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
    }
}