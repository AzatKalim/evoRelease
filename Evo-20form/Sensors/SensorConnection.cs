using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using PacketsLib;

namespace Evo_20form
{
    /// <summary>
    /// Класс для работы с COM портом 
    /// </summary>
    class SensorConnection
    {
        public const int BUFFER_SIZE = 10000;

        public ConnectionState connectionState;
        
        public delegate void SensorHandler();
        // событие прихода нового уведомления 
        protected event SensorHandler EventHandlersListForPacket;

        //protected bool _continue;

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
                if (connectionState == ConnectionState.DISCONNECTED)
                {
                    if (!readThread.IsAlive)
                    {
                        readThread.Start();
                    }
                    connectionState = ConnectionState.CONNECTED;
                    Log.WriteLog("Соединение c датчиком установленно");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool PauseConnection()
        {
            if (connectionState == ConnectionState.CONNECTED)
            {
                if (readThread.IsAlive)
                {
                    readThread.Abort();
                }
                connectionState = ConnectionState.PAUSE;
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
            if (connectionState == ConnectionState.PAUSE)
            {
                readThread.Start();
                connectionState = ConnectionState.CONNECTED;
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
            connectionState = ConnectionState.DISCONNECTED;
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
            connectionState = ConnectionState.DISCONNECTED;
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
                while (connectionState == ConnectionState.CONNECTED)
                {
                    try
                    {
                        bytesCount = serialPort.Read(receiveBytes, 0,serialPort.BytesToRead);
                        if (bytesCount == 0)
                        {
                            continue;
                        }
                        lock (bytesBuffer)
                        {
                            for (int i = 0; i < bytesCount; i++)
                            {
                                bytesBuffer[i] = receiveBytes[i];
                            }
                        }
                        EventHandlersListForPacket();
                    }
                    catch (TimeoutException)
                    {
                        Log.WriteLog("Байты не были доступны для чтения");
                        connectionState = ConnectionState.ERROR;
                        return;
                    }
                    catch (InvalidOperationException)
                    {
                        Log.WriteLog("Указанный порт не открыт " + serialPort.PortName);
                        connectionState = ConnectionState.ERROR;
                        return;
                    }
                }
            }
            catch (ThreadAbortException)
            {
                Log.WriteLog("Поток чтения Com порта закрыт");
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
