﻿using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Evo20.Log;
using System.Configuration;
using System.Collections.Specialized;

namespace Evo20.EvoConnections
{ 
    /// <summary>
    /// Класс для работы с Evo-20 через udp протокол
    /// </summary>
    public class ConnectionSocket
    {
        #region Constants

        //номер порта приходящих сообщений 
        static int LOCAL_PORT_NUMBER = Convert.ToInt32(ConfigurationManager.AppSettings.Get("LocalPortNumber"));

        //номер удаленного порта 
        static int REMOTE_PORT_NUMBER = Convert.ToInt32(ConfigurationManager.AppSettings.Get("RemotePortNumber"));

        //ip адресс удаленного компьютера 
        static string REMOTE_IP_ADRESS = ConfigurationManager.AppSettings.Get("RemoteIPAdress");

        #endregion

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
        private static IPAddress remoteIPAddress = IPAddress.Parse(REMOTE_IP_ADRESS);
          
        IPEndPoint endPoint;

        //Состояние соединения 
        private ConnectionStatus connectionState;

        #endregion

        #region Protected Fields

        protected byte[] buffer;

        protected Thread work_thread;

        protected UdpClient udpClient;

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
            udpClient = new UdpClient(3306);
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
                Evo20.Log.Log.WriteLog("Соединение c Evo 20 установлено");
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
                Evo20.Log.Log.WriteLog("Соединение c Evo 20 приостановлено");
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
                Evo20.Log.Log.WriteLog("Соединение c Evo 20 востановлено");
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
            if(udpClient!=null)
                udpClient.Close();
            Evo20.Log.Log.WriteLog("Соединение c Evo 20 прервано");
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
            endPoint = new IPEndPoint(remoteIPAddress, REMOTE_PORT_NUMBER);
            try
            {
                if (connectionState == ConnectionStatus.CONNECTED)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(message);
                    udpClient.Send(bytes, bytes.Length, endPoint);
                    return true;
                }
                else
                {
                    Evo20.Log.Log.WriteLog("Сообщение " + message + " Evo 20 не доставлено, клиент не подключен");
                    return false;
                }
            }
            catch (Exception exception)
            {
                connectionState = ConnectionStatus.ERROR;
                Evo20.Log.Log.WriteLog("Сообщение " + message + " Evo 20 не доставлено " + "Возникло исключение" + exception);
                udpClient.Close();
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
                    byte[] receiveBytes = udpClient.Receive(
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
                if (EventHandlerListForException != null)
                    EventHandlerListForException(exception);
            }
            finally
            {
                udpClient.Close();
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
            catch (FormatException ex)
            {
                message = String.Empty;
            }
            return message;
        }

        #endregion
    }
}
