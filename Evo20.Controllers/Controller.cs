using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Evo20.Sensors;
using System.Windows.Forms;
using Evo20.Commands.Abstract;
using Evo20.Controllers.Data;
using Evo20.Controllers.EvoControllers;
using Evo20.Controllers.FileWork;
using Evo20.Utils.EventArguments;
using Evo20.Utils;

namespace Evo20.Controllers
{     
    public class Controller
    {
        #region Devegates and Events
        //событие обработки ошибок
        public event ControllerExceptions ControllerExceptionEvent;

        public delegate void TemperatureSabilizationHandler(object sender, EventArgs e);

        public delegate void WorkModeChangeHandler(object sender, EventArgs e);

        public delegate void CycleEndedHandler(object sender, EventArgs e);

        public delegate void ConnectionChangeHandler(object sender, EventArgs e);

        public event WorkModeChangeHandler WorkModeChanged;

        public delegate void ControllerExceptions(object sender, EventArgs e);//exception
        public event CycleEndedHandler CycleEndedEvent;

        public event TemperatureSabilizationHandler TemperatureStabilized;
#if !DEBUG
        private int THREADS_SLEEP_TIME = 100;
#endif
        #endregion

        #region Private Fields

        private Thread _cycleThread;

        #endregion

        #region Properties

        private List<ISensor> _sensorsList;

        public int StabilizationTime
        {
            get
            {
                if (Mode == WorkMode.CalibrationMode)
                {
                    return CycleData.Instance.CalibrationStabTime;
                }
                if (Mode ==WorkMode.CheckMode)
                {
                    return CycleData.Instance.CheckStabTime;
                }
                return 0;
            }
        }
                       
        public int TemperaturesCount
        {
            get
            {
                if(Mode == WorkMode.CalibrationMode)
                    return CycleData.Instance.CalibrationTemperatures.Count;
                if(Mode == WorkMode.CheckMode)
                    return CycleData.Instance.CheckTemperatures.Count;
                return 0;
            }
        }
                      
        private static Controller _controller;

        public static Controller Instance => _controller ?? (_controller = new Controller());

        private WorkMode _mode;

        public WorkMode Mode
        {
            get
            {
                return _mode;
            }
            set
            {
                _mode = value;
                WorkModeChanged?.Invoke(this, new WorkModeEventArgs(_mode));
            }
        }

        public List<ISensor> SensorsList
        {
            get
            {
                return _sensorsList;
            }

            set
            {
                _sensorsList = value;
            }
        }

        #endregion

        #region Constructors

        private Controller()
        {           
            Mode = WorkMode.NoMode;
            _sensorsList = new List<ISensor>();
        }

        #endregion

        #region Start Stop Pause Methods

        /// <summary>
        /// Запуск работы 
        /// </summary>
        /// <param name="comPortName">имя порта для соединения с датчиком</param>
        /// <param name="mode">режим работы</param>
        /// <returns>true-запуск прошел успешно,false-ошибка</returns>
        public bool Start(string comPortName,WorkMode mode)
        {
            if (_cycleThread != null && _cycleThread.IsAlive)
                return false;
            _sensorsList = SensorController.SensorController.Instance.SensorsList;
           
            bool resultEvoStart= ControllerEvo.Instance.StartEvoConnection();
            if (!resultEvoStart)
                return false;
            bool resultComPortStart =SensorController.SensorController.Instance.StartComPortConnection(comPortName);
            if (!resultComPortStart)
                return false;
            switch (mode)
            {
                case WorkMode.CalibrationMode:
                    _cycleThread = new Thread(CalibrationCycle);
                    break;
                case WorkMode.CheckMode:
                    _cycleThread = new Thread(CheckCycle);
                    break;
                default:
                    return false;
            }
            _cycleThread.Start();
            return true;
        }
        /// <summary>
        ///Остановить работу 
        /// </summary>
        public void Stop()
        {          
            if(_cycleThread!=null && _cycleThread.IsAlive)
                _cycleThread.Abort();
            SensorController.SensorController.Instance.StopComPortConnection();
            ControllerEvo.Instance.StopEvoConnection();
        }

        //public void Pause()
        //{
        //    routineThread.Abort();
        //    cycleThread.Suspend();
        //}
        #endregion

        #region Event Handlers 

        //private void EvoConnectionExceptionHandler(Exception evoException)
        //{
        //    Stop();
        //    ControllerExceptionEvent?.Invoke(this,new ExceptionEventArgs(evoException));
        //}

        #endregion

        #region Cycles

        /// <summary>
        /// Запуск цикла калибровки
        /// </summary>
        private void CalibrationCycle()
        {
            try
            {
                Mode = WorkMode.CalibrationMode;
                Cycle(CycleData.Instance.CalibrationTemperatures);
                Mode = WorkMode.NoMode;
            }
            catch(Exception ex)
            {
                Log.Instance.Exception(ex);
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// Запуск цикла проверки
        /// </summary>
        private void CheckCycle()
        {
            Mode = WorkMode.CheckMode;
            Cycle(CycleData.Instance.CheckTemperatures);
            Mode = WorkMode.NoMode;
        }

        /// <summary>
        /// ОСновной метод цикла
        /// </summary>
        /// <param name="temperatures">Список температур</param>
        private void Cycle(List<int> temperatures)
        {
            //профили датчиков
            var profiles = new List<ProfilePart[]>();
            switch (Mode)
            {
                case WorkMode.CalibrationMode:
                    {
                        foreach (var sensor in _sensorsList)
                            profiles.Add(sensor.CalibrationProfile);
                    }
                    break;
                case WorkMode.CheckMode:
                    foreach (var sensor in _sensorsList)
                        profiles.Add(sensor.CheckProfile);
                    break;
                default:
                    Log.Instance.Error("Ошибка:перед запуском цикла. Режим работы не установлен!");
                    CycleEndedEvent?.Invoke(this, new BoolEventArgs(false));
                    return;
            }
            ControllerEvo.Instance.InitEvo();
            Task writePacketsTask = null;
            //цикл по температурам
            for (int i = CycleData.Instance.StartTemperatureIndex; i < temperatures.Count; i++)
            {
                lock (EvoData.Instance)
                    EvoData.Instance.NextTemperature = temperatures[i];
                ControllerEvo.Instance.SetStartPosition();
                ControllerEvo.Instance.SetTemperature(temperatures[i]);
                Log.Instance.Info("Установлена температура камеры " + temperatures[i] + " скорость набора температtуры " + Config.Instance.SpeedOfTemperatureChange);
                //Ожидание достижения температуры
#if !DEBUG
                if (!Config.IsFakeEvo)
                    EvoData.Instance.TemperatureReachedEvent.WaitOne();
#endif
                EvoData.Instance.TemperatureReachedEvent.Reset();
                Log.Instance.Info("Температура  " + temperatures[i] + " достигнута");
                TemperatureStabilized?.Invoke(this, new BoolEventArgs(false));
                Log.Instance.Info("Начало стабилизации температуры.Время стабилизации {0}", StabilizationTime);
#if !DEBUG
                var waitingStartTime = DateTime.Now;
#endif
                if (i != CycleData.Instance.StartTemperatureIndex)
                {
                    writePacketsTask?.Wait();
                }

                writePacketsTask = new Task(CycleTemperatureEnd);
                CycleData.Instance.TemperutureIndex = i;
#if !DEBUG

                
                if (!Config.IsFakeEvo)
                    //ожидание стабилизации температуры
                    Thread.Sleep(StabilizationTime -(DateTime.Now- waitingStartTime).Milliseconds);
#endif
                Log.Instance.Info("Стабилизация температуры завершена");
                SensorController.SensorController.Instance.TemperatureOfCollect = temperatures[i];
                TemperatureStabilized?.Invoke(this, new BoolEventArgs(true));
                //для каждого датчика
                for (int j = 0; j < _sensorsList.Count; j++)
			    {
                    SensorController.SensorController.Instance.CurrentSensor = _sensorsList[j];
                    Log.Instance.Info("Запуск подцикла датчика {0}",SensorController.SensorController.Instance.CurrentSensor.Name);
                    //запуск подцикла датчика
                    bool isCyclePartSuccess = SensorCyclePart(profiles[j]);
                    if (!isCyclePartSuccess)
                    {
                        Log.Instance.Error("Ошибка:Не выполнена часть цикла для датчика :{0} при температуре {1} ",_sensorsList[j].Name,temperatures[i]);
                        CycleEndedEvent?.Invoke(this, new BoolEventArgs(false));
                        return;
                    }
                    Log.Instance.Info("{0}:Подцикл датчика завершен {1}", DateTime.Now.TimeOfDay, SensorController.SensorController.Instance.CurrentSensor.Name);
                }
                writePacketsTask.Start();
            }

            writePacketsTask?.Wait();
            CycleEndedEvent?.Invoke(this, new BoolEventArgs(true));

        }
        /// <summary>
        /// Часть цикла для конкретного датчика
        /// </summary>
        /// <param name="profile">профиль положений</param>
        /// <returns>результат</returns>
        private bool SensorCyclePart(ProfilePart[] profile)
        {
            int j = 0;
            try
            {
                for (; j < profile.Length; j++)
                {
                    Log.Instance.Info("Новое положение {0} для датчика {1}", j, SensorController.SensorController.Instance.CurrentSensor.Name);
                    ControllerEvo.Instance.StopAxis(Axis.All);
#if !DEBUG
                    if(!Config.IsFakeEvo)
                    {
                        EvoData.Instance.MovementEndedEvent.WaitOne(THREADS_SLEEP_TIME);
                        EvoData.Instance.MovementEndedEvent.Reset();
                    }
#endif
                    SensorController.SensorController.Instance.CurrentPositionNumber = j;
                    ControllerEvo.Instance.SetPosition(profile[j]);                  
#if !DEBUG
                    if (!Config.IsFakeEvo)
                        //ожидаем пока установятся позиции
                        Thread.Sleep(THREADS_SLEEP_TIME);
#endif
                    //ожидание сбора пакетов
                    SensorController.SensorController.Instance.CanCollect = true;
                    SensorController.SensorController.Instance.CurrentSensor.PacketsCollectedEvent.WaitOne();
                    SensorController.SensorController.Instance.CanCollect = false;
                    Log.Instance.Info("Пакеты в положении {0} собраны", SensorController.SensorController.Instance.CurrentPositionNumber);
                }

            }
            catch (ThreadAbortException)
            {
                Log.Instance.Info("Поток цикла был прерван при датчике:{0} ,при шаге {1}", SensorController.SensorController.Instance.CurrentSensor.Name, j);
                return false;
            }
            catch (Exception exception)
            {
                Log.Instance.Error(
                    $"Возникло исключение цикла при датчике:{SensorController.SensorController.Instance.CurrentSensor.Name} ,при шаге {j}");
                Log.Instance.Exception(exception);
                ControllerExceptionEvent?.Invoke(this, new ExceptionEventArgs(exception));
                return false;
            }
            SensorController.SensorController.Instance.CurrentSensor = null;
            SensorController.SensorController.Instance.CurrentPositionNumber = 0;
            return true;
        }

#endregion

#region Secondary functions

        public bool ReadDataFromFile()
        {
            _sensorsList = SensorController.SensorController.Instance.SensorsList;
            return FileController.Instance.ReadDataFromFile(ref _sensorsList);
        }
        public bool ComputeCoefficents(StreamWriter file)
        {
            var result = false;
            try
            {
                result= MathController.MathController.Instance.ComputeCoefficents(_sensorsList, file);
            }
            catch (Exception ex)
            {
                Log.Instance.Exception(ex);
            }

            return result;
        }     
        /// <summary>
        /// Выдать среднее значеие кодов АЦП текущего датчика
        /// </summary>
        /// <returns>Список значений</returns>      
#endregion

        private void CycleTemperatureEnd()
        {
            try
            {
                lock (SensorController.SensorController.Instance)
                {
                    Log.Instance.Info("Начало запиcи пакетов");
                    //записываем пакеты
                    FileController.Instance.WriteRedPackets(_sensorsList, CycleData.Instance.TemperutureIndex);
                    SensorController.SensorController.Instance.ClearData(CycleData.Instance.TemperutureIndex,
                            _mode);
                }
            }
            catch (Exception exception)
            {
                Log.Instance.Exception(exception);
            }
        }     
    }
}
