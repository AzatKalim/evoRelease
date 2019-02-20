using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Evo20.Commands;
using Evo20.Sensors;
using System.Windows.Forms;

namespace Evo20.Controllers
{  
    public class Controller
    {
        #region Devegates and Events
        //событие обработки ошибок
        public event ControllerExceptions EventHandlerListForControllerExceptions;

        public delegate void TemperatureSabilizationHandler(bool result);

        public delegate void WorkModeChangeHandler(WorkMode mode);

        public delegate void CycleEndedHandler(bool result);

        public delegate void ConnectionChangeHandler(ConnectionStatus state);

        public WorkModeChangeHandler EventListForWorkModeChange;

        public delegate void ControllerExceptions(Exception exception);
        //событие окончания работы цикла
        public event CycleEndedHandler EventHandlersListCycleEnded;

        public event TemperatureSabilizationHandler EventHandlerListForTemperatureStabilization;

        #endregion

        #region Private Fields

        Thread cycleThread;
       
        public const int THREADS_SLEEP_TIME = 100;

        #endregion

        #region Properties

        public List<ISensor> sensorsList;

        public int StabilizationTime
        {
            get
            {
                if (Mode == WorkMode.CalibrationMode)
                {
                    return CycleData.Instance.calibrationStabTime;
                }
                if (Mode ==WorkMode.CheckMode)
                {
                    return CycleData.Instance.checkStabTime;
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
                      
        private static Controller controller;

        public static Controller Instance
        {
            get
            {
                if (controller == null)
                    controller = new Controller();
                return controller;
            }
        }

        private WorkMode mode;

        public WorkMode Mode
        {
            get
            {
                return mode;
            }
            set
            {
                mode = value;
                if (EventListForWorkModeChange != null)
                    EventListForWorkModeChange(mode);
            }
        }

        #endregion

        #region Constructors

        private Controller():base()
        {           
            Mode = WorkMode.NoMode;
            sensorsList = new List<ISensor>();
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
            if (cycleThread != null && cycleThread.IsAlive)
                return false;
            sensorsList = SensorController.Instance.SensorsList;
           
            bool resultEvoStart= ControllerEvo.Instance.StartEvoConnection();
            if (!resultEvoStart)
                return false;
            bool resultComPortStart =SensorController.Instance.StartComPortConnection(comPortName);
            if (!resultComPortStart)
                return false;
            switch (mode)
            {
                case WorkMode.CalibrationMode:
                    cycleThread = new Thread(CalibrationCycle);
                    break;
                case WorkMode.CheckMode:
                    cycleThread = new Thread(CheckCycle);
                    break;
                default:
                    return false;
            }
            cycleThread.Start();
            return true;
        }
        /// <summary>
        ///Остановить работу 
        /// </summary>
        public void Stop()
        {          
            if(cycleThread!=null && cycleThread.IsAlive)
                cycleThread.Abort();
            SensorController.Instance.StopComPortConnection();
            ControllerEvo.Instance.StopEvoConnection();
        }

        /// <summary>
        /// Приостановить работу
        /// </summary>
        //public void Pause()
        //{
        //    routineThread.Abort();
        //    cycleThread.Suspend();
        //}
        #endregion

        #region Event Handlers 

        /// <summary>
        /// Обработчик ошибок соединения с Evo 
        /// </summary>
        /// <param name="evoException">ошибка</param>
        private void EvoConnectionExceptionHandler(Exception evoException)
        {
            Stop();
            if (EventHandlerListForControllerExceptions != null)
            {
                EventHandlerListForControllerExceptions(evoException);
            }
        }

        #endregion

        #region Cycles

        /// <summary>
        /// Запуск цикла калибровки
        /// </summary>
        public void CalibrationCycle()
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
        public void CheckCycle()
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
                        foreach (var sensor in sensorsList)
                            profiles.Add(sensor.CalibrationProfile);
                    }
                    break;
                case WorkMode.CheckMode:
                    foreach (var sensor in sensorsList)
                        profiles.Add(sensor.CheckProfile);
                    break;
                default:
                    Log.Instance.Error("Ошибка:перед запуском цикла. Режим работы не установлен!");
                    EventHandlersListCycleEnded(false);
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
                EventHandlerListForTemperatureStabilization?.Invoke(false);
                Log.Instance.Info("Начало стабилизации температуры.Время стабилизации {0}", StabilizationTime);
                var waitingStartTime = DateTime.Now;
                if (i != CycleData.Instance.StartTemperatureIndex)
                {
                    writePacketsTask.Wait();
                }
                writePacketsTask = new Task(CycleTemperatureEnd);
                CycleData.Instance.TemperutureIndex = i;
#if !DEBUG

                
                if (!Config.IsFakeEvo)
                    //ожидание стабилизации температуры
                    Thread.Sleep(StabilizationTime -(DateTime.Now- waitingStartTime).Milliseconds);
#endif
        Log.Instance.Info("Стабилизация температуры завершена");
                SensorController.Instance.TemperatureOfCollect = temperatures[i];
                EventHandlerListForTemperatureStabilization?.Invoke(true);
                //для каждого датчика
                for (int j = 0; j < sensorsList.Count; j++)
			    {
                    SensorController.Instance.CurrentSensor = sensorsList[j];
                    Log.Instance.Info("Запуск подцикла датчика {0}",SensorController.Instance.CurrentSensor.Name);
                    //запуск подцикла датчика
                    bool isCyclePartSuccess = SensorCyclePart(profiles[j]);
                    if (!isCyclePartSuccess)
                    {
                        Log.Instance.Error("Ошибка:Не выполнена часть цикла для датчика :{0} при температуре {1} ",sensorsList[j].Name,temperatures[i]);
                        EventHandlersListCycleEnded(false);
                        return;
                    }
                    Log.Instance.Info("{0}:Подцикл датчика завершен {1}", DateTime.Now.TimeOfDay, SensorController.Instance.CurrentSensor.Name);
                }
                writePacketsTask.Start();
            }
            writePacketsTask.Wait();
            EventHandlersListCycleEnded(true);

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
                    Log.Instance.Info("Новое положение {0} для датчика {1}", j, SensorController.Instance.CurrentSensor.Name);
                    ControllerEvo.Instance.StopAxis(Axis.ALL);
#if !DEBUG
                    if(!Config.IsFakeEvo)
                    {
                        EvoData.Instance.movementEndedEvent.WaitOne(THREADS_SLEEP_TIME);
                        EvoData.Instance.movementEndedEvent.Reset();
                    }
#endif
                    SensorController.Instance.CurrentPositionNumber = j;
                    ControllerEvo.Instance.SetPosition(profile[j]);                  
                    //убрать
#if !DEBUG
                    if (!Config.IsFakeEvo)
                        //ожидаем пока установятся позиции
                        Thread.Sleep(THREADS_SLEEP_TIME);
#endif
                    //ожидание сбора пакетов
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
                Log.Instance.Error(string.Format("Возникло исключение цикла при датчике:{0} ,при шаге {1}", SensorController.Instance.CurrentSensor.Name, j));
                Log.Instance.Exception(exception);
                EventHandlerListForControllerExceptions?.Invoke(exception);
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
            sensorsList = SensorController.Instance.SensorsList;
            return FileController.Instance.ReadDataFromFile(ref sensorsList);
        }
        public bool ComputeCoefficents(StreamWriter file)
        {
            return MathController.Instance.ComputeCoefficents(sensorsList, file);
        }
        public bool WritePackets(StreamWriter file)
        {
            return FileController.Instance.WritePackets(sensorsList, file);
        }
        /// <summary>
        /// Выдать среднее значеие кодов АЦП текущего датчика
        /// </summary>
        /// <returns>Список значений</returns>      
        #endregion

        public void CycleTemperatureEnd()
        {
            try
            {
                lock (SensorController.Instance)
                {
                    Log.Instance.Info("Начало запиcи пакетов");
                    //записываем пакеты
                    FileController.Instance.WriteRedPackets(sensorsList, CycleData.Instance.TemperutureIndex);
                    SensorController.Instance.ClearWritedData(CycleData.Instance.TemperutureIndex,
                            mode);
                }
            }
            catch (Exception exception)
            {
                Log.Instance.Exception(exception);
            }
        }     
    }
}
