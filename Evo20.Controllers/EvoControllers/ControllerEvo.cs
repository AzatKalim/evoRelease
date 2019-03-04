using System;
using System.Threading;
using Evo20.Commands.Abstract;
using Evo20.Commands.AnswerCommands;
using Evo20.Commands.CommndsWithAnswer;
using Evo20.Commands.ControlCommands;
using Evo20.Controllers.Data;
using Evo20.EvoConnections;
using Evo20.Sensors;
using Evo20.Utils;
using Evo20.Utils.EventArguments;

namespace Evo20.Controllers.EvoControllers
{
    public class ControllerEvo : IDisposable
    {
        public delegate void EvoConnectionChangedHandler(object sender, EventArgs e);

        public event EvoConnectionChangedHandler EvoConnectionChanged;

        private const int ThreadsSleepTime = 100;

        protected Thread RoutineThread;
    
        private static ControllerEvo _controllerEvo;

        public static ControllerEvo Instance => _controllerEvo ?? (_controllerEvo = new ControllerEvo());

        public double CurrentTemperature => EvoData.Instance.CurrentTemperature;

        public ControllerEvo()
        {
            CommandHandler = new CommandHandler();
            CommandHandler.NewCommandArrived += NewCommandHandler;
            CommandHandler.StateChanged += ConnectionStateChangedHandler;
            RoutineThread = new Thread(ControllerRoutine) {Priority = ThreadPriority.BelowNormal, IsBackground = true};
        }

        protected Command[] RoutineCommands => new Command[] 
        {
            new AxisStatus(),
            new TemperatureStatus(),
            new RotaryJointTemperatureQuery(Axis.First),
            new RotaryJointTemperatureQuery(Axis.Second),
            new ActualTemperatureQuery(),
            new AxisPositionQuery(Axis.First),
            new AxisPositionQuery(Axis.Second),
            new AxisRateQuery(Axis.First),
            new AxisRateQuery(Axis.Second),
            new RequestedAxisPositionReached()
        };

        protected CommandHandler CommandHandler { get; set; }

        public void ControllerRoutine()
        {
            try
            {
                var commands = RoutineCommands;
                while (CommandHandler.ConnectionStatus == ConnectionStatus.Connected)
                {
                    foreach (var item in commands)
                    {
                        lock (CommandHandler)
                        {
                            if (!CommandHandler.SendCommand(item))
                            {
                                Log.Instance.Error($"Не удалось отправить сообщение evo{item}");
                                EvoConnectionChanged?.Invoke(this,
                                    new ConnectionStatusEventArgs(CommandHandler.ConnectionStatus));
                                return;
                            }

                            Thread.Sleep(100);
                        }
                    }

                    Thread.Sleep(ThreadsSleepTime);
                }
            }
            catch (ThreadAbortException)
            {
                Log.Instance.Warning("Опрашивающий поток был прерван");
            }
            catch (Exception exception)
            {
                Log.Instance.Warning("Ошибка рутины");
                Log.Instance.Exception(exception);
            }
        }

        protected void NewCommandHandler(object sender, EventArgs e)
        {
            Command[] commands;
            lock (CommandHandler)
            {
                commands = CommandHandler.GetCommands();
            }
            if (commands == null)
                return;
            foreach (var command in commands)
            {
                var answer = command as AxisStatusAnswer;
                if (answer != null)
                    EvoData.Instance.GetCommandInfo(answer);
                var statusAnswer = command as TemperatureStatusAnswer;
                if (statusAnswer != null)
                    EvoData.Instance.GetCommandInfo(statusAnswer);
                var queryAnswer = command as RotaryJointTemperatureQueryAnswer;
                if (queryAnswer != null)
                    EvoData.Instance.GetCommandInfo(queryAnswer);
                var positionQueryAnswer = command as AxisPositionQueryAnswer;
                if (positionQueryAnswer != null)
                    EvoData.Instance.GetCommandInfo(positionQueryAnswer);
                var rateQueryAnswer = command as AxisRateQueryAnswer;
                if (rateQueryAnswer != null)
                    EvoData.Instance.GetCommandInfo(rateQueryAnswer);
                var temperatureQueryAnswer = command as ActualTemperatureQueryAnswer;
                if (temperatureQueryAnswer != null)
                    EvoData.Instance.GetCommandInfo(temperatureQueryAnswer);
                var reachedAnswer = command as RequestedAxisPositionReachedAnswer;
                if (reachedAnswer != null)
                    EvoData.Instance.GetCommandInfo(reachedAnswer);
            }
        }

        public bool StartEvoConnection()
        {
            bool result = CommandHandler.StartConnection();
            if (!result)
                return false;
            if (!RoutineThread.IsAlive)
            {
                RoutineThread = new Thread(ControllerRoutine) {Priority = ThreadPriority.BelowNormal};
                RoutineThread.Start();
            }
            return true;
        }

        public void PauseEvoConnection()
        {
            CommandHandler.PauseConnection();
			if(RoutineThread!=null && RoutineThread.IsAlive && RoutineThread.ThreadState!=ThreadState.Aborted
                    && RoutineThread.ThreadState != ThreadState.AbortRequested)
				RoutineThread.Abort();
        }

        public void StopEvoConnection()
        {
            CommandHandler.StopConnection();
            if (RoutineThread != null && RoutineThread.IsAlive)
                RoutineThread.Abort();
        }

        public void ConnectionStateChangedHandler(object sender, EventArgs e)
        {
            var args = e as ConnectionStatusEventArgs;
            if (args == null)
                return;
            switch (args.State)
            {
                case ConnectionStatus.Disconnected:
                    {
                        if (RoutineThread.IsAlive)
                        {
                            RoutineThread.Abort();
                            RoutineThread.Join();
                        }
                        Controller.Instance.Mode = WorkMode.Stop;
                        break;
                    }
                case ConnectionStatus.Error:
                    {
                        if (RoutineThread.IsAlive)
                        {
                            RoutineThread.Abort();
                            RoutineThread.Join();
                        }
                        Controller.Instance.Mode = WorkMode.Error;
                        break;
                    }
            }

            EvoConnectionChanged?.Invoke(this, e);
        }

        #region Camera commands

        public void PowerOnCamera(bool value)
        {
            CommandHandler.SendCommand(new PowerOnTemperatureCamera(value));
        }

        public void PowerOnAxis(Axis axis, bool value)
        {
            CommandHandler.SendCommand(new AxisPower(axis, value));
        }

        public void SetAxisRate(Axis axis, double speedOfRate)
        {
            CommandHandler.SendCommand(new AxisRate(axis, speedOfRate));
        }

        public void FindZeroIndex(Axis axis)
        {
            CommandHandler.SendCommand(new ZeroIndexSearch(axis));
        }

        public void SetAxisMode(ModeParam param, Axis axis)
        {
            CommandHandler.SendCommand(new Mode(param, axis));
        }

        public void StopAxis(Axis axis)
        {
            CommandHandler.SendCommand(new StopAxis(axis));
        }

        public void SetAxisPosition(Axis axis, double degree)
        {
            StopAxis(Axis.All);
            if (axis == Axis.First)
            {
                degree+=EvoData.Instance.X.Correction;
            }
            else if (axis == Axis.Second)
            {
                degree += EvoData.Instance.Y.Correction;
            }

            CommandHandler.SendCommand(new AxisPosition(axis, degree));
        }

        public void StartAxis(Axis axis)
        {
            CommandHandler.SendCommand(new StartAxis(axis));    
        }

        public void SetTemperatureChangeSpeed(double slope)
        {
            CommandHandler.SendCommand(new TemperatureSlopeSetPoint(slope));
        }

        public void SetTemperature(double temperature)
        {
            CommandHandler.SendCommand(new TemperatureSetPoint(temperature));
        }

        public void SetPosition(ProfilePart position)
        {
            Log.Instance.Info(
                $"Задание положения осей: X {position.FirstPosition}:{position.SpeedFirst}. Y {position.SecondPosition}:{position.SpeedSecond}");
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
            StartAxis(Axis.All);
        }

        public bool InitEvo()
        {
            PowerOnCamera(true);
            PowerOnAxis(Axis.All, true);
            FindZeroIndex(Axis.All);
            return true;
        }

        public bool SetStartPosition()
        {
            StopAxis(Axis.All);
            SetAxisRate(Axis.All, Config.Instance.BaseMoveSpeed);
            SetAxisPosition(Axis.All, 0);
            StartAxis(Axis.All);
            SetTemperatureChangeSpeed(Config.Instance.SpeedOfTemperatureChange);
            return true;
        }

        #region IDisposable Support
        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    CommandHandler.Dispose();
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
        #endregion
    }
}
