using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using ComputeCoefficients;
//my libs
using Evo_20_commands;
using Evo_20form.Data;
using Evo_20form.Sensors;
using Evo_20form.Utils;
using PacketsLib;
using System.Configuration;

namespace Evo_20form.Controllers
{  
    class Controller:ControllerEvo
    {
        #region Constans

        public static string DEFAULT_SETTINGS_FILE_NAME = ConfigurationManager.AppSettings.Get("DEFAULT_SETTINGS_FILE_NAME");

        #endregion 

        #region Devegates and Events

        public delegate void TemperatureSabilizationHandler(bool result);
        public delegate void CycleEndedHandler(bool result);

        public delegate void ControllerExceptions(Exception exception);
        //событие окончания работы цикла
        public event CycleEndedHandler EventHandlersListCycleEnded;
        //событие обработки ошибок
        public event ControllerExceptions EventHandlerListForControllerExceptions;
        //событие ихменения состояния соединения с датчиком
        public event ConnectionChangeHandler EventHandlerListForSensorConnectionChange;

        public event TemperatureSabilizationHandler EventHandlerListForTemperatureStabilization;

        #endregion

        #region Private Fields

        Thread cycleThread;

        SensorHandler sensorHandler;

        bool canCollect=false;

        int temperatureOfCollect = 0;

        List<ISensor> sensorsList;

        ISensor currentSensor;

        #endregion

        #region Properties

        public int StabilizationTime
        {
            get
            {
                if (Mode == WorkMode.CalibrationMode)
                {
                    return cycleData.calibrationStabTime;
                }
                if (Mode ==WorkMode.CheckMode)
                {
                    return cycleData.checkStabTime;
                }
                return 0;
            }
        }

        public ISensor CurrentSensor
        {
            set
            {
                if (value == null)
                    return;
                currentSensor = value;
            }
            get
            {
                return currentSensor;
            }
        }

        public CycleData cycleData
        {
            get;
            private set;
        }

        public SensorData sensorData
        {
            get;
            private set;
        }

        public double CurrentTemperature
        {
            get
            {
                return temperatureOfCollect;
            }
        }
     
        public int TemperaturesCount
        {
            get
            {
                if (Mode == WorkMode.CalibrationMode)
                    return cycleData.CalibrationTemperatures.Count;
                else
                    return cycleData.CheckTemperatures.Count;
            }
        }

        public int TemperutureIndex
        {
            get
            {
                if (Mode == WorkMode.CalibrationMode)
                    return cycleData.FindCalibrationTemperatureIndex((int)evoData.currentTemperature);
                else
                    return cycleData.FindCheckTemperatureIndex((int)evoData.currentTemperature);
            }
           
        }

        public int PacketsCollectedCount
        {
            get
            {
                if (currentSensor == null)
                {
                    return 0;
                }
                if (Mode == WorkMode.CalibrationMode)
                {                    
                    return CurrentSensor.PacketsArivedCountCalibration(temperatureOfCollect,CurrentPositionNumber);
                }
                if( Mode==WorkMode.CheckMode)
                {
                    return CurrentSensor.PacketsArivedCountCheck(temperatureOfCollect, CurrentPositionNumber);
                }
                return 0;
            }
        }

        public int CurrentPositionNumber
        {
            get;
            private set;
        }

        #endregion

        #region Constructors

        public Controller():base()
        {
            sensorHandler = new SensorHandler();
            //подписка на события датчика
            sensorHandler.EventHandlersListForController += NewPacketDataHandler;
            sensorHandler.EventHandlerListForStateChange += SensorHandlerStatusChanged;
            sensorHandler.EventHandlerListForExeptions += SensorExeptionHandler;
            sensorData = new SensorData();
            cycleData= new CycleData();
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
            bool resultEvoStart= StartEvoConnection();
            if (!resultEvoStart)
                return false;
            bool resultComPortStart =StartComPortConnection(comPortName);
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
            if (routineThread!=null && routineThread.IsAlive)
                routineThread.Abort();
            if(cycleThread!=null && cycleThread.IsAlive)
                cycleThread.Abort();
            StopComPortConnection();
            StopEvoConnection();
        }

        /// <summary>
        /// Приостановить работу
        /// </summary>
        public void Pause()
        {
            routineThread.Abort();
            cycleThread.Suspend();
        }

        public bool StartComPortConnection(string portName)
        {
            return sensorHandler.StartConnection(portName);
        }

        public void PauseComPortConnection()
        {
            commandHandler.PauseConnection();
        }

        public void StopComPortConnection()
        {
            sensorHandler.StopConnection();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Обработка события прихода новогосообщения
        /// </summary>
        private void NewPacketDataHandler()
        {
            //извлекаем новые пакеты 
            PacketsData newPacketsData= sensorHandler.DataHandle();
            if (newPacketsData == null)
            {
                return;
            }
            //Добавляем их 
            if (Mode == WorkMode.CalibrationMode && canCollect)
            {
                currentSensor.AddCalibrationPacketData(newPacketsData,
                    temperatureOfCollect,
                    CurrentPositionNumber);
            }
            else
            {
                if (Mode == WorkMode.CheckMode && canCollect)
                {
                    currentSensor.AddCheckPacketData(newPacketsData,
                        temperatureOfCollect,
                        CurrentPositionNumber);
                }
            }
        }

        /// <summary>
        ///Обработка изменения статуса соединения с датчиком
        /// </summary>
        /// <param name="newState">новое состояние</param>
        private void SensorHandlerStatusChanged(ConnectionStatus newState)
        {
            if (EventHandlerListForSensorConnectionChange != null)
            {
                EventHandlerListForSensorConnectionChange(newState);
            }
        }

        /// <summary>
        /// Обработчик ошибок сенсора
        /// </summary>
        /// <param name="sensorExeption">Ошибка</param>
        private void SensorExeptionHandler(Exception sensorExeption)
        {
            Stop();
            if (EventHandlerListForControllerExceptions != null)
            {
                EventHandlerListForControllerExceptions(sensorExeption);
            }
        }

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
            Cycle(cycleData.CalibrationTemperatures);
            Mode = WorkMode.NoMode;
        }

        /// <summary>
        /// Запуск цикла проверки
        /// </summary>
        public void CheckCycle()
        {
            Mode = WorkMode.CheckMode;
            Cycle(cycleData.CheckTemperatures);
            Mode = WorkMode.NoMode;
        }

        /// <summary>
        /// ОСновной метод цикла
        /// </summary>
        /// <param name="temperatures">Список температур</param>
        private void Cycle(List<int> temperatures)
        {
            //профили датчиков
            List<ProfilePart[]> profiles = null;
            switch (Mode)
            {
                case WorkMode.CalibrationMode:
                    {
                        profiles = new List<ProfilePart[]>();
                        foreach (var sensor in sensorsList)
                        {
                            profiles.Add(sensor.CalibrationProfile);
                        }
                    }
                    break;
                case WorkMode.CheckMode:
                    profiles = new List<ProfilePart[]>();
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

            PowerOnCamera(true);
            PowerOnAxis(Axis.ALL, true);
            FindZeroIndex(Axis.ALL);   
            //цикл по температурам
            for (int i = 0; i < temperatures.Count; i++)
            {
                StopAxis(Axis.ALL);
                SetAxisRate(Axis.ALL, EvoData.BASE_MOVE_SPEED);
                SetAxisPosition(Axis.ALL, 0);
                StartAxis(Axis.ALL);
                SetTemperatureChangeSpeed(EvoData.SPEED_OF_TEMPERATURE_CHANGE);
                lock (evoData)
                {
                    evoData.nextTemperature = temperatures[i];
                }
                SetTemperature(temperatures[i]);              
                Log.WriteLog("Установлена температура камеры " + temperatures[i] + " скорость набора температtуры " + EvoData.SPEED_OF_TEMPERATURE_CHANGE);
                //Ожидание достижения температуры
                evoData.temperatureReachedEvent.WaitOne(1000);
                evoData.temperatureReachedEvent.Reset();
                Log.WriteLog("Температура  " + temperatures[i] + " достигнута");
                if (EventHandlerListForTemperatureStabilization != null)
                    EventHandlerListForTemperatureStabilization(false);
                //ожидание стабилизации температуры
                Thread.Sleep(StabilizationTime);
          
                temperatureOfCollect = temperatures[i];
                if (EventHandlerListForTemperatureStabilization != null)
                    EventHandlerListForTemperatureStabilization(true);
                //для каждого датчика
                for (int j = 0; j < sensorsList.Count; j++)
			    {
                    CurrentSensor = sensorsList[j];
                    //запуск подцикла датчика
                    bool isCyclePartSuccess = SensorCyclePart(profiles[j]);
                    if (!isCyclePartSuccess)
                    {
                        Log.WriteLog("Ошибка:Не выполнена часть цикла для датчика :"+ sensorsList[j].Name + " при температуре" + temperatures[i]);
                        EventHandlersListCycleEnded(false);
                        return;
                    }     
                }                                            
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
                    StopAxis(Axis.ALL);
                    evoData.movementEndedEvent.WaitOne(THREADS_SLEEP_TIME);
                    evoData.movementEndedEvent.Reset();
                    CurrentPositionNumber = j;
                    //задание положений и скоростей
                    if (profile[j].speedX != 0)
                    {
                        SetAxisRate(Axis.X, profile[j].speedX);
                        SetAxisMode(ModeParam.Speed, Axis.X);
                    }
                    else
                    {
                        SetAxisPosition(Axis.X, profile[j].axisX);
                        SetAxisMode(ModeParam.Position, Axis.X);
                    }
                    if (profile[j].speedY != 0)
                    {
                        SetAxisRate(Axis.Y, profile[j].speedY);
                        SetAxisMode(ModeParam.Speed, Axis.Y);
                    }
                    else
                    {
                        SetAxisPosition(Axis.Y, profile[j].axisY);
                        SetAxisMode(ModeParam.Position, Axis.Y);
                    }
                    StartAxis(Axis.ALL);
                    CurrentPositionNumber = j;
                    canCollect = true;
                    //ожидаем пока установятся позиции
                    Thread.Sleep(THREADS_SLEEP_TIME);
                    //ожидание сбора пакетов
                    CurrentSensor.PacketsCollectedEvent.WaitOne();
                    CurrentSensor.PacketsCollectedEvent.Reset();

                    canCollect = false;
                }

            }
            catch (Exception exception)
            {
                Log.WriteLog(string.Format("Возникло исключение цикла при датчике:{0} ,при шаге {1}", CurrentSensor.Name, j));
                Log.WriteLog(exception.ToString());
                if (EventHandlerListForControllerExceptions != null)
                {
                    EventHandlerListForControllerExceptions(exception);
                }
                return false;
            }
            return true;
        }

        #endregion

        #region Secondary functions

        /// <summary>
        /// Выдать среднее значеие кодов АЦП текущего датчика
        /// </summary>
        /// <returns>Список значений</returns>
        public List<double> GetSensorData()
        {
            switch (Mode)
            {
                case WorkMode.CalibrationMode:
                    return CurrentSensor.СalculateCalibrationAverage(temperatureOfCollect,CurrentPositionNumber);
                case WorkMode.CheckMode:
                    return CurrentSensor.СalculateCheckAverage(temperatureOfCollect, CurrentPositionNumber);
                default:
                    return null;
            }           
        }

        /// <summary>
        /// Вычислить калибровочные коэфииценты
        /// </summary>
        /// <param name="file">файл для записи результатов</param>
        /// <returns>true- выполнено успешно,false-возникла ошибка </returns>
        public bool ComputeCoefficents(StreamWriter file)
        {
            bool result= CalculatorCoefficients.CalculateCoefficients(sensorsList[0].CalibrationPacketsCollection,
                sensorsList[1].CalibrationPacketsCollection,
                file);
            if (!result)  
            {
                Log.WriteLog("Вычисление коэфицентов не выполнено!");
                return result;
            }
            Log.WriteLog("Вычисление коэфицентов выполнено!");
            return result;
        }

        /// <summary>
        /// Запись всех пакетов в файл( не работает пока)
        /// </summary>
        /// <param name="file">файл для записи</param>
        /// <returns></returns>
        public bool WritePackets(StreamWriter file)
        {
            //return sensorData.WriteAllPackets(file);
            return true;
        }

        /// <summary>
        /// Чтение пакетов их файла
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool ReadDataFromFile(StreamReader file)
        {

            //sensorData.ReadDataFromFile(file);
            return true;
        }

        /// <summary>
        /// Чтение настроек из файла
        /// </summary>
        /// <returns> результат чтения </returns>
        public bool ReadSettings(string fileName)
        {         
            //Попытка чтения из файла
            try
            {             
                Encoding enc = Encoding.GetEncoding(1251);
                StreamReader file = new StreamReader(fileName, enc);
                if (!cycleData.ReadSettings(ref file))
                {
                    return false;
                }
                if (!sensorData.ReadSettings(ref file))
                {
                    return false;
                }
                file.Close();
            }
            catch (Exception exception)
            {
                Log.WriteLog("Возникла ошибка чтения файла настроек" + exception.ToString());
                throw exception;
            }
            //добавляем в список датчиков ДЛУ и ДУС
            sensorsList.Add(new DLY(cycleData.CalibrationTemperatures,
                cycleData.CheckTemperatures,
                sensorData.CalibrationDLYMaxPacketsCount,
                sensorData.CheckDLYMaxPacketsCount));
            sensorsList.Add(new DYS(cycleData.CalibrationTemperatures,
                cycleData.CheckTemperatures,
                sensorData.CalibrationDYSMaxPacketsCount,
                sensorData.CheckDYSMaxPacketsCount));
            return true;
        }


        #endregion
    }
}
