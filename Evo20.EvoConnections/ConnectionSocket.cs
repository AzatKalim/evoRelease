using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Evo20.Utils;
using Evo20.Utils.EventArguments;

namespace Evo20.EvoConnections
{
    public class ConnectionSocket : IDisposable
    {
        #region Delegates and events

        protected delegate void ConnectionSocketHandler(object sender, EventArgs e);

        public delegate void StateChangeHandler(object sender, EventArgs e);
        public delegate void ExceptionHandler(object sender, EventArgs e);

        protected event ConnectionSocketHandler NewMessageArrived;

        public event StateChangeHandler StateChanged; 

        public event ExceptionHandler ExceptionEvent;

        #endregion

        #region Private Fields

//#if !DEBUG
        private static readonly IPAddress RemoteIpAddress = IPAddress.Parse(Config.Instance.RemoteIpAdress);
//#endif
        private ConnectionStatus _connectionState = ConnectionStatus.Disconnected;

        private int _readedBytesCount;

        readonly object _bufferLocker = new object();
        #endregion

        #region Protected Fields

        protected byte[] Buffer = new byte[2048];

        protected Thread WorkThread;

        protected UdpClient Sender;

        protected UdpClient ReceivingUdpClient;

        #endregion

        public ConnectionStatus ConnectionStatus
        {
            get
            {
                return _connectionState;
            }
            protected set
            {
                _connectionState = value;
                StateChanged?.Invoke(this, new ConnectionStatusEventArgs(value));
            }
        }


        protected ConnectionSocket()
        {
            WorkThread = new Thread(ReadMessage) {IsBackground = true};

            Sender = new UdpClient(Config.Instance.RemotePortNumber);
//#if !DEBUG
            var endPoint = new IPEndPoint(RemoteIpAddress, Config.Instance.RemotePortNumber);
            if(!Config.IsFakeEvo)
                Sender.Connect(endPoint);
//#endif
            switch (Config.Instance.EvoType)
            {
                case 1:
                    {
                        ReceivingUdpClient = Sender;
                        break;
                    }
                case 0:
                    {
                        ReceivingUdpClient = new UdpClient(Config.Instance.LocalPortNumber);
                        break;
                    }
            }
        }

        ~ConnectionSocket()
        {
            Dispose(false);
        }

        #region Start, stop and pause methods
        /// <summary>
        /// Запуск соединения 
        /// </summary>
        /// <returns> результат запуска </returns>
        public bool StartConnection()
        {
            if (ConnectionStatus == ConnectionStatus.Disconnected)
            {
                ConnectionStatus = ConnectionStatus.Connected;
                if (!WorkThread.IsAlive)
                {
                    WorkThread= new Thread(ReadMessage);
                    WorkThread.Start();
                }
                Log.Instance.Info("Соединение c Evo 20 установлено");
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Приостановка соединения 
        /// </summary>
        /// <returns></returns>
        public bool PauseConnection()
        {
            if (ConnectionStatus == ConnectionStatus.Connected)
            {
                WorkThread.Abort();
                ConnectionStatus = ConnectionStatus.Pause;
                Log.Instance.Info("Соединение c Evo 20 приостановлено");
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Востановление работы соединения 
        /// </summary>
        /// <returns></returns>
        public bool ResumeConnection()
        {
            if (ConnectionStatus == ConnectionStatus.Pause)
            {
                WorkThread = new Thread(ReadMessage);
                WorkThread.Start();
                ConnectionStatus = ConnectionStatus.Connected;
                Log.Instance.Info("Соединение c Evo 20 востановлено");
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Завершение работы соединения 
        /// </summary>
        /// <returns></returns>
        public void StopConnection()
        {
            if (WorkThread.IsAlive)
            {
                WorkThread.Abort();
            }
            ConnectionStatus = ConnectionStatus.Disconnected;
            ReceivingUdpClient?.Close();
            Log.Instance.Info("Соединение c Evo 20 прервано");
        }

        #endregion

        #region Read Send Functions

        /// <summary>
        /// Отправка сообщения по udp протоколу
        /// </summary>
        /// <param name="message"> сообщение </param>
        /// <returns>результат </returns>
        protected bool SendMessage(string message)
        {
            if(Config.IsFakeEvo)
                return true;
            try
            {
                if (_connectionState == ConnectionStatus.Connected)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(message);
                    if (bytes.Length == Sender.Send(bytes, bytes.Length))
                        return true;
                    else
                    {
                        Log.Instance.Error("Сообщение {0} Evo 20 не было отправлено,число байт не совпало",message);
                        return false;
                    }
                }
                else
                {
                    Log.Instance.Error("Сообщение {0} Evo 20 не было отправлено, соединение активно? {1}",message,Sender.Client.Connected);
                    if (ConnectionStatus == ConnectionStatus.Connected)
                        ConnectionStatus = ConnectionStatus.Disconnected;
                    return false;
                }
            }
            catch (Exception exception)
            {
                ConnectionStatus = ConnectionStatus.Error;
                Log.Instance.Error("Сообщение {0} Evo 20 не доставлено Возникло исключение",message);
                Log.Instance.Exception(exception);
                Sender.Close();
                ExceptionEvent?.Invoke(this, new ExceptionEventArgs(exception));
                return false;
            }
        }

        protected void ReadMessage()
        {
            IPEndPoint remoteIpEndPoint = null;

            try
            {
                while (_connectionState == ConnectionStatus.Connected)
                {
                    byte[] receiveBytes = ReceivingUdpClient.Receive(
                       ref remoteIpEndPoint);

                    lock (_bufferLocker)
                    {
                        receiveBytes.CopyTo(Buffer, 0);
                        _readedBytesCount = receiveBytes.Length;
                    }

                    NewMessageArrived?.Invoke(this, null);
                }
            }
            catch (ThreadAbortException)
            {
                Log.Instance.Warning("Поток чтения данных evo прерван");
            }
            catch (ObjectDisposedException)
            {
                Log.Instance.Warning("Сокет закрыт");
            }
            catch (Exception exception)
            {
                ConnectionStatus = ConnectionStatus.Error;
                Log.Instance.Error("Невозможно открыть соединение с Evo Возникло исключение");
                Log.Instance.Exception(exception);
                ExceptionEvent?.Invoke(this, new ExceptionEventArgs(exception));
            }
        }

        protected string ReadBuffer()
        {
            string message;
            try
            {
                lock (_bufferLocker)
                {
                    message = Encoding.UTF8.GetString(Buffer, 0, _readedBytesCount);
                    _readedBytesCount=0;
                }
            }
            catch (FormatException exception)
            {
                Log.Instance.Error("ConnectionSocket: Ошибка чтения из буфера!");
                Log.Instance.Exception(exception);
                message = String.Empty;
            }
            return message;
        }

        #region IDisposable Support
        private bool _disposedValue;

        protected void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (_connectionState != ConnectionStatus.Connected)
                    {
                        WorkThread.Abort();
                        Sender.Close();
                        ReceivingUdpClient.Close();
                    }
                    Buffer = null;
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);         
            GC.SuppressFinalize(this);
        }
        #endregion

        #endregion
    }
}
