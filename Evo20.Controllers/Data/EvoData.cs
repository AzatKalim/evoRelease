using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using Evo20.Commands;

namespace Evo20.Controllers
{
    /// <summary>
    /// Класс хранящий информацию о состоянии evo
    /// </summary>
    public class EvoData : AbstractData, IDisposable
    {
        /// <summary>
        /// Структура хранящая информацию об оси
        /// </summary>
        public struct AxisData
        {
            public bool isPositionReached;
            public bool isPowerOn;
            public bool isMove;
            public bool isZeroFound;
            public double position;
            public double speedOfRate;
            public double axisTemperature;
            public double correction;
            public AxisData(bool isZeroFinded, bool isPositionReached, bool isPowerOn,
                bool isMove, double position, double speedOfRate,
                double axisTemperature, double correction)
            {
                this.isZeroFound = isZeroFinded;
                this.isPositionReached = isPositionReached;
                this.isPowerOn = isPowerOn;
                this.isMove = isMove;
                this.position = position;
                this.speedOfRate = speedOfRate;
                this.axisTemperature = axisTemperature;
                this.correction = correction;
            }
        }

        public AxisData X;

        public AxisData Y;

        private double nextTemperature;

        public double NextTemperature
        {
            set
            {
                if (TemperatureReachedEvent != null && nextTemperature != currentTemperature)
                {
                    TemperatureReachedEvent.Reset();
                }
                nextTemperature = value;
            }
            get
            {
                return nextTemperature;
            }
        }

        private double currentTemperature;

        public double CurrentTemperature
        {
            set
            {            
                currentTemperature = value;
            }
            get
            {
                return currentTemperature;
            }
        }

        public bool isCameraPowerOn;

        public bool isTemperatureReached;

        public ManualResetEvent TemperatureReachedEvent;

        public ManualResetEvent movementEndedEvent;

        private static EvoData current;

        public static EvoData Instance
        {
            get
            {
                if (current == null)
                    current = new EvoData();
                return current;
            }
        }

        private EvoData()
        {
            X = new AxisData(false, false, false, false, 0, 0, 0, 0);
            Y = new AxisData(false, false, false, false, 0, 0, 0, 0);
            currentTemperature = 0;
            isCameraPowerOn = false;
            nextTemperature = 0;
            TemperatureReachedEvent = new ManualResetEvent(false);
            movementEndedEvent = new ManualResetEvent(false);
        }

        #region Methods gets information from evo commands

        public void GetCommandInfo(Axis_Status_answer cmd)
        {
            X.isZeroFound = cmd.is_zero_1_found;
            Y.isZeroFound = cmd.is_zero_2_found;

            X.isMove = cmd.is_axis_1_move;
            Y.isMove = cmd.is_axis_2_move;

            if (!cmd.is_axis_1_move && !cmd.is_axis_2_move)
                movementEndedEvent.Set();
        }

        public void GetCommandInfo(Temperature_status_answer cmd)
        {
            if (cmd.is_temperature_reached)
            {
                isTemperatureReached = true;
                TemperatureReachedEvent.Set();
            }
            isCameraPowerOn = cmd.is_power_on;
        }

        public void GetCommandInfo(Rotary_joint_temperature_Query_answer cmd)
        {
            if (cmd.axis == Axis.First)
                X.axisTemperature = cmd.temperture;
            if (cmd.axis == Axis.Second)
                Y.axisTemperature = cmd.temperture;

            currentTemperature = X.axisTemperature;
        }

        public void GetCommandInfo(Axis_Position_Query_answer cmd)
        {
            if (cmd.axis == Axis.First)
                X.position = cmd.position;
            if (cmd.axis == Axis.Second)
                Y.position = cmd.position;
        }

        public void GetCommandInfo(Axis_Rate_Query_answer cmd)
        {
            if (cmd.axis == Axis.First)
                X.speedOfRate = cmd.speedOfRate;
            if (cmd.axis == Axis.Second)
                Y.speedOfRate = cmd.speedOfRate;
        }

        public void GetCommandInfo(Requested_axis_position_reached_answer cmd)
        {
            X.isPositionReached = cmd.axisXreached;
            Y.isPositionReached = cmd.axisYreached;
        }

        public void GetCommandInfo(Actual_temperature_query_answer cmd)
        {
            currentTemperature = cmd.temperature;
            if ((currentTemperature - nextTemperature) < 0.5 && (currentTemperature - nextTemperature) > -0.5)
            {
                TemperatureReachedEvent.Set();
                isTemperatureReached = true;
            }
        }

        #endregion

        public override bool ReadSettings(ref  StreamReader file)
        {
            bool isSuccess = ReadParamFromFile(ref file,
                "Поправка по оси стола №1",
                ref  X.correction);
            if (!isSuccess)
                return false;
            isSuccess = ReadParamFromFile(ref file,
                "Поправка по оси стола №2",
                ref  Y.correction);
            if (!isSuccess)
                return false;
            return true;
        }

        #region IDisposable Support
        private bool disposedValue = false; // Для определения избыточных вызовов

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    movementEndedEvent.Dispose();
                    TemperatureReachedEvent.Dispose();
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить ниже метод завершения.
                // TODO: задать большим полям значение NULL.

                disposedValue = true;
            }
        }

        // TODO: переопределить метод завершения, только если Dispose(bool disposing) выше включает код для освобождения неуправляемых ресурсов.
        // ~EvoData() {
        //   // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
        //   Dispose(false);
        // }

        // Этот код добавлен для правильной реализации шаблона высвобождаемого класса.
        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
            Dispose(true);
            // TODO: раскомментировать следующую строку, если метод завершения переопределен выше.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
