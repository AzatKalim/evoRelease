﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using Evo_20form.Utils;

namespace Evo_20form.Sensors
{
    /// <summary>
    /// Класс для работы с COM портом 
    /// </summary>
    class SensorConnection
    {
        public const int BUFFER_SIZE = 10000;

        public const int THREAD_SLEEP_TIME = 100;

        private ConnectionStatus connectionState;

        public ConnectionStatus ConnectionStatus
        {
            set
            {
                if (connectionState != value)
                {
                    connectionState = value;
                    if (EventHandlerListForStateChange != null)
                    {
                        EventHandlerListForStateChange(connectionState);
                    }
                }               
            }
            get
            {
                return connectionState;
            }
        }
        
        public delegate void SensorHandler();

        public delegate void SensorStatusChange(ConnectionStatus newState);

        public delegate void SensorExeptionHandler(Exception exeption);

        // событие прихода нового уведомления 
        protected event SensorHandler EventHandlersListForPacket;

        public event SensorStatusChange EventHandlerListForStateChange;

        public event SensorExeptionHandler EventHandlerListForExeptions;

        protected SerialPort serialPort;
        //поток чтения байт с порта 
        protected Thread readThread;

        public byte[] bytesBuffer;

        protected int bytesCount;

        public bool StartConnection(string portName)
        {
            if (!serialPort.IsOpen)
            {
                serialPort.PortName = portName;
            }
            try
            {
                if (ConnectionStatus == ConnectionStatus.DISCONNECTED)
                {
                    if (!readThread.IsAlive)
                    {
                        readThread = new Thread(Read);
                        readThread.Start();
                    }
                    ConnectionStatus = ConnectionStatus.CONNECTED;
                    Log.WriteLog("Соединение c датчиком установленно");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public bool PauseConnection()
        {
            if (ConnectionStatus == ConnectionStatus.CONNECTED)
            {
                if (readThread.IsAlive)
                {
                    readThread.Abort();
                }
                ConnectionStatus = ConnectionStatus.PAUSE;
                Log.WriteLog("Соединение c датчиком приостановленно");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ResumeConnection()
        {
            if (ConnectionStatus == ConnectionStatus.PAUSE)
            {
                readThread.Start();
                ConnectionStatus = ConnectionStatus.CONNECTED;
                Log.WriteLog("Соединение c датчиком востановлено");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool StopConnection()
        {
            if (readThread.IsAlive)
            {
                readThread.Abort();
            }
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
            ConnectionStatus = ConnectionStatus.DISCONNECTED;
            Log.WriteLog("Соединение c датчиком прервано");
            return true;
        }

        /// <summary>
        /// Конструктор 
        /// </summary>
        /// <param name="port"> Имя COM порта</param>
        public SensorConnection(String port)
        {
            serialPort.PortName = port;          
        }

        public SensorConnection()
        {
            readThread = new Thread(Read);
            readThread.IsBackground = true;
            serialPort = new SerialPort();
            serialPort.ReadTimeout = 460800;
            serialPort.WriteTimeout = 500;
            serialPort.Parity = 0;
            serialPort.StopBits = StopBits.One;
            serialPort.DataBits = 8;
            ConnectionStatus = ConnectionStatus.DISCONNECTED;
            bytesBuffer = new byte[BUFFER_SIZE];
        }

        /// <summary>
        ///  чтение байт из COM порта ( выполняется в обдельном потоке )
        /// </summary>
        protected void Read()
        {
            byte[] receiveBytes = new byte[BUFFER_SIZE];
            try
            {
                serialPort.Open();
            }
            catch (UnauthorizedAccessException exeption)
            {
                Log.WriteLog("Указанный порт занят" + serialPort.PortName);
                Log.WriteLog(exeption.ToString());
                ConnectionStatus = ConnectionStatus.ERROR;
                if (EventHandlerListForExeptions != null)
                {
                    EventHandlerListForExeptions(exeption);
                }
                return;
            }
            catch (ThreadAbortException exeption)
            {
                Log.WriteLog("Поток чтения Com порта закрыт");
                Log.WriteLog(exeption.ToString());
                if (EventHandlerListForExeptions != null)
                {
                    EventHandlerListForExeptions(exeption);
                }
            }
            while (ConnectionStatus == ConnectionStatus.CONNECTED)
            {
                try
                {
                    if (serialPort.BytesToRead == 0)
                    {
                        Thread.Sleep(THREAD_SLEEP_TIME);
                        continue;
                    }
                    bytesCount = serialPort.Read(receiveBytes, 0, serialPort.BytesToRead);
                    lock (bytesBuffer)
                    {
                        for (int i = 0; i < bytesCount; i++)
                        {
                            bytesBuffer[i] = receiveBytes[i];
                        }
                    }
                    EventHandlersListForPacket();
                }
                catch (TimeoutException exeption)
                {
                    Log.WriteLog("Байты не были доступны для чтения");
                    Log.WriteLog(exeption.ToString());
                    ConnectionStatus = ConnectionStatus.ERROR;
                    if (EventHandlerListForExeptions != null)
                    {
                        EventHandlerListForExeptions(exeption);
                    }
                    return;
                }
                catch (InvalidOperationException exeption)
                {
                    Log.WriteLog("Указанный порт не открыт " + serialPort.PortName);
                    Log.WriteLog(exeption.ToString());
                    ConnectionStatus = ConnectionStatus.ERROR;
                    if (EventHandlerListForExeptions != null)
                    {
                        EventHandlerListForExeptions(exeption);
                    }
                    return;
                }
            }
        }
       
        /// <summary>
        /// Чтение  данных из буфера приннятых байт 
        /// </summary>
        /// <returns></returns>
        public List<byte> ReadBuffer()
        {
            List<byte> message = null;
            lock (bytesBuffer)
            {
                message = new List<byte>(bytesBuffer);              
                bytesCount = 0;
            }
            return message;
        }

        ~SensorConnection()
        {
            if (readThread.IsAlive)
            {
                readThread.Abort();               
            }
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }         
        }
    }
}
