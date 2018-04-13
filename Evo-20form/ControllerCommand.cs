using System.Threading;
using Evo_20_commands;

namespace Evo_20form
{
    public enum WorkMode
    {
        NoMode,
        CheckMode,
        CalibrationMode,
        Stop,
        Pause,
    }
    abstract class ControllerCommand
    {
        protected CommandHandler commandHandler;

        public EvoData evoData;

        protected Thread routineThread;

        protected Command[] GetRoutineCommands()
        {
            Command[] commands = new Command[] 
            {
                new Axis_Status(),
                new Temperature_status(),
                new Rotary_joint_temperature_Query(Axis.X),
                new Rotary_joint_temperature_Query(Axis.Y),
                new Actual_temperature_query(),
                new Axis_Position_Query(Axis.X),
                new Axis_Position_Query(Axis.Y),
                new Axis_Rate_Query(Axis.X),
                new Axis_Rate_Query(Axis.Y),
                new Requested_axis_position_reached()
            };
            return commands;
        }

        protected void NewCommandHandler()
        {
            Command[] commands;
            lock (commandHandler)
            {
                commands = commandHandler.GetCommands();
            }
            for (int i = 0; i < commands.Length; i++)
            {
                if (commands[i] is Axis_Status_answer)
                {
                    evoData.GetCommandInfo(commands[i] as Axis_Status_answer);
                }
                if (commands[i] is Temperature_status_answer)
                {
                    evoData.GetCommandInfo(commands[i] as Temperature_status_answer);
                }
                if (commands[i] is Rotary_joint_temperature_Query_answer)
                {
                    evoData.GetCommandInfo(commands[i] as Rotary_joint_temperature_Query_answer);
                }
                if (commands[i] is Axis_Position_Query_answer)
                {
                    evoData.GetCommandInfo(commands[i] as Axis_Position_Query_answer);
                }
                if (commands[i] is Axis_Rate_Query_answer)
                {
                    evoData.GetCommandInfo(commands[i] as Axis_Rate_Query_answer);
                }
                if (commands[i] is Actual_temperature_query_answer)
                {
                    evoData.GetCommandInfo(commands[i] as Actual_temperature_query_answer);
                }
                if (commands[i] is Requested_axis_position_reached_answer)
                {
                    evoData.GetCommandInfo(commands[i] as Requested_axis_position_reached_answer);
                }
            }
        }

        protected void PowerOnCamera(bool value)
        {
            commandHandler.SendCommand(new PowerOnTemperatureCamera(value));
        }

        protected void PowerOnAxis(Axis axis, bool value)
        {
            commandHandler.SendCommand(new Axis_Power(axis, value));
        }

        protected void SetAxisRate(Axis axis, double speedOfRate)
        {
            commandHandler.SendCommand(new Axis_Rate(axis, speedOfRate));
        }

        protected void FindZeroIndex(Axis axis)
        {
            commandHandler.SendCommand(new Zero_Index_Search(axis));
        }

        protected void SetAxisMode(ModeParam param,Axis axis)
        {
            commandHandler.SendCommand(new Mode(param, axis));
        }

        protected void StopAxis(Axis axis)
        {
            commandHandler.SendCommand(new Stop_axis(axis));
        }

        protected void SetAxisPosition(Axis axis,double degree)
        {
            commandHandler.SendCommand(new Axis_Position(axis, degree));
        }

        protected void StartAxis(Axis axis)
        {
            commandHandler.SendCommand(new Start_axis(axis));    
        }

        protected void SetTemperatureChangeSpeed(double slope)
        {
            commandHandler.SendCommand(new Temperature_slope_set_point(slope));
        }

        protected void SetTemperature(double temperature)
        {
            commandHandler.SendCommand(new Temperature_Set_point(temperature));
        }
    }
}
