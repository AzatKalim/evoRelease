using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;
using System.Configuration;

using ZedGraph;
using Evo20.Controllers;
using Evo20;
using Evo20.EvoConnections;



namespace Evo20.GUI
{
    public partial class MainForm : Form
    {
        const int DRAW_LINE_LENGTH = 20;
        DateTime startTime;
        double prevX;
        double prevY;

        bool isStabilized = true;

        DateTime stabilizationStartTime;

        bool isSettingsEntered = false;

        bool IsStabilized
        {
            set
            {
                if (value == false)
                    stabilizationStartTime = DateTime.Now;
                isStabilized = value;
            }
            get
            {
                return isStabilized;
            }
        }

        string settingsFileName;

        delegate void FormReseter();

        delegate void WorkModDel(WorkMode mode);
        delegate void SensorConnectionDel(ConnectionStatus state);
        delegate void EvoConnectionDel(ConnectionStatus state);

        #region Form load-close functions

        public MainForm()
        {
            InitializeComponent();
            prevX = 0;
            prevY = 0;
            settingsFileName = Config.Instance.DefaultSettingsFileName;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            string[] comPorts = SerialPort.GetPortNames();
            comPortComboBox.DataSource = comPorts;
            Controller.Instance.EventHandlersListCycleEnded += CycleEndedHandler;
            ControllerEvo.Instance.EventListForEvoConnectionChange += EvoConnectionChangeHandler;
            Controller.Instance.EventListForWorkModeChange += EvoWorkModeChangeHandler;
            SensorController.Instance.SensorControllerException += ControllerExсeptoinsHandler;
            SensorController.Instance.SensorConnectionChanged += SensorConnectionChangeHandler;
            Controller.Instance.EventHandlerListForTemperatureStabilization += TemperatureStabilizationHandler;
            startTime = DateTime.Now;
            graph.GraphPane.XAxis.Title = "Время";
            graph.GraphPane.YAxis.Title = "Температура";
            graph.GraphPane.Title = "График температуры от времени";
            lblVersion.Text = "Версия: " +Application.ProductVersion;
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            workTimer.Stop();
            timer.Stop();
            SensorController.Instance.StopComPortConnection();
            ControllerEvo.Instance.StopEvoConnection();
            Controller.Instance.Stop();
        }

        ~MainForm()
        {
            Dispose();
            workTimer.Stop();
            timer.Stop();
            SensorController.Instance.StopComPortConnection();
            ControllerEvo.Instance.StopEvoConnection();
            Controller.Instance.Stop();
        }

        #endregion

        #region Buttons Click functions

        #region Evo buttons

        private void evoStartButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ReadSettings())
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                DialogResult result = MessageBox.Show("Проблемы с файлом: возникло исключение" + ex.ToString(), "Открыть файл настроек ?", MessageBoxButtons.YesNo);
            }
            ControllerEvo.Instance.StartEvoConnection();
            startTime = DateTime.Now;
            workTimer.Start();
            timer.Start();
            evoStopButton.Enabled = true;
            evoPauseButton.Enabled = true;
        }

        private void evoPauseButton_Click(object sender, EventArgs e)
        {
            ControllerEvo.Instance.PauseEvoConnection();
            evoPauseButton.Enabled = false;
            evoStartButton.Enabled = true;
        }

        private void evoStopButton_Click(object sender, EventArgs e)
        {
            ControllerEvo.Instance.StopEvoConnection();
            evoStopButton.Enabled = false;
            evoPauseButton.Enabled = false;
            evoStartButton.Enabled = true;
        }

        #endregion

        #region Sensor buttons

        private void sensorStartButton_Click(object sender, EventArgs e)
        {
            bool result = false;
            if (comPortComboBox.SelectedItem != null)
            {
                result = SensorController.Instance.StartComPortConnection(comPortComboBox.SelectedItem.ToString());
            }
            else
            {
                string caption = "OK";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                string message = "Порт не выбран!";
                MessageBox.Show(message, caption, buttons);
                return;
            }
            SensorDataGridView.Rows.Clear();
            if (!result)
            {
                MessageBox.Show("Проблемы с com портом", "Не удалось запустить соединение!", MessageBoxButtons.OK);
                return;
            }
            SensorDataGridView.Visible = true;
            SensorDataGridView.Rows.Add("Гироскопы", "0", "0", "0");
            SensorDataGridView.Rows.Add("Температуры гироскопов", "0", "0", "0");
            SensorDataGridView.Rows.Add("Акселерометры", "0", "0", "0");
            SensorDataGridView.Rows.Add("Температуры акселерометров", "0", "0", "0");
            sensorStopButton.Enabled = true;
            sensorPauseButton.Enabled = true;
            SensorTimer.Start();
        }

        private void sensorPauseButton_Click(object sender, EventArgs e)
        {
            SensorController.Instance.PauseComPortConnection();
            sensorStartButton.Enabled = true;
            sensorPauseButton.Enabled = false;
        }

        private void sensorStopButton_Click(object sender, EventArgs e)
        {
            SensorController.Instance.StopComPortConnection();
            sensorStopButton.Enabled = false;
            sensorPauseButton.Enabled = false;
            sensorStartButton.Enabled = true;
        }

        #endregion

        #region Main start, pause, stop

        private void startButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ReadSettings())
                    return;
            }
            catch (Exception ex)
            {
                DialogResult result = MessageBox.Show("Проблемы с файлом: возникло исключение " + ex.Message, "Открыть файл настроек ?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            }
            if (comPortComboBox.SelectedItem == null)
            {
                MessageBox.Show("Ошибка: Com порт не выбран!", "Выбирете один из портов для соединения !", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (modeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Ошибка: не выбран режим работы ", "Небходимо выбрать режим работы перед нажатием пуск !", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            WorkMode workMode = WorkMode.NoMode;
            switch (modeComboBox.SelectedIndex)
            {
                case 0:
                    workMode = WorkMode.CalibrationMode;
                    break;
                case 1:
                    workMode = WorkMode.CheckMode;
                    break;
            }

            using (var fbd = new FolderBrowserDialog())
            {
                var result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrEmpty(fbd.SelectedPath))
                {
                     FileController.filesPath= fbd.SelectedPath;
                     Log.Instance.Info("Выбрана папка {0}", fbd.SelectedPath);
                }
                else
                {
                    Log.Instance.Error("Папка не выбрана");
                    MessageBox.Show("Ошибка: не выбрана папка ", "Небходимо выбрать папку для сохранения файлов !", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            try
            {
                bool isStarted = Controller.Instance.Start(comPortComboBox.SelectedItem.ToString(), workMode);
                if (!isStarted)
                {
                    MessageBox.Show("Ошибка:запуска цикла! ", "Возникла ошибка,цикл не запущен", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log.Instance.Error("Ошибка:запуска цикла! Возникла ошибка,цикл не запущен");
                    Controller.Instance.Stop();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: запуска цикла!", "Возникла ошибка " + ex.Message + " цикл не запущен! ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.Instance.Error("Ошибка: запуска цикла!.Возникла ошибка {0} цикл не запущен! ",ex.Message);
                Controller.Instance.Stop();
                return;
            }
            SensorDataGridView.Rows.Clear();
            SensorDataGridView.Visible = true;
            SensorDataGridView.Rows.Add("Гироскопы", "0", "0", "0");
            SensorDataGridView.Rows.Add("Температуры гироскопов", "0", "0", "0");
            SensorDataGridView.Rows.Add("Акселерометры", "0", "0", "0");
            SensorDataGridView.Rows.Add("Температуры акселерометров", "0", "0", "0");

            workTimer.Start();
            timer.Start();
            SensorTimer.Start();

            SettingsToolStripMenuItem.Enabled = false;
            FileToolStripMenuItem.Enabled = false;
            startButton.Enabled = false;
            pauseButton.Enabled = true;
            stopButton.Enabled = true;

            evoStartButton.Enabled = false;
            evoPauseButton.Enabled = true;
            evoStopButton.Enabled = true;

            sensorStartButton.Enabled = false;
            sensorPauseButton.Enabled = true;
            sensorStopButton.Enabled = true;
        }

        //private void pauseButton_Click(object sender, EventArgs e)
        //{
        //    Controller.Current.Pause();

        //    startButton.Enabled = true;
        //    pauseButton.Enabled = false;
        //    stopButton.Enabled = true;

        //    evoStartButton.Enabled = true;
        //    evoPauseButton.Enabled = false;
        //    evoStopButton.Enabled = true;

        //    sensorStartButton.Enabled = true;
        //    sensorPauseButton.Enabled = false;
        //    sensorStopButton.Enabled = true;

        //}

        private void stopButton_Click(object sender, EventArgs e)
        {
            Controller.Instance.Stop();
            ResetForm();
        }

        #endregion

        #region Other buttons       
     
        private void getDataFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ReadSettings())            
                return;
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrEmpty(fbd.SelectedPath))
                {
                    FileController.filesPath = fbd.SelectedPath;
                    Log.Instance.Info("Выбрана папка для загрузки {0}", fbd.SelectedPath);
                }
                else
                {
                    Log.Instance.Error("Папка не выбрана");
                    MessageBox.Show("Ошибка: не выбрана папка ", "Небходимо выбрать папку для четния файлов !", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }      
            try
            {
                if (!Controller.Instance.ReadDataFromFile())
                {
                    MessageBox.Show("Ошибка: чтения пакетов из файла", "Не удалось считать пакеты из файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log.Instance.Error("Ошибка: чтения пакетов из файла: Не удалось считать пакеты из файла");                   
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "Ошибка: чтения пакетов из файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.Instance.Exception(exception);
            }
        }

        #endregion

        #endregion

        #region Timers tick functions

        private void workTimer_Tick(object sender, EventArgs e)
        {
            if (Controller.Instance.TemperutureIndex < 0)
                countTemperaturesReachedLabel.Text = string.Format("{0}/{1}", 0, Controller.Instance.TemperaturesCount);
            else
                countTemperaturesReachedLabel.Text = string.Format("{0}/{1}", Controller.Instance.TemperutureIndex, Controller.Instance.TemperaturesCount);
            if (SensorController.Instance.CurrentPositionNumber < 0)
                currentPositionNumberLable.Text = string.Format("{0}/{1}", 0, SensorController.Instance.CurrentPositionCount);
            else
                currentPositionNumberLable.Text = string.Format("{0}/{1}", SensorController.Instance.CurrentPositionNumber, SensorController.Instance.CurrentPositionCount);
            CurrentTemperatureLabel.Text = EvoData.Instance.CurrentTemperature.ToString();
            nextTemperatureLable.Text = EvoData.Instance.NextTemperature.ToString();
            CheckParam(EvoData.Instance.isCameraPowerOn, powerCameraIndic);
            CheckParam(EvoData.Instance.isTemperatureReached, temperatureReachedIndic);
            CheckParam(EvoData.Instance.X.isZeroFound, xZeroFindedIndic);
            CheckParam(EvoData.Instance.Y.isZeroFound, yZeroFindedIndic);
            CheckParam(EvoData.Instance.X.isPowerOn, xPowerIndic);
            CheckParam(EvoData.Instance.Y.isPowerOn, yPowerIndic);
            CheckParam(EvoData.Instance.X.isMove, xMoveIndic);
            CheckParam(EvoData.Instance.Y.isMove, yMoveIndic);
            xPositionLabel.Text = EvoData.Instance.X.position.ToString();
            yPositionLabel.Text = EvoData.Instance.Y.position.ToString();
            xSpeedOfRateLabel.Text = EvoData.Instance.X.speedOfRate.ToString();
            ySpeedOfRateLabel.Text = EvoData.Instance.Y.speedOfRate.ToString();
            DrawGrapfic();

            if (!IsStabilized)
            {
                var t = TimeSpan.FromMilliseconds(Controller.Instance.StabilizationTime - (DateTime.Now - stabilizationStartTime).TotalMilliseconds);
                temperatureStabLabel.Text = string.Format("{0:D2}:{1:D2}:{2:D2}",
                        t.Hours,
                        t.Minutes,
                        t.Seconds);
            }
            else
            {
                temperatureStabLabel.Text = "Стабилизирована";
            }

        }

        private void timer_Tick(object sender, EventArgs e)
        {
            TimeSpan difference = DateTime.Now - startTime;
            difference -= TimeSpan.FromMilliseconds(difference.Milliseconds);
            timeLeftlabel.Text = string.Format("{0,2}:{1,2}:{2,2}",difference.Hours,difference.Minutes,difference.Seconds);
        }

        private void SensorTimer_Tick(object sender, EventArgs e)
        {
            ShowSensorParams();
        }

        #endregion

        #region Controller's events handlers

        private void EvoConnectionChange(ConnectionStatus state)
        {
            connectionStateLabel.Text = state.ToText();
        }

        private void EvoConnectionChangeHandler(ConnectionStatus state)
        {           
            EvoConnectionDel del = EvoConnectionChange;
            connectionStateLabel.Invoke(del,state);
        }

        private void SensorConnectionChange(ConnectionStatus state)
        {
            sensorConnectionStateLabel.Text = state.ToText();
        }

        private void SensorConnectionChangeHandler(ConnectionStatus state)
        {
            SensorConnectionDel del = SensorConnectionChange;
            sensorConnectionStateLabel.Invoke(del,state);
        }

        private void EvoWorkModeChange(WorkMode mode)
        {
            controllerWokModeLabel.Text = mode.ToView();
        }

        private void EvoWorkModeChangeHandler(WorkMode mode)
        {
            WorkModDel del = EvoWorkModeChange;
            controllerWokModeLabel.Invoke(del,mode);
        }

        private void ControllerExсeptoinsHandler(Exception exception)
        {
            string message = string.Format("Возникла ошибка во время работы !");
            DialogResult diaologResult = MessageBox.Show(exception.Message, message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            ResetForm();
        }

        private void TemperatureStabilizationHandler(bool result)
        {
            IsStabilized = result;
        }

        #endregion

        #region ToolStripMenu handlers

        private void cycleSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!isSettingsEntered)
            {
                ReadSettings();
                isSettingsEntered = true;
            }
            var cycleForm = new CycleSettingsForm();
            cycleForm.ShowDialog();
        }

        private void CommonSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(settingsFileName))
            {
                var procces = System.Diagnostics.Process.Start(settingsFileName);
                procces.WaitForExit();
                ReadSettings();
            }
        }

        private void savePacketsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WritePackets();
        }

        #endregion

        #region Other functions

        public bool WritePackets()
        {
            var dlg = new SaveFileDialog();
            dlg.Filter = "Все файлы (*.*)|*.*";
            dlg.CheckFileExists = false;
            var res = dlg.ShowDialog();

            if (res != DialogResult.OK)
            {
                return false;
            }
            bool result = true;
            Log.Instance.Info("Старт записи в файл");
            using (StreamWriter file = new StreamWriter(dlg.FileName))
            {
                try
                {
                    if (!Controller.Instance.WritePackets(file))
                    {
                        MessageBox.Show("Ошибка: записи файлов!", "Не удалось записать информацию в файл.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Log.Instance.Error("Ошибка: записи файлов.Не удалось записать информацию в файл");
                        result = false;
                    }
                }
                catch (Exception exception)
                {
                    result = false;
                    Log.Instance.Error("Ошибка записи файла пакетов.");
                    Log.Instance.Exception(exception);
                }
            }
            return result;
        }

        public void ResetForm()
        {
            FormReseter del = Reset;
            startButton.Invoke(del);
            workTimer.Stop();
            timer.Stop();
        }

        private void Reset()
        {
            startButton.Enabled = true;
            pauseButton.Enabled = false; 
            stopButton.Enabled = false; 

            evoStartButton.Enabled = true;
            evoPauseButton.Enabled = false; 
            evoStopButton.Enabled = false; 

            sensorStartButton.Enabled = true; 
            sensorPauseButton.Enabled = false; 
            sensorStopButton.Enabled = false; 

            //SensorDataGridView.Rows.Clear(); 
            SensorDataGridView.Visible = false;
            FileToolStripMenuItem.Enabled = true;
            SettingsToolStripMenuItem.Enabled = true; 
        }
        private void CycleEndedHandler(bool result)
        {
            if (!result)
            {
                string message = " Возникла ошибка смотрите log файл";
                MessageBox.Show("Ошибка: цикл завершился неуспешно !", message, MessageBoxButtons.OK);
            }
            else
            {
                WritePackets();
                string message = "Цикл окончен!";
                DialogResult diaologResult = MessageBox.Show(message, "Выполнить расчет коэффицентов ?", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (diaologResult == DialogResult.Yes)
                {
                    ComputeCoefficents();
                }
            }
        }

        private void DrawGrapfic()
        {
            double x = (DateTime.Now - startTime).TotalMinutes;
            double y = ControllerEvo.Instance.CurrentTemperature;
            double[] tempX = new double[] { prevX, x };
            double[] tempY = new double[] { prevY, y };
            graph.GraphPane.AddCurve("", tempX, tempY, Color.Red, SymbolType.None);
            graph.AxisChange();
            graph.Invalidate();
            if (graph.GraphPane.CurveList.Count > 100)
            {
                graph.GraphPane.CurveList.RemoveAt(0);
            }
            prevX = x;
            prevY = y;
        }

        private void CheckParam(bool param, PictureBox picture)
        {
            if (param)
                picture.BackColor = Color.Green;
            else
                picture.BackColor = Color.Red;
        }

        private void ComputeCoefficents()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Все файлы (*.*)|*.*";
            dlg.CheckFileExists = true;
            DialogResult res = dlg.ShowDialog();

            if (res != DialogResult.OK)
            {
                return;
            }
            string FileName = dlg.FileName;
            StreamWriter file = new StreamWriter(FileName);
            if (!Controller.Instance.ComputeCoefficents(file))
            {
                MessageBox.Show("Проблемы с файлом", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            file.Close();
        }

        private void ShowSensorParams()
        {
            packetsArrivedLabel.Text = SensorController.Instance.PacketsCollectedCount.ToString();
            if (SensorController.Instance.CurrentSensor != null)
            {
                sensorTypeLabel.Text = SensorController.Instance.CurrentSensor.Name;
                SensorDataGridView.Visible = true;
            }
            var data = SensorController.Instance.GetSensorData();
            if (data == null)
                return;
            int k = 0;
            for (int j = 0; j < SensorDataGridView.Rows.Count; j++)
            {
                for (int i = 1; i < 4; i++)
                {
                    SensorDataGridView.Rows[j].Cells[i].Value = System.Math.Round(data[k++], 5);
                }
            }
        }

        private bool ReadSettings()
        {
            if (isSettingsEntered)
            {
                return true;
            }
            //проверка нахождения этого файла в директории
            if (!File.Exists(settingsFileName))
            {
                string message = "Файл " + settingsFileName + "  не найден!";
                var dialogResult = MessageBox.Show("Открыть другой файл настроек ?", message, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error);
                if (dialogResult == DialogResult.None || dialogResult == DialogResult.No)
                    return false;
                //Пользователь выбирает другой файл.
                if (dialogResult == DialogResult.Yes)
                {
                    var openFileDialog = new OpenFileDialog();
                    openFileDialog.Filter = "txt files (*.txt)";
                    openFileDialog.RestoreDirectory = true;
                    dialogResult = openFileDialog.ShowDialog();
                    if (dialogResult == DialogResult.Yes)
                    {
                        settingsFileName = openFileDialog.FileName;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            var result = FileController.Instance.ReadSettings(settingsFileName);
            if (result)
            {
                isSettingsEntered = true;
            }
            else
                MessageBox.Show("Проблемы чтения файла настроек", "Проблемы чтения из файла!", MessageBoxButtons.OK);
            return result;
        }

        #endregion

        private void currentPositionNumberLable_Click(object sender, EventArgs e)
        {

        }
    }
}
