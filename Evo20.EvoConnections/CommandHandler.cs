using System.Collections.Generic;
using System;
using System.Text;
using System.Threading;
using Evo20.Commands;

namespace Evo20.EvoConnections
{
    /// <summary>
    /// Класс обертка над ConnectionSocket для работы с коммандами
    /// </summary>
    public class CommandHandler : ConnectionSocket
    {
        #region Commands 

        static string AXIS_STATUS = Axis_Status.Command;
        static string TEMPERATURE_STATUS = Temperature_status.Command;
        static string REQUESTED_AXIS_POSITION_REACHED = Requested_axis_position_reached.Command;
        static string ACTUAL_TEMPERATURE_QUERY = Actual_temperature_query.Command;

        static string ROTARY_JOINT_TEMPERATURE_QUERY_X = new Rotary_joint_temperature_Query(Axis.First).ToString();
        static string ROTARY_JOINT_TEMPERATURE_QUERY_Y = new Rotary_joint_temperature_Query(Axis.Second).ToString();
        static string AXIS_POSITION_QUERY_X = new Axis_Position_Query(Axis.First).ToString();
        static string AXIS_POSITION_QUERY_Y = new Axis_Position_Query(Axis.Second).ToString();
        static string AXIS_RATE_QUERY_X = new Axis_Rate_Query(Axis.First).ToString();
        static string AXIS_RATE_QUERY_Y = new Axis_Rate_Query(Axis.Second).ToString();

        #endregion

        // очередь обработанных комманд
        Queue<Command> bufferCommand;

        //буффер входящих сообщений 
        StringBuilder bufferMessage;
        //буффер сообщений для отправки 
        StringBuilder sendBuffer;

        // делегат
        public delegate void NewCommandHandler();
        // событие, уведомляющее о приходе новой команды 
        public event NewCommandHandler CommandHandlersListForController;
  
        public CommandHandler()
        {
            buffer = new byte[2048];
            bufferCommand = new Queue<Command>();
            bufferMessage = new StringBuilder();
            sendBuffer = new StringBuilder();
            //подписываемся на уведомления ConnectionSocket
            EventHandlersListForCommand += NewMessageHandler;
            work_thread = new Thread((ReadMessage));
            ConnectionStatus = ConnectionStatus.DISCONNECTED;   

        }

        /// <summary>
        /// Обработчик события прихода нового сообщения подписан на ConnectionSocket.EventHandlersListForCommand
        /// извлекает из строки команду и добавляет ее в очередь команд
        /// </summary>
        public void NewMessageHandler()
        {
            bufferMessage.Append(ReadBuffer());
            if (bufferMessage.Length != 0)
            {              
                String temp = bufferMessage.ToString();
                Command serializedCommand = RecognizeCommand(temp);
                if (serializedCommand == null)
                {
                    return;
                }
                lock (bufferCommand)
                {
                    bufferCommand.Enqueue(serializedCommand);
                }   
                bufferMessage.Remove(0, bufferMessage.Length);
            }
            if (CommandHandlersListForController != null)
            {
                CommandHandlersListForController();
            }
        }

        /// <summary>
        /// Извлечение комманд из очереди ожидающих
        /// </summary>
        /// <returns></returns>
        public Command[] GetCommands()
        {
            if (bufferCommand.Count > 0)
            {
                Command[] array=null;
                lock (bufferCommand)
                {
                    array = bufferCommand.ToArray();
                    bufferCommand.Clear();
                }
                return array;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Отправка комманды 
        /// </summary>
        /// <param name="command">комманда Evo_20_commands</param>
        /// <returns>результат отправки </returns>
        public bool SendCommand(Command command)
        {
            Log.Instance.Info("Отправлена команда: {0}",command);
            string newMessage = command.ToString();
            return SendMessage(newMessage);
        }
        /// <summary>
        /// Извлечение комманды из строки 
        /// </summary>
        /// <param name="cmd"> строковое представление комманды </param>
        /// <returns>комманда Evo_20_commands</returns>
        public static Command RecognizeCommand(String cmd)
        {
            if (cmd==null || cmd.Length == 0)
                return null;
            string[] command_parts = cmd.Split('=');
            if(command_parts.Length!=2)
                return null;
            StringBuilder temp = new StringBuilder();
            int i = 0;
            while ( i< command_parts[1].Length && command_parts[1][i] != '\0')
            {
                if (command_parts[1][i] != '.')
                {
                    temp.Append(command_parts[1][i]);
                }
                else
                {
                    temp.Append(',');
                }

                i++;
            }
            //Пришла команда Axis_Status
            if (command_parts[0] == AXIS_STATUS)
            {
                Log.Instance.Info("Сообщение:статус осей ");            
                return new Axis_Status_answer(temp.ToString());
            }
            //Пришла команда Temperature_status
            if (command_parts[0] == TEMPERATURE_STATUS)
            {
                Log.Instance.Info("Сообщение:статус термокамеры принято ");
                return new Temperature_status_answer(temp.ToString());
            }

            //Пришла команда температура оси x
            if (command_parts[0] == ROTARY_JOINT_TEMPERATURE_QUERY_X)
            {
                Log.Instance.Info("Сообщение:температура оси x принято ");
                return new Rotary_joint_temperature_Query_answer(temp.ToString(),Axis.First);
            }
            //Пришла команда температура оси y
            if (command_parts[0] == ROTARY_JOINT_TEMPERATURE_QUERY_Y)
            {
                Log.Instance.Info("Сообщение:температура оси y принято");
                return new Rotary_joint_temperature_Query_answer(temp.ToString(), Axis.Second);
            }

            //Пришла команда положение оси x
            if (command_parts[0] == AXIS_POSITION_QUERY_X)
            {
                Log.Instance.Info("Сообщение:положение оси x принято");
                return new Axis_Position_Query_answer(temp.ToString(), Axis.First);
            }
            //Пришла команда положение оси y
            if (command_parts[0] == AXIS_POSITION_QUERY_Y)
            {
                Log.Instance.Info("Сообщение:положение оси y принято");
                return new Axis_Position_Query_answer(temp.ToString(), Axis.Second);
            }

            //Пришла команда скорость оси x
            if (command_parts[0] == AXIS_RATE_QUERY_X)
            {
                Log.Instance.Info("Сообщение:скорость оси x принято");
                return new Axis_Rate_Query_answer(temp.ToString(), Axis.First);
            }
            //Пришла команда скорость оси y
            if (command_parts[0] == AXIS_RATE_QUERY_Y)
            {
                Log.Instance.Info("Сообщение:скорость оси y принято");
                return new Axis_Rate_Query_answer(temp.ToString(), Axis.Second);
            }
            //Пришла команда о достигнутом положении осей
            if (command_parts[0] == REQUESTED_AXIS_POSITION_REACHED)
            {
                Log.Instance.Info("Сообщение:достигнутые положение осей");
                return new Requested_axis_position_reached_answer(temp.ToString());
            }
            if (command_parts[0] == ACTUAL_TEMPERATURE_QUERY)
            {
                Log.Instance.Info("Сообщение:достигнутые положение осей");
                return new Actual_temperature_query_answer(temp.ToString());
            }
            return null;
        }
    }
}
