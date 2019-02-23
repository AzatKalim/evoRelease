using System.Collections.Generic;
using System;
using System.Text;
using System.Threading;
using Evo20.Commands.Abstract;
using Evo20.Commands.AnswerCommands;
using Evo20.Commands.CommndsWithAnswer;
using Evo20.Utils;

namespace Evo20.EvoConnections
{
    public class CommandHandler : ConnectionSocket
    {
        #region Commands 

        static readonly string AxisStatus = Commands.CommndsWithAnswer.AxisStatus.Command;
        static readonly string TemperatureStatus = Commands.CommndsWithAnswer.TemperatureStatus.Command;
        static readonly string RequestedAxisPositionReached = Commands.CommndsWithAnswer.RequestedAxisPositionReached.Command;
        static readonly string ActualTemperatureQuery = Commands.CommndsWithAnswer.ActualTemperatureQuery.Command;

        static readonly string RotaryJointTemperatureQueryX = new RotaryJointTemperatureQuery(Axis.First).ToString();
        static readonly string RotaryJointTemperatureQueryY = new RotaryJointTemperatureQuery(Axis.Second).ToString();
        static readonly string AxisPositionQueryX = new AxisPositionQuery(Axis.First).ToString();
        static readonly string AxisPositionQueryY = new AxisPositionQuery(Axis.Second).ToString();
        static readonly string AxisRateQueryX = new AxisRateQuery(Axis.First).ToString();
        static readonly string AxisRateQueryY = new AxisRateQuery(Axis.Second).ToString();

        #endregion

        readonly Queue<Command> _bufferCommand;

        readonly StringBuilder _bufferMessage;

        public delegate void NewCommandHandler(object sender, EventArgs e);

        public event NewCommandHandler NewCommandArrived;
  
        public CommandHandler()
        {
            Buffer = new byte[2048];
            _bufferCommand = new Queue<Command>();
            _bufferMessage = new StringBuilder();
            NewMessageArrived += NewMessageHandler;
            WorkThread = new Thread((ReadMessage));
            ConnectionStatus = ConnectionStatus.Disconnected;   

        }

        public void NewMessageHandler(object sender, EventArgs e)
        {
            _bufferMessage.Append(ReadBuffer());
            if (_bufferMessage.Length != 0)
            {              
                String temp = _bufferMessage.ToString();
                Command serializedCommand = RecognizeCommand(temp);
                if (serializedCommand == null)
                {
                    return;
                }
                lock (_bufferCommand)
                {
                    _bufferCommand.Enqueue(serializedCommand);
                }   
                _bufferMessage.Remove(0, _bufferMessage.Length);
            }
            NewCommandArrived?.Invoke(this,null);
        }

        public Command[] GetCommands()
        {
            lock (_bufferCommand)
            {
                if (_bufferCommand.Count > 0)
                {
                    Command[] array;
                    lock (_bufferCommand)
                    {
                        array = _bufferCommand.ToArray();
                        _bufferCommand.Clear();
                    }
                    return array;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool SendCommand(Command command)
        {
            if (Config.IsFakeEvo)
                return true;
            Log.Instance.Info("Отправлена команда: {0}",command);
            string newMessage = command.ToString();
            return SendMessage(newMessage);
        }


        public static Command RecognizeCommand(string cmd)
        {
            if (string.IsNullOrEmpty(cmd))
            { 
                Log.Instance.Warning("Пустая команда");
                return null;
            }
            string[] commandParts = cmd.Split('=');
            if (commandParts.Length != 2)
            {
                Log.Instance.Warning("Команда незвестного формата {0}", cmd);
                return null;
            }
            StringBuilder temp = new StringBuilder();
            int i = 0;
            while (i< commandParts[1].Length && commandParts[1][i] != '\0')
            {
                temp.Append(commandParts[1][i] != '.' ? commandParts[1][i] : ',');
                i++;
            }

            if (commandParts[0] == AxisStatus)
            {
                Log.Instance.Info("Сообщение:статус осей ");            
                return new AxisStatusAnswer(temp.ToString());
            }

            if (commandParts[0] == TemperatureStatus)
            {
                Log.Instance.Info("Сообщение:статус термокамеры принято ");
                return new TemperatureStatusAnswer(temp.ToString());
            }

            //Пришла команда температура оси x
            if (commandParts[0] == RotaryJointTemperatureQueryX)
            {
                Log.Instance.Info("Сообщение:температура оси x принято ");
                return new RotaryJointTemperatureQueryAnswer(temp.ToString(),Axis.First);
            }
            //Пришла команда температура оси y
            if (commandParts[0] == RotaryJointTemperatureQueryY)
            {
                Log.Instance.Info("Сообщение:температура оси y принято");
                return new RotaryJointTemperatureQueryAnswer(temp.ToString(), Axis.Second);
            }

            //Пришла команда положение оси x
            if (commandParts[0] == AxisPositionQueryX)
            {
                Log.Instance.Info("Сообщение:положение оси x принято");
                return new AxisPositionQueryAnswer(temp.ToString(), Axis.First);
            }
            //Пришла команда положение оси y
            if (commandParts[0] == AxisPositionQueryY)
            {
                Log.Instance.Info("Сообщение:положение оси y принято");
                return new AxisPositionQueryAnswer(temp.ToString(), Axis.Second);
            }

            //Пришла команда скорость оси x
            if (commandParts[0] == AxisRateQueryX)
            {
                Log.Instance.Info("Сообщение:скорость оси x принято");
                return new AxisRateQueryAnswer(temp.ToString(), Axis.First);
            }
            //Пришла команда скорость оси y
            if (commandParts[0] == AxisRateQueryY)
            {
                Log.Instance.Info("Сообщение:скорость оси y принято");
                return new AxisRateQueryAnswer(temp.ToString(), Axis.Second);
            }
            //Пришла команда о достигнутом положении осей
            if (commandParts[0] == RequestedAxisPositionReached)
            {
                Log.Instance.Info("Сообщение:достигнутые положение осей");
                return new RequestedAxisPositionReachedAnswer(temp.ToString());
            }
            if (commandParts[0] == ActualTemperatureQuery)
            {
                Log.Instance.Info("Сообщение:достигнутые положение осей");
                return new ActualTemperatureQueryAnswer(temp.ToString());
            }
            Log.Instance.Warning("Неизвестная команда {0}", cmd);

            return null;
        }
    }
}
