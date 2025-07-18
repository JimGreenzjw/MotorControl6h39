﻿using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO.Ports;

namespace MotorControl6h39
{
    partial class MotorControl
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btdisc = new System.Windows.Forms.Button();
            this.btconnect = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.cbBit = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbParity = new System.Windows.Forms.ComboBox();
            this.cbBits = new System.Windows.Forms.ComboBox();
            this.cbRate = new System.Windows.Forms.ComboBox();
            this.cbCom = new System.Windows.Forms.ComboBox();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label10 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox21 = new System.Windows.Forms.TextBox();
            this.textBox20 = new System.Windows.Forms.TextBox();
            this.textBox19 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.button9 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.label30 = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.label31 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.graphPanel = new System.Windows.Forms.Panel();
            this.zedGraphControl1 = new ZedGraph.ZedGraphControl();
            this.leftControlPanel = new System.Windows.Forms.Panel();
            this.button12 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label14 = new System.Windows.Forms.Label();
            this.textBox15 = new System.Windows.Forms.TextBox();
            this.button5 = new System.Windows.Forms.Button();
            this.label26 = new System.Windows.Forms.Label();
            this.textBox16 = new System.Windows.Forms.TextBox();
            this.textBox13 = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.textBox14 = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.label32 = new System.Windows.Forms.Label();
            this.textBox18 = new System.Windows.Forms.TextBox();
            this.label33 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.button6 = new System.Windows.Forms.Button();
            this.textBox17 = new System.Windows.Forms.TextBox();
            this.label29 = new System.Windows.Forms.Label();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timercheck = new System.Windows.Forms.Timer(this.components);
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label25 = new System.Windows.Forms.Label();
            this.textBox12 = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.textBox11 = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.textBox9 = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.zedGraphControlBode = new ZedGraph.ZedGraphControl();
            this.bodeGraphPanel = new System.Windows.Forms.Panel();
            this.bodeControlPanel = new System.Windows.Forms.Panel();
            this.buttonGenerateBode = new System.Windows.Forms.Button();
            this.labelBodeInfo = new System.Windows.Forms.Label();
            this.richTextBoxBode = new System.Windows.Forms.RichTextBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.graphPanel.SuspendLayout();
            this.leftControlPanel.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.bodeGraphPanel.SuspendLayout();
            this.bodeControlPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // btdisc
            // 
            this.btdisc.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(72)))), ((int)(((byte)(65)))));
            this.btdisc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btdisc.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btdisc.ForeColor = System.Drawing.Color.White;
            this.btdisc.Location = new System.Drawing.Point(282, 426);
            this.btdisc.Margin = new System.Windows.Forms.Padding(4);
            this.btdisc.Name = "btdisc";
            this.btdisc.Size = new System.Drawing.Size(160, 68);
            this.btdisc.TabIndex = 30;
            this.btdisc.Text = "断开连接";
            this.btdisc.UseVisualStyleBackColor = false;
            this.btdisc.Click += new System.EventHandler(this.btdisc_Click);
            // 
            // btconnect
            // 
            this.btconnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(177)))), ((int)(((byte)(89)))));
            this.btconnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btconnect.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btconnect.ForeColor = System.Drawing.Color.White;
            this.btconnect.Location = new System.Drawing.Point(51, 426);
            this.btconnect.Margin = new System.Windows.Forms.Padding(4);
            this.btconnect.Name = "btconnect";
            this.btconnect.Size = new System.Drawing.Size(194, 68);
            this.btconnect.TabIndex = 27;
            this.btconnect.Text = "连接";
            this.btconnect.UseVisualStyleBackColor = false;
            this.btconnect.Click += new System.EventHandler(this.btconnect_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.label5.Location = new System.Drawing.Point(56, 285);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(139, 51);
            this.label5.TabIndex = 26;
            this.label5.Text = "停止位";
            // 
            // cbBit
            // 
            this.cbBit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbBit.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.cbBit.FormattingEnabled = true;
            this.cbBit.Location = new System.Drawing.Point(202, 282);
            this.cbBit.Margin = new System.Windows.Forms.Padding(4);
            this.cbBit.Name = "cbBit";
            this.cbBit.Size = new System.Drawing.Size(240, 58);
            this.cbBit.TabIndex = 25;
            this.cbBit.SelectedIndexChanged += new System.EventHandler(this.cbBit_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.label4.Location = new System.Drawing.Point(56, 225);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 51);
            this.label4.TabIndex = 24;
            this.label4.Text = "校验";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.label3.Location = new System.Drawing.Point(56, 166);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(139, 51);
            this.label3.TabIndex = 23;
            this.label3.Text = "数据位";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.label2.Location = new System.Drawing.Point(56, 110);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(139, 51);
            this.label2.TabIndex = 22;
            this.label2.Text = "波特率";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.label1.Location = new System.Drawing.Point(66, 50);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 51);
            this.label1.TabIndex = 21;
            this.label1.Text = "端口";
            // 
            // cbParity
            // 
            this.cbParity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbParity.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbParity.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.cbParity.FormattingEnabled = true;
            this.cbParity.Location = new System.Drawing.Point(202, 218);
            this.cbParity.Margin = new System.Windows.Forms.Padding(4);
            this.cbParity.Name = "cbParity";
            this.cbParity.Size = new System.Drawing.Size(240, 58);
            this.cbParity.TabIndex = 19;
            this.cbParity.SelectedIndexChanged += new System.EventHandler(this.cbParity_SelectedIndexChanged);
            // 
            // cbBits
            // 
            this.cbBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBits.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbBits.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.cbBits.FormattingEnabled = true;
            this.cbBits.Location = new System.Drawing.Point(202, 164);
            this.cbBits.Margin = new System.Windows.Forms.Padding(4);
            this.cbBits.Name = "cbBits";
            this.cbBits.Size = new System.Drawing.Size(240, 58);
            this.cbBits.TabIndex = 18;
            this.cbBits.SelectedIndexChanged += new System.EventHandler(this.cbBits_SelectedIndexChanged);
            // 
            // cbRate
            // 
            this.cbRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbRate.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.cbRate.FormattingEnabled = true;
            this.cbRate.Location = new System.Drawing.Point(202, 100);
            this.cbRate.Margin = new System.Windows.Forms.Padding(4);
            this.cbRate.Name = "cbRate";
            this.cbRate.Size = new System.Drawing.Size(240, 58);
            this.cbRate.TabIndex = 17;
            this.cbRate.SelectedIndexChanged += new System.EventHandler(this.cbRate_SelectedIndexChanged);
            // 
            // cbCom
            // 
            this.cbCom.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbCom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCom.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbCom.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.cbCom.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.cbCom.FormattingEnabled = true;
            this.cbCom.Location = new System.Drawing.Point(202, 42);
            this.cbCom.Margin = new System.Windows.Forms.Padding(4);
            this.cbCom.Name = "cbCom";
            this.cbCom.Size = new System.Drawing.Size(240, 58);
            this.cbCom.TabIndex = 20;
            this.cbCom.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cbCom_DrawItem);
            this.cbCom.DropDown += new System.EventHandler(this.cbCom_DropDown);
            // 
            // serialPort1
            // 
            this.serialPort1.ReadTimeout = 1000;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold);
            this.tabControl1.ItemSize = new System.Drawing.Size(240, 50);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.Padding = new System.Drawing.Point(10, 3);
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(2826, 1532);
            this.tabControl1.TabIndex = 35;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.White;
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.richTextBox1);
            this.tabPage1.Controls.Add(this.groupBox4);
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.groupBox8);
            this.tabPage1.Controls.Add(this.groupBox7);
            this.tabPage1.Controls.Add(this.groupBox6);
            this.tabPage1.Location = new System.Drawing.Point(8, 58);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage1.Size = new System.Drawing.Size(2810, 1466);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "连接设置";
            this.tabPage1.Click += new System.EventHandler(this.tabPage1_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label10.Location = new System.Drawing.Point(1210, 560);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(182, 51);
            this.label10.TabIndex = 58;
            this.label10.Text = "串口输出";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(1208, 617);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(2);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(408, 282);
            this.richTextBox1.TabIndex = 57;
            this.richTextBox1.Text = "";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.textBox5);
            this.groupBox4.Controls.Add(this.button3);
            this.groupBox4.Controls.Add(this.textBox6);
            this.groupBox4.Controls.Add(this.label18);
            this.groupBox4.Controls.Add(this.label19);
            this.groupBox4.Controls.Add(this.label20);
            this.groupBox4.Controls.Add(this.textBox7);
            this.groupBox4.Location = new System.Drawing.Point(750, 566);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(430, 340);
            this.groupBox4.TabIndex = 55;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "电流输出";
            // 
            // textBox5
            // 
            this.textBox5.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox5.Location = new System.Drawing.Point(232, 108);
            this.textBox5.Margin = new System.Windows.Forms.Padding(4);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(178, 53);
            this.textBox5.TabIndex = 48;
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.button3.ForeColor = System.Drawing.Color.White;
            this.button3.Location = new System.Drawing.Point(52, 248);
            this.button3.Margin = new System.Windows.Forms.Padding(4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(172, 68);
            this.button3.TabIndex = 48;
            this.button3.Text = "设置";
            this.button3.UseVisualStyleBackColor = false;
            // 
            // textBox6
            // 
            this.textBox6.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox6.Location = new System.Drawing.Point(232, 160);
            this.textBox6.Margin = new System.Windows.Forms.Padding(4);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(178, 53);
            this.textBox6.TabIndex = 49;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label18.Location = new System.Drawing.Point(28, 50);
            this.label18.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(182, 51);
            this.label18.TabIndex = 49;
            this.label18.Text = "最大电流";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label19.Location = new System.Drawing.Point(28, 110);
            this.label19.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(182, 51);
            this.label19.TabIndex = 50;
            this.label19.Text = "最小电流";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label20.Location = new System.Drawing.Point(28, 162);
            this.label20.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(262, 51);
            this.label20.TabIndex = 51;
            this.label20.Text = "输出转换系数";
            // 
            // textBox7
            // 
            this.textBox7.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox7.Location = new System.Drawing.Point(232, 50);
            this.textBox7.Margin = new System.Windows.Forms.Padding(4);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(178, 53);
            this.textBox7.TabIndex = 48;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBox3);
            this.groupBox3.Controls.Add(this.button2);
            this.groupBox3.Controls.Add(this.textBox4);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label17);
            this.groupBox3.Controls.Add(this.textBox2);
            this.groupBox3.Location = new System.Drawing.Point(352, 566);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(344, 340);
            this.groupBox3.TabIndex = 54;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "速度环";
            // 
            // textBox3
            // 
            this.textBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox3.Location = new System.Drawing.Point(88, 108);
            this.textBox3.Margin = new System.Windows.Forms.Padding(4);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(228, 53);
            this.textBox3.TabIndex = 48;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Location = new System.Drawing.Point(52, 248);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(172, 68);
            this.button2.TabIndex = 48;
            this.button2.Text = "设置";
            this.button2.UseVisualStyleBackColor = false;
            // 
            // textBox4
            // 
            this.textBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox4.Location = new System.Drawing.Point(88, 164);
            this.textBox4.Margin = new System.Windows.Forms.Padding(4);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(228, 53);
            this.textBox4.TabIndex = 49;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label6.Location = new System.Drawing.Point(28, 50);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 51);
            this.label6.TabIndex = 49;
            this.label6.Text = "Kp";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label11.Location = new System.Drawing.Point(28, 110);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(58, 51);
            this.label11.TabIndex = 50;
            this.label11.Text = "Ki";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label17.Location = new System.Drawing.Point(28, 164);
            this.label17.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(71, 51);
            this.label17.TabIndex = 51;
            this.label17.Text = "Kd";
            // 
            // textBox2
            // 
            this.textBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.Location = new System.Drawing.Point(88, 50);
            this.textBox2.Margin = new System.Windows.Forms.Padding(4);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(228, 53);
            this.textBox2.TabIndex = 48;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox21);
            this.groupBox1.Controls.Add(this.textBox20);
            this.groupBox1.Controls.Add(this.textBox19);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Location = new System.Drawing.Point(9, 566);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(312, 340);
            this.groupBox1.TabIndex = 53;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "位置环";
            // 
            // textBox21
            // 
            this.textBox21.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox21.Location = new System.Drawing.Point(76, 164);
            this.textBox21.Margin = new System.Windows.Forms.Padding(4);
            this.textBox21.Name = "textBox21";
            this.textBox21.Size = new System.Drawing.Size(228, 53);
            this.textBox21.TabIndex = 51;
            // 
            // textBox20
            // 
            this.textBox20.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox20.Location = new System.Drawing.Point(76, 111);
            this.textBox20.Margin = new System.Windows.Forms.Padding(4);
            this.textBox20.Name = "textBox20";
            this.textBox20.Size = new System.Drawing.Size(228, 53);
            this.textBox20.TabIndex = 50;
            // 
            // textBox19
            // 
            this.textBox19.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox19.Location = new System.Drawing.Point(76, 58);
            this.textBox19.Margin = new System.Windows.Forms.Padding(4);
            this.textBox19.Name = "textBox19";
            this.textBox19.Size = new System.Drawing.Size(228, 53);
            this.textBox19.TabIndex = 49;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(14, 248);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(172, 68);
            this.button1.TabIndex = 47;
            this.button1.Text = "设置";
            this.button1.UseVisualStyleBackColor = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label9.Location = new System.Drawing.Point(8, 50);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(71, 51);
            this.label9.TabIndex = 28;
            this.label9.Text = "Kp";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label7.Location = new System.Drawing.Point(8, 111);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 51);
            this.label7.TabIndex = 29;
            this.label7.Text = "Ki";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label8.Location = new System.Drawing.Point(8, 160);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(71, 51);
            this.label8.TabIndex = 30;
            this.label8.Text = "Kd";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.comboBox3);
            this.groupBox8.Controls.Add(this.button9);
            this.groupBox8.Controls.Add(this.button10);
            this.groupBox8.Controls.Add(this.label30);
            this.groupBox8.Location = new System.Drawing.Point(1090, 32);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(524, 494);
            this.groupBox8.TabIndex = 33;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "网络";
            // 
            // comboBox3
            // 
            this.comboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBox3.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Location = new System.Drawing.Point(184, 68);
            this.comboBox3.Margin = new System.Windows.Forms.Padding(4);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(272, 58);
            this.comboBox3.TabIndex = 17;
            // 
            // button9
            // 
            this.button9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(72)))), ((int)(((byte)(65)))));
            this.button9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button9.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.button9.ForeColor = System.Drawing.Color.White;
            this.button9.Location = new System.Drawing.Point(279, 402);
            this.button9.Margin = new System.Windows.Forms.Padding(4);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(160, 68);
            this.button9.TabIndex = 30;
            this.button9.Text = "断开连接";
            this.button9.UseVisualStyleBackColor = false;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // button10
            // 
            this.button10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(177)))), ((int)(((byte)(89)))));
            this.button10.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button10.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.button10.ForeColor = System.Drawing.Color.White;
            this.button10.Location = new System.Drawing.Point(51, 402);
            this.button10.Margin = new System.Windows.Forms.Padding(4);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(194, 68);
            this.button10.TabIndex = 27;
            this.button10.Text = "连接";
            this.button10.UseVisualStyleBackColor = false;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.label30.Location = new System.Drawing.Point(44, 70);
            this.label30.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(100, 51);
            this.label30.TabIndex = 22;
            this.label30.Text = "网络";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.comboBox2);
            this.groupBox7.Controls.Add(this.button7);
            this.groupBox7.Controls.Add(this.button8);
            this.groupBox7.Controls.Add(this.label31);
            this.groupBox7.Location = new System.Drawing.Point(544, 21);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(502, 505);
            this.groupBox7.TabIndex = 32;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "CAN";
            // 
            // comboBox2
            // 
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBox2.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(184, 68);
            this.comboBox2.Margin = new System.Windows.Forms.Padding(4);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(240, 58);
            this.comboBox2.TabIndex = 17;
            // 
            // button7
            // 
            this.button7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(72)))), ((int)(((byte)(65)))));
            this.button7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button7.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.button7.ForeColor = System.Drawing.Color.White;
            this.button7.Location = new System.Drawing.Point(287, 413);
            this.button7.Margin = new System.Windows.Forms.Padding(4);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(160, 68);
            this.button7.TabIndex = 30;
            this.button7.Text = "断开连接";
            this.button7.UseVisualStyleBackColor = false;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button8
            // 
            this.button8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(177)))), ((int)(((byte)(89)))));
            this.button8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button8.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.button8.ForeColor = System.Drawing.Color.White;
            this.button8.Location = new System.Drawing.Point(51, 413);
            this.button8.Margin = new System.Windows.Forms.Padding(4);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(194, 68);
            this.button8.TabIndex = 27;
            this.button8.Text = "连接";
            this.button8.UseVisualStyleBackColor = false;
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.label31.Location = new System.Drawing.Point(44, 70);
            this.label31.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(139, 51);
            this.label31.TabIndex = 22;
            this.label31.Text = "波特率";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.checkBox2);
            this.groupBox6.Controls.Add(this.cbRate);
            this.groupBox6.Controls.Add(this.cbCom);
            this.groupBox6.Controls.Add(this.cbBits);
            this.groupBox6.Controls.Add(this.btdisc);
            this.groupBox6.Controls.Add(this.cbParity);
            this.groupBox6.Controls.Add(this.cbBit);
            this.groupBox6.Controls.Add(this.label1);
            this.groupBox6.Controls.Add(this.btconnect);
            this.groupBox6.Controls.Add(this.label2);
            this.groupBox6.Controls.Add(this.label5);
            this.groupBox6.Controls.Add(this.label3);
            this.groupBox6.Controls.Add(this.label4);
            this.groupBox6.Location = new System.Drawing.Point(9, 8);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(502, 518);
            this.groupBox6.TabIndex = 31;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "串口";
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(283, 356);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(210, 55);
            this.checkBox2.TabIndex = 33;
            this.checkBox2.Text = "调试模式";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.graphPanel);
            this.tabPage2.Controls.Add(this.leftControlPanel);
            this.tabPage2.Location = new System.Drawing.Point(8, 58);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(10);
            this.tabPage2.Size = new System.Drawing.Size(2810, 1466);
            this.tabPage2.TabIndex = 2;
            this.tabPage2.Text = "追焦测试";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // graphPanel
            // 
            this.graphPanel.Controls.Add(this.zedGraphControl1);
            this.graphPanel.Location = new System.Drawing.Point(490, 10);
            this.graphPanel.Name = "graphPanel";
            this.graphPanel.Padding = new System.Windows.Forms.Padding(10);
            this.graphPanel.Size = new System.Drawing.Size(2313, 1278);
            this.graphPanel.TabIndex = 0;
            // 
            // zedGraphControl1
            // 
            this.zedGraphControl1.AutoScroll = true;
            this.zedGraphControl1.BackColor = System.Drawing.Color.White;
            this.zedGraphControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.zedGraphControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold);
            this.zedGraphControl1.Location = new System.Drawing.Point(10, 10);
            this.zedGraphControl1.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
            this.zedGraphControl1.Name = "zedGraphControl1";
            this.zedGraphControl1.ScrollGrace = 0D;
            this.zedGraphControl1.ScrollMaxX = 0D;
            this.zedGraphControl1.ScrollMaxY = 0D;
            this.zedGraphControl1.ScrollMaxY2 = 0D;
            this.zedGraphControl1.ScrollMinX = 0D;
            this.zedGraphControl1.ScrollMinY = 0D;
            this.zedGraphControl1.ScrollMinY2 = 0D;
            this.zedGraphControl1.Size = new System.Drawing.Size(2293, 1252);
            this.zedGraphControl1.TabIndex = 1;
            // 
            // leftControlPanel
            // 
            this.leftControlPanel.Controls.Add(this.button12);
            this.leftControlPanel.Controls.Add(this.button11);
            this.leftControlPanel.Controls.Add(this.groupBox2);
            this.leftControlPanel.Controls.Add(this.groupBox9);
            this.leftControlPanel.Controls.Add(this.groupBox10);
            this.leftControlPanel.Location = new System.Drawing.Point(8, 10);
            this.leftControlPanel.Name = "leftControlPanel";
            this.leftControlPanel.Padding = new System.Windows.Forms.Padding(10);
            this.leftControlPanel.Size = new System.Drawing.Size(482, 1278);
            this.leftControlPanel.TabIndex = 1;
            // 
            // button12
            // 
            this.button12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(72)))), ((int)(((byte)(65)))));
            this.button12.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button12.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.button12.ForeColor = System.Drawing.Color.White;
            this.button12.Location = new System.Drawing.Point(243, 873);
            this.button12.Margin = new System.Windows.Forms.Padding(4);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(208, 50);
            this.button12.TabIndex = 45;
            this.button12.Text = "保存数据";
            this.button12.UseVisualStyleBackColor = false;
            this.button12.Click += new System.EventHandler(this.button12_Click);
            // 
            // button11
            // 
            this.button11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(177)))), ((int)(((byte)(89)))));
            this.button11.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button11.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.button11.ForeColor = System.Drawing.Color.White;
            this.button11.Location = new System.Drawing.Point(26, 874);
            this.button11.Margin = new System.Windows.Forms.Padding(4);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(208, 50);
            this.button11.TabIndex = 44;
            this.button11.Text = "开始记录";
            this.button11.UseVisualStyleBackColor = false;
            this.button11.Click += new System.EventHandler(this.button11_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.textBox15);
            this.groupBox2.Controls.Add(this.button5);
            this.groupBox2.Controls.Add(this.label26);
            this.groupBox2.Controls.Add(this.textBox16);
            this.groupBox2.Controls.Add(this.textBox13);
            this.groupBox2.Controls.Add(this.checkBox1);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.textBox14);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Location = new System.Drawing.Point(10, 220);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(454, 381);
            this.groupBox2.TabIndex = 38;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "扫频测试";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label14.Location = new System.Drawing.Point(9, 41);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(102, 51);
            this.label14.TabIndex = 42;
            this.label14.Text = "中心";
            // 
            // textBox15
            // 
            this.textBox15.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox15.Location = new System.Drawing.Point(153, 39);
            this.textBox15.Margin = new System.Windows.Forms.Padding(4);
            this.textBox15.Name = "textBox15";
            this.textBox15.Size = new System.Drawing.Size(228, 53);
            this.textBox15.TabIndex = 41;
            this.textBox15.Text = "-1";
            // 
            // button5
            // 
            this.button5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button5.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.button5.ForeColor = System.Drawing.Color.White;
            this.button5.Location = new System.Drawing.Point(302, 314);
            this.button5.Margin = new System.Windows.Forms.Padding(4);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(138, 50);
            this.button5.TabIndex = 39;
            this.button5.Text = "发送";
            this.button5.UseVisualStyleBackColor = false;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label26.Location = new System.Drawing.Point(8, 202);
            this.label26.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(182, 51);
            this.label26.TabIndex = 39;
            this.label26.Text = "结束频率";
            // 
            // textBox16
            // 
            this.textBox16.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox16.Location = new System.Drawing.Point(152, 201);
            this.textBox16.Margin = new System.Windows.Forms.Padding(4);
            this.textBox16.Name = "textBox16";
            this.textBox16.Size = new System.Drawing.Size(228, 53);
            this.textBox16.TabIndex = 40;
            this.textBox16.Text = "5";
            // 
            // textBox13
            // 
            this.textBox13.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox13.Location = new System.Drawing.Point(153, 91);
            this.textBox13.Margin = new System.Windows.Forms.Padding(4);
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new System.Drawing.Size(228, 53);
            this.textBox13.TabIndex = 33;
            this.textBox13.Text = "0.5";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(3, 258);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(561, 55);
            this.checkBox1.TabIndex = 32;
            this.checkBox1.Text = "是否在电流输出叠加正弦信号";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label13.Location = new System.Drawing.Point(8, 145);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(182, 51);
            this.label13.TabIndex = 35;
            this.label13.Text = "开始频率";
            // 
            // textBox14
            // 
            this.textBox14.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox14.Location = new System.Drawing.Point(153, 143);
            this.textBox14.Margin = new System.Windows.Forms.Padding(4);
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new System.Drawing.Size(228, 53);
            this.textBox14.TabIndex = 36;
            this.textBox14.Text = "0.1";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label12.Location = new System.Drawing.Point(8, 90);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(182, 51);
            this.label12.TabIndex = 34;
            this.label12.Text = "正弦幅值";
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.label32);
            this.groupBox9.Controls.Add(this.textBox18);
            this.groupBox9.Controls.Add(this.label33);
            this.groupBox9.Controls.Add(this.label28);
            this.groupBox9.Controls.Add(this.button6);
            this.groupBox9.Controls.Add(this.textBox17);
            this.groupBox9.Controls.Add(this.label29);
            this.groupBox9.Location = new System.Drawing.Point(4, 616);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(460, 243);
            this.groupBox9.TabIndex = 42;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "追焦测试";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label32.Location = new System.Drawing.Point(384, 117);
            this.label32.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label32.Name = "label32";
            this.label32.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label32.Size = new System.Drawing.Size(92, 51);
            this.label32.TabIndex = 44;
            this.label32.Text = "mm";
            // 
            // textBox18
            // 
            this.textBox18.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox18.Location = new System.Drawing.Point(150, 114);
            this.textBox18.Margin = new System.Windows.Forms.Padding(4);
            this.textBox18.Name = "textBox18";
            this.textBox18.Size = new System.Drawing.Size(228, 53);
            this.textBox18.TabIndex = 42;
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label33.Location = new System.Drawing.Point(8, 116);
            this.label33.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(182, 51);
            this.label33.TabIndex = 43;
            this.label33.Text = "最小距离";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label28.Location = new System.Drawing.Point(388, 58);
            this.label28.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label28.Name = "label28";
            this.label28.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label28.Size = new System.Drawing.Size(92, 51);
            this.label28.TabIndex = 41;
            this.label28.Text = "mm";
            // 
            // button6
            // 
            this.button6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.button6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button6.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.button6.ForeColor = System.Drawing.Color.White;
            this.button6.Location = new System.Drawing.Point(308, 186);
            this.button6.Margin = new System.Windows.Forms.Padding(4);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(138, 50);
            this.button6.TabIndex = 41;
            this.button6.Text = "发送";
            this.button6.UseVisualStyleBackColor = false;
            // 
            // textBox17
            // 
            this.textBox17.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox17.Location = new System.Drawing.Point(153, 58);
            this.textBox17.Margin = new System.Windows.Forms.Padding(4);
            this.textBox17.Name = "textBox17";
            this.textBox17.Size = new System.Drawing.Size(228, 53);
            this.textBox17.TabIndex = 39;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label29.Location = new System.Drawing.Point(8, 58);
            this.label29.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(182, 51);
            this.label29.TabIndex = 40;
            this.label29.Text = "最大距离";
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.textBox1);
            this.groupBox10.Controls.Add(this.label15);
            this.groupBox10.Controls.Add(this.label16);
            this.groupBox10.Controls.Add(this.button4);
            this.groupBox10.Location = new System.Drawing.Point(10, 14);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(459, 196);
            this.groupBox10.TabIndex = 43;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "电机位置控制";
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(153, 68);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(228, 53);
            this.textBox1.TabIndex = 29;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label15.Location = new System.Drawing.Point(8, 68);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(182, 51);
            this.label15.TabIndex = 30;
            this.label15.Text = "电机位置";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label16.Location = new System.Drawing.Point(382, 68);
            this.label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label16.Name = "label16";
            this.label16.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label16.Size = new System.Drawing.Size(92, 51);
            this.label16.TabIndex = 31;
            this.label16.Text = "mm";
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.button4.ForeColor = System.Drawing.Color.White;
            this.button4.Location = new System.Drawing.Point(302, 129);
            this.button4.Margin = new System.Windows.Forms.Padding(4);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(138, 50);
            this.button4.TabIndex = 37;
            this.button4.Text = "发送";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 10;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timercheck
            // 
            this.timercheck.Enabled = true;
            this.timercheck.Interval = 20;
            this.timercheck.Tick += new System.EventHandler(this.timercheck_Tick);
            // 
            // groupBox5
            // 
            this.groupBox5.BackColor = System.Drawing.Color.White;
            this.groupBox5.Controls.Add(this.label25);
            this.groupBox5.Controls.Add(this.textBox12);
            this.groupBox5.Controls.Add(this.label24);
            this.groupBox5.Controls.Add(this.textBox11);
            this.groupBox5.Controls.Add(this.label23);
            this.groupBox5.Controls.Add(this.textBox10);
            this.groupBox5.Controls.Add(this.label22);
            this.groupBox5.Controls.Add(this.textBox9);
            this.groupBox5.Controls.Add(this.label21);
            this.groupBox5.Controls.Add(this.textBox8);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox5.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold);
            this.groupBox5.Location = new System.Drawing.Point(0, 1392);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(10);
            this.groupBox5.Size = new System.Drawing.Size(2826, 140);
            this.groupBox5.TabIndex = 36;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "输出";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label25.Location = new System.Drawing.Point(1190, 64);
            this.label25.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(142, 51);
            this.label25.TabIndex = 57;
            this.label25.Text = "光强度";
            // 
            // textBox12
            // 
            this.textBox12.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox12.Location = new System.Drawing.Point(1298, 63);
            this.textBox12.Margin = new System.Windows.Forms.Padding(4);
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new System.Drawing.Size(118, 53);
            this.textBox12.TabIndex = 56;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label24.Location = new System.Drawing.Point(909, 66);
            this.label24.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(182, 51);
            this.label24.TabIndex = 55;
            this.label24.Text = "位置指令";
            // 
            // textBox11
            // 
            this.textBox11.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox11.Location = new System.Drawing.Point(1054, 64);
            this.textBox11.Margin = new System.Windows.Forms.Padding(4);
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new System.Drawing.Size(118, 53);
            this.textBox11.TabIndex = 54;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label23.Location = new System.Drawing.Point(606, 68);
            this.label23.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(222, 51);
            this.label23.TabIndex = 53;
            this.label23.Text = "编码器位置";
            // 
            // textBox10
            // 
            this.textBox10.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox10.Location = new System.Drawing.Point(780, 64);
            this.textBox10.Margin = new System.Windows.Forms.Padding(4);
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new System.Drawing.Size(118, 53);
            this.textBox10.TabIndex = 52;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label22.Location = new System.Drawing.Point(334, 64);
            this.label22.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(182, 51);
            this.label22.TabIndex = 51;
            this.label22.Text = "电机位置";
            // 
            // textBox9
            // 
            this.textBox9.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox9.Location = new System.Drawing.Point(480, 64);
            this.textBox9.Margin = new System.Windows.Forms.Padding(4);
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new System.Drawing.Size(118, 53);
            this.textBox9.TabIndex = 50;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.label21.Location = new System.Drawing.Point(26, 62);
            this.label21.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(222, 51);
            this.label21.TabIndex = 49;
            this.label21.Text = "电流环输出";
            // 
            // textBox8
            // 
            this.textBox8.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox8.Location = new System.Drawing.Point(200, 62);
            this.textBox8.Margin = new System.Windows.Forms.Padding(4);
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new System.Drawing.Size(118, 53);
            this.textBox8.TabIndex = 48;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.bodeGraphPanel);
            this.tabPage3.Controls.Add(this.bodeControlPanel);
            this.tabPage3.Location = new System.Drawing.Point(8, 58);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(10);
            this.tabPage3.Size = new System.Drawing.Size(2810, 1466);
            this.tabPage3.TabIndex = 3;
            this.tabPage3.Text = "伯德图";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // zedGraphControlBode
            // 
            this.zedGraphControlBode.AutoScroll = true;
            this.zedGraphControlBode.BackColor = System.Drawing.Color.White;
            this.zedGraphControlBode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.zedGraphControlBode.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold);
            this.zedGraphControlBode.Location = new System.Drawing.Point(10, 10);
            this.zedGraphControlBode.Margin = new System.Windows.Forms.Padding(6, 4, 6, 4);
            this.zedGraphControlBode.Name = "zedGraphControlBode";
            this.zedGraphControlBode.ScrollGrace = 0D;
            this.zedGraphControlBode.ScrollMaxX = 0D;
            this.zedGraphControlBode.ScrollMaxY = 0D;
            this.zedGraphControlBode.ScrollMaxY2 = 0D;
            this.zedGraphControlBode.ScrollMinX = 0D;
            this.zedGraphControlBode.ScrollMinY = 0D;
            this.zedGraphControlBode.ScrollMinY2 = 0D;
            this.zedGraphControlBode.Size = new System.Drawing.Size(2293, 1258);
            this.zedGraphControlBode.TabIndex = 1;
            // 
            // bodeGraphPanel
            // 
            this.bodeGraphPanel.Controls.Add(this.zedGraphControlBode);
            this.bodeGraphPanel.Location = new System.Drawing.Point(490, 10);
            this.bodeGraphPanel.Name = "bodeGraphPanel";
            this.bodeGraphPanel.Padding = new System.Windows.Forms.Padding(10);
            this.bodeGraphPanel.Size = new System.Drawing.Size(2313, 1278);
            this.bodeGraphPanel.TabIndex = 0;
            // 
            // bodeControlPanel
            // 
            this.bodeControlPanel.Controls.Add(this.buttonGenerateBode);
            this.bodeControlPanel.Controls.Add(this.labelBodeInfo);
            this.bodeControlPanel.Controls.Add(this.richTextBoxBode);
            this.bodeControlPanel.Location = new System.Drawing.Point(8, 10);
            this.bodeControlPanel.Name = "bodeControlPanel";
            this.bodeControlPanel.Padding = new System.Windows.Forms.Padding(10);
            this.bodeControlPanel.Size = new System.Drawing.Size(482, 1278);
            this.bodeControlPanel.TabIndex = 1;
            // 
            // buttonGenerateBode
            // 
            this.buttonGenerateBode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(177)))), ((int)(((byte)(89)))));
            this.buttonGenerateBode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonGenerateBode.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.buttonGenerateBode.ForeColor = System.Drawing.Color.White;
            this.buttonGenerateBode.Location = new System.Drawing.Point(26, 20);
            this.buttonGenerateBode.Margin = new System.Windows.Forms.Padding(4);
            this.buttonGenerateBode.Name = "buttonGenerateBode";
            this.buttonGenerateBode.Size = new System.Drawing.Size(194, 50);
            this.buttonGenerateBode.TabIndex = 44;
            this.buttonGenerateBode.Text = "生成伯德图";
            this.buttonGenerateBode.UseVisualStyleBackColor = false;
            this.buttonGenerateBode.Click += new System.EventHandler(this.buttonGenerateBode_Click);
            // 
            // labelBodeInfo
            // 
            this.labelBodeInfo.AutoSize = true;
            this.labelBodeInfo.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.labelBodeInfo.Location = new System.Drawing.Point(26, 90);
            this.labelBodeInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelBodeInfo.Name = "labelBodeInfo";
            this.labelBodeInfo.Size = new System.Drawing.Size(182, 51);
            this.labelBodeInfo.TabIndex = 43;
            this.labelBodeInfo.Text = "伯德图信息";
            // 
            // richTextBoxBode
            // 
            this.richTextBoxBode.Location = new System.Drawing.Point(26, 150);
            this.richTextBoxBode.Margin = new System.Windows.Forms.Padding(2);
            this.richTextBoxBode.Name = "richTextBoxBode";
            this.richTextBoxBode.Size = new System.Drawing.Size(430, 300);
            this.richTextBoxBode.TabIndex = 57;
            this.richTextBoxBode.Text = "";
            // 
            // MotorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(2826, 1532);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.tabControl1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(770, 581);
            this.Name = "MotorControl";
            this.Text = "追焦系统";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.graphPanel.ResumeLayout(false);
            this.leftControlPanel.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.bodeGraphPanel.ResumeLayout(false);
            this.bodeControlPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void cbCom_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            ComboBox combo = (ComboBox)sender;
            string text = combo.Items[e.Index].ToString();

            // Set background color
            e.DrawBackground();
            using (SolidBrush brush = new SolidBrush(e.ForeColor))
            {
                e.Graphics.DrawString(text, e.Font, brush, e.Bounds);
            }

            // Draw border if selected
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                e.Graphics.DrawRectangle(Pens.DarkSlateGray, e.Bounds);
            }

            e.DrawFocusRectangle();
        }


        private void InitializeButtonStyle(Button btn, Color backColor)
        {
            btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = backColor;
            btn.ForeColor = System.Drawing.Color.White;
            btn.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            btn.Cursor = System.Windows.Forms.Cursors.Hand;
            
            btn.MouseEnter += (s, e) => {
                btn.BackColor = ControlPaint.Light(backColor);
            };
            btn.MouseLeave += (s, e) => {
                btn.BackColor = backColor;
            };
        }

        private void InitializeComboBoxStyle(ComboBox cb)
        {
            cb.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            cb.Font = new System.Drawing.Font("Segoe UI", 14F);
            cb.BackColor = System.Drawing.Color.White;
            cb.ForeColor = System.Drawing.Color.DarkSlateGray;
        }

        private void InitializeTextBoxStyle(TextBox tb)
        {
            tb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            tb.Font = new System.Drawing.Font("Segoe UI", 14F);
            tb.BackColor = System.Drawing.Color.White;
            tb.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
        }

        private void InitializeGroupBoxStyle(GroupBox gb)
        {
            gb.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold);
            gb.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            gb.BackColor = System.Drawing.Color.White;
        }

        #endregion
        private System.Windows.Forms.Button btdisc;
        private System.Windows.Forms.Button btconnect;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbBit;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbParity;
        private System.Windows.Forms.ComboBox cbBits;
        private System.Windows.Forms.ComboBox cbRate;
        private System.Windows.Forms.ComboBox cbCom;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timercheck;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private ZedGraph.ZedGraphControl zedGraphControlBode;
        private System.Windows.Forms.Panel bodeGraphPanel;
        private System.Windows.Forms.Panel bodeControlPanel;
        private System.Windows.Forms.Button buttonGenerateBode;
        private System.Windows.Forms.Label labelBodeInfo;
        private System.Windows.Forms.RichTextBox richTextBoxBode;
        private GroupBox groupBox5;
        private Label label25;
        private TextBox textBox12;
        private Label label24;
        private TextBox textBox11;
        private Label label23;
        private TextBox textBox10;
        private Label label22;
        private TextBox textBox9;
        private Label label21;
        private TextBox textBox8;
        private GroupBox groupBox4;
        private TextBox textBox5;
        private Button button3;
        private TextBox textBox6;
        private Label label18;
        private Label label19;
        private Label label20;
        private TextBox textBox7;
        private GroupBox groupBox3;
        private TextBox textBox3;
        private Button button2;
        private TextBox textBox4;
        private Label label6;
        private Label label11;
        private Label label17;
        private TextBox textBox2;
        private GroupBox groupBox1;
        private Button button1;
        private Label label9;
        private Label label7;
        private Label label8;
        private TextBox textBox21;
        private TextBox textBox20;
        private TextBox textBox19;
        private RichTextBox richTextBox1;
        private Label label10;
        private Button button12;
        private Button button11;
        private CheckBox checkBox2;
        private TextBox textBox15;
        private Label label14;
        private GroupBox groupBox9;
        private Label label32;
        private TextBox textBox18;
        private Label label33;
        private Label label28;
        private Button button6;
        private TextBox textBox17;
        private Label label29;
        private GroupBox groupBox10;
        private TextBox textBox1;
        private Label label15;
        private Label label16;
        private Button button4;
        private CheckBox checkBox1;
        private TextBox textBox14;
        private TextBox textBox13;
        private Label label13;
        private Label label12;
        private GroupBox groupBox2;
        private Button button5;
        private Label label26;
        private TextBox textBox16;
        private GroupBox groupBox6;
        private GroupBox groupBox8;
        private ComboBox comboBox3;
        private Button button9;
        private Button button10;
        private Label label30;
        private GroupBox groupBox7;
        private ComboBox comboBox2;
        private Button button7;
        private Button button8;
        private Label label31;
        private System.Windows.Forms.Panel leftControlPanel;
        private System.Windows.Forms.Panel graphPanel;
        private ZedGraph.ZedGraphControl zedGraphControl1;
    }
}
