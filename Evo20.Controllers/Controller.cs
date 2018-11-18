using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Configuration;
using Evo20.Config;
using Evo20.Commands;
using Evo20.Controllers;
using Evo20.SensorsConnection;
using Evo20.Evo20.Packets;
using Evo20.Math;
using Evo20.Sensors;

namespace Evo20.Controllers
{  
    public class Controller
    {
        #region Devegates and Events
        //событие обработки ошибок
        public event ControllerExceptions EventHandlerListForControllerExceptions;
        //событие изменения состояния соединения с датчиком
        public event ConnectionChangeHandler EventHandlerListForSensorConnectionChange;

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

        int currentTemperature = 0;
       
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
                    return CycleData.Current.calibrationStabTime;
                }
                if (Mode ==WorkMode.CheckMode)
                {
                    return CycleData.Current.checkStabTime;
                }
                return 0;
            }
        }
                       
        public int TemperaturesCount
        {
            get
            {
                if(Mode == WorkMode.CalibrationMode)
                    return CycleData.Current.CalibrationTemperatures.Count;
                if(Mode == WorkMode.CheckMode)
                    return CycleData.Current.CheckTemperatures.Count;
                return 0;
            }
        }

        public int TemperutureIndex
        {
            get;
            private set;
        }
                  
        private static Controller controller;

        public static Controller Current
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
            if (sensorsList == null || sensorsList.Count==0)
            {
                sensorsList = SensorData.Current.GetSensors();
            }
            switch(mode)
            {
                case WorkMode.CalibrationMode:              
                    cycleThread = new Thread(CalibrationCycle);
                    break;
                case WorkMode.CheckMode:
                    cycleThread= new Thread(CheckCycle);
                    break;
                default:
                    return false;
            }
            bool resultEvoStart= ControllerEvo.Current.StartEvoConnection();
            if (!resultEvoStart)
                return false;
            bool resultComPortStart =SensorController.Current.StartComPortConnection(comPortName);
            if (!resultComPortStart)
                return false;
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
            SensorController.Current.StopComPortConnection();
            ControllerEvo.Current.StopEvoConnection();
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
            Mode = WorkMode.CalibrationMode;
            Cycle(CycleData.Current.CalibrationTemperatures);
            Mode = WorkMode.NoMode;
        }

        /// <summary>
        /// Запуск цикла проверки
        /// </summary>
        public void CheckCycle()
        {
            Mode = WorkMode.CheckMode;
            Cycle(CycleData.Current.CheckTemperatures);
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
                        {
                            profiles.Add(sensor.CalibrationProfile);
                        }
                    }
                    break;
                case WorkMode.CheckMode:
                    foreach (var sensor in sensorsList)
                    {
                        profiles.Add(sensor.CheckProfile);
                    }
                    break;
                default:
                    Log.WriteLog("Ошибка:перед запуском цикла. Режим работы не установлен!");
                    EventHandlersListCycleEnded(false);
                    return;
            }
            ControllerEvo.Current.InitEvo();   
            //цикл по температурам
            for (int i = CycleData.Current.StartTemperatureIndex; i < temperatures.Count; i++)
            {
                lock (EvoData.Current)
                {
                    EvoData.Current.NextTemperature = temperatures[i];
                }
                ControllerEvo.Current.SetStartPosition();
                ControllerEvo.Current.SetTemperature(temperatures[i]);
                Log.WriteLog("Установлена температура камеры " + temperatures[i] + " скорость набора температtуры " + Config.Config.SPEED_OF_TEMPERATURE_CHANGE);
                //убрать
                //Ожидание достижения температуры
//#if !DEBUG
                EvoData.Current.TemperatureReachedEvent.WaitOne();
//#endif
                EvoData.Current.TemperatureReachedEvent.Reset();
                Log.WriteLog("Температура  " + temperatures[i] + " достигнута");
                if (EventHandlerListForTemperatureStabilization != null)
                    EventHandlerListForTemperatureStabilization(false);
                Log.WriteLog(string.Format("{0}:Начало стабилизации температуры.Время стабилизации {1}", DateTime.Now.TimeOfDay, StabilizationTime));
//#if !DEBUG
//                //ожидание стабилизации температуры
                //убрать
                Thread.Sleep(StabilizationTime);
//#endif
                Log.WriteLog(string.Format("{0}:Стабилизация температуры завершена",DateTime.Now.TimeOfDay));
                currentTemperature = temperatures[i];
                if (EventHandlerListForTemperatureStabilization != null)
                    EventHandlerListForTemperatureStabilization(true);
                //для каждого датчика
                for (int j = 0; j < sensorsList.Count; j++)
			    {
                    SensorController.Current.CurrentSensor = sensorsList[j];
                    Log.WriteLog(string.Format("{0}:Запуск подцикла датчика {1}", DateTime.Now.TimeOfDay,
                        SensorController.Current.CurrentSensor.Name));
                    //запуск подцикла датчика
                    bool isCyclePartSuccess = SensorCyclePart(profiles[j]);
                    if (!isCyclePartSuccess)
                    {
                        Log.WriteLog("Ошибка:Не выполнена часть цикла для датчика :" + sensorsList[j].Name + " при температуре" + temperatures[i]);
                        EventHandlersListCycleEnded(false);
                        return;
                    }
                    Log.WriteLog(string.Format("{0}:Подцикл датчика завершен {1}", DateTime.Now.TimeOfDay, SensorController.Current.CurrentSensor.Name));
                }
                TemperutureIndex = i+1;
                //записываем пакеты
                FileController.Current.WriteRedPackets(sensorsList);                            
            }
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
                    Log.WriteLog(string.Format("{0} Новое положение для датчика {1}", DateTime.Now.TimeOfDay, j));
                    ControllerEvo.Current.StopAxis(Axis.ALL);
                    //убрать
                    EvoData.Current.movementEndedEvent.WaitOne(THREADS_SLEEP_TIME);
                    EvoData.Current.movementEndedEvent.Reset();
                    SensorController.Current.CurrentPositionNumber = j;
                    ControllerEvo.Current.SetPosition(profile[j]);                  
                    //убрать
                    //ожидаем пока установятся позиции
                    Thread.Sleep(THREADS_SLEEP_TIME);
                    //ожидание сбора пакетов
                    SensorController.Current.CanCollect = true;
                    SensorController.Current.CurrentSensor.PacketsCollectedEvent.WaitOne();
                    SensorController.Current.CanCollect = false;
                    Log.WriteLog(string.Format("{0}: Пакеты в положении {1} собраны", DateTime.Now.TimeOfDay, SensorController.Current.CurrentPositionNumber));
                }

            }
            catch (ThreadAbortException)
            {
                Log.WriteLog(string.Format("Поток цикла был прерван при датчике:{0} ,при шаге {1}", SensorController.Current.CurrentSensor.Name, j));
                return false;
            }
            catch (Exception exception)
            {
                Log.WriteLog(string.Format("Возникло исключение цикла при датчике:{0} ,при шаге {1}", SensorController.Current.CurrentSensor.Name, j));
                Log.WriteLog(exception.ToString());
                if (EventHandlerListForControllerExceptions != null)
                {
                    EventHandlerListForControllerExceptions(exception);
                }
                return false;
            }
            SensorController.Current.CurrentSensor = null;
            SensorController.Current.CurrentPositionNumber = 0;
            return true;
        }

        #endregion

        #region Secondary functions

        public bool ReadDataFromFile(StreamReader reader)
        {
            return FileController.Current.ReadDataFromFile(ref sensorsList, reader);
        }
        public bool ComputeCoefficents(StreamWriter file)
        {
            return MathController.Current.ComputeCoefficents(sensorsList, file);
        }
        public bool WritePackets(StreamWriter file)
        {
            return FileController.Current.WritePackets(sensorsList, file);
        }
        /// <summary>
        /// Выдать среднее значеие кодов АЦП текущего датчика
        /// </summary>
        /// <returns>Список значений</returns>      
        #endregion

        //var allocationThread = new System.Threading.Thread(new System.Threading.ThreadStart(() =>
        //       {
        //           try
        //           {
        //               lock (controller)
        //                   terminal = controller.AllocateTerminal();
        //           }
        //           catch (Exception ex)
        //           {
        //               log.Error(string.Format("Allocate terminal error: ", ex.Message));
        //           }
        //       }))
        //       { IsBackground = true };

        //       allocationThread.Start();
        //       if (!allocationThread.Join(new TimeSpan(0, 0, 0, 0, AllocationTimeout)))
        //       {
        //           allocationThread.Abort();
        //           log.Error("Allocation terminal timeout");
        //           terminalID= -1;
        //       }
    }
}
