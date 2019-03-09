using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;
using ZedGraph;
using Evo20.Controllers;
using Evo20.Controllers.Data;
using Evo20.Controllers.EvoControllers;
using Evo20.Controllers.FileWork;
using Evo20.Controllers.SensorController;
using Evo20.Utils.EventArguments;
using Evo20.Utils;

namespace Evo20.GUI
{
    public partial class MainForm : Form
    {
        DateTime _startTime;
        double _prevX;
        double _prevY;

        bool _isStabilized = true;

        DateTime _stabilizationStartTime;

        bool _isSettingsEntered;

        bool IsStabilized
        {
            set
            {
                if (value == false)
                    _stabilizationStartTime = DateTime.Now;
                _isStabilized = value;
            }
            get
            {
                return _isStabilized;
            }
        }

        string _settingsFileName = Config.Instance.DefaultSettingsFileName;

        delegate void FormReseter();

        delegate void WorkModDel(WorkMode mode);
        delegate void SensorConnectionDel(ConnectionStatus state);
        delegate void EvoConnectionDel(ConnectionStatus state);

        delegate void CompCoeff();

        #region Form load-close functions

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var comPorts = SerialPort.GetPortNames();
            comPortComboBox.DataSource = comPorts;
            Controller.Instance.CycleEndedEvent += CycleEndedHandler;
            ControllerEvo.Instance.EvoConnectionChanged += EvoConnectionChangeHandler;
            Controller.Instance.WorkModeChanged += EvoWorkModeChangeHandler;
            Controller.Instance.ControllerExceptionEvent += ControllerExсeptoinsHandler;
            SensorController.Instance.SensorControllerException += ControllerExсeptoinsHandler;
            SensorController.Instance.SensorConnectionChanged += SensorConnectionChangeHandler;
            Controller.Instance.TemperatureStabilized += TemperatureStabilizationHandler;
            _startTime = DateTime.Now;
            graph.GraphPane.XAxis.Title = @"Время";
            graph.GraphPane.YAxis.Title = @"Температура";
            graph.GraphPane.Title = @"График температуры от времени";
            lblVersion.Text = @"Версия: " +Application.ProductVersion;
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
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
                MessageBox.Show(@"Проблемы с файлом: возникло исключение" + ex, @"Открыть файл настроек ?", MessageBoxButtons.YesNo);
            }
            ControllerEvo.Instance.StartEvoConnection();
            _startTime = DateTime.Now;
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
            bool result;
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
                MessageBox.Show(@"Проблемы с com портом", @"Не удалось запустить соединение!", MessageBoxButtons.OK);
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
                MessageBox.Show(@"Проблемы с файлом: возникло исключение " + ex.Message, @"Открыть файл настроек ?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            }
            if (comPortComboBox.SelectedItem == null)
            {
                MessageBox.Show(@"Ошибка: Com порт не выбран!", @"Выбирете один из портов для соединения !", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (modeComboBox.SelectedItem == null)
            {
                MessageBox.Show(@"Ошибка: не выбран режим работы ", @"Небходимо выбрать режим работы перед нажатием пуск !", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                     FileController.FilesPath= fbd.SelectedPath;
                     Log.Instance.Info(@"Выбрана папка {0}", fbd.SelectedPath);
                }
                else
                {
                    Log.Instance.Error("Папка не выбрана");
                    MessageBox.Show(@"Ошибка: не выбрана папка ",@"Небходимо выбрать папку для сохранения файлов !", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            try
            {
                bool isStarted = Controller.Instance.Start(comPortComboBox.SelectedItem.ToString(), workMode);
                if (!isStarted)
                {
                    MessageBox.Show(@"Ошибка:запуска цикла! ", @"Возникла ошибка,цикл не запущен", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log.Instance.Error("Ошибка:запуска цикла! Возникла ошибка,цикл не запущен");
                    Controller.Instance.Stop();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Ошибка: запуска цикла!", @"Возникла ошибка " + ex.Message + @" цикл не запущен! ", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                fbd.Description = @"Выберете папку с файлами пакетов";
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrEmpty(fbd.SelectedPath))
                {
                    FileController.FilesPath = fbd.SelectedPath;
                    Log.Instance.Info("Выбрана папка для загрузки {0}", fbd.SelectedPath);
                }
                else
                {
                    Log.Instance.Error("Папка не выбрана");                 
                    return;
                }
            }      
            try
            {
                if (!Controller.Instance.ReadDataFromFile())
                {
                    MessageBox.Show(@"Ошибка: чтения пакетов из файла", @"Не удалось считать пакеты из файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log.Instance.Error("Ошибка: чтения пакетов из файла: Не удалось считать пакеты из файла");                   
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), @"Ошибка: чтения пакетов из файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.Instance.Exception(exception);
            }
            MessageBox.Show(@"Чтение завершено ");
        }

        #endregion

        #endregion

        #region Timers tick functions

        private void workTimer_Tick(object sender, EventArgs e)
        {
            countTemperaturesReachedLabel.Text =
                $@"{(CycleData.Instance.TemperutureIndex < 0 ? 0 : CycleData.Instance.TemperutureIndex)}/{Controller.Instance.TemperaturesCount}";
            currentPositionNumberLbl.Text =
                $@"{(SensorController.Instance.CurrentPositionNumber < 0 ? 0 : SensorController.Instance.CurrentPositionNumber)}/{SensorController.Instance.CurrentPositionCount}";
            CurrentTemperatureLabel.Text = EvoData.Instance.CurrentTemperature.ToString(CultureInfo.InvariantCulture);
            nextTemperatureLable.Text = EvoData.Instance.NextTemperature.ToString(CultureInfo.InvariantCulture);
            CheckParam(EvoData.Instance.IsCameraPowerOn, powerCameraIndic);
            CheckParam(EvoData.Instance.IsTemperatureReached, temperatureReachedIndic);
            CheckParam(EvoData.Instance.X.IsZeroFound, xZeroFindedIndic);
            CheckParam(EvoData.Instance.Y.IsZeroFound, yZeroFindedIndic);
            CheckParam(EvoData.Instance.X.IsPowerOn, xPowerIndic);
            CheckParam(EvoData.Instance.Y.IsPowerOn, yPowerIndic);
            CheckParam(EvoData.Instance.X.IsMove, xMoveIndic);
            CheckParam(EvoData.Instance.Y.IsMove, yMoveIndic);
            xPositionLabel.Text = EvoData.Instance.X.Position.ToString(CultureInfo.InvariantCulture);
            yPositionLabel.Text = EvoData.Instance.Y.Position.ToString(CultureInfo.InvariantCulture);
            xSpeedOfRateLabel.Text = EvoData.Instance.X.SpeedOfRate.ToString(CultureInfo.InvariantCulture);
            ySpeedOfRateLabel.Text = EvoData.Instance.Y.SpeedOfRate.ToString(CultureInfo.InvariantCulture);
            DrawGrapfic();

            if (!IsStabilized)
            {
                var t = TimeSpan.FromMilliseconds(Controller.Instance.StabilizationTime - (DateTime.Now - _stabilizationStartTime).TotalMilliseconds);
                temperatureStabLabel.Text = $@"{t.Hours:D2}:{t.Minutes:D2}:{t.Seconds:D2}";
            }
            else
            {
                temperatureStabLabel.Text = @"Стабилизирована";
            }

        }

        private void timer_Tick(object sender, EventArgs e)
        {
            TimeSpan difference = DateTime.Now - _startTime;
            difference -= TimeSpan.FromMilliseconds(difference.Milliseconds);
            timeLeftlabel.Text = $@"{difference.Hours,2}:{difference.Minutes,2}:{difference.Seconds,2}";
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

        private void EvoConnectionChangeHandler(object sender,EventArgs e)
        {
            var args = e as ConnectionStatusEventArgs;
            if (args == null)
                return;
            EvoConnectionDel del = EvoConnectionChange;
            connectionStateLabel.Invoke(del, args.State);
        }

        private void SensorConnectionChange(ConnectionStatus state)
        {
            sensorConnectionStateLabel.Text = state.ToText();
        }

        private void SensorConnectionChangeHandler(object sender, EventArgs e)
        {
            var arg = e as ConnectionStatusEventArgs;
            if (arg == null)
                return;
            SensorConnectionDel del = SensorConnectionChange;
            sensorConnectionStateLabel.Invoke(del, arg.State);
        }

        private void EvoWorkModeChange(WorkMode mode)
        {
            controllerWokModeLabel.Text = mode.ToView();
        }

        private void EvoWorkModeChangeHandler(object sender, EventArgs e)
        {
            WorkModDel del = EvoWorkModeChange;
            var args = e as WorkModeEventArgs;
            if (args != null)
                controllerWokModeLabel.Invoke(del, args.Mode);
        }

        private void ControllerExсeptoinsHandler(object sender, EventArgs e)
        {
            var args = e as ExceptionEventArgs;
            if (args != null)
            {
                string message = "Возникла ошибка во время работы !";
                MessageBox.Show(args.Exception.Message, message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetForm();
            }
        }

        private void TemperatureStabilizationHandler(object sender,EventArgs e)
        {
            var args = e as BoolEventArgs;
            if (args == null)
                return;
            IsStabilized = args.Result;
        }

        #endregion

        #region ToolStripMenu handlers

        private void cycleSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_isSettingsEntered)
            {
                ReadSettings();
                _isSettingsEntered = true;
            }
            var cycleForm = new CycleSettingsForm();
            cycleForm.ShowDialog();
        }

        private void CommonSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(_settingsFileName))
            {
                var dialog = System.Diagnostics.Process.Start(_settingsFileName);
                dialog?.WaitForExit();
                ReadSettings();
            }
        }

        #endregion

        #region Other functions

        private void ResetForm()
        {
            FormReseter del = Reset;
            if(InvokeRequired)
                startButton.Invoke(del);
            else
            {
                Reset();
            }
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

            sensorTypeLabel.Text = @"нет данных";
            currentPositionNumberLbl.Text = @"нет данных";
            packetsArrivedLbl.Text= @"нет данных";
            sensorConnectionStateLabel.Text= @"нет данных";
            //SensorDataGridView.Rows.Clear(); 
            SensorDataGridView.Visible = false;
            FileToolStripMenuItem.Enabled = true;
            SettingsToolStripMenuItem.Enabled = true; 
        }
        private void CycleEndedHandler(object sender,EventArgs e)// bool result)
        {
            var args = e as BoolEventArgs;
            if (args == null)
                return;
            if (!args.Result)
            {
                string message = @" Возникла ошибка смотрите log файл";
                MessageBox.Show(@"Ошибка: цикл завершился неуспешно !", message, MessageBoxButtons.OK);
            }
            else
            {
                string message = @"Цикл окончен!";
                if (CycleData.Instance.IsFullCycle)
                {
                    DialogResult diaologResult = MessageBox.Show("Выполнить расчет коэффицентов ?",message,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (diaologResult == DialogResult.Yes)
                    {
                        CompCoeff del = ComputeCoefficents;
                        Invoke(del);
                    }
                }
                else
                    MessageBox.Show(@"Цикл завершен !", message, MessageBoxButtons.OK);

                ResetForm();
            }
        }

        private void DrawGrapfic()
        {
            double x = (DateTime.Now - _startTime).TotalMinutes;
            double y = ControllerEvo.Instance.CurrentTemperature;
            double[] tempX = new double[] { _prevX, x };
            double[] tempY = new double[] { _prevY, y };
            graph.GraphPane.AddCurve("", tempX, tempY, Color.Red, SymbolType.None);
            graph.AxisChange();
            graph.Invalidate();
            if (graph.GraphPane.CurveList.Count > 100)
            {
                graph.GraphPane.CurveList.RemoveAt(0);
            }
            _prevX = x;
            _prevY = y;
        }

        private void CheckParam(bool param, PictureBox picture)
        {
            picture.BackColor = param ? Color.Green : Color.Red;
        }

        private void ComputeCoefficents()
        {
            var dlg = new SaveFileDialog
            {
                Filter = @"Все файлы (*.*)|*.*"
            };
            DialogResult res = dlg.ShowDialog();

            if (res != DialogResult.OK)
            {
                return;
            }
            string fileName = dlg.FileName;
            StreamWriter file = new StreamWriter(fileName);
            if (!Controller.Instance.ComputeCoefficents(file))
            {
                MessageBox.Show(@"Проблемы с файлом", @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            file.Close();
        }

        private void ShowSensorParams()
        {
            packetsArrivedLbl.Text = SensorController.Instance.PacketsCollectedCount.ToString();
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
            if (_isSettingsEntered)
            {
                return true;
            }
            if (!File.Exists(_settingsFileName))
            {
                string message = "Файл " + _settingsFileName + "  не найден!";
                var dialogResult = MessageBox.Show(@"Открыть другой файл настроек ?", message, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error);
                if (dialogResult == DialogResult.None || dialogResult == DialogResult.No)
                    return false;

                if (dialogResult == DialogResult.Yes)
                {
                    var openFileDialog = new OpenFileDialog {Filter = @"txt files (*.txt)", RestoreDirectory = true};
                    dialogResult = openFileDialog.ShowDialog();
                    if (dialogResult == DialogResult.Yes)
                    {
                        _settingsFileName = openFileDialog.FileName;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            var result = FileController.Instance.ReadSettings(_settingsFileName);
            if (result)
            {
                _isSettingsEntered = true;
            }
            else
                MessageBox.Show(@"Проблемы чтения файла настроек", @"Проблемы чтения из файла!", MessageBoxButtons.OK);
            return result;
        }

        #endregion
    }
}
