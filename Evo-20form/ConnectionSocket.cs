using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Evo_20_commands;

namespace Evo_20form
{
    public enum ConnectionState
    {
        CONNECTED,
        DISCONNECTED,
        PAUSE,
        ERROR
    }
    class ConnectionSocket
    {
        public delegate void ConnectionHandler();

        protected event ConnectionHandler EventHandlersListForCommand;

        const int PORT_NUMBER = 531;

        const string ipAddress = "255.255.255.255";

        protected byte[] buffer;

        protected Thread work_thread;

        public ConnectionState connectionState;

        protected UdpClient receivingUdpClient;

        protected UdpClient sender;

        private static IPAddress remoteIPAddress = IPAddress.Parse(ipAddress);

        //IPEndPoint RemoteIpEndPoint;

        private static int localPort = 531;

        IPEndPoint endPoint;

        public ConnectionSocket()
        {
            buffer = new byte[2048];
            work_thread = new Thread(new ThreadStart(ReadMessage));
            connectionState = ConnectionState.DISCONNECTED;
        }

        public bool StartConnection()
        {
            if (connectionState == ConnectionState.DISCONNECTED)
            {                                
                connectionState = ConnectionState.CONNECTED;
                work_thread.Start();
                connectionState = ConnectionState.CONNECTED;
                Log.WriteLog("Соединение c Evo 20 установлено");
                return true;
            }
            else
            {
                return false;
            }
        }

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

        public bool StopConnection()
        {
            if (connectionState == ConnectionState.CONNECTED )
            {
                work_thread.Abort();
                connectionState = ConnectionState.DISCONNECTED;
                Log.WriteLog("Соединение c Evo 20 прервано");
                return true;
            }
            else
            {
                if (connectionState == ConnectionState.PAUSE)
                {
                    return true;
                }
                return false;
            }
        }

        ~ConnectionSocket()
        {
            if (connectionState != ConnectionState.CONNECTED)
            {
                work_thread.Abort();
                //work_thread.Join();
            }
        }

        public bool SendMessage(string message)
        {
            sender = new UdpClient();
            IPEndPoint endPoint = new IPEndPoint(remoteIPAddress,PORT_NUMBER);
            try
            {
                if (connectionState == ConnectionState.CONNECTED)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(message);
                    sender.Send(bytes, bytes.Length, endPoint);
                    Log.WriteLog("Отправлено сообщение Evo 20 "+message);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                connectionState = ConnectionState.ERROR;
                Log.WriteLog("Сообщение "+ message +" Evo 20 не доставлено " + "Возникло исключение" +ex);
                return false;
            }
            finally
            {
                sender.Close();
            }    
        }

        protected void ReadMessage()
        {
            UdpClient receivingUdpClient = new UdpClient(localPort);

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
                    EventHandlersListForCommand();
                    string message = Encoding.UTF8.GetString(buffer);
                    Log.WriteLog("Получено сообщение " + message.ToString());
                }              
            }
            catch (ThreadAbortException)
            {
                Log.WriteLog("Поток чтения данных с Evo прерван");
            }
        }

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

        public void Disconnect()
        {
            connectionState = ConnectionState.DISCONNECTED;
            work_thread.Abort(); 
            work_thread.Join();                  
        }    
    }
}
