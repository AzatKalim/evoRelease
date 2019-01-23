using System.Threading;

using Evo20.Commands;
using Evo20.Controllers;
using Evo20.EvoConnections;
using Evo20;
using Evo20.Sensors;

namespace Evo20.Controllers
{
    /// <summary>
    /// Класс  с evo, обрабатывающий команды и следящий за состоянием evo.
    /// </summary>
    public class ControllerEvo
    {
        public delegate void EvoConnectionChangeHandler(ConnectionStatus state);

        public EvoConnectionChangeHandler EventListForEvoConnectionChange;

        public const int THREADS_SLEEP_TIME = 100;

        //обработчик новых команд
        protected CommandHandler commandHandler;

        // поток, который проверяет состояние системы отправляя команды опроса
        protected Thread routineThread;
    
        private static ControllerEvo controllerEvo;

        public static ControllerEvo Instance
        {
            get
            {
                if (controllerEvo == null)
                    controllerEvo = new ControllerEvo();
                return controllerEvo;
            }
        }

        public double CurrentTemperature
        {
            get
            {
                return EvoData.Instance.CurrentTemperature;
            }
        }

        public ControllerEvo()
        {
            commandHandler = new CommandHandler();
            commandHandler.CommandHandlersListForController += NewCommandHandler;
            commandHandler.EventHandlerListForStateChanged += ConnectionStateChangedHandler;
            routineThread = new Thread(ControllerRoutine);
            routineThread.Priority = ThreadPriority.BelowNormal;
            routineThread.IsBackground = true;
        }

        //команды опроса системы
        protected Command[] RoutineCommands
        {
            get
            {
                return new Command[] 
                    {
                        new Axis_Status(),
                        new Temperature_status(),
                        new Rotary_joint_temperature_Query(Axis.First),
                        new Rotary_joint_temperature_Query(Axis.Second),
                        new Actual_temperature_query(),
                        new Axis_Position_Query(Axis.First),
                        new Axis_Position_Query(Axis.Second),
                        new Axis_Rate_Query(Axis.First),
                        new Axis_Rate_Query(Axis.Second),
                        new Requested_axis_position_reached()
                    };
            }
        }

        public void ControllerRoutine()
        {
            var commands = RoutineCommands;
            while (commandHandler.ConnectionStatus == ConnectionStatus.CONNECTED)
            {
                foreach (var item in commands)
                {
                    lock (commandHandler)
                    {
                        if (!commandHandler.SendCommand(item))
                        {
                            Log.Instance.Error("Не удалось отправить сообщение evo" + item.ToString());
                            EventListForEvoConnectionChange(commandHandler.ConnectionStatus);
                            return;
                        }

                        Thread.Sleep(100);
                    }
                }
                Thread.Sleep(THREADS_SLEEP_TIME);
            }
        }

        //обработчик новых команд
        protected void NewCommandHandler()
        {
            Command[] commands;
            lock (commandHandler)
            {
                //извлекаем получные команды
                commands = commandHandler.GetCommands();
            }
            if (commands == null)
                return;
            //передача evoData команд-ответов
            for (int i = 0; i < commands.Length; i++)
            {
                if (commands[i] is Axis_Status_answer)
                    EvoData.Instance.GetCommandInfo(commands[i] as Axis_Status_answer);
                if (commands[i] is Temperature_status_answer)
                    EvoData.Instance.GetCommandInfo(commands[i] as Temperature_status_answer);
                if (commands[i] is Rotary_joint_temperature_Query_answer)
                    EvoData.Instance.GetCommandInfo(commands[i] as Rotary_joint_temperature_Query_answer);
                if (commands[i] is Axis_Position_Query_answer)
                    EvoData.Instance.GetCommandInfo(commands[i] as Axis_Position_Query_answer);
                if (commands[i] is Axis_Rate_Query_answer)
                    EvoData.Instance.GetCommandInfo(commands[i] as Axis_Rate_Query_answer);
                if (commands[i] is Actual_temperature_query_answer)
                    EvoData.Instance.GetCommandInfo(commands[i] as Actual_temperature_query_answer);
                if (commands[i] is Requested_axis_position_reached_answer)
                    EvoData.Instance.GetCommandInfo(commands[i] as Requested_axis_position_reached_answer);
            }
        }

        public bool StartEvoConnection()
        {
            bool result = commandHandler.StartConnection();
            if (!result)
                return result;
            if (!routineThread.IsAlive)
            {
                routineThread = new Thread(ControllerRoutine);
                routineThread.Priority = ThreadPriority.BelowNormal;
                routineThread.Start();
            }
            return true;
        }

        public void PauseEvoConnection()
        {
            commandHandler.PauseConnection();
            routineThread.Abort();
        }

        public void StopEvoConnection()
        {
            commandHandler.StopConnection();
            if (routineThread != null && routineThread.IsAlive)
                routineThread.Abort();
        }


        /// <summary>
        /// Обработка изменения состояния соединения
        /// </summary>
        /// <param name="state">новое состояние соединения</param>
        public void ConnectionStateChangedHandler(ConnectionStatus state)
        {
            switch (state)
            {
                case ConnectionStatus.DISCONNECTED:
                    {
                        if (routineThread.IsAlive)
                        {
                            routineThread.Abort();
                            routineThread.Join();
                        }
                        Controller.Instance.Mode = WorkMode.Stop;
                        break;
                    }
                case ConnectionStatus.ERROR:
                    {
                        if (routineThread.IsAlive)
                        {
                            routineThread.Abort();
                            routineThread.Join();
                        }
                        Controller.Instance.Mode = WorkMode.Error;
                        break;
                    }
            }
            EventListForEvoConnectionChange(state);
        }

        #region Camera commands

        /// <summary>
        /// Запуск температурной камеры 
        /// </summary>
        /// <param name="value">true-запуск,false-отключить</param>
        public void PowerOnCamera(bool value)
        {
            commandHandler.SendCommand(new PowerOnTemperatureCamera(value));
        }

        /// <summary>
        /// Запуск питания осей
        /// </summary>
        /// <param name="axis">ось</param>
        /// <param name="value"></param>
        public void PowerOnAxis(Axis axis, bool value)
        {
            commandHandler.SendCommand(new Axis_Power(axis, value));
        }

        /// <summary>
        /// Задать скорость вращения оси
        /// </summary>
        /// <param name="axis">ось</param>
        /// <param name="speedOfRate">скорость</param>
        public void SetAxisRate(Axis axis, double speedOfRate)
        {
            commandHandler.SendCommand(new Axis_Rate(axis, speedOfRate));
        }

        /// <summary>
        /// Поиск нуля
        /// </summary>
        /// <param name="axis">ось</param>
        public void FindZeroIndex(Axis axis)
        {
            commandHandler.SendCommand(new Zero_Index_Search(axis));
        }

        /// <summary>
        /// Задать режим оси
        /// </summary>
        /// <param name="param">режим</param>
        /// <param name="axis">ось</param>
        public void SetAxisMode(ModeParam param, Axis axis)
        {
            commandHandler.SendCommand(new Mode(param, axis));
        }

        public void StopAxis(Axis axis)
        {
            commandHandler.SendCommand(new Stop_axis(axis));
        }

        public void SetAxisPosition(Axis axis, double degree)
        {
            StopAxis(Axis.ALL);
            if (axis == Axis.First)
            {
                degree+=EvoData.Instance.X.correction;
            }
            else if (axis == Axis.Second)
            {
                degree += EvoData.Instance.Y.correction;
            }

            commandHandler.SendCommand(new Axis_Position(axis, degree));
        }

        public void StartAxis(Axis axis)
        {
            commandHandler.SendCommand(new Start_axis(axis));    
        }

        public void SetTemperatureChangeSpeed(double slope)
        {
            commandHandler.SendCommand(new Temperature_slope_set_point(slope));
        }

        public void SetTemperature(double temperature)
        {
            commandHandler.SendCommand(new Temperature_Set_point(temperature));
        }

        public void SetPosition(ProfilePart position)
        {
            Log.Instance.Info(string.Format("Задание положения осей: X {0}:{1}. Y {2}:{3}", position.FirstPosition, position.SpeedFirst,
                position.SecondPosition, position.SpeedSecond));
            //задание положений и скоростей
            if (position.SpeedFirst != 0)
            {
                SetAxisRate(Axis.First, position.SpeedFirst);
                SetAxisMode(ModeParam.Speed, Axis.First);
            }
            else
            {
                SetAxisPosition(Axis.First, position.FirstPosition);
                SetAxisMode(ModeParam.Position, Axis.First);
            }
            if (position.SpeedSecond != 0)
            {
                SetAxisRate(Axis.Second, position.SpeedSecond);
                SetAxisMode(ModeParam.Speed, Axis.Second);
            }
            else
            {
                SetAxisPosition(Axis.Second, position.SecondPosition);
                SetAxisMode(ModeParam.Position, Axis.Second);
            }
            StartAxis(Axis.ALL);
        }

        public bool InitEvo()
        {
            PowerOnCamera(true);
            PowerOnAxis(Axis.ALL, true);
            FindZeroIndex(Axis.ALL);
            return true;
        }

        public bool SetStartPosition()
        {
            StopAxis(Axis.ALL);
            SetAxisRate(Axis.ALL, Config.BASE_MOVE_SPEED);
            SetAxisPosition(Axis.ALL, 0);
            StartAxis(Axis.ALL);
            SetTemperatureChangeSpeed(Config.SPEED_OF_TEMPERATURE_CHANGE);
            return true;
        }
        #endregion
    }
}
