using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Evo20;
using System.Configuration;
using System.Collections.Specialized;

namespace Evo20.EvoConnections
{ 
    /// <summary>
    /// Класс для работы с Evo-20 через udp протокол
    /// </summary>
    public class ConnectionSocket
    {
        #region Delegates and events
        //делегат для события 
        public delegate void ConnectionSocketHandler();

        //делегат для события изменения состояния
        public delegate void ConnectionSocketStateHandler(ConnectionStatus state);
        //делегат для события возникновения ошибки
        public delegate void ConnectionSocketExceptionHandler(Exception exeption);

        // События прихода нового сообщения 
        protected event ConnectionSocketHandler EventHandlersListForCommand;
        // Событие изменения состояния 
        public event ConnectionSocketStateHandler EventHandlerListForStateChanged; 

        //Событие ошибки выполнения
        public event ConnectionSocketExceptionHandler EventHandlerListForException;

        #endregion

        #region Private Fields

        //ip адресс удаленного компьютера 
        private static IPAddress remoteIPAddress = IPAddress.Parse(Config.REMOTE_IP_ADRESS);
          
        IPEndPoint endPoint;

        //Состояние соединения 
        private ConnectionStatus connectionState;

        #endregion

        #region Protected Fields

        protected byte[] buffer;

        protected Thread work_thread;

        protected UdpClient Sender;

        protected UdpClient ReceivingUdpClient;

        #endregion

        //Свойство соятояния соединения
        public ConnectionStatus ConnectionStatus
        {
            get
            {
                return connectionState;
            }
            protected set
            {
                // изменяем сотояние системы
                connectionState = value;
                // вызываем событие изменения состояния 
                if(EventHandlerListForStateChanged!=null)                
                    EventHandlerListForStateChanged(value);
            }
        }

 
        public ConnectionSocket()
        {
            buffer = new byte[2048];
            work_thread = new Thread(ReadMessage);
            work_thread.IsBackground = true;
            connectionState = ConnectionStatus.DISCONNECTED;
            ReceivingUdpClient = null;
            endPoint = new IPEndPoint(remoteIPAddress, Config.REMOTE_PORT_NUMBER);
            Sender = new UdpClient(Config.REMOTE_PORT_NUMBER);
            //убрать
#if !DEBUG
            if(!Config.IsFakeEvo)
                Sender.Connect(endPoint);
#endif
            switch (Config.EvoType)
            {
                case 1:
                    {
                        ReceivingUdpClient = Sender;
                        break;
                    }
                case 0:
                    {
                        ReceivingUdpClient = new UdpClient(Config.LOCAL_PORT_NUMBER);
                        break;
                    }
            }
        }

        //Деструктор класса
        ~ConnectionSocket()
        {
            if (connectionState != ConnectionStatus.CONNECTED)
            {
                work_thread.Abort();
            }
        }

        #region Start, stop and pause methods
        /// <summary>
        /// Запуск соединения 
        /// </summary>
        /// <returns> результат запуска </returns>
        public bool StartConnection()
        {
            if (ConnectionStatus == ConnectionStatus.DISCONNECTED)
            {
                ConnectionStatus = ConnectionStatus.CONNECTED;
                if (!work_thread.IsAlive)
                {
                    work_thread= new Thread(ReadMessage);
                    work_thread.Start();
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
            if (ConnectionStatus == ConnectionStatus.CONNECTED)
            {
                work_thread.Abort();
                ConnectionStatus = ConnectionStatus.PAUSE;
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
            if (ConnectionStatus == ConnectionStatus.PAUSE)
            {
                work_thread = new Thread(ReadMessage);
                work_thread.Start();
                ConnectionStatus = ConnectionStatus.CONNECTED;
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
        public bool StopConnection()
        {
            if (work_thread.IsAlive)
            {
                work_thread.Abort();
            }
            ConnectionStatus = ConnectionStatus.DISCONNECTED;
            if(ReceivingUdpClient!=null)
                ReceivingUdpClient.Close();
            Log.Instance.Info("Соединение c Evo 20 прервано");
            return true;
        }

        #endregion

        #region Read Send Functions

        /// <summary>
        /// Отправка сообщения по udp протоколу
        /// </summary>
        /// <param name="message"> сообщение </param>
        /// <returns>результат </returns>
        public bool SendMessage(string message)
        {
            //убрать
            if(Config.IsFakeEvo)
                return true;
            try
            {
                if (connectionState == ConnectionStatus.CONNECTED)
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
                    if (this.ConnectionStatus == ConnectionStatus.CONNECTED)
                        this.ConnectionStatus = ConnectionStatus.DISCONNECTED;
                    return false;
                }
            }
            catch (Exception exception)
            {
                ConnectionStatus = ConnectionStatus.ERROR;
                Log.Instance.Error("Сообщение {0} Evo 20 не доставлено Возникло исключение",message);
                Log.Instance.Exception(exception);
                Sender.Close();
                if (EventHandlerListForException != null)
                    EventHandlerListForException(exception);
                return false;
            }
        }

        /// <summary>
        /// Бесконечное считывание приходящих сообщений изапись их в буффер ( выполняется в обдельном потоке)
        /// </summary>
        protected void ReadMessage()
        {
            //try
            //{
            //    receivingUdpClient = new UdpClient(LOCAL_PORT_NUMBER);
            //}
            //catch(Exception exception)
            //{
            //    connectionState = ConnectionStatus.ERROR;
            //    Evo20.Log.Log.WriteLog("Невозможно открыть соединение с Evo " + "Возникло исключение" + exception.ToString());
            //    if (EventHandlerListForException != null)
            //        EventHandlerListForException(exception);
            //    return;
            //}

            IPEndPoint RemoteIpEndPoint = null;

            try
            {
                while (connectionState == ConnectionStatus.CONNECTED)
                {
                    byte[] receiveBytes = ReceivingUdpClient.Receive(
                       ref RemoteIpEndPoint);

                    lock (buffer)
                    {
                        receiveBytes.CopyTo(buffer, 0);
                    }
                    EventHandlersListForCommand();
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
                ConnectionStatus = ConnectionStatus.ERROR;
                Log.Instance.Error("Невозможно открыть соединение с Evo Возникло исключение");
                Log.Instance.Exception(exception);
                if (EventHandlerListForException != null)
                    EventHandlerListForException(exception);
            }
        }


        ///// <summary>
        ///// Отправка сообщения по udp протоколу
        ///// </summary>
        ///// <param name="message"> сообщение </param>
        ///// <returns>результат </returns>
        //public bool SendMessage(string message)
        //{
        //    var sender = new UdpClient();
        //    endPoint = new IPEndPoint(remoteIPAddress, Config.REMOTE_PORT_NUMBER);
        //    try
        //    {
        //        if (connectionState == ConnectionStatus.CONNECTED)
        //        {
        //            byte[] bytes = Encoding.UTF8.GetBytes(message);
        //            sender.Send(bytes, bytes.Length, endPoint);
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        connectionState = ConnectionStatus.ERROR;
        //        Log.WriteLog("Сообщение " + message + " Evo 20 не доставлено " + "Возникло исключение" + ex);
        //        sender.Close();
        //        return false;
        //    }
        //    finally
        //    {
        //        sender.Close();
        //    }
        //    return true;
        //}

        ///// <summary>
        ///// Бесконечное считывание приходящих сообщений изапись их в буффер ( выполняется в обдельном потоке)
        ///// </summary>
        //protected void ReadMessage()
        //{
        //    UdpClient receivingUdpClient = null;
        //    try
        //    {
        //        receivingUdpClient = new UdpClient(Config.LOCAL_PORT_NUMBER);
        //    }
        //    catch (Exception ex)
        //    {
        //        connectionState = ConnectionStatus.ERROR;
        //        Log.WriteLog("Невозможно открыть соединение с Evo " + "Возникло исключение" + ex);
        //        return;
        //    }

        //    IPEndPoint RemoteIpEndPoint = null;

        //    try
        //    {
        //        while (connectionState == ConnectionStatus.CONNECTED)
        //        {
        //            byte[] receiveBytes = receivingUdpClient.Receive(
        //               ref RemoteIpEndPoint);

        //            lock (buffer)
        //            {
        //                receiveBytes.CopyTo(buffer, 0);
        //            }
        //            string message = Encoding.UTF8.GetString(buffer);
        //            Log.WriteLog("Получено сообщение " + message.ToString());
        //            EventHandlersListForCommand();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.WriteLog("Ошибка запуска цикла " + ex.ToString());
        //    }
        //    finally
        //    {
        //        receivingUdpClient.Close();
        //    }
        //}


        /// <summary>
        /// Метод возвращает значение хранящееся в буффере 
        /// </summary>
        /// <returns></returns>
        public string ReadBuffer()
        {
            var message= string.Empty;
            try
            {
                lock (buffer)
                {
                    message = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                    buffer = new byte[2048];
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

        #endregion
    }
}
