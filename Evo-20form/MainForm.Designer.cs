namespace Evo_20form
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
            this.StartEvoButton = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.Evo_control_page = new System.Windows.Forms.TabPage();
            this.EvoParamsGroupBox = new System.Windows.Forms.GroupBox();
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
            this.StopEvoButton = new System.Windows.Forms.Button();
            this.PauseEvoButton = new System.Windows.Forms.Button();
            this.Sensor_control_page = new System.Windows.Forms.TabPage();
            this.savePacketsButton = new System.Windows.Forms.Button();
            this.sensorGroupBox = new System.Windows.Forms.GroupBox();
            this.SensorDataGridView = new System.Windows.Forms.DataGridView();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.xColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.yColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.zColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sensorTypeLabel = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.packetsArrivedLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.sensorStopButton = new System.Windows.Forms.Button();
            this.sensorPauseButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.comPortComboBox = new System.Windows.Forms.ComboBox();
            this.startSensorButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.modeComboBox = new System.Windows.Forms.ComboBox();
            this.startButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.pauseButton = new System.Windows.Forms.Button();
            this.parameGroupBox = new System.Windows.Forms.GroupBox();
            this.countTemperaturesReachedLabel = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.timeLeftlabel = new System.Windows.Forms.Label();
            this.workTimer = new System.Windows.Forms.Timer(this.components);
            this.settingsButton = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.readDatabutton = new System.Windows.Forms.Button();
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
            this.SuspendLayout();
            // 
            // StartEvoButton
            // 
            this.StartEvoButton.Image = global::Evo_20form.Properties.Resources.play__1_;
            this.StartEvoButton.Location = new System.Drawing.Point(10, 6);
            this.StartEvoButton.Name = "StartEvoButton";
            this.StartEvoButton.Size = new System.Drawing.Size(44, 41);
            this.StartEvoButton.TabIndex = 0;
            this.StartEvoButton.UseVisualStyleBackColor = true;
            this.StartEvoButton.Click += new System.EventHandler(this.StartEvo_button_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.Evo_control_page);
            this.tabControl1.Controls.Add(this.Sensor_control_page);
            this.tabControl1.Location = new System.Drawing.Point(12, 152);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(5);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1128, 482);
            this.tabControl1.TabIndex = 1;
            // 
            // Evo_control_page
            // 
            this.Evo_control_page.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.Evo_control_page.Controls.Add(this.EvoParamsGroupBox);
            this.Evo_control_page.Controls.Add(this.graph);
            this.Evo_control_page.Controls.Add(this.StartEvoButton);
            this.Evo_control_page.Controls.Add(this.StopEvoButton);
            this.Evo_control_page.Controls.Add(this.PauseEvoButton);
            this.Evo_control_page.Location = new System.Drawing.Point(4, 22);
            this.Evo_control_page.Name = "Evo_control_page";
            this.Evo_control_page.Padding = new System.Windows.Forms.Padding(3);
            this.Evo_control_page.Size = new System.Drawing.Size(1120, 456);
            this.Evo_control_page.TabIndex = 0;
            this.Evo_control_page.Text = "Состояние  Evo 20";
            // 
            // EvoParamsGroupBox
            // 
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
            this.EvoParamsGroupBox.Location = new System.Drawing.Point(13, 65);
            this.EvoParamsGroupBox.Name = "EvoParamsGroupBox";
            this.EvoParamsGroupBox.Size = new System.Drawing.Size(389, 397);
            this.EvoParamsGroupBox.TabIndex = 11;
            this.EvoParamsGroupBox.TabStop = false;
            this.EvoParamsGroupBox.Text = "Параметры";
            // 
            // nextTemperatureLable
            // 
            this.nextTemperatureLable.AutoSize = true;
            this.nextTemperatureLable.Location = new System.Drawing.Point(193, 82);
            this.nextTemperatureLable.Name = "nextTemperatureLable";
            this.nextTemperatureLable.Size = new System.Drawing.Size(64, 13);
            this.nextTemperatureLable.TabIndex = 6;
            this.nextTemperatureLable.Text = "нет данных";
            // 
            // ySpeedOfRateLabel
            // 
            this.ySpeedOfRateLabel.AutoSize = true;
            this.ySpeedOfRateLabel.Location = new System.Drawing.Point(237, 174);
            this.ySpeedOfRateLabel.Name = "ySpeedOfRateLabel";
            this.ySpeedOfRateLabel.Size = new System.Drawing.Size(64, 13);
            this.ySpeedOfRateLabel.TabIndex = 7;
            this.ySpeedOfRateLabel.Text = "нет данных";
            // 
            // CurrentTemperatureLabel
            // 
            this.CurrentTemperatureLabel.AutoSize = true;
            this.CurrentTemperatureLabel.Location = new System.Drawing.Point(193, 44);
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
            this.identifieGoupBox.Location = new System.Drawing.Point(12, 201);
            this.identifieGoupBox.Name = "identifieGoupBox";
            this.identifieGoupBox.Size = new System.Drawing.Size(185, 175);
            this.identifieGoupBox.TabIndex = 10;
            this.identifieGoupBox.TabStop = false;
            this.identifieGoupBox.Text = "Идентификаторы";
            // 
            // temperatureReachedIndic
            // 
            this.temperatureReachedIndic.BackColor = System.Drawing.Color.Red;
            this.temperatureReachedIndic.Location = new System.Drawing.Point(155, 63);
            this.temperatureReachedIndic.Name = "temperatureReachedIndic";
            this.temperatureReachedIndic.Size = new System.Drawing.Size(12, 12);
            this.temperatureReachedIndic.TabIndex = 22;
            this.temperatureReachedIndic.TabStop = false;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(8, 62);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(133, 13);
            this.label13.TabIndex = 21;
            this.label13.Text = "Температура достигнута";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(156, 83);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(14, 13);
            this.label12.TabIndex = 20;
            this.label12.Text = "Y";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(140, 83);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(14, 13);
            this.label11.TabIndex = 19;
            this.label11.Text = "X";
            // 
            // powerCameraIndic
            // 
            this.powerCameraIndic.BackColor = System.Drawing.Color.Red;
            this.powerCameraIndic.Location = new System.Drawing.Point(155, 33);
            this.powerCameraIndic.Name = "powerCameraIndic";
            this.powerCameraIndic.Size = new System.Drawing.Size(12, 12);
            this.powerCameraIndic.TabIndex = 17;
            this.powerCameraIndic.TabStop = false;
            // 
            // yMoveIndic
            // 
            this.yMoveIndic.BackColor = System.Drawing.Color.Red;
            this.yMoveIndic.Location = new System.Drawing.Point(158, 152);
            this.yMoveIndic.Name = "yMoveIndic";
            this.yMoveIndic.Size = new System.Drawing.Size(12, 12);
            this.yMoveIndic.TabIndex = 16;
            this.yMoveIndic.TabStop = false;
            // 
            // xMoveIndic
            // 
            this.xMoveIndic.BackColor = System.Drawing.Color.Red;
            this.xMoveIndic.Location = new System.Drawing.Point(140, 152);
            this.xMoveIndic.Name = "xMoveIndic";
            this.xMoveIndic.Size = new System.Drawing.Size(12, 12);
            this.xMoveIndic.TabIndex = 15;
            this.xMoveIndic.TabStop = false;
            // 
            // yPowerIndic
            // 
            this.yPowerIndic.BackColor = System.Drawing.Color.Red;
            this.yPowerIndic.Location = new System.Drawing.Point(158, 125);
            this.yPowerIndic.Name = "yPowerIndic";
            this.yPowerIndic.Size = new System.Drawing.Size(12, 12);
            this.yPowerIndic.TabIndex = 14;
            this.yPowerIndic.TabStop = false;
            // 
            // xPowerIndic
            // 
            this.xPowerIndic.BackColor = System.Drawing.Color.Red;
            this.xPowerIndic.Location = new System.Drawing.Point(140, 125);
            this.xPowerIndic.Name = "xPowerIndic";
            this.xPowerIndic.Size = new System.Drawing.Size(12, 12);
            this.xPowerIndic.TabIndex = 13;
            this.xPowerIndic.TabStop = false;
            // 
            // yZeroFindedIndic
            // 
            this.yZeroFindedIndic.BackColor = System.Drawing.Color.Red;
            this.yZeroFindedIndic.Location = new System.Drawing.Point(158, 99);
            this.yZeroFindedIndic.Name = "yZeroFindedIndic";
            this.yZeroFindedIndic.Size = new System.Drawing.Size(12, 12);
            this.yZeroFindedIndic.TabIndex = 12;
            this.yZeroFindedIndic.TabStop = false;
            // 
            // xZeroFindedIndic
            // 
            this.xZeroFindedIndic.BackColor = System.Drawing.Color.Red;
            this.xZeroFindedIndic.Location = new System.Drawing.Point(140, 99);
            this.xZeroFindedIndic.Name = "xZeroFindedIndic";
            this.xZeroFindedIndic.Size = new System.Drawing.Size(12, 12);
            this.xZeroFindedIndic.TabIndex = 4;
            this.xZeroFindedIndic.TabStop = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 151);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(87, 13);
            this.label9.TabIndex = 2;
            this.label9.Text = "Движение осей";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 124);
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
            this.label7.Location = new System.Drawing.Point(6, 98);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(100, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Каллибровка нуля";
            // 
            // xSpeedOfRateLabel
            // 
            this.xSpeedOfRateLabel.AutoSize = true;
            this.xSpeedOfRateLabel.Location = new System.Drawing.Point(166, 174);
            this.xSpeedOfRateLabel.Name = "xSpeedOfRateLabel";
            this.xSpeedOfRateLabel.Size = new System.Drawing.Size(64, 13);
            this.xSpeedOfRateLabel.TabIndex = 6;
            this.xSpeedOfRateLabel.Text = "нет данных";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(9, 174);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(145, 13);
            this.label17.TabIndex = 5;
            this.label17.Text = "Текущая угловая скорость";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 82);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(134, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Следующая температура";
            // 
            // yPositionLabel
            // 
            this.yPositionLabel.AutoSize = true;
            this.yPositionLabel.Location = new System.Drawing.Point(237, 143);
            this.yPositionLabel.Name = "yPositionLabel";
            this.yPositionLabel.Size = new System.Drawing.Size(64, 13);
            this.yPositionLabel.TabIndex = 4;
            this.yPositionLabel.Text = "нет данных";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Текущая температура";
            // 
            // xPositionLabel
            // 
            this.xPositionLabel.AutoSize = true;
            this.xPositionLabel.Location = new System.Drawing.Point(166, 143);
            this.xPositionLabel.Name = "xPositionLabel";
            this.xPositionLabel.Size = new System.Drawing.Size(64, 13);
            this.xPositionLabel.TabIndex = 3;
            this.xPositionLabel.Text = "нет данных";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(243, 118);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(14, 13);
            this.label16.TabIndex = 2;
            this.label16.Text = "Y";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(168, 118);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(14, 13);
            this.label15.TabIndex = 1;
            this.label15.Text = "X";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(9, 143);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(140, 13);
            this.label14.TabIndex = 0;
            this.label14.Text = "Текущая угловая позиция";
            // 
            // graph
            // 
            this.graph.IsShowPointValues = false;
            this.graph.Location = new System.Drawing.Point(440, 3);
            this.graph.Name = "graph";
            this.graph.PointValueFormat = "G";
            this.graph.Size = new System.Drawing.Size(677, 435);
            this.graph.TabIndex = 8;
            // 
            // StopEvoButton
            // 
            this.StopEvoButton.Image = global::Evo_20form.Properties.Resources.stop;
            this.StopEvoButton.Location = new System.Drawing.Point(87, 6);
            this.StopEvoButton.Name = "StopEvoButton";
            this.StopEvoButton.Size = new System.Drawing.Size(44, 41);
            this.StopEvoButton.TabIndex = 7;
            this.StopEvoButton.UseVisualStyleBackColor = true;
            this.StopEvoButton.Click += new System.EventHandler(this.StopEvo_button_Click);
            // 
            // PauseEvoButton
            // 
            this.PauseEvoButton.Image = global::Evo_20form.Properties.Resources.pause_;
            this.PauseEvoButton.Location = new System.Drawing.Point(48, 6);
            this.PauseEvoButton.Name = "PauseEvoButton";
            this.PauseEvoButton.Size = new System.Drawing.Size(44, 41);
            this.PauseEvoButton.TabIndex = 6;
            this.PauseEvoButton.UseVisualStyleBackColor = true;
            this.PauseEvoButton.Click += new System.EventHandler(this.PauseEvo_button_Click);
            // 
            // Sensor_control_page
            // 
            this.Sensor_control_page.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.Sensor_control_page.Controls.Add(this.savePacketsButton);
            this.Sensor_control_page.Controls.Add(this.sensorGroupBox);
            this.Sensor_control_page.Controls.Add(this.sensorStopButton);
            this.Sensor_control_page.Controls.Add(this.sensorPauseButton);
            this.Sensor_control_page.Controls.Add(this.label4);
            this.Sensor_control_page.Controls.Add(this.comPortComboBox);
            this.Sensor_control_page.Controls.Add(this.startSensorButton);
            this.Sensor_control_page.Location = new System.Drawing.Point(4, 22);
            this.Sensor_control_page.Margin = new System.Windows.Forms.Padding(5);
            this.Sensor_control_page.Name = "Sensor_control_page";
            this.Sensor_control_page.Padding = new System.Windows.Forms.Padding(3);
            this.Sensor_control_page.Size = new System.Drawing.Size(1120, 456);
            this.Sensor_control_page.TabIndex = 1;
            this.Sensor_control_page.Text = "Состояние блока";
            this.Sensor_control_page.Click += new System.EventHandler(this.Sensor_control_page_Click);
            // 
            // savePacketsButton
            // 
            this.savePacketsButton.Location = new System.Drawing.Point(426, 10);
            this.savePacketsButton.Name = "savePacketsButton";
            this.savePacketsButton.Size = new System.Drawing.Size(179, 27);
            this.savePacketsButton.TabIndex = 11;
            this.savePacketsButton.Text = "Сохранить пакеты в файл";
            this.savePacketsButton.UseVisualStyleBackColor = true;
            this.savePacketsButton.Click += new System.EventHandler(this.savePacketsButton_Click);
            // 
            // sensorGroupBox
            // 
            this.sensorGroupBox.Controls.Add(this.SensorDataGridView);
            this.sensorGroupBox.Controls.Add(this.sensorTypeLabel);
            this.sensorGroupBox.Controls.Add(this.label18);
            this.sensorGroupBox.Controls.Add(this.packetsArrivedLabel);
            this.sensorGroupBox.Controls.Add(this.label6);
            this.sensorGroupBox.Location = new System.Drawing.Point(40, 72);
            this.sensorGroupBox.Name = "sensorGroupBox";
            this.sensorGroupBox.Size = new System.Drawing.Size(609, 321);
            this.sensorGroupBox.TabIndex = 5;
            this.sensorGroupBox.TabStop = false;
            this.sensorGroupBox.Text = "Данные с датчика";
            // 
            // SensorDataGridView
            // 
            this.SensorDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.SensorDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Type,
            this.xColumn,
            this.yColumn,
            this.zColumn});
            this.SensorDataGridView.Location = new System.Drawing.Point(26, 112);
            this.SensorDataGridView.Name = "SensorDataGridView";
            this.SensorDataGridView.Size = new System.Drawing.Size(564, 135);
            this.SensorDataGridView.TabIndex = 4;
            this.SensorDataGridView.Visible = false;
            this.SensorDataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.SensorDataGridView_CellContentClick);
            // 
            // Type
            // 
            this.Type.HeaderText = "Тип";
            this.Type.Name = "Type";
            // 
            // xColumn
            // 
            this.xColumn.HeaderText = "X";
            this.xColumn.Name = "xColumn";
            this.xColumn.ReadOnly = true;
            // 
            // yColumn
            // 
            this.yColumn.HeaderText = "Y";
            this.yColumn.Name = "yColumn";
            // 
            // zColumn
            // 
            this.zColumn.HeaderText = "Z";
            this.zColumn.Name = "zColumn";
            // 
            // sensorTypeLabel
            // 
            this.sensorTypeLabel.AutoSize = true;
            this.sensorTypeLabel.Location = new System.Drawing.Point(192, 76);
            this.sensorTypeLabel.Name = "sensorTypeLabel";
            this.sensorTypeLabel.Size = new System.Drawing.Size(64, 13);
            this.sensorTypeLabel.TabIndex = 3;
            this.sensorTypeLabel.Text = "нет данных";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(23, 76);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(69, 13);
            this.label18.TabIndex = 2;
            this.label18.Text = "Тип датчика";
            // 
            // packetsArrivedLabel
            // 
            this.packetsArrivedLabel.AutoSize = true;
            this.packetsArrivedLabel.Location = new System.Drawing.Point(192, 43);
            this.packetsArrivedLabel.Name = "packetsArrivedLabel";
            this.packetsArrivedLabel.Size = new System.Drawing.Size(64, 13);
            this.packetsArrivedLabel.TabIndex = 1;
            this.packetsArrivedLabel.Text = "нет данных";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(23, 43);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(102, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Получено пакетов ";
            // 
            // sensorStopButton
            // 
            this.sensorStopButton.Image = global::Evo_20form.Properties.Resources.stop;
            this.sensorStopButton.Location = new System.Drawing.Point(300, 8);
            this.sensorStopButton.Name = "sensorStopButton";
            this.sensorStopButton.Size = new System.Drawing.Size(42, 39);
            this.sensorStopButton.TabIndex = 4;
            this.sensorStopButton.UseVisualStyleBackColor = true;
            this.sensorStopButton.Click += new System.EventHandler(this.sensorStopbutton_Click);
            // 
            // sensorPauseButton
            // 
            this.sensorPauseButton.Image = global::Evo_20form.Properties.Resources.pause_;
            this.sensorPauseButton.Location = new System.Drawing.Point(263, 7);
            this.sensorPauseButton.Name = "sensorPauseButton";
            this.sensorPauseButton.Size = new System.Drawing.Size(40, 40);
            this.sensorPauseButton.TabIndex = 3;
            this.sensorPauseButton.UseVisualStyleBackColor = true;
            this.sensorPauseButton.Click += new System.EventHandler(this.sensorPauseButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Выбирите порт";
            // 
            // comPortComboBox
            // 
            this.comPortComboBox.FormattingEnabled = true;
            this.comPortComboBox.Location = new System.Drawing.Point(100, 16);
            this.comPortComboBox.Name = "comPortComboBox";
            this.comPortComboBox.Size = new System.Drawing.Size(117, 21);
            this.comPortComboBox.TabIndex = 1;
            // 
            // startSensorButton
            // 
            this.startSensorButton.Image = global::Evo_20form.Properties.Resources.play__1_;
            this.startSensorButton.Location = new System.Drawing.Point(223, 6);
            this.startSensorButton.Name = "startSensorButton";
            this.startSensorButton.Size = new System.Drawing.Size(44, 41);
            this.startSensorButton.TabIndex = 0;
            this.startSensorButton.UseVisualStyleBackColor = true;
            this.startSensorButton.Click += new System.EventHandler(this.Start_sensor_connection_Click);
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
            this.modeComboBox.Location = new System.Drawing.Point(318, 20);
            this.modeComboBox.Name = "modeComboBox";
            this.modeComboBox.Size = new System.Drawing.Size(96, 21);
            this.modeComboBox.TabIndex = 2;
            // 
            // startButton
            // 
            this.startButton.Image = ((System.Drawing.Image)(resources.GetObject("startButton.Image")));
            this.startButton.Location = new System.Drawing.Point(477, 9);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(44, 41);
            this.startButton.TabIndex = 3;
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.start_Click);
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Image = global::Evo_20form.Properties.Resources.stop;
            this.stopButton.Location = new System.Drawing.Point(559, 9);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(44, 41);
            this.stopButton.TabIndex = 8;
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(163, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(134, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Выберете режим работы";
            // 
            // pauseButton
            // 
            this.pauseButton.Enabled = false;
            this.pauseButton.Image = global::Evo_20form.Properties.Resources.pause_;
            this.pauseButton.Location = new System.Drawing.Point(518, 9);
            this.pauseButton.Name = "pauseButton";
            this.pauseButton.Size = new System.Drawing.Size(44, 41);
            this.pauseButton.TabIndex = 7;
            this.pauseButton.UseVisualStyleBackColor = true;
            this.pauseButton.Visible = false;
            this.pauseButton.Click += new System.EventHandler(this.pauseButton_Click);
            // 
            // parameGroupBox
            // 
            this.parameGroupBox.Controls.Add(this.countTemperaturesReachedLabel);
            this.parameGroupBox.Controls.Add(this.label19);
            this.parameGroupBox.Controls.Add(this.timeLeftlabel);
            this.parameGroupBox.Controls.Add(this.label3);
            this.parameGroupBox.Location = new System.Drawing.Point(166, 51);
            this.parameGroupBox.Name = "parameGroupBox";
            this.parameGroupBox.Size = new System.Drawing.Size(279, 92);
            this.parameGroupBox.TabIndex = 9;
            this.parameGroupBox.TabStop = false;
            this.parameGroupBox.Text = "Текущие параметры";
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
            this.workTimer.Interval = 1000;
            this.workTimer.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // settingsButton
            // 
            this.settingsButton.Location = new System.Drawing.Point(16, 15);
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new System.Drawing.Size(95, 26);
            this.settingsButton.TabIndex = 10;
            this.settingsButton.Text = "Настройки";
            this.settingsButton.UseVisualStyleBackColor = true;
            this.settingsButton.Click += new System.EventHandler(this.settingsButton_Click);
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // readDatabutton
            // 
            this.readDatabutton.Location = new System.Drawing.Point(671, 9);
            this.readDatabutton.Name = "readDatabutton";
            this.readDatabutton.Size = new System.Drawing.Size(178, 32);
            this.readDatabutton.TabIndex = 11;
            this.readDatabutton.Text = "Получить данные из файла ";
            this.readDatabutton.UseVisualStyleBackColor = true;
            this.readDatabutton.Click += new System.EventHandler(this.button1_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1131, 646);
            this.Controls.Add(this.readDatabutton);
            this.Controls.Add(this.settingsButton);
            this.Controls.Add(this.parameGroupBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.pauseButton);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.modeComboBox);
            this.Controls.Add(this.tabControl1);
            this.Name = "MainForm";
            this.Text = "Evo20";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.tabControl1.ResumeLayout(false);
            this.Evo_control_page.ResumeLayout(false);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button StartEvoButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage Evo_control_page;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage Sensor_control_page;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comPortComboBox;
        private System.Windows.Forms.Button startSensorButton;
        private System.Windows.Forms.Button PauseEvoButton;
        private System.Windows.Forms.Button StopEvoButton;
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
        private System.Windows.Forms.Label packetsArrivedLabel;
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
        private System.Windows.Forms.Button settingsButton;
        private System.Windows.Forms.Button savePacketsButton;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Button readDatabutton;
    }
}

