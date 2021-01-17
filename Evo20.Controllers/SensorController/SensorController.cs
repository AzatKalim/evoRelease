using System;
using System.Collections.Generic;
using System.Timers;
using Evo20.Controllers.Data;
using Evo20.Packets;
using Evo20.Sensors;
using Evo20.SensorsConnection;
using Evo20.Utils;
using Evo20.Utils.Extensions;

namespace Evo20.Controllers
{ 
    public sealed class SensorController : IDisposable
    {
        public event ControllerExceptions SensorControllerException;

        public event ConnectionChangeHandler SensorConnectionChanged;

        public event PacketReceiveTimeoutHandler OnPacketReceiveTimeout;

        public delegate void ConnectionChangeHandler(object sender, EventArgs e);

        public delegate void ControllerExceptions(object sender, EventArgs e);

        public delegate void PacketReceiveTimeoutHandler(object sender, EventArgs e);

        private static SensorController _sensorController;

        public static SensorController Instance => _sensorController ?? (_sensorController = new SensorController());

        readonly SensorHandler _sensorHandler;

        readonly Timer packetReceiveTimer = new Timer(TimeSpan.FromMinutes(Config.Instance.PacketReceiveTimeout).TotalMilliseconds);

        bool _canCollect;

        object _locker = new object();
        public bool CanCollect
        {
            get { return _canCollect; }
            set { _canCollect = value; }
        }

        ISensor _currentSensor;

        public ISensor CurrentSensor
        {
            set
            {
                if (value == null)
                    return;
                _currentSensor = value;
            }
            get
            {
                return _currentSensor;
            }
        }

        public PacketsData LastPacket;

        public int TemperatureOfCollect = 0;

        public int CurrentPositionNumber {get;set;}

        public int TemperatureIndex { get; set; }

        public int CurrentPositionCount
        {
            get
            {
                if (CurrentSensor == null)
                    return 0;
                if (Controller.Instance.Mode == WorkMode.CalibrationMode && CurrentSensor.CalibrationProfile != null)
                    return CurrentSensor.CalibrationProfile.Length;
                if (Controller.Instance.Mode == WorkMode.CheckMode && CurrentSensor.CheckProfile!= null)
                    return CurrentSensor.CheckProfile.Length;
                return 0;
            }
        }

        private List<ISensor> _sensorsList;

        public List<ISensor> SensorsList => _sensorsList ?? (_sensorsList = new List<ISensor>
        {
            new DLY(CycleData.Instance.CalibrationTemperatures,
                CycleData.Instance.CheckTemperatures,
                SensorData.Instance.CalibrationDlyMaxPacketsCount,
                SensorData.Instance.CheckDlyMaxPacketsCount),
            new Dys(CycleData.Instance.CalibrationTemperatures,
                CycleData.Instance.CheckTemperatures,
                SensorData.Instance.CalibrationDysMaxPacketsCount,
                SensorData.Instance.CheckDysMaxPacketsCount)
        });

        public void StartCollect()
        {           
            Log.Instance.Info($"{nameof(StartCollect)}");
            _currentSensor.PacketsCollectedEvent.Reset();
            CanCollect = true;
        }
        public void StopCollect()
        {
            Log.Instance.Info($"{nameof(StopCollect)}");
            CanCollect = false;
        }
        public int PacketsCollectedCount
        {
            get
            {
                if (CurrentSensor == null)
                    return 0;
                switch (Controller.Instance.Mode)
                {
                    case WorkMode.CalibrationMode:
                        return CurrentSensor.PacketsArivedCountCalibration(TemperatureIndex, CurrentPositionNumber);
                    case WorkMode.CheckMode:
                        return CurrentSensor.PacketsArivedCountCheck(TemperatureIndex,CurrentPositionNumber);
                    default:
                        return 0;
                }
            }
        }

        private void OnPacketReceiveTimer(object sender, EventArgs e)
        {
            packetReceiveTimer.Stop();
            OnPacketReceiveTimeout.Invoke(sender, e);
        }

        public void ResetPacketReceiveTimer()
        {
            packetReceiveTimer.Reset();
        }

        private void NewPacketDataHandler(object sender,EventArgs e)
        {
            var newPacketsData = _sensorHandler.DataHandle();
            if (newPacketsData == null)
            {
                return;
            }

            packetReceiveTimer.Reset();
            LastPacket = newPacketsData;
            if (Controller.Instance.Mode == WorkMode.CalibrationMode && _canCollect)
            {
                _currentSensor.AddCalibrationPacketData(newPacketsData,
                    TemperatureIndex,
                    CurrentPositionNumber);
            }
            else
            {
                if (Controller.Instance.Mode == WorkMode.CheckMode && _canCollect)
                {
                    _currentSensor.AddCheckPacketData(newPacketsData,
                        TemperatureIndex,
                        CurrentPositionNumber);
                }
            }
        }

        private void SensorHandlerStatusChanged(object sender,EventArgs e)
        {
            SensorConnectionChanged?.Invoke(this,e);
        }
       
        private void SensorExeptionHandler(object sender,EventArgs e)
        {
            Controller.Instance.Stop();
            SensorControllerException?.Invoke(this,e);
        }

        public List<double> GetSensorData()
        {
            if (_currentSensor != null)
            {
                switch (Controller.Instance.Mode)
                {
                    case WorkMode.CalibrationMode:
                        return CurrentSensor.СalculateCalibrationAverage(TemperatureIndex, CurrentPositionNumber);
                    case WorkMode.CheckMode:
                        return CurrentSensor.СalculateCheckAverage(TemperatureIndex, CurrentPositionNumber);
                }
            }
            if (LastPacket == null)
                return null;
            double[] ua = LastPacket.MeanUa;
            double[] w = LastPacket.MeanW;
            double[] a = LastPacket.MeanA;
            double[] uw = LastPacket.MeanUw;
            if (ua == null || w == null || a == null || uw==null)
                return null;
            var result = new List<double>();
            result.AddRange(w);
            result.AddRange(uw);
            result.AddRange(a);
            result.AddRange(ua);
            return result;

        }

        public SensorController()
        {
            _sensorHandler = new SensorHandler();
            _sensorHandler.PacketDataCollected += NewPacketDataHandler;
            _sensorHandler.EventHandlerListForStateChange += SensorHandlerStatusChanged;
            _sensorHandler.EventHandlerListForExeptions += SensorExeptionHandler;
            packetReceiveTimer.Elapsed += OnPacketReceiveTimer;
        }

        public bool StartComPortConnection(string portName)
        {
            packetReceiveTimer.Start();
            return _sensorHandler.StartConnection(portName);
        }

        public void PauseComPortConnection()
        {
            _sensorHandler.PauseConnection();
        }

        public void StopComPortConnection()
        {
            packetReceiveTimer.Stop();
            _sensorHandler.StopConnection();
        }

        public void ClearData(int temperatureIndex,WorkMode mode)
        {
            Log.Instance.Debug("Очистка данных :индекс температуры :{0}, режим :{1}", temperatureIndex, mode);
            foreach (var sensor in _sensorsList)
            {
                if( mode == WorkMode.CalibrationMode)
                    sensor.CalibrationPacketsCollection[temperatureIndex].ClearData();
                else
                    sensor.CheckPacketsCollection[temperatureIndex].ClearData();
            }
        }

        #region IDisposable Support
        private bool _disposedValue;

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _sensorHandler.Dispose();
                }

                _disposedValue = true;
            }
        }
       
        ~SensorController()
        {         
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
