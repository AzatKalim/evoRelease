﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Configuration;
using Evo20.Config;
using Evo20.EvoCommandsLib;
using Evo20.Controllers;
using Evo20.SensorsConnection;
using Evo20.PacketsLib;
using Evo20.Math;


namespace Evo20.Controllers
{  
    public class Controller:ControllerEvo
    {
        #region Devegates and Events

        public delegate void TemperatureSabilizationHandler(bool result);

        public delegate void CycleEndedHandler(bool result);

        public delegate void ControllerExceptions(Exception exception);
        //событие окончания работы цикла
        public event CycleEndedHandler EventHandlersListCycleEnded;
        //событие обработки ошибок
        public event ControllerExceptions EventHandlerListForControllerExceptions;
        //событие изменения состояния соединения с датчиком
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

        public PacketsData lastPacket;

        #endregion

        #region Properties

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
                if(Mode == WorkMode.CalibrationMode)
                    return CycleData.Current.CalibrationTemperatures.Count;
                if(Mode == WorkMode.CheckMode)
                    return CycleData.Current.CheckTemperatures.Count;
                return 0;
            }
        }

        public int TemperutureIndex
        {
            get
            {
                if (Mode == WorkMode.CalibrationMode)
                    return CycleData.Current.FindCalibrationTemperatureIndex((int)EvoData.Current.CurrentTemperature);
                if (Mode == WorkMode.CheckMode)
                    return CycleData.Current.FindCheckTemperatureIndex((int)EvoData.Current.CurrentTemperature);
                return 0;
            }
           
        }

        public int PacketsCollectedCount
        {
            get
            {
                if (CurrentSensor == null)
                    return 0;
                if (Mode == WorkMode.CalibrationMode)                   
                    return CurrentSensor.PacketsArivedCountCalibration(temperatureOfCollect,CurrentPositionNumber);
                if( Mode==WorkMode.CheckMode)
                    return CurrentSensor.PacketsArivedCountCheck(temperatureOfCollect, CurrentPositionNumber);
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

        private Controller():base()
        {
            sensorHandler = new SensorHandler();
            //подписка на события датчика
            sensorHandler.EventHandlersListForController += NewPacketDataHandler;
            sensorHandler.EventHandlerListForStateChange += SensorHandlerStatusChanged;
            sensorHandler.EventHandlerListForExeptions += SensorExeptionHandler;
            sensorData = new SensorData();
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
            //добавляем в список датчиков ДЛУ и ДУС
            sensorsList.Add(new DLY(CycleData.Current.CalibrationTemperatures,
                CycleData.Current.CheckTemperatures,
                sensorData.CalibrationDLYMaxPacketsCount,
                sensorData.CheckDLYMaxPacketsCount));
            sensorsList.Add(new DYS(CycleData.Current.CalibrationTemperatures,
                CycleData.Current.CheckTemperatures,
                sensorData.CalibrationDYSMaxPacketsCount,
                sensorData.CheckDYSMaxPacketsCount));
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
            var newPacketsData= sensorHandler.DataHandle();
            if (newPacketsData == null)
            {
                return;
            }
            lastPacket=newPacketsData;
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
                    Evo20.Log.WriteLog("Ошибка:перед запуском цикла. Режим работы не установлен!");
                    EventHandlersListCycleEnded(false);
                    return;
            }

            PowerOnCamera(true);
            PowerOnAxis(Axis.ALL, true);
            FindZeroIndex(Axis.ALL);   
            //цикл по температурам
            for (int i = CycleData.Current.StartTemperatureIndex; i < temperatures.Count; i++)
            {
                StopAxis(Axis.ALL);
                SetAxisRate(Axis.ALL, Config.Config.BASE_MOVE_SPEED);
                SetAxisPosition(Axis.ALL, 0);
                StartAxis(Axis.ALL);
                SetTemperatureChangeSpeed(Config.Config.SPEED_OF_TEMPERATURE_CHANGE);
                lock (EvoData.Current)
                {
                    EvoData.Current.NextTemperature = temperatures[i];
                }
                SetTemperature(temperatures[i]);
                Evo20.Log.WriteLog("Установлена температура камеры " + temperatures[i] + " скорость набора температtуры " + Config.Config.SPEED_OF_TEMPERATURE_CHANGE);
                //Ожидание достижения температуры
//#if !DEBUG
                EvoData.Current.TemperatureReachedEvent.WaitOne();
//#endif
                EvoData.Current.TemperatureReachedEvent.Reset();
                Evo20.Log.WriteLog("Температура  " + temperatures[i] + " достигнута");
                if (EventHandlerListForTemperatureStabilization != null)
                    EventHandlerListForTemperatureStabilization(false);
                Evo20.Log.WriteLog(string.Format("{0}:Начало стабилизации температуры.Время стабилизации {1}", DateTime.Now.TimeOfDay, StabilizationTime));
//#if !DEBUG
//                //ожидание стабилизации температуры
                Thread.Sleep(StabilizationTime);
//#endif
                Evo20.Log.WriteLog(string.Format("{0}:Стабилизация температуры завершена",DateTime.Now.TimeOfDay));
                temperatureOfCollect = temperatures[i];
                if (EventHandlerListForTemperatureStabilization != null)
                    EventHandlerListForTemperatureStabilization(true);
                //для каждого датчика
                for (int j = 0; j < sensorsList.Count; j++)
			    {
                    CurrentSensor = sensorsList[j];
                    Evo20.Log.WriteLog(string.Format("{0}:Запуск подцикла датчика {1}", DateTime.Now.TimeOfDay,CurrentSensor.Name));
                    //запуск подцикла датчика
                    bool isCyclePartSuccess = SensorCyclePart(profiles[j]);
                    if (!isCyclePartSuccess)
                    {
                        Evo20.Log.WriteLog("Ошибка:Не выполнена часть цикла для датчика :" + sensorsList[j].Name + " при температуре" + temperatures[i]);
                        EventHandlersListCycleEnded(false);
                        return;
                    }
                    Evo20.Log.WriteLog(string.Format("{0}:Подцикл датчика завершен {1}", DateTime.Now.TimeOfDay, CurrentSensor.Name));
                }
                //записываем пакеты
                WriteRedPackets();                            
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
                    Evo20.Log.WriteLog(string.Format("{0} Новое положение для датчика {1}",DateTime.Now.TimeOfDay,j));
                    StopAxis(Axis.ALL);
                    EvoData.Current.movementEndedEvent.WaitOne(THREADS_SLEEP_TIME);
                    EvoData.Current.movementEndedEvent.Reset();
                    CurrentPositionNumber = j;
                    Evo20.Log.WriteLog(string.Format("Задание положения осей: X {0}:{1}. Y {2}:{3}", profile[j].axisX,profile[j].speedX,profile[j].axisY,profile[j].speedY));
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
                    //ожидаем пока установятся позиции
                    Thread.Sleep(THREADS_SLEEP_TIME);
                    //ожидание сбора пакетов
                    canCollect = true;
                    CurrentSensor.PacketsCollectedEvent.WaitOne();
                    canCollect = false;
                    Evo20.Log.WriteLog(string.Format("{0}: Пакеты в положении {1} собраны",DateTime.Now.TimeOfDay,CurrentPositionNumber));
                }

            }
            catch (Exception exception)
            {
                Evo20.Log.WriteLog(string.Format("Возникло исключение цикла при датчике:{0} ,при шаге {1}", CurrentSensor.Name, j));
                Evo20.Log.WriteLog(exception.ToString());
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
            if (currentSensor != null)
            {
                switch (Mode)
                {
                    case WorkMode.CalibrationMode:
                        return CurrentSensor.СalculateCalibrationAverage(temperatureOfCollect, CurrentPositionNumber);
                    case WorkMode.CheckMode:
                        return CurrentSensor.СalculateCheckAverage(temperatureOfCollect, CurrentPositionNumber);
                    default:
                        break;
                }
            }
            if (lastPacket == null)
                return null;
            double[] ua = lastPacket.MeanUA;
            double[] w = lastPacket.MeanW;
            double[] a = lastPacket.MeanA;
            double[] uw = lastPacket.MeanUW;
            if (ua == null || w == null || a == null || ua == null)
                return null;
            var result = new List<double>();
            result.AddRange(ua);
            result.AddRange(w);
            result.AddRange(a);
            result.AddRange(uw);
            double mul= 0.5/System.Math.Pow(2,28);
            for (int i = 0; i < result.Count; i++)
            {
                result[i] *= mul;
            }
            return result;
            
        }

        /// <summary>
        /// Вычислить калибровочные коэфииценты
        /// </summary>
        /// <param name="file">файл для записи результатов</param>
        /// <returns>true- выполнено успешно,false-возникла ошибка </returns>
        public bool ComputeCoefficents(StreamWriter file)
        {
            bool result= CalculatorCoefficients.CalculateCoefficients(sensorsList[0].GetCalibrationADCCodes(),
                sensorsList[1].GetCalibrationADCCodes(),
                file);
            if (!result)  
            {
                Evo20.Log.WriteLog("Вычисление коэфицентов не выполнено!");
                return result;
            }
            Evo20.Log.WriteLog("Вычисление коэфицентов выполнено!");
            return result;
        }

        /// <summary>
        /// Запись всех пакетов в файл( не работает пока)
        /// </summary>
        /// <param name="file">файл для записи</param>
        /// <returns></returns>
        public bool WritePackets(StreamWriter file)
        {
            return sensorData.WriteAllPackets(sensorsList.ToArray(),file);
        }

        /// <summary>
        /// Чтение пакетов их файла
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool ReadDataFromFile(StreamReader file )
        {
            var result=sensorData.ReadDataFromFile(sensorsList.ToArray(), file);
            if (result)
                CycleData.Current.StartTemperatureIndex = sensorsList[0].CalibrationPacketsCollection.Count;
            return result;
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
                var file = new StreamReader(fileName, Encoding.GetEncoding(1251));
                if (!CycleData.Current.ReadSettings(ref file))
                    return false;
                if (!sensorData.ReadSettings(ref file))
                    return false;
                file.Close();
            }
            catch (Exception exception)
            {
                Evo20.Log.WriteLog("Возникла ошибка чтения файла настроек" + exception.ToString());
                throw exception;
            }        
            return true;
        }

        private bool WriteRedPackets()
        {
            Evo20.Log.WriteLog("Запись уже считанных пакетов в файл");

            foreach (var sensor in sensorsList)
            {
                
                if (!sensor.WriteRedPackets())
                {
                    Log.WriteLog(string.Format("Запись прервана на датчике {0}", sensor.Name));
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}
