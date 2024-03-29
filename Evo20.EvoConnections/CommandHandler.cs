﻿using System.Collections.Generic;
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

        private void NewMessageHandler(object sender, EventArgs e)
        {
            _bufferMessage.Append(ReadBuffer());
            if (_bufferMessage.Length != 0)
            {
                string temp = _bufferMessage.ToString();
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
            NewCommandArrived?.Invoke(this, null);
        }

        public Command[] Commands
        {
            get
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
        }

        public bool SendCommand(Command command)
        {
            if (Config.Instance.IsFakeEvo)
                return true;
            if (command is CommandWithAnswer)
                Log.Instance.Debug("Отправлена команда:{0}", command);
            else
                Log.Instance.Info("Отправлена команда: {0}", command);
            string newMessage = command.ToString();
            return SendMessage(newMessage);
        }


        public static Command RecognizeCommand(string cmd)
        {
            if (string.IsNullOrEmpty(cmd))
            {
                Log.Instance.Warning("Полученна пустая команда");
                return null;
            }
            string[] commandParts = cmd.Split('=');
            if (commandParts.Length != 2)
            {
                Log.Instance.Warning("Полученна команда незвестного формата {0}", cmd);
                return null;
            }
            var foramtedCommand = new StringBuilder();
            int i = 0;
            while (i < commandParts[1].Length && commandParts[1][i] != '\0')
            {
                foramtedCommand.Append(commandParts[1][i] != '.' ? commandParts[1][i] : ',');
                i++;
            }

            if (commandParts[0] == AxisStatus.Command)
            {
                Log.Instance.Info("Полученна:статус осей {0}", cmd);
                return new AxisStatusAnswer(foramtedCommand.ToString());
            }

            if (commandParts[0] == TemperatureStatus.Command)
            {
                Log.Instance.Info("Полученна:статус термокамеры {0}", cmd);
                return new TemperatureStatusAnswer(foramtedCommand.ToString());
            }

            //Пришла команда температура оси x
            if (commandParts[0] == RotaryJointTemperatureQueryX)
            {
                Log.Instance.Info("Полученна:температура оси x {0}", cmd);
                return new RotaryJointTemperatureQueryAnswer(foramtedCommand.ToString(), Axis.First);
            }
            //Пришла команда температура оси y
            if (commandParts[0] == RotaryJointTemperatureQueryY)
            {
                Log.Instance.Info("Полученна:температура оси y {0}", cmd);
                return new RotaryJointTemperatureQueryAnswer(foramtedCommand.ToString(), Axis.Second);
            }

            //Пришла команда положение оси x
            if (commandParts[0] == AxisPositionQueryX)
            {
                Log.Instance.Info("Полученна:положение оси x принято {0}", cmd);
                return new AxisPositionQueryAnswer(foramtedCommand.ToString(), Axis.First);
            }
            //Пришла команда положение оси y
            if (commandParts[0] == AxisPositionQueryY)
            {
                Log.Instance.Info("Полученна:положение оси y принято {0}", cmd);
                return new AxisPositionQueryAnswer(foramtedCommand.ToString(), Axis.Second);
            }

            //Пришла команда скорость оси x
            if (commandParts[0] == AxisRateQueryX)
            {
                Log.Instance.Info("Полученна:скорость оси x принято {0}", cmd);
                return new AxisRateQueryAnswer(foramtedCommand.ToString(), Axis.First);
            }
            //Пришла команда скорость оси y
            if (commandParts[0] == AxisRateQueryY)
            {
                Log.Instance.Info("Полученна:скорость оси y принято {0}", cmd);
                return new AxisRateQueryAnswer(foramtedCommand.ToString(), Axis.Second);
            }
            //Пришла команда о достигнутом положении осей
            if (commandParts[0] == RequestedAxisPositionReached.Command)
            {
                Log.Instance.Info("Полученна:достигнутые положение осей {0}", cmd);
                return new RequestedAxisPositionReachedAnswer(foramtedCommand.ToString());
            }
            if (commandParts[0] == ActualTemperatureQuery.Command)
            {
                Log.Instance.Info("Полученна:текущая температура осей {0}", cmd);
                return new ActualTemperatureQueryAnswer(foramtedCommand.ToString());
            }
            Log.Instance.Warning("Неизвестная команда {0}", cmd);

            return null;
        }
    }
}
