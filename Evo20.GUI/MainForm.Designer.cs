namespace Evo20.GUI
{
    partial class MainForm
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.Evo_control_page = new System.Windows.Forms.TabPage();
            this.RemotePortTextBox = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.IPTextBox = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.EvoParamsGroupBox = new System.Windows.Forms.GroupBox();
            this.temperatureStabLabel = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.connectionStateLabel = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.nextTemperatureLable = new System.Windows.Forms.Label();
            this.ySpeedOfRateLabel = new System.Windows.Forms.Label();
            this.CurrentTemperatureLabel = new System.Windows.Forms.Label();
            this.identifieGoupBox = new System.Windows.Forms.GroupBox();
            this.temperatureReachedIndic = new System.Windows.Forms.PictureBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.powerCameraIndic = new System.Windows.Forms.PictureBox();
            this.yMoveIndic = new System.Windows.Forms.PictureBox();
            this.xMoveIndic = new System.Windows.Forms.PictureBox();
            this.yPowerIndic = new System.Windows.Forms.PictureBox();
            this.xPowerIndic = new System.Windows.Forms.PictureBox();
            this.yZeroFindedIndic = new System.Windows.Forms.PictureBox();
            this.xZeroFindedIndic = new System.Windows.Forms.PictureBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.xSpeedOfRateLabel = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.yPositionLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.xPositionLabel = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.graph = new ZedGraph.ZedGraphControl();
            this.evoStopButton = new System.Windows.Forms.Button();
            this.evoStartButton = new System.Windows.Forms.Button();
            this.evoPauseButton = new System.Windows.Forms.Button();
            this.Sensor_control_page = new System.Windows.Forms.TabPage();
            this.sensorGroupBox = new System.Windows.Forms.GroupBox();
            this.currentPositionNumberLbl = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.sensorConnectionStateLabel = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.SensorDataGridView = new System.Windows.Forms.DataGridView();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.xColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.yColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.zColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sensorTypeLabel = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.packetsArrivedLbl = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.comPortComboBox = new System.Windows.Forms.ComboBox();
            this.sensorStopButton = new System.Windows.Forms.Button();
            this.sensorPauseButton = new System.Windows.Forms.Button();
            this.sensorStartButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.modeComboBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.parameGroupBox = new System.Windows.Forms.GroupBox();
            this.controllerWokModeLabel = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.countTemperaturesReachedLabel = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.timeLeftlabel = new System.Windows.Forms.Label();
            this.workTimer = new System.Windows.Forms.Timer(this.components);
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.stopButton = new System.Windows.Forms.Button();
            this.pauseButton = new System.Windows.Forms.Button();
            this.SensorTimer = new System.Windows.Forms.Timer(this.components);
            this.startButton = new System.Windows.Forms.Button();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.FileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.getDataFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cycleSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CommonSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lblVersion = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.Evo_control_page.SuspendLayout();
            this.EvoParamsGroupBox.SuspendLayout();
            this.identifieGoupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.temperatureReachedIndic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.powerCameraIndic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yMoveIndic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xMoveIndic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yPowerIndic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xPowerIndic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yZeroFindedIndic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xZeroFindedIndic)).BeginInit();
            this.Sensor_control_page.SuspendLayout();
            this.sensorGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SensorDataGridView)).BeginInit();
            this.parameGroupBox.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.Evo_control_page);
            this.tabControl1.Controls.Add(this.Sensor_control_page);
            this.tabControl1.Location = new System.Drawing.Point(14, 180);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(5);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1180, 565);
            this.tabControl1.TabIndex = 1;
            // 
            // Evo_control_page
            // 
            this.Evo_control_page.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.Evo_control_page.Controls.Add(this.RemotePortTextBox);
            this.Evo_control_page.Controls.Add(this.label26);
            this.Evo_control_page.Controls.Add(this.IPTextBox);
            this.Evo_control_page.Controls.Add(this.label24);
            this.Evo_control_page.Controls.Add(this.EvoParamsGroupBox);
            this.Evo_control_page.Controls.Add(this.graph);
            this.Evo_control_page.Controls.Add(this.evoStopButton);
            this.Evo_control_page.Controls.Add(this.evoStartButton);
            this.Evo_control_page.Controls.Add(this.evoPauseButton);
            this.Evo_control_page.ImeMode = System.Windows.Forms.ImeMode.Hiragana;
            this.Evo_control_page.Location = new System.Drawing.Point(4, 22);
            this.Evo_control_page.Name = "Evo_control_page";
            this.Evo_control_page.Padding = new System.Windows.Forms.Padding(3);
            this.Evo_control_page.Size = new System.Drawing.Size(1172, 539);
            this.Evo_control_page.TabIndex = 0;
            this.Evo_control_page.Text = "Состояние  Evo 20";
            // 
            // RemotePortTextBox
            // 
            this.RemotePortTextBox.Location = new System.Drawing.Point(284, 52);
            this.RemotePortTextBox.Name = "RemotePortTextBox";
            this.RemotePortTextBox.Size = new System.Drawing.Size(74, 20);
            this.RemotePortTextBox.TabIndex = 15;
            this.RemotePortTextBox.Text = "531";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(218, 55);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(32, 13);
            this.label26.TabIndex = 14;
            this.label26.Text = "Порт";
            // 
            // IPTextBox
            // 
            this.IPTextBox.Location = new System.Drawing.Point(63, 52);
            this.IPTextBox.Name = "IPTextBox";
            this.IPTextBox.Size = new System.Drawing.Size(100, 20);
            this.IPTextBox.TabIndex = 13;
            this.IPTextBox.Text = "192.168.0.1";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(45, 55);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(17, 13);
            this.label24.TabIndex = 12;
            this.label24.Text = "IP";
            // 
            // EvoParamsGroupBox
            // 
            this.EvoParamsGroupBox.Controls.Add(this.temperatureStabLabel);
            this.EvoParamsGroupBox.Controls.Add(this.label23);
            this.EvoParamsGroupBox.Controls.Add(this.connectionStateLabel);
            this.EvoParamsGroupBox.Controls.Add(this.label20);
            this.EvoParamsGroupBox.Controls.Add(this.nextTemperatureLable);
            this.EvoParamsGroupBox.Controls.Add(this.ySpeedOfRateLabel);
            this.EvoParamsGroupBox.Controls.Add(this.CurrentTemperatureLabel);
            this.EvoParamsGroupBox.Controls.Add(this.identifieGoupBox);
            this.EvoParamsGroupBox.Controls.Add(this.xSpeedOfRateLabel);
            this.EvoParamsGroupBox.Controls.Add(this.label17);
            this.EvoParamsGroupBox.Controls.Add(this.label2);
            this.EvoParamsGroupBox.Controls.Add(this.yPositionLabel);
            this.EvoParamsGroupBox.Controls.Add(this.label1);
            this.EvoParamsGroupBox.Controls.Add(this.xPositionLabel);
            this.EvoParamsGroupBox.Controls.Add(this.label16);
            this.EvoParamsGroupBox.Controls.Add(this.label15);
            this.EvoParamsGroupBox.Controls.Add(this.label14);
            this.EvoParamsGroupBox.Location = new System.Drawing.Point(18, 87);
            this.EvoParamsGroupBox.Name = "EvoParamsGroupBox";
            this.EvoParamsGroupBox.Size = new System.Drawing.Size(389, 456);
            this.EvoParamsGroupBox.TabIndex = 11;
            this.EvoParamsGroupBox.TabStop = false;
            this.EvoParamsGroupBox.Text = "Параметры";
            // 
            // temperatureStabLabel
            // 
            this.temperatureStabLabel.AutoSize = true;
            this.temperatureStabLabel.Location = new System.Drawing.Point(204, 29);
            this.temperatureStabLabel.Name = "temperatureStabLabel";
            this.temperatureStabLabel.Size = new System.Drawing.Size(64, 13);
            this.temperatureStabLabel.TabIndex = 14;
            this.temperatureStabLabel.Text = "нет данных";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(25, 29);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(166, 13);
            this.label23.TabIndex = 13;
            this.label23.Text = "До стабилизации температуры";
            // 
            // connectionStateLabel
            // 
            this.connectionStateLabel.AutoSize = true;
            this.connectionStateLabel.Location = new System.Drawing.Point(204, 57);
            this.connectionStateLabel.Name = "connectionStateLabel";
            this.connectionStateLabel.Size = new System.Drawing.Size(0, 13);
            this.connectionStateLabel.TabIndex = 12;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(25, 57);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(127, 13);
            this.label20.TabIndex = 11;
            this.label20.Text = "Состояние соединения ";
            // 
            // nextTemperatureLable
            // 
            this.nextTemperatureLable.AutoSize = true;
            this.nextTemperatureLable.Location = new System.Drawing.Point(204, 118);
            this.nextTemperatureLable.Name = "nextTemperatureLable";
            this.nextTemperatureLable.Size = new System.Drawing.Size(64, 13);
            this.nextTemperatureLable.TabIndex = 6;
            this.nextTemperatureLable.Text = "нет данных";
            // 
            // ySpeedOfRateLabel
            // 
            this.ySpeedOfRateLabel.AutoSize = true;
            this.ySpeedOfRateLabel.Location = new System.Drawing.Point(276, 201);
            this.ySpeedOfRateLabel.Name = "ySpeedOfRateLabel";
            this.ySpeedOfRateLabel.Size = new System.Drawing.Size(64, 13);
            this.ySpeedOfRateLabel.TabIndex = 7;
            this.ySpeedOfRateLabel.Text = "нет данных";
            // 
            // CurrentTemperatureLabel
            // 
            this.CurrentTemperatureLabel.AutoSize = true;
            this.CurrentTemperatureLabel.Location = new System.Drawing.Point(204, 89);
            this.CurrentTemperatureLabel.Name = "CurrentTemperatureLabel";
            this.CurrentTemperatureLabel.Size = new System.Drawing.Size(64, 13);
            this.CurrentTemperatureLabel.TabIndex = 5;
            this.CurrentTemperatureLabel.Text = "нет данных";
            // 
            // identifieGoupBox
            // 
            this.identifieGoupBox.Controls.Add(this.temperatureReachedIndic);
            this.identifieGoupBox.Controls.Add(this.label13);
            this.identifieGoupBox.Controls.Add(this.label12);
            this.identifieGoupBox.Controls.Add(this.label11);
            this.identifieGoupBox.Controls.Add(this.powerCameraIndic);
            this.identifieGoupBox.Controls.Add(this.yMoveIndic);
            this.identifieGoupBox.Controls.Add(this.xMoveIndic);
            this.identifieGoupBox.Controls.Add(this.yPowerIndic);
            this.identifieGoupBox.Controls.Add(this.xPowerIndic);
            this.identifieGoupBox.Controls.Add(this.yZeroFindedIndic);
            this.identifieGoupBox.Controls.Add(this.xZeroFindedIndic);
            this.identifieGoupBox.Controls.Add(this.label9);
            this.identifieGoupBox.Controls.Add(this.label8);
            this.identifieGoupBox.Controls.Add(this.label10);
            this.identifieGoupBox.Controls.Add(this.label7);
            this.identifieGoupBox.Location = new System.Drawing.Point(17, 228);
            this.identifieGoupBox.Name = "identifieGoupBox";
            this.identifieGoupBox.Size = new System.Drawing.Size(232, 175);
            this.identifieGoupBox.TabIndex = 10;
            this.identifieGoupBox.TabStop = false;
            this.identifieGoupBox.Text = "Идентификаторы";
            // 
            // temperatureReachedIndic
            // 
            this.temperatureReachedIndic.BackColor = System.Drawing.Color.Red;
            this.temperatureReachedIndic.Location = new System.Drawing.Point(186, 60);
            this.temperatureReachedIndic.Name = "temperatureReachedIndic";
            this.temperatureReachedIndic.Size = new System.Drawing.Size(12, 10);
            this.temperatureReachedIndic.TabIndex = 22;
            this.temperatureReachedIndic.TabStop = false;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(9, 60);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(133, 13);
            this.label13.TabIndex = 21;
            this.label13.Text = "Температура достигнута";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(187, 80);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(14, 13);
            this.label12.TabIndex = 20;
            this.label12.Text = "Y";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(171, 80);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(14, 13);
            this.label11.TabIndex = 19;
            this.label11.Text = "X";
            // 
            // powerCameraIndic
            // 
            this.powerCameraIndic.BackColor = System.Drawing.Color.Red;
            this.powerCameraIndic.Location = new System.Drawing.Point(186, 30);
            this.powerCameraIndic.Name = "powerCameraIndic";
            this.powerCameraIndic.Size = new System.Drawing.Size(12, 10);
            this.powerCameraIndic.TabIndex = 17;
            this.powerCameraIndic.TabStop = false;
            // 
            // yMoveIndic
            // 
            this.yMoveIndic.BackColor = System.Drawing.Color.Red;
            this.yMoveIndic.Location = new System.Drawing.Point(189, 149);
            this.yMoveIndic.Name = "yMoveIndic";
            this.yMoveIndic.Size = new System.Drawing.Size(12, 10);
            this.yMoveIndic.TabIndex = 16;
            this.yMoveIndic.TabStop = false;
            // 
            // xMoveIndic
            // 
            this.xMoveIndic.BackColor = System.Drawing.Color.Red;
            this.xMoveIndic.Location = new System.Drawing.Point(171, 149);
            this.xMoveIndic.Name = "xMoveIndic";
            this.xMoveIndic.Size = new System.Drawing.Size(12, 10);
            this.xMoveIndic.TabIndex = 15;
            this.xMoveIndic.TabStop = false;
            // 
            // yPowerIndic
            // 
            this.yPowerIndic.BackColor = System.Drawing.Color.Red;
            this.yPowerIndic.Location = new System.Drawing.Point(189, 122);
            this.yPowerIndic.Name = "yPowerIndic";
            this.yPowerIndic.Size = new System.Drawing.Size(12, 10);
            this.yPowerIndic.TabIndex = 14;
            this.yPowerIndic.TabStop = false;
            // 
            // xPowerIndic
            // 
            this.xPowerIndic.BackColor = System.Drawing.Color.Red;
            this.xPowerIndic.Location = new System.Drawing.Point(171, 122);
            this.xPowerIndic.Name = "xPowerIndic";
            this.xPowerIndic.Size = new System.Drawing.Size(12, 10);
            this.xPowerIndic.TabIndex = 13;
            this.xPowerIndic.TabStop = false;
            // 
            // yZeroFindedIndic
            // 
            this.yZeroFindedIndic.BackColor = System.Drawing.Color.Red;
            this.yZeroFindedIndic.Location = new System.Drawing.Point(189, 96);
            this.yZeroFindedIndic.Name = "yZeroFindedIndic";
            this.yZeroFindedIndic.Size = new System.Drawing.Size(12, 10);
            this.yZeroFindedIndic.TabIndex = 12;
            this.yZeroFindedIndic.TabStop = false;
            // 
            // xZeroFindedIndic
            // 
            this.xZeroFindedIndic.BackColor = System.Drawing.Color.Red;
            this.xZeroFindedIndic.Location = new System.Drawing.Point(171, 96);
            this.xZeroFindedIndic.Name = "xZeroFindedIndic";
            this.xZeroFindedIndic.Size = new System.Drawing.Size(12, 10);
            this.xZeroFindedIndic.TabIndex = 4;
            this.xZeroFindedIndic.TabStop = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(16, 149);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(87, 13);
            this.label9.TabIndex = 2;
            this.label9.Text = "Движение осей";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(16, 122);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Питание осей";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(8, 32);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(130, 13);
            this.label10.TabIndex = 3;
            this.label10.Text = "Питаение термокамеры";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 96);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(100, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Каллибровка нуля";
            // 
            // xSpeedOfRateLabel
            // 
            this.xSpeedOfRateLabel.AutoSize = true;
            this.xSpeedOfRateLabel.Location = new System.Drawing.Point(200, 201);
            this.xSpeedOfRateLabel.Name = "xSpeedOfRateLabel";
            this.xSpeedOfRateLabel.Size = new System.Drawing.Size(64, 13);
            this.xSpeedOfRateLabel.TabIndex = 6;
            this.xSpeedOfRateLabel.Text = "нет данных";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(26, 201);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(145, 13);
            this.label17.TabIndex = 5;
            this.label17.Text = "Текущая угловая скорость";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 118);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(134, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Следующая температура";
            // 
            // yPositionLabel
            // 
            this.yPositionLabel.AutoSize = true;
            this.yPositionLabel.Location = new System.Drawing.Point(276, 170);
            this.yPositionLabel.Name = "yPositionLabel";
            this.yPositionLabel.Size = new System.Drawing.Size(64, 13);
            this.yPositionLabel.TabIndex = 4;
            this.yPositionLabel.Text = "нет данных";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 89);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Текущая температура";
            // 
            // xPositionLabel
            // 
            this.xPositionLabel.AutoSize = true;
            this.xPositionLabel.Location = new System.Drawing.Point(200, 170);
            this.xPositionLabel.Name = "xPositionLabel";
            this.xPositionLabel.Size = new System.Drawing.Size(64, 13);
            this.xPositionLabel.TabIndex = 3;
            this.xPositionLabel.Text = "нет данных";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(276, 145);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(14, 13);
            this.label16.TabIndex = 2;
            this.label16.Text = "Y";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(204, 145);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(14, 13);
            this.label15.TabIndex = 1;
            this.label15.Text = "X";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(25, 170);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(140, 13);
            this.label14.TabIndex = 0;
            this.label14.Text = "Текущая угловая позиция";
            // 
            // graph
            // 
            this.graph.IsShowPointValues = false;
            this.graph.Location = new System.Drawing.Point(428, 0);
            this.graph.Name = "graph";
            this.graph.PointValueFormat = "G";
            this.graph.Size = new System.Drawing.Size(741, 454);
            this.graph.TabIndex = 8;
            // 
            // evoStopButton
            // 
            this.evoStopButton.Enabled = false;
            this.evoStopButton.Image = global::Evo20.GUI.Properties.Resources.stop;
            this.evoStopButton.Location = new System.Drawing.Point(95, 7);
            this.evoStopButton.Name = "evoStopButton";
            this.evoStopButton.Size = new System.Drawing.Size(44, 41);
            this.evoStopButton.TabIndex = 7;
            this.evoStopButton.UseVisualStyleBackColor = true;
            this.evoStopButton.Click += new System.EventHandler(this.evoStopButton_Click);
            // 
            // evoStartButton
            // 
            this.evoStartButton.Cursor = System.Windows.Forms.Cursors.AppStarting;
            this.evoStartButton.Image = global::Evo20.GUI.Properties.Resources.play;
            this.evoStartButton.Location = new System.Drawing.Point(18, 7);
            this.evoStartButton.Name = "evoStartButton";
            this.evoStartButton.Size = new System.Drawing.Size(44, 41);
            this.evoStartButton.TabIndex = 0;
            this.evoStartButton.UseVisualStyleBackColor = false;
            this.evoStartButton.Click += new System.EventHandler(this.evoStartButton_Click);
            // 
            // evoPauseButton
            // 
            this.evoPauseButton.Cursor = System.Windows.Forms.Cursors.No;
            this.evoPauseButton.Enabled = false;
            this.evoPauseButton.Image = global::Evo20.GUI.Properties.Resources.pauseImg;
            this.evoPauseButton.Location = new System.Drawing.Point(56, 7);
            this.evoPauseButton.Name = "evoPauseButton";
            this.evoPauseButton.Size = new System.Drawing.Size(44, 41);
            this.evoPauseButton.TabIndex = 6;
            this.evoPauseButton.UseVisualStyleBackColor = true;
            this.evoPauseButton.Click += new System.EventHandler(this.evoPauseButton_Click);
            // 
            // Sensor_control_page
            // 
            this.Sensor_control_page.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.Sensor_control_page.Controls.Add(this.sensorGroupBox);
            this.Sensor_control_page.Controls.Add(this.label4);
            this.Sensor_control_page.Controls.Add(this.comPortComboBox);
            this.Sensor_control_page.Controls.Add(this.sensorStopButton);
            this.Sensor_control_page.Controls.Add(this.sensorPauseButton);
            this.Sensor_control_page.Controls.Add(this.sensorStartButton);
            this.Sensor_control_page.Location = new System.Drawing.Point(4, 22);
            this.Sensor_control_page.Margin = new System.Windows.Forms.Padding(5);
            this.Sensor_control_page.Name = "Sensor_control_page";
            this.Sensor_control_page.Padding = new System.Windows.Forms.Padding(3);
            this.Sensor_control_page.Size = new System.Drawing.Size(1172, 539);
            this.Sensor_control_page.TabIndex = 1;
            this.Sensor_control_page.Text = "Состояние блока";
            // 
            // sensorGroupBox
            // 
            this.sensorGroupBox.Controls.Add(this.currentPositionNumberLbl);
            this.sensorGroupBox.Controls.Add(this.label25);
            this.sensorGroupBox.Controls.Add(this.sensorConnectionStateLabel);
            this.sensorGroupBox.Controls.Add(this.label22);
            this.sensorGroupBox.Controls.Add(this.SensorDataGridView);
            this.sensorGroupBox.Controls.Add(this.sensorTypeLabel);
            this.sensorGroupBox.Controls.Add(this.label18);
            this.sensorGroupBox.Controls.Add(this.packetsArrivedLbl);
            this.sensorGroupBox.Controls.Add(this.label6);
            this.sensorGroupBox.Location = new System.Drawing.Point(40, 72);
            this.sensorGroupBox.Name = "sensorGroupBox";
            this.sensorGroupBox.Size = new System.Drawing.Size(808, 321);
            this.sensorGroupBox.TabIndex = 5;
            this.sensorGroupBox.TabStop = false;
            this.sensorGroupBox.Text = "Данные с датчика";
            // 
            // currentPositionNumberLbl
            // 
            this.currentPositionNumberLbl.AutoSize = true;
            this.currentPositionNumberLbl.Location = new System.Drawing.Point(205, 43);
            this.currentPositionNumberLbl.Name = "currentPositionNumberLbl";
            this.currentPositionNumberLbl.Size = new System.Drawing.Size(64, 13);
            this.currentPositionNumberLbl.TabIndex = 8;
            this.currentPositionNumberLbl.Text = "нет данных";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(36, 43);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(132, 13);
            this.label25.TabIndex = 7;
            this.label25.Text = "Номер текущей позиции";
            // 
            // sensorConnectionStateLabel
            // 
            this.sensorConnectionStateLabel.AutoSize = true;
            this.sensorConnectionStateLabel.Location = new System.Drawing.Point(205, 72);
            this.sensorConnectionStateLabel.Name = "sensorConnectionStateLabel";
            this.sensorConnectionStateLabel.Size = new System.Drawing.Size(64, 13);
            this.sensorConnectionStateLabel.TabIndex = 6;
            this.sensorConnectionStateLabel.Text = "нет данных";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(36, 72);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(128, 13);
            this.label22.TabIndex = 5;
            this.label22.Text = "Соединение с датчиком";
            // 
            // SensorDataGridView
            // 
            this.SensorDataGridView.AllowUserToAddRows = false;
            this.SensorDataGridView.AllowUserToDeleteRows = false;
            this.SensorDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.SensorDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.SensorDataGridView.BackgroundColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.SensorDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.SensorDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.SensorDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Type,
            this.xColumn,
            this.yColumn,
            this.zColumn});
            this.SensorDataGridView.GridColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.SensorDataGridView.Location = new System.Drawing.Point(37, 157);
            this.SensorDataGridView.Name = "SensorDataGridView";
            this.SensorDataGridView.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            this.SensorDataGridView.Size = new System.Drawing.Size(713, 145);
            this.SensorDataGridView.TabIndex = 4;
            this.SensorDataGridView.Visible = false;
            // 
            // Type
            // 
            this.Type.HeaderText = "Тип";
            this.Type.Name = "Type";
            this.Type.Width = 51;
            // 
            // xColumn
            // 
            this.xColumn.HeaderText = "X";
            this.xColumn.Name = "xColumn";
            this.xColumn.ReadOnly = true;
            this.xColumn.Width = 39;
            // 
            // yColumn
            // 
            this.yColumn.HeaderText = "Y";
            this.yColumn.Name = "yColumn";
            this.yColumn.Width = 39;
            // 
            // zColumn
            // 
            this.zColumn.HeaderText = "Z";
            this.zColumn.Name = "zColumn";
            this.zColumn.Width = 39;
            // 
            // sensorTypeLabel
            // 
            this.sensorTypeLabel.AutoSize = true;
            this.sensorTypeLabel.Location = new System.Drawing.Point(205, 128);
            this.sensorTypeLabel.Name = "sensorTypeLabel";
            this.sensorTypeLabel.Size = new System.Drawing.Size(64, 13);
            this.sensorTypeLabel.TabIndex = 3;
            this.sensorTypeLabel.Text = "нет данных";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(36, 128);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(69, 13);
            this.label18.TabIndex = 2;
            this.label18.Text = "Тип датчика";
            // 
            // packetsArrivedLbl
            // 
            this.packetsArrivedLbl.AutoSize = true;
            this.packetsArrivedLbl.Location = new System.Drawing.Point(205, 101);
            this.packetsArrivedLbl.Name = "packetsArrivedLbl";
            this.packetsArrivedLbl.Size = new System.Drawing.Size(64, 13);
            this.packetsArrivedLbl.TabIndex = 1;
            this.packetsArrivedLbl.Text = "нет данных";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(36, 101);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(102, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Получено пакетов ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(36, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Выбирите порт";
            // 
            // comPortComboBox
            // 
            this.comPortComboBox.FormattingEnabled = true;
            this.comPortComboBox.Location = new System.Drawing.Point(125, 18);
            this.comPortComboBox.Name = "comPortComboBox";
            this.comPortComboBox.Size = new System.Drawing.Size(117, 21);
            this.comPortComboBox.TabIndex = 1;
            // 
            // sensorStopButton
            // 
            this.sensorStopButton.Enabled = false;
            this.sensorStopButton.Image = global::Evo20.GUI.Properties.Resources.stop;
            this.sensorStopButton.Location = new System.Drawing.Point(325, 8);
            this.sensorStopButton.Name = "sensorStopButton";
            this.sensorStopButton.Size = new System.Drawing.Size(43, 41);
            this.sensorStopButton.TabIndex = 4;
            this.sensorStopButton.UseVisualStyleBackColor = true;
            this.sensorStopButton.Click += new System.EventHandler(this.sensorStopButton_Click);
            // 
            // sensorPauseButton
            // 
            this.sensorPauseButton.Enabled = false;
            this.sensorPauseButton.Image = global::Evo20.GUI.Properties.Resources.pauseImg;
            this.sensorPauseButton.Location = new System.Drawing.Point(288, 8);
            this.sensorPauseButton.Name = "sensorPauseButton";
            this.sensorPauseButton.Size = new System.Drawing.Size(40, 41);
            this.sensorPauseButton.TabIndex = 3;
            this.sensorPauseButton.UseVisualStyleBackColor = true;
            this.sensorPauseButton.Click += new System.EventHandler(this.sensorPauseButton_Click);
            // 
            // sensorStartButton
            // 
            this.sensorStartButton.Image = global::Evo20.GUI.Properties.Resources.play;
            this.sensorStartButton.Location = new System.Drawing.Point(248, 8);
            this.sensorStartButton.Name = "sensorStartButton";
            this.sensorStartButton.Size = new System.Drawing.Size(44, 41);
            this.sensorStartButton.TabIndex = 0;
            this.sensorStartButton.UseVisualStyleBackColor = true;
            this.sensorStartButton.Click += new System.EventHandler(this.sensorStartButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Прошло времени";
            // 
            // modeComboBox
            // 
            this.modeComboBox.FormattingEnabled = true;
            this.modeComboBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.modeComboBox.Items.AddRange(new object[] {
            "калибровка",
            "проверка"});
            this.modeComboBox.Location = new System.Drawing.Point(174, 29);
            this.modeComboBox.Name = "modeComboBox";
            this.modeComboBox.Size = new System.Drawing.Size(92, 21);
            this.modeComboBox.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 32);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(134, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Выберете режим работы";
            // 
            // parameGroupBox
            // 
            this.parameGroupBox.Controls.Add(this.controllerWokModeLabel);
            this.parameGroupBox.Controls.Add(this.label21);
            this.parameGroupBox.Controls.Add(this.countTemperaturesReachedLabel);
            this.parameGroupBox.Controls.Add(this.label19);
            this.parameGroupBox.Controls.Add(this.timeLeftlabel);
            this.parameGroupBox.Controls.Add(this.label3);
            this.parameGroupBox.Location = new System.Drawing.Point(4, 53);
            this.parameGroupBox.Name = "parameGroupBox";
            this.parameGroupBox.Size = new System.Drawing.Size(292, 123);
            this.parameGroupBox.TabIndex = 9;
            this.parameGroupBox.TabStop = false;
            this.parameGroupBox.Text = "Текущие параметры";
            // 
            // controllerWokModeLabel
            // 
            this.controllerWokModeLabel.AutoSize = true;
            this.controllerWokModeLabel.Location = new System.Drawing.Point(168, 92);
            this.controllerWokModeLabel.Name = "controllerWokModeLabel";
            this.controllerWokModeLabel.Size = new System.Drawing.Size(64, 13);
            this.controllerWokModeLabel.TabIndex = 8;
            this.controllerWokModeLabel.Text = "нет данных";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(9, 92);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(130, 13);
            this.label21.TabIndex = 7;
            this.label21.Text = "Режим работы системы";
            // 
            // countTemperaturesReachedLabel
            // 
            this.countTemperaturesReachedLabel.AutoSize = true;
            this.countTemperaturesReachedLabel.Location = new System.Drawing.Point(168, 62);
            this.countTemperaturesReachedLabel.Name = "countTemperaturesReachedLabel";
            this.countTemperaturesReachedLabel.Size = new System.Drawing.Size(64, 13);
            this.countTemperaturesReachedLabel.TabIndex = 6;
            this.countTemperaturesReachedLabel.Text = "нет данных";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(9, 62);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(119, 13);
            this.label19.TabIndex = 5;
            this.label19.Text = "Температур пройдено";
            // 
            // timeLeftlabel
            // 
            this.timeLeftlabel.AutoSize = true;
            this.timeLeftlabel.Location = new System.Drawing.Point(168, 32);
            this.timeLeftlabel.Name = "timeLeftlabel";
            this.timeLeftlabel.Size = new System.Drawing.Size(64, 13);
            this.timeLeftlabel.TabIndex = 4;
            this.timeLeftlabel.Text = "нет данных";
            // 
            // workTimer
            // 
            this.workTimer.Tick += new System.EventHandler(this.workTimer_Tick);
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Image = global::Evo20.GUI.Properties.Resources.stop;
            this.stopButton.Location = new System.Drawing.Point(342, 24);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(44, 41);
            this.stopButton.TabIndex = 8;
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // pauseButton
            // 
            this.pauseButton.Enabled = false;
            this.pauseButton.Image = global::Evo20.GUI.Properties.Resources.pauseImg;
            this.pauseButton.Location = new System.Drawing.Point(408, 24);
            this.pauseButton.Name = "pauseButton";
            this.pauseButton.Size = new System.Drawing.Size(44, 41);
            this.pauseButton.TabIndex = 7;
            this.pauseButton.UseVisualStyleBackColor = true;
            this.pauseButton.Visible = false;
            // 
            // SensorTimer
            // 
            this.SensorTimer.Interval = 1000;
            this.SensorTimer.Tick += new System.EventHandler(this.SensorTimer_Tick);
            // 
            // startButton
            // 
            this.startButton.Image = global::Evo20.GUI.Properties.Resources.play;
            this.startButton.Location = new System.Drawing.Point(302, 24);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(44, 41);
            this.startButton.TabIndex = 3;
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // menuStrip
            // 
            this.menuStrip.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileToolStripMenuItem,
            this.SettingsToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1218, 24);
            this.menuStrip.TabIndex = 13;
            this.menuStrip.Text = "menuStrip1";
            // 
            // FileToolStripMenuItem
            // 
            this.FileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.getDataFromFileToolStripMenuItem});
            this.FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            this.FileToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.FileToolStripMenuItem.Text = "Файл";
            // 
            // getDataFromFileToolStripMenuItem
            // 
            this.getDataFromFileToolStripMenuItem.Name = "getDataFromFileToolStripMenuItem";
            this.getDataFromFileToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.getDataFromFileToolStripMenuItem.Text = "Получить данные из файла ";
            this.getDataFromFileToolStripMenuItem.Click += new System.EventHandler(this.getDataFromFileToolStripMenuItem_Click);
            // 
            // SettingsToolStripMenuItem
            // 
            this.SettingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cycleSettingsToolStripMenuItem,
            this.CommonSettingsToolStripMenuItem});
            this.SettingsToolStripMenuItem.Name = "SettingsToolStripMenuItem";
            this.SettingsToolStripMenuItem.Size = new System.Drawing.Size(79, 20);
            this.SettingsToolStripMenuItem.Text = "Настройки";
            // 
            // cycleSettingsToolStripMenuItem
            // 
            this.cycleSettingsToolStripMenuItem.Name = "cycleSettingsToolStripMenuItem";
            this.cycleSettingsToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.cycleSettingsToolStripMenuItem.Text = "Настройки температур проверки";
            this.cycleSettingsToolStripMenuItem.Click += new System.EventHandler(this.cycleSettingsToolStripMenuItem_Click);
            // 
            // CommonSettingsToolStripMenuItem
            // 
            this.CommonSettingsToolStripMenuItem.Name = "CommonSettingsToolStripMenuItem";
            this.CommonSettingsToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.CommonSettingsToolStripMenuItem.Text = "Общие настройки";
            this.CommonSettingsToolStripMenuItem.Click += new System.EventHandler(this.CommonSettingsToolStripMenuItem_Click);
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(1109, 29);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(47, 13);
            this.lblVersion.TabIndex = 14;
            this.lblVersion.Text = "Версия:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(1218, 735);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.parameGroupBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.pauseButton);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.modeComboBox);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Text = "Evo20";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.Evo_control_page.ResumeLayout(false);
            this.Evo_control_page.PerformLayout();
            this.EvoParamsGroupBox.ResumeLayout(false);
            this.EvoParamsGroupBox.PerformLayout();
            this.identifieGoupBox.ResumeLayout(false);
            this.identifieGoupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.temperatureReachedIndic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.powerCameraIndic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yMoveIndic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xMoveIndic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yPowerIndic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xPowerIndic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yZeroFindedIndic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xZeroFindedIndic)).EndInit();
            this.Sensor_control_page.ResumeLayout(false);
            this.Sensor_control_page.PerformLayout();
            this.sensorGroupBox.ResumeLayout(false);
            this.sensorGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SensorDataGridView)).EndInit();
            this.parameGroupBox.ResumeLayout(false);
            this.parameGroupBox.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button evoStartButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage Evo_control_page;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage Sensor_control_page;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comPortComboBox;
        private System.Windows.Forms.Button sensorStartButton;
        private System.Windows.Forms.Button evoPauseButton;
        private System.Windows.Forms.Button evoStopButton;
        private System.Windows.Forms.Button sensorStopButton;
        private System.Windows.Forms.Button sensorPauseButton;
        private ZedGraph.ZedGraphControl graph;
        private System.Windows.Forms.ComboBox modeComboBox;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button pauseButton;
        private System.Windows.Forms.GroupBox parameGroupBox;
        private System.Windows.Forms.Timer workTimer;
        private System.Windows.Forms.Label timeLeftlabel;
        private System.Windows.Forms.Label CurrentTemperatureLabel;
        private System.Windows.Forms.Label nextTemperatureLable;
        private System.Windows.Forms.GroupBox sensorGroupBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label packetsArrivedLbl;
        private System.Windows.Forms.GroupBox identifieGoupBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.PictureBox xZeroFindedIndic;
        private System.Windows.Forms.PictureBox powerCameraIndic;
        private System.Windows.Forms.PictureBox yMoveIndic;
        private System.Windows.Forms.PictureBox xMoveIndic;
        private System.Windows.Forms.PictureBox yPowerIndic;
        private System.Windows.Forms.PictureBox xPowerIndic;
        private System.Windows.Forms.PictureBox yZeroFindedIndic;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.PictureBox temperatureReachedIndic;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.GroupBox EvoParamsGroupBox;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label xPositionLabel;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label yPositionLabel;
        private System.Windows.Forms.Label ySpeedOfRateLabel;
        private System.Windows.Forms.Label xSpeedOfRateLabel;
        private System.Windows.Forms.Label sensorTypeLabel;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.DataGridView SensorDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn xColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn yColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn zColumn;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label countTemperaturesReachedLabel;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label connectionStateLabel;
        private System.Windows.Forms.Label controllerWokModeLabel;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label sensorConnectionStateLabel;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label temperatureStabLabel;
        private System.Windows.Forms.Timer SensorTimer;
        private System.Windows.Forms.Label currentPositionNumberLbl;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem FileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem getDataFromFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cycleSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CommonSettingsToolStripMenuItem;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.TextBox IPTextBox;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox RemotePortTextBox;
        private System.Windows.Forms.Label label26;
    }
}

