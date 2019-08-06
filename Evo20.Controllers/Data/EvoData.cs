using System;
using System.CodeDom;
using System.IO;
using System.Threading;
using Evo20.Commands.Abstract;
using Evo20.Commands.AnswerCommands;
using Evo20.Sensors;

namespace Evo20.Controllers.Data
{
    public class EvoData : AbstractData, IDisposable
    {
       
        public struct AxisData
        {
            public bool IsPositionReached;
            public bool IsPowerOn;
            public bool IsMove;
            public bool IsZeroFound;
            public double Position;
            public double SpeedOfRate;
            public double AxisTemperature;
            public double Correction;
            public AxisData(bool isZeroFunded, bool isPositionReached, bool isPowerOn,
                bool isMove, double position, double speedOfRate,
                double axisTemperature, double correction)
            {
                IsZeroFound = isZeroFunded;
                IsPositionReached = isPositionReached;
                IsPowerOn = isPowerOn;
                IsMove = isMove;
                Position = position;
                SpeedOfRate = speedOfRate;
                AxisTemperature = axisTemperature;
                Correction = correction;
            }
        }

        public AxisData X;

        public AxisData Y;

        private double _nextTemperature;

        public double NextTemperature
        {
            set
            {
                if (TemperatureReachedEvent != null && System.Math.Abs(_nextTemperature - _currentTemperature) > 0)
                {
                    TemperatureReachedEvent.Reset();
                }
                _nextTemperature = value;
            }
            get
            {
                return _nextTemperature;
            }
        }

        private double _currentTemperature;

        public double CurrentTemperature
        {
            set
            {            
                _currentTemperature = value;
            }
            get
            {
                return _currentTemperature;
            }
        }

        public Position NextPosition;

        private Position _currentPosition;

        public Position CurrentPosition
        {
            get { return _currentPosition; }
            set
            {
                _currentPosition = value;
                if (value.Equals(NextPosition))
                {
                    PositionReachedEvent?.Set();
                }
            }
        }

        public bool IsCameraPowerOn;

        public bool IsTemperatureReached;

        public readonly ManualResetEvent TemperatureReachedEvent;

        public readonly ManualResetEvent MovementEndedEvent;

        public readonly ManualResetEvent PositionReachedEvent;

        private static EvoData _current;

        public static EvoData Instance => _current ?? (_current = new EvoData());

        private EvoData()
        {
            X = new AxisData(false, false, false, false, 0, 0, 0, 0);
            Y = new AxisData(false, false, false, false, 0, 0, 0, 0);
            _currentTemperature = 0;
            IsCameraPowerOn = false;
            _nextTemperature = 0;
            TemperatureReachedEvent = new ManualResetEvent(false);
            MovementEndedEvent = new ManualResetEvent(false);
            PositionReachedEvent = new ManualResetEvent(false);
            _currentPosition = new Position();

        }

        #region Methods gets information from evo commands

        public void GetCommandInfo(AxisStatusAnswer cmd)
        {
            X.IsZeroFound = cmd.IsZero1Found;
            Y.IsZeroFound = cmd.IsZero2Found;

            X.IsMove = cmd.IsAxis1Move;
            Y.IsMove = cmd.IsAxis2Move;

            if (!cmd.IsAxis1Move && !cmd.IsAxis2Move)
                MovementEndedEvent.Set();
        }

        public void GetCommandInfo(TemperatureStatusAnswer cmd)
        {
            if (cmd.IsTemperatureReached)
            {
                IsTemperatureReached = true;
                TemperatureReachedEvent.Set();
            }
            IsCameraPowerOn = cmd.IsPowerOn;
        }

        public void GetCommandInfo(RotaryJointTemperatureQueryAnswer cmd)
        {
            if (cmd.Axis == Axis.First)
                X.AxisTemperature = cmd.Temperture;
            if (cmd.Axis == Axis.Second)
                Y.AxisTemperature = cmd.Temperture;

            _currentTemperature = X.AxisTemperature;
        }

        public void GetCommandInfo(AxisPositionQueryAnswer cmd)
        {
            if (cmd.Axis == Axis.First)
            {
                X.Position = cmd.Position;
                CurrentPosition.FirstPosition = (int)cmd.Position;
            }

            if (cmd.Axis == Axis.Second)
            {
                Y.Position = cmd.Position;
                CurrentPosition.SecondPosition = (int)cmd.Position;
            }
            if(CurrentPosition.Equals(NextPosition))
                PositionReachedEvent?.Set();
        }

        public void GetCommandInfo(AxisRateQueryAnswer cmd)
        {
            if (cmd.Axis == Axis.First)
            {
                X.SpeedOfRate = cmd.SpeedOfRate;
                CurrentPosition.SpeedFirst = (int)cmd.SpeedOfRate;
            }

            if (cmd.Axis == Axis.Second)
            {
                Y.SpeedOfRate = cmd.SpeedOfRate;
                CurrentPosition.SpeedSecond = (int)cmd.SpeedOfRate;
            }
            if (CurrentPosition.Equals(NextPosition))
                PositionReachedEvent?.Set();
        }

        public void GetCommandInfo(RequestedAxisPositionReachedAnswer cmd)
        {
            X.IsPositionReached = cmd.AxisXReached;
            Y.IsPositionReached = cmd.AxisYReached;
        }

        public void GetCommandInfo(ActualTemperatureQueryAnswer cmd)
        {
            _currentTemperature = cmd.Temperature;
            if ((_currentTemperature - _nextTemperature) < 0.5 && (_currentTemperature - _nextTemperature) > -0.5)
            {
                TemperatureReachedEvent.Set();
                IsTemperatureReached = true;
            }
        }

        #endregion

        public override bool ReadSettings(ref  StreamReader file)
        {
            bool isSuccess = ReadParamFromFile(ref file,
                "Поправка по оси стола №1",
                ref  X.Correction);
            if (!isSuccess)
                return false;
            isSuccess = ReadParamFromFile(ref file,
                "Поправка по оси стола №2",
                ref  Y.Correction);
            if (!isSuccess)
                return false;
            return true;
        }

        #region IDisposable Support
        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    MovementEndedEvent.Dispose();
                    TemperatureReachedEvent.Dispose();
                }
                _disposedValue = true;
            }
        }
     
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
