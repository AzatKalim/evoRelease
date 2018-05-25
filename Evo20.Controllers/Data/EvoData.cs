using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Configuration;
using Evo20.EvoCommandsLib;

namespace Evo20.Controllers
{
    /// <summary>
    /// Класс хранящий информацию о состоянии evo
    /// </summary>
    public class EvoData:AbstractData
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

        public static int SPEED_OF_TEMPERATURE_CHANGE = Convert.ToInt32(ConfigurationManager.AppSettings.Get("SPEED_OF_TEMPERATURE_CHANGE"));
        public static int BASE_MOVE_SPEED = Convert.ToInt32(ConfigurationManager.AppSettings.Get("BASE_MOVE_SPEED"));

        public AxisData x;
        public AxisData y;

        public double nextTemperature;
        public double NextTemperature
        {
            set
            {
                if (temperatureReachedEvent != null && nextTemperature != currentTemperature)
                {
                    temperatureReachedEvent.Reset();
                }
                nextTemperature = value;
            }
            get
            {
                return nextTemperature;
            }
        }
        public double currentTemperature;
        public bool isCameraPowerOn;
        public bool isTemperatureReached;

        public ManualResetEvent temperatureReachedEvent;
        public ManualResetEvent movementEndedEvent;


        public EvoData()
        {
            x = new AxisData(false, false, false, false, 0, 0, 0,0);
            y = new AxisData(false, false, false, false, 0, 0, 0,0);
            currentTemperature = 0;
            isCameraPowerOn = false;
            nextTemperature = 0;
            temperatureReachedEvent = new ManualResetEvent(false);
            movementEndedEvent = new ManualResetEvent(false);
        }

        #region Methods gets information from evo commands

        public void GetCommandInfo(Axis_Status_answer cmd)
        {
            x.isZeroFound = cmd.is_zero_1_found;
            y.isZeroFound = cmd.is_zero_2_found;

            x.isMove = cmd.is_axis_1_move;
            y.isMove = cmd.is_axis_2_move;

            if (!cmd.is_axis_1_move && !cmd.is_axis_2_move)
            {
                movementEndedEvent.Set();
            }
        }

        public void GetCommandInfo(Temperature_status_answer cmd)
        {
            if (cmd.is_temperature_reached)
            {
                isTemperatureReached = true;
                temperatureReachedEvent.Set();
            }

            isCameraPowerOn = cmd.is_power_on;
        }

        public void GetCommandInfo(Rotary_joint_temperature_Query_answer cmd)
        {
            if (cmd.axis== Axis.X)
            {
                x.axisTemperature = cmd.temperture;
            }
            if (cmd.axis == Axis.Y)
            {
                y.axisTemperature = cmd.temperture;
            }
            currentTemperature = x.axisTemperature;
        }

        public void GetCommandInfo(Axis_Position_Query_answer cmd)
        {
            if (cmd.axis == Axis.X)
            {
                x.position = cmd.position;
            }
            if (cmd.axis == Axis.Y)
            {
                y.position = cmd.position;
            }
        }

        public void GetCommandInfo(Axis_Rate_Query_answer cmd)
        {
            if (cmd.axis == Axis.X)
            {
                x.speedOfRate = cmd.speedOfRate;
            }
            if (cmd.axis == Axis.Y)
            {
                y.speedOfRate = cmd.speedOfRate;
            }
        }

        public void GetCommandInfo(Requested_axis_position_reached_answer cmd)
        {
            x.isPositionReached = cmd.axisXreached;
            y.isPositionReached = cmd.axisYreached;
        }

        public void GetCommandInfo(Actual_temperature_query_answer cmd)
        {
            currentTemperature = cmd.temperature;
            if ((currentTemperature - nextTemperature) < 0.5 && (currentTemperature - nextTemperature) > -0.5)
            {
                temperatureReachedEvent.Set();
                isTemperatureReached = true;
            }

        }

        #endregion

        public override bool ReadSettings(ref  StreamReader file)
        {
            bool isSuccess = ReadParamFromFile(ref file,
                "Поправка по оси стола №1",
                ref  x.correction);
            if (!isSuccess)
                return false;
            isSuccess = ReadParamFromFile(ref file,
                "Поправка по оси стола №2",
                ref  y.correction);
            if (!isSuccess)
                return false;
            return true;
        }
    }
}
