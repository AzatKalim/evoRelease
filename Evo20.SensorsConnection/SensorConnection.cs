using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using Evo20.Utils;
using Evo20.Utils.EventArguments;

namespace Evo20.SensorsConnection
{
    /// <summary>
    /// Класс для работы с COM портом 
    /// </summary>
    public class SensorConnection : IDisposable
    {
        public const int BufferSize = 10000;

        private ConnectionStatus _connectionState;

        public ConnectionStatus ConnectionStatus
        {
            set
            {
                if (_connectionState != value)
                {
                    _connectionState = value;
                    EventHandlerListForStateChange?.Invoke(this, new ConnectionStatusEventArgs(_connectionState));
                }               
            }
            get
            {
                return _connectionState;
            }
        }
        
        public delegate void SensorHandler(object sender, EventArgs e);

        public delegate void SensorStatusChange(object sender, EventArgs e);

        public delegate void SensorExeptionHandler(object sender, EventArgs e);

        protected event SensorHandler EventHandlersListForPacket;

        public event SensorStatusChange EventHandlerListForStateChange;

        public event SensorExeptionHandler EventHandlerListForExeptions;

        protected SerialPort SerialPort;

        protected Thread ReadThread;

        public byte[] BytesBuffer;

        protected int BytesCount;

        readonly object _bufferLocker = new object();

        public virtual bool StartConnection(string portName)
        {
            if (!SerialPort.IsOpen)
            {
                SerialPort.PortName = portName;
            }
            try
            {
                SerialPort.Open();
            }
            catch (UnauthorizedAccessException exeption)
            {
                Log.Instance.Error("Указанный порт занят {0}",SerialPort.PortName);
                Log.Instance.Exception(exeption);
                ConnectionStatus = ConnectionStatus.Error;
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

            if (ConnectionStatus == ConnectionStatus.Disconnected)
            {
                if (!ReadThread.IsAlive)
                {
                    ReadThread = new Thread(Read);
                    ReadThread.Start();
                }
                ConnectionStatus = ConnectionStatus.Connected;
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
            if (ConnectionStatus == ConnectionStatus.Connected)
            {
                if (ReadThread.IsAlive)
                {
                    ReadThread.Abort();
                }
                ConnectionStatus = ConnectionStatus.Pause;
                Log.Instance.Info("Соединение c датчиком приостановленно");
                return true;
            }
            else
            {
                return false;
            }
        }

        //public bool ResumeConnection()
        //{
        //    if (ConnectionStatus == ConnectionStatus.Pause)
        //    {
        //        ReadThread.Start();
        //        ConnectionStatus = ConnectionStatus.Connected;
        //        Log.Instance.Info("Соединение c датчиком востановлено");
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        public virtual bool StopConnection()
        {
            if (ReadThread.IsAlive)
            {
                ReadThread.Abort();
            }
            if (SerialPort.IsOpen)
            {
                SerialPort.Close();
            }
            ConnectionStatus = ConnectionStatus.Disconnected;
            Log.Instance.Info("Соединение c датчиком прервано");
            return true;
        }

        public SensorConnection(string port)
        {
            SerialPort.PortName = port;          
        }

        public SensorConnection()
        {
            ReadThread = new Thread(Read) {IsBackground = true};
            SerialPort = new SerialPort
            {
                BaudRate = 460800,
                ReadTimeout = 100000,
                WriteTimeout = 500,
                Parity = 0,
                StopBits = StopBits.One,
                DataBits = 8
            };
            ConnectionStatus = ConnectionStatus.Disconnected;
            BytesBuffer = new byte[BufferSize];
        }

        protected void Read()
        {
            byte[] receiveBytes = new byte[BufferSize];

            while (ConnectionStatus == ConnectionStatus.Connected)
            {
                try
                {
                    //if (serialPort.BytesToRead == 0)
                    //{
                    //    Thread.Sleep(100);
                    //    continue;
                    //}
                    lock (_bufferLocker)
                    {
                        BytesCount = SerialPort.Read(receiveBytes, 0, SerialPort.BytesToRead);
                    }

                    if (BytesCount == 0)
                        continue;
                    lock (_bufferLocker)
                    {
                        for (int i = 0; i < BytesCount; i++)
                        {
                            BytesBuffer[i] = receiveBytes[i];
                        }
                    }

                    EventHandlersListForPacket?.Invoke(this, null);
                }
                catch (TimeoutException exception)
                {
                    Log.Instance.Error("Байты не были доступны для чтения");
                    Log.Instance.Exception(exception);
                    ConnectionStatus = ConnectionStatus.Error;
                    EventHandlerListForExeptions?.Invoke(this, new ExceptionEventArgs(exception));
                    return;
                }
                catch (InvalidOperationException exception)
                {
                    Log.Instance.Error("Указанный порт не открыт {0}", SerialPort.PortName);
                    Log.Instance.Exception(exception);
                    ConnectionStatus = ConnectionStatus.Error;
                    EventHandlerListForExeptions?.Invoke(this, new ExceptionEventArgs(exception));
                    return;
                }
                catch (Exception exception)
                {
                    Log.Instance.Warning("Неизвестная ошибка");
                    Log.Instance.Exception(exception);
                    ConnectionStatus = ConnectionStatus.Error;
                    EventHandlerListForExeptions?.Invoke(this, new ExceptionEventArgs(exception));
                    return;
                }
            }
        }
       
        public List<byte> ReadBuffer()
        {
            List<byte> message = new List<byte>(BytesCount);
            lock (_bufferLocker)
            {
                for (int i = 0; i < BytesCount; i++)
                {
                    message.Add(BytesBuffer[i]);
                }
                BytesCount = 0;
            }
            return message;
        }

        #region IDisposable Support
        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (ReadThread.IsAlive)
                    {
                        ReadThread.Abort();
                    }
                    if (SerialPort.IsOpen)
                    {
                        SerialPort.Close();
                    }
                    SerialPort.Dispose();
                }
                lock (_bufferLocker)
                {
                    BytesBuffer = null;
                }
                _disposedValue = true;
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
