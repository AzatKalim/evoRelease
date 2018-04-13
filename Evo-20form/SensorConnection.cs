using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace Evo_20form
{
    class SensorConnection
    {
        public const int BUFFER_SIZE = 1024;
        public const int PACKET_SIZE = 42;

        public ConnectionState connectionState;
        
        public delegate void SensorHandler();

        protected event SensorHandler EventHandlersListForPacket;

        protected bool _continue;

        protected SerialPort serialPort;

        protected Thread readThread;

        public byte[] bytesBuffer;

        public List<byte> packetBuffer;

        protected int bytesCount;

        public SensorConnection(String port)
        {
            readThread = new Thread(Read);
            serialPort = new SerialPort();
            serialPort.PortName = port; //Указываем наш порт - в данном случае COM1.
            serialPort.ReadTimeout = 100000;
            serialPort.WriteTimeout = 500;
            serialPort.Open();
            connectionState = ConnectionState.DISCONNECTED;
            _continue = true;
            bytesBuffer = new byte[BUFFER_SIZE];
            packetBuffer = new List<byte>();
            readThread.Start();
        }

        public SensorConnection() { }

        public void Read()
        {
            byte[] receiveBytes = new byte[PACKET_SIZE];
            while (connectionState == ConnectionState.CONNECTED)
            {
                try
                {
                    bytesCount = serialPort.Read(receiveBytes, 0, PACKET_SIZE);
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

        /*static bool FindPacketBegin(ref List<byte> buffer)
        {
            if (buffer.Count < 2)
            {
                return false;
            }
            byte first = buffer[0];
            byte second = buffer[1];
            int i = 2;
            while (BitConverter.ToUInt16(new byte[] { first, second }, 0) != 0xFACE && buffer.Count>i)
            {
                first = second;
                second =buffer[i];
                i++;
            }
            if (i == buffer.Count)
            {
                buffer= new List<byte>();
                //Начало пакета не найдено
                return false;
            }
            else
            {
                buffer.RemoveRange(0, i);
                return true;
            }
        }
        */
        public List<byte> ReadBuffer()
        {
            List<byte> message = new List<byte>();
            lock (bytesBuffer)
            {
                for (int i = 0; i < bytesCount; i++)
                {
                    message.Add(bytesBuffer[i]);
                }
                bytesCount = 0;
            }
            return message;
        }

        ~SensorConnection()
        {
            if (readThread.IsAlive)
            {
                readThread.Abort();
                readThread.Join();
            }
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }         
        }
    }
}
