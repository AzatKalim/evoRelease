using System.Collections.Generic;
using System;
using System.Text;
using System.Threading;
using Evo_20_commands;


namespace Evo_20form
{
    /// <summary>
    /// Класс обертка над ConnectionSocket для работы с коммандами
    /// </summary>
    class CommandHandler : ConnectionSocket
    {
        
        public int countCommand;

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
            connectionState = ConnectionState.DISCONNECTED;   

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
                countCommand++;
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
            Log.WriteLog("Отправлена команда " + command+ " в "+ DateTime.Now.ToShortTimeString());
            string newMessage = command.ToString();
            return SendMessage(newMessage);
        }

        /// <summary>
        /// Извлечение комманды из строки 
        /// </summary>
        /// <param name="cmd"> строковое представление комманды </param>
        /// <returns>комманда Evo_20_commands</returns>
        Command RecognizeCommand(String cmd)
        {
            if (cmd.Length == 0)
            {
                return null;
            }
            string[] command_parts = cmd.Split('=');
            StringBuilder temp = new StringBuilder();
            int i = 0;
            while (command_parts[1][i] != '\0')
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
            if (command_parts[0]+ ' ' == (new Axis_Status()).ToString())
            {
                Log.WriteLog("Сообщение:статус осей "+ DateTime.Now.ToString());            
                return new Axis_Status_answer (temp.ToString());
            }
            //Пришла команда Temperature_status
            if (command_parts[0] + ' ' == (new Temperature_status()).ToString())
            {
                Log.WriteLog("Сообщение:статус термокамеры принято "+ DateTime.Now.ToString());
                return new Temperature_status_answer(temp.ToString());
            }

            //Пришла команда температура оси x
            if (command_parts[0] == new Rotary_joint_temperature_Query(Axis.X).ToString())
            {
                Log.WriteLog("Сообщение:температура оси x принято " + DateTime.Now.ToString());
                return new Rotary_joint_temperature_Query_answer(temp.ToString(),Axis.X);
            }
            //Пришла команда температура оси y
            if (command_parts[0] == new Rotary_joint_temperature_Query(Axis.Y).ToString())
            {
                Log.WriteLog("Сообщение:температура оси y принято  " + DateTime.Now.ToString());
                return new Rotary_joint_temperature_Query_answer(temp.ToString(), Axis.Y);
            }

            //Пришла команда положение оси x
            if (command_parts[0] == new Axis_Position_Query(Axis.X).ToString())
            {
                Log.WriteLog("Сообщение:положение оси x принято  " + DateTime.Now.ToString());
                return new Axis_Position_Query_answer(temp.ToString(), Axis.X);
            }
            //Пришла команда положение оси y
            if (command_parts[0] == new Axis_Position_Query(Axis.Y).ToString())
            {
                Log.WriteLog("Сообщение:положение оси y принято  " + DateTime.Now.ToString());
                return new Axis_Position_Query_answer(temp.ToString(), Axis.Y);
            }

            //Пришла команда скорость оси x
            if (command_parts[0] == new Axis_Rate_Query(Axis.X).ToString())
            {
                Log.WriteLog("Сообщение:скорость оси x принято  " + DateTime.Now.ToString());
                return new Axis_Rate_Query_answer(temp.ToString(), Axis.X);
            }
            //Пришла команда скорость оси y
            if (command_parts[0] == new Axis_Rate_Query(Axis.Y).ToString())
            {
                Log.WriteLog("Сообщение:скорость оси y принято   " + DateTime.Now.ToString());
                return new Axis_Rate_Query_answer(temp.ToString(), Axis.Y);
            }
            //Пришла команда о достигнутом положении осей
            if (command_parts[0] == new Requested_axis_position_reached ().ToString())
            {
                Log.WriteLog("Сообщение:достигнутые положение осей   " + DateTime.Now.ToString());
                return new Requested_axis_position_reached_answer(temp.ToString());
            }
            if (command_parts[0] == (new Actual_temperature_query()).ToString())
            {
                Log.WriteLog("Сообщение:достигнутые положение осей   " + DateTime.Now.ToString());
                return new Actual_temperature_query_answer(temp.ToString());
            }
            return null;
        }
    }
}
