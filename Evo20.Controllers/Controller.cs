using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Evo20.Sensors;
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

        public event ControllerExceptions ControllerExceptionEvent;

        public delegate void TemperatureSabilizationHandler(object sender, EventArgs e);

        public delegate void WorkModeChangeHandler(object sender, EventArgs e);

        public delegate void CycleEndedHandler(object sender, EventArgs e);

        public delegate void ConnectionChangeHandler(object sender, EventArgs e);

        public event WorkModeChangeHandler WorkModeChanged;

        public delegate void ControllerExceptions(object sender, EventArgs e);

        private event CycleEndedHandler _CycleEndedEvent;

        public event CycleEndedHandler CycleEndedEvent
        {
            add
            {
                lock (this)
                {
                    _CycleEndedEvent += value;
                }
            }
            remove { _CycleEndedEvent -= value; }
        }

        public event TemperatureSabilizationHandler TemperatureStabilized;
        #endregion

        #region Private Fields

        private Thread _cycleThread;

        #endregion

        #region Properties

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
            get { return _mode; }
            set
            {
                _mode = value;
                WorkModeChanged?.Invoke(this, new WorkModeEventArgs(_mode));
            }
        }

        private List<ISensor> _sensorsList;

        private List<ISensor> SensorsList => _sensorsList;

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
            _sensorsList = SensorController.Instance.SensorsList;
           
            bool resultEvoStart = ControllerEvo.Instance.StartEvoConnection();
            if (!resultEvoStart)
                return false;
            bool resultComPortStart = SensorController.Instance.StartComPortConnection(comPortName);
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
            try
            {
                if (_cycleThread != null && _cycleThread.IsAlive && _cycleThread.ThreadState!=ThreadState.Aborted
                    && _cycleThread.ThreadState != ThreadState.AbortRequested && (_cycleThread.ThreadState==ThreadState.Running || _cycleThread.ThreadState == ThreadState.WaitSleepJoin))
                    _cycleThread.Abort();
                SensorController.Instance.StopComPortConnection();
                ControllerEvo.Instance.StopEvoConnection();
            }
            catch (Exception exception)
            {
                Log.Instance.Warning("Ошибка при остановке цикла");
                Log.Instance.Exception(exception);
            }
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
            catch (ThreadAbortException)
            {
                Log.Instance.Warning("CalibrationCycle поток тыл прерван");
            }
            catch(Exception ex)
            {
                Log.Instance.Exception(ex);
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
            var profiles = new List<Position[]>();
            switch (Mode)
            {
                case WorkMode.CalibrationMode:
                {
                    foreach (var sensor in _sensorsList) profiles.Add(sensor.CalibrationProfile);
                }
                    break;
                case WorkMode.CheckMode:
                    foreach (var sensor in _sensorsList) profiles.Add(sensor.CheckProfile);
                    break;
                default:
                    Log.Instance.Error("Ошибка:перед запуском цикла. Режим работы не установлен!");
                    _CycleEndedEvent?.Invoke(this, new BoolEventArgs(false));
                    return;
            }
            ControllerEvo.Instance.InitEvo();
            Task writePacketsTask = null;
            //цикл по температурам
            Log.Instance.Info($"Начало цикла по температурам {temperatures.Count}");
            for (int i = CycleData.Instance.StartTemperatureIndex; i < temperatures.Count; i++)
            {
                ControllerEvo.Instance.SetStartPosition();
                ControllerEvo.Instance.SetTemperature(temperatures[i]);             
                //Ожидание достижения температуры
                if (!Config.Instance.IsFakeEvo)
                    EvoData.Instance.TemperatureReachedEvent.WaitOne();
                EvoData.Instance.TemperatureReachedEvent.Reset();
                Log.Instance.Info("Температура  " + temperatures[i] + " достигнута");
                TemperatureStabilized?.Invoke(this, new BoolEventArgs(false));
                Log.Instance.Info("Начало стабилизации температуры.Время стабилизации {0}", StabilizationTime);
                var waitingStartTime = DateTime.Now;
                if (i != CycleData.Instance.StartTemperatureIndex)
                    writePacketsTask?.Wait();
                writePacketsTask = new Task(WritePackets);
                CycleData.Instance.TemperutureIndex = i;
                SensorController.Instance.TemperatureIndex = i;
                if (!Config.Instance.IsFakeEvo)
                    Thread.Sleep(StabilizationTime -(DateTime.Now- waitingStartTime).Milliseconds);
                Log.Instance.Info("Стабилизация температуры завершена");
                SensorController.Instance.TemperatureOfCollect = temperatures[i];
                TemperatureStabilized?.Invoke(this, new BoolEventArgs(true));

                for (int j = 0; j < _sensorsList.Count; j++)
			    {
                    SensorController.Instance.CurrentSensor = _sensorsList[j];
                    Log.Instance.Info("Запуск подцикла датчика {0}",SensorController.Instance.CurrentSensor.Name);
                    bool isCyclePartSuccess = SensorCyclePart(profiles[j]);
                    if (!isCyclePartSuccess)
                    {
                        Log.Instance.Error("Ошибка:Не выполнена часть цикла для датчика :{0} при температуре {1} ",_sensorsList[j].Name,temperatures[i]);
                        _CycleEndedEvent?.Invoke(this, new BoolEventArgs(false));
                        return;
                    }
                    Log.Instance.Info("Подцикл датчика завершен {0}", SensorController.Instance.CurrentSensor.Name);
                }
                SensorController.Instance.TemperatureIndex = -1;
                writePacketsTask.Start();
            }
            Log.Instance.Info($"Цикл по температурам завершен");
            ControllerEvo.Instance.SetStartPosition();
            writePacketsTask?.Wait();
            _CycleEndedEvent?.Invoke(this, new BoolEventArgs(true));

        }
        /// <summary>
        /// Часть цикла для конкретного датчика
        /// </summary>
        /// <param name="profile">профиль положений</param>
        /// <returns>результат</returns>
        private bool SensorCyclePart(Position[] profile)
        {
            int j = 0;
            try
            {
                for (; j < profile.Length; j++)
                {
                    Log.Instance.Info("Новое положение {0} для датчика {1}", j, SensorController.Instance.CurrentSensor.Name);
                    SensorController.Instance.CurrentPositionNumber = j;
                    ControllerEvo.Instance.SetPosition(profile[j]);
                    if (!Config.Instance.IsFakeEvo)
                        EvoData.Instance.PositionReachedEvent.WaitOne();

                    SensorController.Instance.CanCollect = true;
                    SensorController.Instance.CurrentSensor.PacketsCollectedEvent.WaitOne();
                    SensorController.Instance.CanCollect = false;
                    Log.Instance.Info("Пакеты в положении {0} собраны", SensorController.Instance.CurrentPositionNumber);
                }

            }
            catch (ThreadAbortException)
            {
                Log.Instance.Info("Поток цикла был прерван при датчике:{0} ,при шаге {1}", SensorController.Instance.CurrentSensor.Name, j);
                return false;
            }
            catch (Exception exception)
            {
                Log.Instance.Error(
                    $"Возникло исключение цикла при датчике:{SensorController.Instance.CurrentSensor.Name} ,при шаге {j}");
                Log.Instance.Exception(exception);
                ControllerExceptionEvent?.Invoke(this, new ExceptionEventArgs(exception));
                return false;
            }
            SensorController.Instance.CurrentSensor = null;
            SensorController.Instance.CurrentPositionNumber = 0;
            return true;
        }

        #endregion

        #region Secondary functions

        public bool ReadDataFromFile()
        {
            _sensorsList = SensorController.Instance.SensorsList;
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

        private void WritePackets()
        {
            try
            {
                lock (SensorController.Instance)
                {
                    Log.Instance.Info("Начало запиcи пакетов");
                    FileController.Instance.WriteRedPackets(SensorsList, CycleData.Instance.TemperutureIndex);
                    FileController.Instance.WriteMeanParams(SensorsList, CycleData.Instance.TemperutureIndex);
                    SensorController.Instance.ClearData(CycleData.Instance.TemperutureIndex, Mode);
                }
            }
            catch (Exception exception)
            {
                Log.Instance.Exception(exception);
            }
        }
        
#endregion
    }
}
