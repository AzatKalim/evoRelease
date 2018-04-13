using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Evo_20_commands;


namespace Evo_20form
{
    class CommandHandler : ConnectionSocket
    {
        public int countCommand;

        Queue<Command> bufferCommand;

        StringBuilder bufferMessage;

        StringBuilder sendBuffer;

        //public delegate void CommandHandler(Command command);

        //public event CommandHandler CommandHandlersList;

        public CommandHandler()
        {
            connectionState = ConnectionState.CONNECTED;
            buffer = new byte[2048];
            bufferCommand = new Queue<Command>();
            bufferMessage = new StringBuilder();
            sendBuffer = new StringBuilder();
            EventHandlersListForCommand += NewMessageHandler;
            work_thread = new Thread(new ThreadStart(ReadMessage));
            connectionState = ConnectionState.DISCONNECTED;
            //work_thread.Start();           

        }

        public void NewMessageHandler()
        {
            bufferMessage.Append(ReadBuffer());
            if (bufferMessage.Length != 0)
            {              
                String temp = bufferMessage.ToString();
                Command serializedCommand = RecognizeCommand(temp);
                lock (bufferCommand)
                {
                    bufferCommand.Enqueue(serializedCommand);
                }   
                countCommand++;
                bufferMessage.Remove(0, bufferMessage.Length);
            }
            /*if (//ClientCommandHandlersListForServer != null)
            {
                ClientCommandHandlersListForServer(this);
            }*/
        }

        public Command[] GetCommands()
        {
            if (bufferCommand.Count > 0)
            {
                Command[] temp = new Command[bufferCommand.Count];
                for (int i = 0; i < bufferCommand.Count; i++)
                {
                    temp[i] = bufferCommand.Dequeue();
                    Console.WriteLine("Get({0}): {1} on {2}", temp[i], DateTime.Now.ToShortTimeString());

                }
                return temp;
            }
            else
            {
                return null;
            }
        }

        public bool SendCommand(Command command)
        {
            Log.WriteLog("Отправлена команда " + command+ " в "+ DateTime.Now.ToShortTimeString());
            string newMessage = command.ToString();
            return SendMessage(newMessage);
        }

        public Command RecognizeCommand(String cmd)
        {
            if (cmd.Length == 0)
            {
                return null;
            }
            string[] command_parts = cmd.Split('=');
            if (command_parts[0] == (new Axis_Status()).ToString())
            {
                Log.WriteLog("Сообщение о состоянии принято");
                return new Axis_Status_Answer(command_parts[1]);
            }
            if (command_parts[0] == (new Temperature_status()).ToString())
            {
                Log.WriteLog("Сообщение о температуре принято");
                return new Temperature_status_answer(command_parts[1]); ;
            }
            return null;
        }
    }
}
