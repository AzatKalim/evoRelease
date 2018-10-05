using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Evo20.Config;
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
        private static IPAddress remoteIPAddress = IPAddress.Parse(Config.Config.REMOTE_IP_ADRESS);
          
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
            endPoint = new IPEndPoint(remoteIPAddress, Config.Config.REMOTE_PORT_NUMBER);
            Sender = new UdpClient(Config.Config.REMOTE_PORT_NUMBER);
            switch (Config.Config.EvoType)
            {
                case 1:
                    {
                        ReceivingUdpClient = Sender;
                        break;
                    }
                case 0:
                    {
                        ReceivingUdpClient = new UdpClient(Config.Config.LOCAL_PORT_NUMBER);
                        break;
                    }
            }
           
            if (Config.Config.EvoType == 1)
            {
                ReceivingUdpClient = Sender;
            }
            if (Config.Config.EvoType == 0)
            {

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
            if (connectionState == ConnectionStatus.DISCONNECTED)
            {
                ConnectionStatus = ConnectionStatus.CONNECTED;
                if (!work_thread.IsAlive)
                {
                    work_thread= new Thread(ReadMessage);
                    work_thread.Start();
                }
                Evo20.Log.WriteLog("Соединение c Evo 20 установлено");
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
            if (connectionState == ConnectionStatus.CONNECTED)
            {
                work_thread.Abort();
                ConnectionStatus = ConnectionStatus.PAUSE;
                Evo20.Log.WriteLog("Соединение c Evo 20 приостановлено");
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
            if (connectionState == ConnectionStatus.PAUSE)
            {
                work_thread = new Thread(ReadMessage);
                work_thread.Start();
                ConnectionStatus = ConnectionStatus.CONNECTED;
                Evo20.Log.WriteLog("Соединение c Evo 20 востановлено");
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
            Evo20.Log.WriteLog("Соединение c Evo 20 прервано");
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
            try
            {
                if (connectionState == ConnectionStatus.CONNECTED)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(message);
                    Sender.Send(bytes, bytes.Length, endPoint);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exception)
            {
                connectionState = ConnectionStatus.ERROR;
                Evo20.Log.WriteLog("Сообщение " + message + " Evo 20 не доставлено " + "Возникло исключение" + exception);
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
            catch (Exception exception)
            {
                connectionState = ConnectionStatus.ERROR;
                Evo20.Log.WriteLog("Невозможно открыть соединение с Evo " + "Возникло исключение" + exception.ToString());
                if (EventHandlerListForException != null)
                    EventHandlerListForException(exception);
            }
            finally
            {
                Sender.Close();
            }
        }


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
                Evo20.Log.WriteLog(string.Format("ConnectionSocket: Ошибка чтения из буфера {0}!", exception.ToString()));
                message = String.Empty;
            }
            return message;
        }

        #endregion
    }
}
