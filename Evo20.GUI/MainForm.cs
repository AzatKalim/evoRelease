using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;
using System.Configuration;

using ZedGraph;
using Evo20.Controllers;
using Evo20.Log;
using Evo20.EvoConnections;



namespace Evo_20form
{
    public partial class MainForm : Form
    {
        const int DRAW_LINE_LENGTH = 20;

        Controller controller;
        DateTime startTime;
        double prevX;
        double prevY;

        bool isStabilized=true;

        DateTime stabilizationStartTime;

        bool isSettingsEntered= false;

        bool IsStabilized
        {
            set
            {
                if(value==false)
                    stabilizationStartTime= DateTime.Now;
                isStabilized=false;
            }
            get
            {
                return isStabilized;
            }
        }

        string settingsFileName = ConfigurationManager.AppSettings.Get("DEFAULT_SETTINGS_FILE_NAME");

        #region Form load-close functions

        public MainForm()
        {           
            InitializeComponent();
            prevX = 0;
            prevY = 0;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            string[] comPorts = SerialPort.GetPortNames();
            comPortComboBox.DataSource = comPorts;
            controller = new Controller();
            controller.EventHandlersListCycleEnded += CycleEndedHandler;
            controller.EventListForEvoConnectionChange += EvoConnectionChangeHandler;
            controller.EventListForWorkModeChange += EvoWorkModeChangeHandler;
            controller.EventHandlerListForControllerExceptions += ControllerExсeptoinsHandler;
            controller.EventHandlerListForSensorConnectionChange += SensorConnectionChangeHandler;
            controller.EventHandlerListForTemperatureStabilization += TemperatureStabilizationHandler;
            startTime = DateTime.Now;
            graph.GraphPane.XAxis.Title = "Время";
            graph.GraphPane.YAxis.Title = "Температура";
            graph.GraphPane.Title = "График температуры от времени";
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            workTimer.Stop();
            timer.Stop();
            controller.StopComPortConnection();
            controller.StopEvoConnection();
            controller.Stop();
        }

        ~MainForm()
        {
            workTimer.Stop();
            timer.Stop();
            controller.StopComPortConnection();
            controller.StopEvoConnection();
            controller.Stop();
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
                DialogResult result = MessageBox.Show("Проблемы с файлом: возникло исключение"+ ex.ToString(), "Открыть файл настроек ?", MessageBoxButtons.YesNo);
            }
            controller.StartEvoConnection();
            startTime = DateTime.Now;
            workTimer.Start();
            timer.Start();
            evoStopButton.Enabled = true;
            evoPauseButton.Enabled = true;
        }

        private void evoPauseButton_Click(object sender, EventArgs e)
        {
            controller.PauseEvoConnection();
            evoPauseButton.Enabled = false;
            evoStartButton.Enabled = true;
        }

        private void evoStopButton_Click(object sender, EventArgs e)
        {
            controller.StopEvoConnection();
            evoStopButton.Enabled = false;
            evoPauseButton.Enabled = false;
            evoStartButton.Enabled = true;
        }

        #endregion

        #region Sensor buttons

        private void sensorStartButton_Click(object sender, EventArgs e)
        {
            if (comPortComboBox.SelectedItem != null)
            {
                controller.StartComPortConnection(comPortComboBox.SelectedItem.ToString());
            }
            else
            {
                string caption = "OK";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                string message = "Порт не выбран!";
                MessageBox.Show(message, caption, buttons);
                return;
            }
            SensorDataGridView.Visible = true;
            SensorDataGridView.Rows.Add("Гироскопы", "0", "0", "0");
            SensorDataGridView.Rows.Add("Температуры гироскопов", "0", "0", "0");
            SensorDataGridView.Rows.Add("Акселерометры", "0", "0", "0");
            SensorDataGridView.Rows.Add("Температуры акселерометров", "0", "0", "0");           
            sensorStopButton.Enabled = true;
            sensorPauseButton.Enabled = true;
        }

        private void sensorPauseButton_Click(object sender, EventArgs e)
        {
            controller.PauseComPortConnection();
            sensorStartButton.Enabled = true;
            sensorPauseButton.Enabled = false;
        }

        private void sensorStopButton_Click(object sender, EventArgs e)
        {
            controller.StopComPortConnection();
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
                {
                    return;
                }
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
            if(modeComboBox.SelectedItem==null)
            {
                MessageBox.Show("Ошибка: не выбран режим работы ", "Небходимо выбрать режим работы перед нажатием пуск !", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            WorkMode workMode= WorkMode.NoMode;
            switch(modeComboBox.SelectedIndex)
            {
                case 0:
                    workMode=WorkMode.CalibrationMode;
                    break;
                case 1:
                    workMode= WorkMode.CheckMode;
                    break;
            }
            try
            {
                bool isStarted = controller.Start(comPortComboBox.SelectedItem.ToString(), workMode);
                if (!isStarted)
                {
                    MessageBox.Show("Ошибка:запуска цикла! ", "Возникла ошибка,цикл не запущен", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    controller.Stop();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: запуска цикла!", "Возникла ошибка " + ex.Message + " цикл не запущен! ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                controller.Stop();
                return;
            }
                  
            SensorDataGridView.Visible = true;
            SensorDataGridView.Rows.Add("Гироскопы", "0", "0", "0");
            SensorDataGridView.Rows.Add("Акселерометры", "0", "0", "0");
            SensorDataGridView.Rows.Add("Температуры гироскопов", "0", "0", "0");
            SensorDataGridView.Rows.Add("Температуры акселерометров", "0", "0", "0");            
            workTimer.Start();
            timer.Start();

            settingsButton.Enabled = false;

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

        private void pauseButton_Click(object sender, EventArgs e)
        {
            controller.Pause();

            startButton.Enabled = true;
            pauseButton.Enabled = false;          
            stopButton.Enabled = true;

            evoStartButton.Enabled = true;
            evoPauseButton.Enabled = false;
            evoStopButton.Enabled = true;

            sensorStartButton.Enabled = true;         
            sensorPauseButton.Enabled = false;
            sensorStopButton.Enabled = true;
          
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            controller.Stop();
            ResetForm();
        }

        #endregion

        #region Other buttons 

        private void settingsButton_Click(object sender, EventArgs e)
        {
            if(File.Exists(settingsFileName))
            {
                var procces = System.Diagnostics.Process.Start(settingsFileName);
                procces.WaitForExit();
                ReadSettings();
            }         
        }
        private void cycleSettingsButton_Click(object sender, EventArgs e)
        {
            if (controller.cycleData == null)
                ReadSettings();
            var cycleForm = new CycleSettingsForm(controller.cycleData);
            cycleForm.ShowDialog();
        }

        private void savePacketsButton_Click(object sender, EventArgs e)
        {
            WritePackets();          
        }

        private void readDataButton_Click(object sender, EventArgs e)
        {
            //return;
            var dlg = new OpenFileDialog();
            dlg.Filter = "Все файлы (*.*)|*.*";
            dlg.CheckFileExists = true;
            var res = dlg.ShowDialog();

            if (res != DialogResult.OK)
            {
                return;
            }
            var file = new StreamReader(dlg.FileName);
            bool result = true;
            try
            {
                if (!controller.ReadDataFromFile(file))
                {
                    MessageBox.Show("Ошибка: чтения пакетов из файла", "Не удалось считать пакеты из файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Evo20.Log.Log.WriteLog("Ошибка: чтения пакетов из файла: Не удалось считать пакеты из файла");
                    result = false;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(),"Ошибка: чтения пакетов из файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Evo20.Log.Log.WriteLog(string.Format("Ошибка: чтения пакетов из файла: возникло исключение{0}",exception.ToString()));               
            }
            file.Close();
        }

        #endregion

        #endregion

        #region Timers tick functions

        private void workTimer_Tick(object sender, EventArgs e)
        {
            CurrentTemperatureLabel.Text = controller.evoData.currentTemperature.ToString();
            nextTemperatureLable.Text = controller.evoData.nextTemperature.ToString();
            CheckParam(controller.evoData.isCameraPowerOn, powerCameraIndic);
            CheckParam(controller.evoData.isTemperatureReached, temperatureReachedIndic);
            CheckParam(controller.evoData.x.isZeroFound, xZeroFindedIndic);
            CheckParam(controller.evoData.y.isZeroFound, yZeroFindedIndic);
            CheckParam(controller.evoData.x.isPowerOn, xPowerIndic);
            CheckParam(controller.evoData.y.isPowerOn, yPowerIndic);
            CheckParam(controller.evoData.x.isMove, xMoveIndic);
            CheckParam(controller.evoData.y.isMove, yMoveIndic);
            xPositionLabel.Text = controller.evoData.x.position.ToString();
            yPositionLabel.Text = controller.evoData.y.position.ToString();
            xSpeedOfRateLabel.Text = controller.evoData.x.speedOfRate.ToString();
            ySpeedOfRateLabel.Text = controller.evoData.y.speedOfRate.ToString();
            ShowSensorParams();
            DrawGrapfic();

            if (!IsStabilized)
            {
                var t = TimeSpan.FromMilliseconds(controller.StabilizationTime - (DateTime.Now - stabilizationStartTime).TotalMilliseconds);
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
            timeLeftlabel.Text = difference.Hours + ":" + difference.Minutes + ":" + difference.Seconds;
        }

        #endregion

        #region Controller's events handlers

        private void EvoConnectionChangeHandler(Evo20.EvoConnections.ConnectionStatus state)
        {
            connectionStateLabel.Invoke(new Action(() => { connectionStateLabel.Text = state.ToString(); }));
        }

        private void SensorConnectionChangeHandler(Evo20.SensorsConnection.ConnectionStatus state)
        {
            sensorConnectionStateLabel.Invoke(new Action(() =>
            {
                sensorConnectionStateLabel.Text = state.ToString();
            }));
        }

        private void EvoWorkModeChangeHandler(WorkMode mode)
        {
            controllerWokModeLabel.Invoke(new Action(() => { controllerWokModeLabel.Text = mode.ToString(); }));
        }

        private void ControllerExсeptoinsHandler(Exception exception)
        {
            string message = string.Format("Возникла ошибка во время работы !");
            DialogResult diaologResult = MessageBox.Show(exception.Message, message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            ResetForm();
        }

        private void TemperatureStabilizationHandler(bool result)
        {
            IsStabilized=result;
        }

        #endregion

        #region Other functions

        public bool WritePackets()
        {
            var dlg = new SaveFileDialog();
            dlg.Filter = "Все файлы (*.*)|*.*";
            dlg.CheckFileExists = true;
            var res = dlg.ShowDialog();

            if (res != DialogResult.OK)
            {
                return false;
            }
            bool result=true;
            using (StreamWriter file = new StreamWriter(dlg.FileName))
            {
                try
                {
                    if (!controller.WritePackets(file))
                    {
                        MessageBox.Show("Ошибка: записи файлов!", "Не удалось записать информацию в файл.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Evo20.Log.Log.WriteLog("Ошибка: записи файлов.Не удалось записать информацию в файл");        
                        result = false;
                    }
                }
                catch(Exception exception)
                {
                    result = false;
                    Evo20.Log.Log.WriteLog(string.Format("Ошибка записи файла пакетов. Возникло исключение {0}",exception.ToString()));
                }
            }
            return result;
        }
        public void ResetForm()
        {
            startButton.Invoke(new Action(() => {startButton.Enabled= true; }));
            pauseButton.Invoke(new Action(() => {pauseButton.Enabled = false; }));
            stopButton.Invoke(new Action(() => {stopButton.Enabled = false; }));

            evoStartButton.Invoke(new Action(() => { evoStartButton.Enabled = true; }));
            evoPauseButton.Invoke(new Action(() => { evoPauseButton.Enabled = false; }));
            evoStopButton.Invoke(new Action(() => { evoStopButton.Enabled = false; }));

            sensorStartButton.Invoke(new Action(() => { sensorStartButton.Enabled = true; }));
            sensorPauseButton.Invoke(new Action(() => { sensorPauseButton.Enabled = false; }));
            sensorStopButton.Invoke(new Action(() => { sensorStopButton.Enabled = false; }));

            SensorDataGridView.Invoke(new Action(() => { SensorDataGridView.Rows.Clear(); }));
            SensorDataGridView.Invoke(new Action(() => { SensorDataGridView.Visible=false; }));

            settingsButton.Invoke(new Action(() => {settingsButton.Enabled=true;}));
            workTimer.Stop();
            timer.Stop();
        }

        private void CycleEndedHandler(bool result)
        {
            if (!result)
            {
                string message = " Возникла ошибка смотрите log файл";
                MessageBox.Show("Ошибка: цикл завершился неуспешно !" ,message,MessageBoxButtons.OK);
            }
            else
            {
                WritePackets();
                string message = "Цикл окончен!";
                DialogResult diaologResult = MessageBox.Show(message, "Выполнить расчет коэффицентов ?", MessageBoxButtons.YesNo,MessageBoxIcon.Information);
                if (diaologResult == DialogResult.Yes)
                {
                    ComputeCoefficents();
                }
            }           
        }

        private void DrawGrapfic()
        {
            double x = (DateTime.Now - startTime).TotalMinutes;
            double y = controller.CurrentTemperature;
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
            if (!controller.ComputeCoefficents(file))
            {
                MessageBox.Show("Проблемы с файлом", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            file.Close();
        }

        private void ShowSensorParams()
        {
            packetsArrivedLabel.Text = controller.PacketsCollectedCount.ToString();
            if (controller.CurrentSensor == null)
            {
                return;
            }
            sensorTypeLabel.Text = controller.CurrentSensor.Name;
            if (SensorDataGridView.Visible == false)
            {
                SensorDataGridView.Visible = true;
            }
            List<double> data = controller.GetSensorData();
            if (data == null)
            {
                return;
            }
            int k = 0;
            for (int j = 0; j < SensorDataGridView.Rows.Count - 1; j++)
            {
                for (int i = 1; i < 4; i++)
                {
                    SensorDataGridView.Rows[j].Cells[i].Value = Math.Round(data[k++],3);
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
            var result=controller.ReadSettings(settingsFileName);
            if (result)
                isSettingsEntered = true;
            return result;
        }
    
        #endregion
    
    }
}
