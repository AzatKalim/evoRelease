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
        public event ControllerExceptions SensorControllerException;
        //событие изменения состояния соединения с датчиком
        public event ConnectionChangeHandler SensorConnectionChanged;

        public delegate void ConnectionChangeHandler(ConnectionStatus state);

        public delegate void ControllerExceptions(Exception exception);

        private static SensorController sensorController;

        public static SensorController Instance
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
            get { return canCollect; }
            set { canCollect = value; }
        }

        ISensor currentSensor;

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

        public PacketsData lastPacket;

        public int TemperatureOfCollect = 0;

        public int CurrentPositionNumber
        {
            get;
            set;
        }

        public int CurrentPositionCount
        {
            get
            {
                if (CurrentSensor == null)
                    return 0;
                if (Controller.Instance.Mode == WorkMode.CalibrationMode)
                    return CurrentSensor.CalibrationProfile.Length;
                if (Controller.Instance.Mode == WorkMode.CheckMode)
                    return CurrentSensor.CheckProfile.Length;
                return 0;
            }
        }

        public List<ISensor> sensorsList;

        public List<ISensor> SensorsList
        {
           get
            { 
                if(sensorsList==null)
                {
                    sensorsList = new List<ISensor>();
                    //добавляем в список датчиков ДЛУ и ДУС
                    sensorsList.Add(new DLY(CycleData.Instance.CalibrationTemperatures,
                        CycleData.Instance.CheckTemperatures,
                        SensorData.Instance.CalibrationDLYMaxPacketsCount,
                        SensorData.Instance.CheckDLYMaxPacketsCount));
                    sensorsList.Add(new DYS(CycleData.Instance.CalibrationTemperatures,
                        CycleData.Instance.CheckTemperatures,
                        SensorData.Instance.CalibrationDYSMaxPacketsCount,
                        SensorData.Instance.CheckDYSMaxPacketsCount));
                }
                return sensorsList;
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
                if (Controller.Instance.Mode == WorkMode.CalibrationMode)
                    return CurrentSensor.PacketsArivedCountCalibration(TemperatureOfCollect, CurrentPositionNumber);
                if (Controller.Instance.Mode == WorkMode.CheckMode)
                    return CurrentSensor.PacketsArivedCountCheck(TemperatureOfCollect,CurrentPositionNumber);
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
            if (Controller.Instance.Mode == WorkMode.CalibrationMode && canCollect)
            {
                currentSensor.AddCalibrationPacketData(newPacketsData,
                    TemperatureOfCollect,
                    CurrentPositionNumber);
            }
            else
            {
                if (Controller.Instance.Mode == WorkMode.CheckMode && canCollect)
                {
                    currentSensor.AddCheckPacketData(newPacketsData,
                        TemperatureOfCollect,
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
            if (SensorConnectionChanged != null)
            {
                SensorConnectionChanged(newState);
            }
        }

        /// <summary>
        /// Обработчик ошибок сенсора
        /// </summary>
        /// <param name="sensorExeption">Ошибка</param>
        private void SensorExeptionHandler(Exception sensorExeption)
        {
            Controller.Instance.Stop();
            if (SensorControllerException != null)
            {
                SensorControllerException(sensorExeption);
            }
        }

        public List<double> GetSensorData()
        {
            if (currentSensor != null)
            {
                switch (Controller.Instance.Mode)
                {
                    case WorkMode.CalibrationMode:
                        return CurrentSensor.СalculateCalibrationAverage(TemperatureOfCollect, CurrentPositionNumber);
                    case WorkMode.CheckMode:
                        return CurrentSensor.СalculateCheckAverage(TemperatureOfCollect, CurrentPositionNumber);
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

        public void ClearWritedData(int temperatureIndex,WorkMode mode)
        {
            Log.Instance.Debug("Очистка данных :индекс температуры :{0}, режим :{1}", temperatureIndex, mode);
            foreach (var sensor in sensorsList)
            {
                if( mode == WorkMode.CalibrationMode)
                    sensor.CalibrationPacketsCollection[temperatureIndex].ClearUnneedInfo();
                else
                    sensor.CheckPacketsCollection[temperatureIndex].ClearUnneedInfo();
            }
        }
    }
}
