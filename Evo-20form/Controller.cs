using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Threading;
//my libs
using Evo_20_commands;
using PacketsLib;
using ComputeCoefficients;

namespace Evo_20form
{  
    class Controller:ControllerCommand
    {
        #region Constans

        const int THREADS_SLEEP_TIME = 1000;

        public const string SETTINGS_FILE_NAME = "settings.txt";

        #endregion 

        public delegate void ConnectionHandler(bool result);

        public event ConnectionHandler EventHandlersListCycleEnded;

        Thread cycleThread;

        SensorHandler sensorHandler;

        bool canCollect=false;

        int temperatureOfCollect = 0;

        #region Properties 

        public CycleData cycleData
        {
            get;
            private set;
        }

        public SensorData sensorData
        {
            get;
            private set;
        }

        public WorkMode mode
        {
            get;
            private set;
        }

        public double currentTemperature
        {
            get
            {
                return temperatureOfCollect;
            }
        }
     
        public int temperaturesCount
        {
            get
            {
                if (mode == WorkMode.CalibrationMode)
                    return cycleData.calibration_temperatures.Count;
                else
                    return cycleData.check_temperatures.Count;
            }
        }

        public int temperutureIndex
        {
            get
            {
                if (mode == WorkMode.CalibrationMode)
                    return cycleData.FindCalibrationTemperatureIndex((int)evoData.currentTemperature);
                else
                    return cycleData.FindCheckTemperatureIndex((int)evoData.currentTemperature);
            }
           
        }

        public int packetsCollectedCount
        {
            get
            {
                if (mode == WorkMode.CalibrationMode)
                {
                    return sensorData.PacketsArivedCountCalibration(temperatureOfCollect,currentPositionNumber);
                }
                if( mode==WorkMode.CheckMode)
                {
                    return sensorData.PacketsArivedCountCheck(temperatureOfCollect, currentPositionNumber);
                }
                return 0;
            }
        }

        public int currentPositionNumber
        {
            get;
            private set;
        }

        #endregion

        public Controller()
        {
            sensorHandler = new SensorHandler();
            sensorHandler.EventHandlersListForController += NewPacketDataHandler;
            commandHandler= new CommandHandler();
            commandHandler.CommandHandlersListForController += NewCommandHandler;
            sensorData = new SensorData();
            cycleData= new CycleData();
            routineThread = new Thread(ControllerRoutine);
            routineThread.Priority = ThreadPriority.BelowNormal;
            routineThread.IsBackground = true;
            mode = WorkMode.NoMode;
            evoData = new EvoData();
        }

        #region Start Stop Pause Methods

        public bool Start(string comPortName,WorkMode mode)
        {
            if (cycleThread != null && cycleThread.IsAlive)
                return false;
            switch(mode)
            {
                case WorkMode.CalibrationMode:              
                    cycleThread = new Thread(CalibrationCycle);
                    break;
                case WorkMode.CheckMode:
                    cycleThread= new Thread(CheckCycle);
                    break;
                default:
                    return false;
            }
            bool resultEvoStart= StartEvoConnection();
            if (!resultEvoStart)
                return false;
            bool resultComPortStart =StartComPortConnection(comPortName);
            if (!resultComPortStart)
                return false;
            cycleThread.Start();
            return true;
        }
        
        public void Stop()
        {
            if (routineThread!=null && routineThread.IsAlive)
                routineThread.Abort();
            if(cycleThread!=null && cycleThread.IsAlive)
                cycleThread.Abort();
            StopComPortConnection();
            StopEvoConnection();
        }

        public void Pause()
        {
            routineThread.Abort();
            cycleThread.Suspend();
        }

        public bool StartEvoConnection()
        {
            bool result=commandHandler.StartConnection();
            if (!result)
            {
                return result;
            }
            if (!routineThread.IsAlive)
            {
                
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
            routineThread.Abort();
        }

        public bool StartComPortConnection(string portName)
        {
            return sensorHandler.StartConnection(portName);
        }

        public void PauseComPortConnection()
        {
            commandHandler.PauseConnection();
        }

        public void StopComPortConnection()
        {
            sensorHandler.StopConnection();
        }

        #endregion

        public bool ReadSettings()
        {
            try
            {
                if (!File.Exists(SETTINGS_FILE_NAME))
                {
                    string caption = "OK";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    string message = "Файл " + SETTINGS_FILE_NAME + "  не найден!";
                    MessageBox.Show(message, caption, buttons);
                    return false;
                }
                Encoding enc = Encoding.GetEncoding(1251);
                StreamReader file = new StreamReader(@SETTINGS_FILE_NAME, enc);
                if (!cycleData.ReadSettings(ref file))
                {
                    return false;
                }
                if (!sensorData.ReadSettings(ref file))
                {
                    return false;
                }
                file.Close();
            }
            catch (Exception ex)
            {
                Log.WriteLog("Возникла ошибка файла настроек" + ex.ToString());
                throw ex;
            }
            sensorData.InitialDictionaries(cycleData.GetCollibrationTemperatures(),
                cycleData.GetCheckTemperatures(),
                CycleData.PROFILE_DLY_LENGHT,
                CycleData.PROFILE_DYS_LENGHT);
            return true;
        }

        public void ControllerRoutine()
        {
            Command[] commands = GetRoutineCommands();
            while (commandHandler.connectionState == ConnectionState.CONNECTED)
            {
                foreach (var item in commands)
                {
                    lock(commandHandler)
                    {
                        commandHandler.SendCommand(item);
                        Thread.Sleep(100);
                    }
                }
                Thread.Sleep(THREADS_SLEEP_TIME);
            }
        }

        private void NewPacketDataHandler()
        {
            PacketsData temp= sensorHandler.DataHandle();
            if (temp == null)
            {
                return;
            }
            if (mode == WorkMode.CalibrationMode && canCollect)
            {
                sensorData.AddCalibrationPacketData(temp,
                    temperatureOfCollect,
                    currentPositionNumber);
            }
            else
            {
                if (mode == WorkMode.CheckMode && canCollect)
                {
                    sensorData.AddCheckPacketData(temp,
                        temperatureOfCollect,
                        currentPositionNumber);
                }
            }
        }

        #region Cycles

        public void CalibrationCycle()
        {
            mode = WorkMode.CalibrationMode;
            Cycle(cycleData.GetCollibrationTemperatures());
            mode = WorkMode.NoMode;
        }

        public void CheckCycle()
        {
            mode = WorkMode.CheckMode;
            Cycle(cycleData.GetCheckTemperatures());
            mode = WorkMode.NoMode;
        }

        private void Cycle(int[] temperatures)
        {
            PowerOnCamera(true);
            PowerOnAxis(Axis.ALL, true);
            FindZeroIndex(Axis.ALL);
            for (int i = 0; i < temperatures.Length; i++)
            {
                StopAxis(Axis.ALL);
                SetAxisRate(Axis.ALL, EvoData.BASE_MOVE_SPEED);
                SetAxisPosition(Axis.ALL, 0);
                StartAxis(Axis.ALL);
                SetTemperatureChangeSpeed(EvoData.SPEED_OF_TEMPERATURE_CHANGE);
                lock (sensorData)
                {
                    evoData.nextTemperature = temperatures[i];
                }
                SetTemperature(temperatures[i]);
                evoData.nextTemperature = temperatures[i];
                sensorData.sensorType = SensorType.DLY;
                Log.WriteLog("Установлена температура камеры " + temperatures[i] + " скорость набора температtуры " + EvoData.SPEED_OF_TEMPERATURE_CHANGE);
                //TODO: убрать врема ожидания 
                evoData.temperatureReachedEvent.WaitOne(100);
                evoData.temperatureReachedEvent.Reset();
                Log.WriteLog("Температура  " + temperatures[i] + " достигнута");
                Thread.Sleep(cycleData.check_stab_time);
                evoData.temperatureStabilazedEvent.Set();
                DLYProfilePart[] profileDLY = null;
                switch (mode)
                {
                    case WorkMode.CalibrationMode:
                        profileDLY = CycleData.GetDLYCalibrationProfile();
                        break;
                    case WorkMode.CheckMode:
                        profileDLY = CycleData.GetDLYCheckProfile();
                        break;
                    default:
                        Log.WriteLog("Ошибка:перед запуском цикла ДЛУ режим работы не установлен");
                        EventHandlersListCycleEnded(false);
                        return;
                }
                temperatureOfCollect = temperatures[i];
                bool isDLYCyclePartSuccess = DLYCyclePart(profileDLY);
                
                if (!isDLYCyclePartSuccess)
                {
                    Log.WriteLog("Ошибка:Не выполнен цикл ДЛУ при температуре" + temperatures[i]);
                    EventHandlersListCycleEnded(false);
                    return;
                }

                DYSProfilePart[] profileDYS = null;
                switch (mode)
                {
                    case WorkMode.CalibrationMode:
                        profileDYS = CycleData.GetDYSCalibrationProfile();
                        break;
                    case WorkMode.CheckMode:
                        profileDYS = CycleData.GetDYSCheckProfile();
                        break;
                    default:
                        Log.WriteLog("Ошибка:перед запуском цикла ДЛУ режим работы не установлен");
                        EventHandlersListCycleEnded(false);
                        return;
                }

                bool isDYSCyclePartSuccess = DYSCyclePart(profileDYS);

                if (!isDYSCyclePartSuccess)
                {
                    Log.WriteLog("Ошибка:Не выполнен цикл ДУС при температуре" + temperatures[i]);
                    EventHandlersListCycleEnded(false);
                    return;
                }               
            }
            sensorData.WriteAllPackets();
            EventHandlersListCycleEnded(true);

        }

        private bool DLYCyclePart(DLYProfilePart[] profile)
        {
            int j = 0;
            try
            {
                for (; j < profile.Length; j++)
                {
                    evoData.movementEndedEvent.WaitOne(THREADS_SLEEP_TIME);
                    evoData.movementEndedEvent.Reset();
                    StopAxis(Axis.ALL);
                    SetAxisPosition(Axis.X, profile[j].axisX);
                    SetAxisPosition(Axis.Y, profile[j].axisY);
                    StartAxis(Axis.ALL);
                    SetAxisMode(ModeParam.Position, Axis.ALL);
                    Thread.Sleep(THREADS_SLEEP_TIME);
                    evoData.movementEndedEvent.WaitOne();
                    evoData.movementEndedEvent.Reset();
                    sensorData.sensorType = SensorType.DLY;
                    currentPositionNumber = j;
                    canCollect = true;
                    //sensorData.packetsCollectedEvent.WaitOne();
                    sensorData.packetsCollectedEvent.Reset();
                    canCollect = false;
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("Возникло исключение цикла ДЛУ при шаге"+ j+" текст ошибки" + ex.ToString());
                return false;
            }
            return true;
        }

        private bool DYSCyclePart(DYSProfilePart[] profile)
        {
            int j = 0;
            try
            {
                for (; j < profile.Length; j++)
                {
                    StopAxis(Axis.ALL);
                    currentPositionNumber = j;
                    if (profile[j].speedX != 0)
                    {
                        SetAxisRate(Axis.X, profile[j].speedX);
                        SetAxisMode(ModeParam.Speed, Axis.X);
                    }
                    else
                    {
                        SetAxisPosition(Axis.X, profile[j].axisX);
                        SetAxisMode(ModeParam.Position, Axis.X);
                    }
                    if (profile[j].speedY != 0)
                    {
                        SetAxisRate(Axis.Y, profile[j].speedY);
                        SetAxisMode(ModeParam.Speed, Axis.Y);
                    }
                    else
                    {
                        SetAxisPosition(Axis.Y, profile[j].axisY);
                        SetAxisMode(ModeParam.Position, Axis.Y);
                    }
                    StartAxis(Axis.ALL);
                    sensorData.sensorType = SensorType.DYS;
                    currentPositionNumber = j;
                    canCollect = true;
                    //sensorData.packetsCollectedEvent.WaitOne();
                    //sensorData.packetsCollectedEvent.Reset();
                    canCollect = false;
                }

            }
            catch (Exception ex)
            {
                Log.WriteLog("Возникло исключение цикла  ДYS при шаге"+ j+" текст ошибки" + ex.ToString());
                return false;
            }
            return true;

        }

        #endregion

        #region Secondary functions

        public List<double> GetSensorData()
        {
            switch (mode)
            {
                case WorkMode.CalibrationMode:
                    return sensorData.СalculateCalibrationAverage(temperatureOfCollect,currentPositionNumber);
                case WorkMode.CheckMode:
                    return sensorData.СalculateCheckAverage(temperatureOfCollect, currentPositionNumber);
                default:
                    return null;
            }           
        }

        public bool ComputeCoefficents(StreamWriter file)
        {
            bool result= CalculatorCoefficients.CalculateCoefficients(sensorData.calibrationDLYPacketsCollection,
                sensorData.calibrationDYSPacketsCollection,
                file);
            if (!result)
            {
                Log.WriteLog("Вычисление коэфицентов не выполнено!");
                return result;
            }
            Log.WriteLog("Вычисление коэфицентов выполнено!");
            return result;
        }

        public bool WritePackets(StreamWriter file)
        {
            return sensorData.WriteAllPackets(file);
        }

        public bool ReadDataFromFile(StreamReader file)
        {
            return sensorData.ReadDataFromFile(file);
        }

        #endregion

        ~Controller()
        {           
            sensorData.WriteAllPackets();
        }
    }
}
