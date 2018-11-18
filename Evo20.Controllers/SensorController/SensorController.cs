using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Evo20.SensorsConnection;
using Evo20.Sensors;
using Evo20.Evo20.Packets;

namespace Evo20.Controllers
{
    public class SensorController
    {
        //событие обработки ошибок
        public event ControllerExceptions EventHandlerListForControllerExceptions;
        //событие изменения состояния соединения с датчиком
        public event ConnectionChangeHandler EventHandlerListForSensorConnectionChange;

        public delegate void ConnectionChangeHandler(ConnectionStatus state);

        public delegate void ControllerExceptions(Exception exception);

        private static SensorController sensorController;

        public static SensorController Current
        {
            get
            {
                if (sensorController == null)
                    sensorController = new SensorController();
                return sensorController;
            }
        }

        SensorHandler sensorHandler;

        bool canCollect = false;

        public bool CanCollect
        {
            get
            {
                return canCollect;
            }
            set
            {
                canCollect = value;
            }
        }

        List<ISensor> sensorsList;

        ISensor currentSensor;

        public PacketsData lastPacket;

        int temperatureOfCollect = 0;

        public int CurrentPositionNumber
        {
            get;
            set;
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

        public int CurrentPositionCount
        {
            get
            {
                if (CurrentSensor == null)
                    return 0;
                if (Controller.Current.Mode == WorkMode.CalibrationMode)
                    return CurrentSensor.CalibrationProfile.Length;
                if (Controller.Current.Mode == WorkMode.CheckMode)
                    return CurrentSensor.CheckProfile.Length;
                return 0;
            }
        }

        public SensorData sensorData
        {
            get;
            private set;
        }

        public int PacketsCollectedCount
        {
            get
            {
                if (CurrentSensor == null)
                    return 0;
                if (Controller.Current.Mode == WorkMode.CalibrationMode)
                    return CurrentSensor.PacketsArivedCountCalibration(temperatureOfCollect, CurrentPositionNumber);
                if (Controller.Current.Mode == WorkMode.CheckMode)
                    return CurrentSensor.PacketsArivedCountCheck(temperatureOfCollect,CurrentPositionNumber);
                return 0;
            }
        }
        /// <summary>
        /// Обработка события прихода новогосообщения
        /// </summary>
        private void NewPacketDataHandler()
        {
            //извлекаем новые пакеты 
            var newPacketsData = sensorHandler.DataHandle();
            if (newPacketsData == null)
            {
                return;
            }
            lastPacket = newPacketsData;
            //Добавляем их 
            if (Controller.Current.Mode == WorkMode.CalibrationMode && canCollect)
            {
                currentSensor.AddCalibrationPacketData(newPacketsData,
                    temperatureOfCollect,
                    CurrentPositionNumber);
            }
            else
            {
                if (Controller.Current.Mode == WorkMode.CheckMode && canCollect)
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
            Controller.Current.Stop();
            if (EventHandlerListForControllerExceptions != null)
            {
                EventHandlerListForControllerExceptions(sensorExeption);
            }
        }

        public List<double> GetSensorData()
        {
            if (currentSensor != null)
            {
                switch (Controller.Current.Mode)
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
            double mul = 0.5 / System.Math.Pow(2, 28);
            for (int i = 0; i < result.Count; i++)
            {
                result[i] *= mul;
            }
            return result;

        }

        public SensorController()
        {
            sensorHandler = new SensorHandler();
            //подписка на события датчика
            sensorHandler.EventHandlersListForController += NewPacketDataHandler;
            sensorHandler.EventHandlerListForStateChange += SensorHandlerStatusChanged;
            sensorHandler.EventHandlerListForExeptions += SensorExeptionHandler;
            sensorData = new SensorData();
        }

        public bool StartComPortConnection(string portName)
        {
            return sensorHandler.StartConnection(portName);
        }

        public void PauseComPortConnection()
        {
            sensorHandler.PauseConnection();
        }

        public void StopComPortConnection()
        {
            sensorHandler.StopConnection();
        }

    }
}
