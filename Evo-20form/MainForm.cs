using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.IO.Ports;
using ZedGraph;
using PacketsLib;

namespace Evo_20form
{
    public partial class MainForm : Form
    {
        Controller controller;
        DateTime startTime;
        double prevX;
        double prevY;

        const int DRAW_LINE_LENGTH = 20;

        bool isStarted = false;

        public MainForm()
        {           
            InitializeComponent();
            prevX = 0;
            prevY = 0;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            string[] comPorts=SerialPort.GetPortNames();
            comPortComboBox.DataSource = comPorts;
            controller = new Controller();
            controller.EventHandlersListCycleEnded += CycleEndedHandler;
            startTime = DateTime.Now;
        }   

        private void Sensor_control_page_Click(object sender, EventArgs e)
        {

        }

        private void StartEvo_button_Click(object sender, EventArgs e)
        {
            try
            {
                if (!controller.ReadSettings())
                {
                    DialogResult result = MessageBox.Show("Проблемы с файлом настроек просмотрите лог файл", "Открыть файл настроек ?", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(Controller.SETTINGS_FILE_NAME);
                    }
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
        }

        private void Start_sensor_connection_Click(object sender, EventArgs e)
        {
            SensorDataGridView.Visible = true;
            SensorDataGridView.Rows.Add("Гироскопы", "0", "0", "0");
            SensorDataGridView.Rows.Add("Температуры гироскопов", "0", "0", "0");
            SensorDataGridView.Rows.Add("Акселерометры", "0", "0", "0");
            SensorDataGridView.Rows.Add("Температуры акселерометров", "0", "0", "0");
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
            }      
        }

        private void PauseEvo_button_Click(object sender, EventArgs e)
        {
            controller.PauseEvoConnection();
        }

        private void StopEvo_button_Click(object sender, EventArgs e)
        {
            controller.StopEvoConnection();
        }

        private void sensorStopbutton_Click(object sender, EventArgs e)
        {
            controller.StopComPortConnection();
        }

        private void sensorPauseButton_Click(object sender, EventArgs e)
        {
            controller.PauseComPortConnection();
        }

        private void DrawGrapfic()
        {
            double x= (DateTime.Now - startTime).TotalSeconds;
            double y=controller.currentTemperature;
            double [] tempX = new double[]{prevX,x};
            double [] tempY = new double[]{prevY,y};
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

        private void start_Click(object sender, EventArgs e)
        {
            try
            {
                if (!controller.ReadSettings())
                {
                    DialogResult result = MessageBox.Show("Проблемы с файлом настроек просмотрите лог файл", "Открыть файл настроек ?", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(Controller.SETTINGS_FILE_NAME);
                    }
                }
            }
            catch (Exception ex)
            {
                DialogResult result = MessageBox.Show("Проблемы с файлом: возникло исключение " + ex.Message, "Открыть файл настроек ?", MessageBoxButtons.YesNo);
            }
             if (comPortComboBox.SelectedItem == null)
            {
                MessageBox.Show("Com порт не выбран!", "Ошибка", MessageBoxButtons.OK);
                return;
            }
            if(modeComboBox.SelectedItem==null)
            {
                MessageBox.Show("Режим работы не выбран", "Ошибка", MessageBoxButtons.OK);
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
                    MessageBox.Show("Возникла ошибка,цикл не запущен! ", "Ошибка", MessageBoxButtons.OK);
                    controller.Stop();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникла ошибка " + ex.Message + " цикл не запущен! ", "Ошибка", MessageBoxButtons.OK);
                controller.Stop();
                return;
            }
                  
            SensorDataGridView.Visible = true;
            SensorDataGridView.Rows.Add("Гироскопы", "0", "0", "0");
            SensorDataGridView.Rows.Add("Акселерометры", "0", "0", "0");
            SensorDataGridView.Rows.Add("Температуры гироскопов", "0", "0", "0");
            SensorDataGridView.Rows.Add("Температуры акселерометров", "0", "0", "0");            
            pauseButton.Enabled = true;
            StopEvoButton.Enabled = true;
            stopButton.Enabled = true;
            workTimer.Start();
            timer.Start();
            startButton.Image = Evo_20form.Properties.Resources.pause_;
        }

        private void CheckParam(bool param, PictureBox picture)
        {
            if (param)
                picture.BackColor = Color.Green;
            else
                picture.BackColor = Color.Red;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            CurrentTemperatureLabel.Text = controller.evoData.currentTemperature.ToString();
            nextTemperatureLable.Text = controller.evoData.nextTemperature.ToString();
            CheckParam(controller.evoData.isCameraPowerOn,powerCameraIndic);
            CheckParam(controller.evoData.isTemperatureReached, temperatureReachedIndic);
            CheckParam(controller.evoData.x.isZeroFound, xZeroFindedIndic);
            CheckParam(controller.evoData.y.isZeroFound, yZeroFindedIndic);
            CheckParam(controller.evoData.x.isPowerOn, xPowerIndic);
            CheckParam(controller.evoData.y.isPowerOn, yPowerIndic);
            CheckParam(controller.evoData.x.isMove, xMoveIndic);
            CheckParam(controller.evoData.y.isMove, yMoveIndic);
            xPositionLabel.Text= controller.evoData.x.position.ToString();
            yPositionLabel.Text = controller.evoData.y.position.ToString();
            xSpeedOfRateLabel.Text = controller.evoData.x.speedOfRate.ToString();
            ySpeedOfRateLabel.Text = controller.evoData.y.speedOfRate.ToString();
            ShowSensorParams();
            DrawGrapfic();

        }

        private void ShowSensorParams()
        {
            packetsArrivedLabel.Text = controller.packetsCollectedCount.ToString();
            sensorTypeLabel.Text = controller.sensorData.sensorType.ToString();
            if (SensorDataGridView.Visible == false)
            {
                SensorDataGridView.Visible = true;
            }
            List<double> data = controller.GetSensorData();
            if (data == null)
            {
                return;
            }
            int k=0;
            for (int j  = 0; j < SensorDataGridView.Rows.Count-1; j++)
            {
                for (int i = 1; i <Packet.PARAMS_COUNT+1; i++)
                {
                    SensorDataGridView.Rows[j].Cells[i].Value = data[k++];
                }
            }
        }

        private void doWorkCalibration()
        {
            graph.GraphPane.Title = "График " + WorkMode.CalibrationMode;
            graph.GraphPane.XAxis.Title = "Время";
            graph.GraphPane.YAxis.Title = "Температура";
            graph.IsShowPointValues = true;
            graph.GraphPane.CurveList.Clear();
            controller.CalibrationCycle();                  
        }

        private void doWorkCheck()
        {
            graph.GraphPane.Title = "График " + WorkMode.CheckMode;
            graph.GraphPane.XAxis.Title = "Время";
            graph.GraphPane.YAxis.Title = "Температура";
            graph.IsShowPointValues = true;
            graph.GraphPane.CurveList.Clear();
            controller.CheckCycle();
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            countTemperaturesReachedLabel.Text = controller.temperutureIndex + " из " + controller.temperaturesCount;
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled == true))
            {
                string caption = "OK";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                string message = "Работа остановслена";
                MessageBox.Show(message, caption, buttons); 
            }

            else if (!(e.Error == null))
            {
                string caption = "OK";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                string message = "Error: " + e.Error.Message;
                MessageBox.Show(message, caption, buttons); 
            }
            else
            {
                string caption = "OK";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                string message = "Цикл окончен";
                MessageBox.Show(message, caption, buttons);
            }
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Controller.SETTINGS_FILE_NAME);
        }

        private void savePacketsButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Все файлы (*.*)|*.*";
            dlg.CheckFileExists = true;
            DialogResult res = dlg.ShowDialog();
            
            if (res!=DialogResult.OK)
            {
                return;
            }
            string FileName = dlg.FileName;
            StreamWriter file = new StreamWriter(FileName);
            if (!controller.WritePackets(file))
            {
                MessageBox.Show("Проблемы с файлом", "Ошибка", MessageBoxButtons.OK);
            }
            file.Close();
        }

        private void pauseButton_Click(object sender, EventArgs e)
        {
            controller.Pause();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            controller.Stop();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            TimeSpan difference = DateTime.Now - startTime;
            difference -= TimeSpan.FromMilliseconds(difference.Milliseconds);
            timeLeftlabel.Text = difference.Hours + ":" + difference.Minutes + ":" + difference.Seconds;
        }

        private void CycleEndedHandler(bool result)
        {
            if (!result)
            {
                string caption = "OK";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                string message = "Цикл не завершился! Возникла ошибка смотрите log ";
                MessageBox.Show(message, caption, buttons);
            }
            else
            {
                string message = "Цикл окончен!";
                DialogResult diaologResult = MessageBox.Show(message, "Выполнить расчет коэффицентов ?", MessageBoxButtons.YesNo);
                if (diaologResult == DialogResult.Yes)
                {
                    ComputeCoefficents();
                }
            }
        }

        private void ComputeCoefficents()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Все файлы (*.*)|*.*";
            dlg.CheckFileExists = true;
            //DialogResult res = dlg.ShowDialog();

            //if (res != DialogResult.OK)
            //{
             //   return;
            //}
            string FileName = "coef.txt";
            StreamWriter file = new StreamWriter(FileName);
            if (!controller.ComputeCoefficents(file))
            {
                MessageBox.Show("Проблемы с файлом", "Ошибка", MessageBoxButtons.OK);
            }
            file.Close();
        }

        ~MainForm()
        {
            workTimer.Stop();
            timer.Stop();
            controller.StopComPortConnection();
            controller.StopEvoConnection();
            controller.Stop();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            workTimer.Stop();
            timer.Stop();
            controller.StopComPortConnection();          
            controller.StopEvoConnection();
            controller.Stop();
        }

        private void button1_Click(object sender, EventArgs e)
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
            StreamReader file = new StreamReader(FileName);
            if (!controller.ReadDataFromFile(file))
            {
                MessageBox.Show("Проблемы с файлом", "Ошибка", MessageBoxButtons.OK);
            }
            file.Close();

        }

        private void SensorDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
