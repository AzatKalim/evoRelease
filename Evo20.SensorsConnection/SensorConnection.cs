using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using Evo20;
using Evo20.Utils.EventArguments;

namespace Evo20.SensorsConnection
{
    /// <summary>
    /// Класс для работы с COM портом 
    /// </summary>
    public class SensorConnection : IDisposable
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
                    EventHandlerListForStateChange?.Invoke(this, new ConnectionStatusEventArgs(connectionState));
                }               
            }
            get
            {
                return connectionState;
            }
        }
        
        public delegate void SensorHandler(object sender, EventArgs e);//();

        public delegate void SensorStatusChange(object sender, EventArgs e);//(ConnectionStatus newState);

        public delegate void SensorExeptionHandler(object sender, EventArgs e);//(Exception exeption);

        // событие прихода нового уведомления 
        protected event SensorHandler EventHandlersListForPacket;

        public event SensorStatusChange EventHandlerListForStateChange;

        public event SensorExeptionHandler EventHandlerListForExeptions;

        protected SerialPort serialPort;
        //поток чтения байт с порта 
        protected Thread readThread;

        public byte[] bytesBuffer;

        protected int bytesCount;

        object bufferLocker = new object();

        public virtual bool StartConnection(string portName)
        {
            if (!serialPort.IsOpen)
            {
                serialPort.PortName = portName;
            }
            try
            {
                serialPort.Open();
            }
            catch (UnauthorizedAccessException exeption)
            {
                Log.Instance.Error("Указанный порт занят {0}",serialPort.PortName);
                Log.Instance.Exception(exeption);
                ConnectionStatus = ConnectionStatus.ERROR;
                EventHandlerListForExeptions?.Invoke(this, new ExceptionEventArgs(exeption));
                return false;
            }
            catch (ThreadAbortException exeption)
            {
                Log.Instance.Warning("Поток чтения Com порта закрыт");
                EventHandlerListForExeptions?.Invoke(this,new ExceptionEventArgs(exeption));
                return false;
            }
            catch (InvalidOperationException exeption)
            {
                Log.Instance.Error("Порт уже открыт");
                Log.Instance.Exception(exeption);
                EventHandlerListForExeptions?.Invoke(this, new ExceptionEventArgs(exeption));
                return false;
            }

            if (ConnectionStatus == ConnectionStatus.DISCONNECTED)
            {
                if (!readThread.IsAlive)
                {
                    readThread = new Thread(Read);
                    readThread.Start();
                }
                ConnectionStatus = ConnectionStatus.CONNECTED;
                Log.Instance.Info("Соединение c датчиком установленно");
                return true;
            }
            else
            {
                return false;
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
                Log.Instance.Info("Соединение c датчиком приостановленно");
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
                Log.Instance.Info("Соединение c датчиком востановлено");
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual bool StopConnection()
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
            Log.Instance.Info("Соединение c датчиком прервано");
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
            serialPort.BaudRate = 460800;
            serialPort.ReadTimeout = 100000;
            serialPort.WriteTimeout = 500;     
            //serialPort.ReadTimeout = 460800;
            //serialPort.WriteTimeout = 500;
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

            while (ConnectionStatus == ConnectionStatus.CONNECTED)
            {
                try
                {
                    //if (serialPort.BytesToRead == 0)
                    //{
                    //    Thread.Sleep(100);
                    //    continue;
                    //}
                    bytesCount = serialPort.Read(receiveBytes, 0, serialPort.BytesToRead);
                    if (bytesCount == 0)
                        continue;
                    lock (bufferLocker)
                    {
                        for (int i = 0; i < bytesCount; i++)
                        {
                            bytesBuffer[i] = receiveBytes[i];
                        }
                    }
                    EventHandlersListForPacket(this,null);
                }
                catch (TimeoutException exeption)
                {
                    Log.Instance.Error("Байты не были доступны для чтения");
                    Log.Instance.Exception(exeption);
                    ConnectionStatus = ConnectionStatus.ERROR;
                    EventHandlerListForExeptions?.Invoke(this, new ExceptionEventArgs(exeption));
                    return;
                }
                catch (InvalidOperationException exeption)
                {
                    Log.Instance.Error("Указанный порт не открыт {0}",serialPort.PortName);
                    Log.Instance.Exception(exeption);
                    ConnectionStatus = ConnectionStatus.ERROR;
                    EventHandlerListForExeptions?.Invoke(this, new ExceptionEventArgs(exeption));
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
            var message = new List<byte>(bytesCount);
            lock (bufferLocker)
            {
                for (int i = 0; i < bytesCount; i++)
                {
                    message.Add(bytesBuffer[i]);
                }
                bytesCount = 0;
            }
            return message;
        }

        #region IDisposable Support
        private bool disposedValue = false; // Для определения избыточных вызовов

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (readThread.IsAlive)
                    {
                        readThread.Abort();
                    }
                    if (serialPort.IsOpen)
                    {
                        serialPort.Close();
                    }
                    serialPort.Dispose();
                }
                bytesBuffer = null;
                disposedValue = true;
            }
        }

       
        ~SensorConnection()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        //public void Dispose()
        //{
        //    if (readThread.IsAlive)
        //    {
        //        readThread.Abort();
        //    }
        //    if (serialPort.IsOpen)
        //    {
        //        serialPort.Close();
        //    }
        //    serialPort.Dispose();
        //}
    }
}
