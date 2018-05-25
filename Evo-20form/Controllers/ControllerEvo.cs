using System.Threading;
using Evo_20_commands;
using Evo_20form.Data;
using Evo_20form.EvoConnections;
using Evo_20form.Utils;

namespace Evo_20form.Controllers
{
    /// <summary>
    /// Класс  с evo, обрабатывающий команды и следящий за состоянием evo.
    /// </summary>
    abstract class ControllerEvo
    {
        public delegate void ConnectionChangeHandler(ConnectionStatus state);

        public delegate void WorkModeChangeHandler(WorkMode mode);

        public ConnectionChangeHandler EventListForEvoConnectionChange;

        public WorkModeChangeHandler EventListForWorkModeChange;

        public const int THREADS_SLEEP_TIME = 1000;

        //обработчик новых команд
        protected CommandHandler commandHandler;

        //класс, хронящий состояние evo 
        public EvoData evoData;

        // поток, который проверяет состояние системы отправляя команды опроса
        protected Thread routineThread;

        private WorkMode mode;

        public WorkMode Mode
        {
            get
            {
                return mode;
            }
            protected set
            {
                mode = value;
                if(EventListForWorkModeChange!=null)
                    EventListForWorkModeChange(mode);
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
            evoData = new EvoData();
        }
        //команды опроса системы
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


        public void ControllerRoutine()
        {
            Command[] commands = GetRoutineCommands();
            while (commandHandler.ConnectionStatus == ConnectionStatus.CONNECTED)
            {
                foreach (var item in commands)
                {
                    lock (commandHandler)
                    {
                        commandHandler.SendCommand(item);
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

        public bool StartEvoConnection()
        {
            bool result = commandHandler.StartConnection();
            if (!result)
            {
                return result;
            }
            if (!routineThread.IsAlive)
            {
                routineThread = new Thread(ControllerRoutine);
                routineThread.Priority = ThreadPriority.BelowNormal;
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
                        Mode = WorkMode.Stop;
                        break;
                    }
                case ConnectionStatus.ERROR:
                    {
                        if (routineThread.IsAlive)
                        {
                            routineThread.Abort();
                            routineThread.Join();
                        }
                        Mode = WorkMode.Error;
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
        protected void PowerOnCamera(bool value)
        {
            commandHandler.SendCommand(new PowerOnTemperatureCamera(value));
        }

        /// <summary>
        /// Запуск питания осей
        /// </summary>
        /// <param name="axis">ось</param>
        /// <param name="value"></param>
        protected void PowerOnAxis(Axis axis, bool value)
        {
            commandHandler.SendCommand(new Axis_Power(axis, value));
        }

        /// <summary>
        /// Задать скорость вращения оси
        /// </summary>
        /// <param name="axis">ось</param>
        /// <param name="speedOfRate">скорость</param>
        protected void SetAxisRate(Axis axis, double speedOfRate)
        {
            commandHandler.SendCommand(new Axis_Rate(axis, speedOfRate));
        }

        /// <summary>
        /// Поиск нуля
        /// </summary>
        /// <param name="axis">ось</param>
        protected void FindZeroIndex(Axis axis)
        {
            commandHandler.SendCommand(new Zero_Index_Search(axis));
        }

        /// <summary>
        /// Задать режим оси
        /// </summary>
        /// <param name="param">режим</param>
        /// <param name="axis">ось</param>
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

        #endregion
    }
}
