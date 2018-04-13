using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Evo_20form
{
    /// <summary>
    /// Состояния соединения 
    /// </summary>
    public enum ConnectionState
    {
        CONNECTED,
        DISCONNECTED,
        PAUSE,
        ERROR
    }

    /// <summary>
    /// Класс для работы с Evo-20 через udp 
    /// </summary>
    class ConnectionSocket
    {
        //делегат для события 
        public delegate void ConnectionHandler();

        /// <summary>
        /// События прихода нового сообщения 
        /// </summary>
        protected event ConnectionHandler EventHandlersListForCommand;

        //номер порта приходящих сообщений 
        const int LOCAL_PORT_NUMBER = 1068;
        //номер удаленного порта 
        const int REMOTE_PORT_NUMBER = 531;
    
        protected byte[] buffer;

        protected Thread work_thread;

        public ConnectionState connectionState;

        protected UdpClient sender;

        IPEndPoint endPoint;

        //ip адресс удаленного компьютера 
        private static IPAddress remoteIPAddress = IPAddress.Parse("192.168.0.1");

        public ConnectionSocket()
        {
            buffer = new byte[2048];
            work_thread = new Thread(ReadMessage);
            work_thread.IsBackground = true;
            connectionState = ConnectionState.DISCONNECTED;
        }
        /// <summary>
        /// Запуск соединения 
        /// </summary>
        /// <returns> результат запуска </returns>
        public bool StartConnection()
        {
            if (connectionState == ConnectionState.DISCONNECTED)
            {                                
                connectionState = ConnectionState.CONNECTED;
                if (!work_thread.IsAlive)
                {
                    work_thread.Start();
                }
                Log.WriteLog("Соединение c Evo 20 установлено");
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
            if (connectionState == ConnectionState.CONNECTED)
            {
                work_thread.Abort();
                connectionState = ConnectionState.PAUSE;
                Log.WriteLog("Соединение c Evo 20 приостановлено");
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
            if (connectionState == ConnectionState.PAUSE)
            {
                work_thread.Start();
                connectionState = ConnectionState.CONNECTED;
                Log.WriteLog("Соединение c Evo 20 востановлено");
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
            connectionState = ConnectionState.DISCONNECTED;
            Log.WriteLog("Соединение c Evo 20 прервано");
            return true;
        }

        //Деструктор класса
        ~ConnectionSocket()
        {
            if (connectionState != ConnectionState.CONNECTED)
            {
                work_thread.Abort();
            }
        }

        /// <summary>
        /// Отправка сообщения по udp протоколу
        /// </summary>
        /// <param name="message"> сообщение </param>
        /// <returns>результат </returns>
        public bool SendMessage(string message)
        {
            sender = new UdpClient();
            endPoint = new IPEndPoint(remoteIPAddress, REMOTE_PORT_NUMBER);
            try
            {
                if (connectionState == ConnectionState.CONNECTED)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(message);
                    sender.Send(bytes, bytes.Length,endPoint);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                connectionState = ConnectionState.ERROR;
                Log.WriteLog("Сообщение " + message + " Evo 20 не доставлено " + "Возникло исключение" + ex);
                sender.Close();
                return false;
            }
            finally
            {
                sender.Close();
            }
        }

        /// <summary>
        /// Бесконечное считывание приходящих сообщений изапись их в буффер ( выполняется в обдельном потоке)
        /// </summary>
        protected void ReadMessage()
        {
            UdpClient receivingUdpClient=null;
            try
            {
                receivingUdpClient = new UdpClient(LOCAL_PORT_NUMBER);
            }
            catch(Exception ex)
            {
                connectionState = ConnectionState.ERROR;
                Log.WriteLog("Невозможно открыть соединение с Evo " + "Возникло исключение" + ex);
                return;
            }

            IPEndPoint RemoteIpEndPoint = null;

            try
            {
                while (connectionState == ConnectionState.CONNECTED)
                {
                    byte[] receiveBytes = receivingUdpClient.Receive(
                       ref RemoteIpEndPoint);

                    lock (buffer)
                    {
                        receiveBytes.CopyTo(buffer, 0);
                    }
                    string message = Encoding.UTF8.GetString(buffer);
                    Log.WriteLog("Получено сообщение " + message.ToString());
                    EventHandlersListForCommand();
                }
            }
            finally
            {
                receivingUdpClient.Close();
            }
        }

        /// <summary>
        /// Метод возвращает значение хранящееся в буффере 
        /// </summary>
        /// <returns></returns>
        public string ReadBuffer()
        {
            StringBuilder message = new StringBuilder();
            lock (buffer)
            {
                message.Append(Encoding.UTF8.GetString(buffer, 0, buffer.Length));
                buffer=new byte[2048];
            }
            return message.ToString();
        }  
    }
}
