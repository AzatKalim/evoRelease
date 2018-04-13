using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace Evo_20form
{
    public delegate void DataHandler();

    class SensorHandler:SensorConnection 
    {
        //Константы 
        const int PACKETS_COUNT_IN_MESSAGE = 4;
        //позиция служебных  байт в пакете
        const int HEAD_BEGIN = 0;
        const int ID_BEGIN = 38;
        const int CHECK_BEGIN = 40;
        //позиция информационных байт в пакете
        const int W_X_BEGIN = 2;
        const int W_Y_BEGIN = 6;
        const int W_Z_BEGIN = 10;

        const int A_X_BEGIN = 14;
        const int A_Y_BEGIN = 18;
        const int A_Z_BEGIN = 22;

        const int U_X_BEGIN = 26;
        const int U_Y_BEGIN = 30; 
        const int U_Z_BEGIN = 34;


        Queue<Packet> bufferPacket;

        List<byte>  bufferMessage;

        StringBuilder sendBuffer;

        public ConnectionState connectionState;

        public event DataHandler EventHandlersListForController;

        public SensorHandler()
        {
            readThread = new Thread(Read);
            serialPort = new SerialPort();
            serialPort.BaudRate = 9600;
            serialPort.ReadTimeout = 100000;
            serialPort.WriteTimeout = 500;            
            _continue = true;
            bufferMessage = new List<byte>();
            bytesBuffer = new byte[BUFFER_SIZE];
            EventHandlersListForPacket += NewPacketHandler;
            connectionState = ConnectionState.DISCONNECTED;           
        }

        public bool StartConnection(string portName)
        {
            if (!serialPort.IsOpen)
            {
                serialPort.PortName = portName;
            }
            if (connectionState == ConnectionState.DISCONNECTED)
            {
                if (!serialPort.IsOpen)
                {
                    serialPort.Open();
                }
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
            if (connectionState !=ConnectionState.DISCONNECTED)
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
            else
            {
                return false;
            }           
        }

        public PacketsData DataHandle()
        {
            List<Packet> packets = new List<Packet>();
            lock (bufferPacket)
            {
                packets.AddRange(bufferPacket);
                bufferPacket.Clear();
            }
            while (packets.Count > 0 && packets[0].id != 1)
            {
                packets.RemoveAt(0);
            }
            if (packets.Count == 0)
            {
                return null;
            }
            return CollectPackages(ref packets);
        }

        private PacketsData CollectPackages(ref List<Packet> packets)
        {
            PacketsData data = null;
            if (packets== null || packets.Count==0 || packets[0]==null)
            {
                return null;
            }
            for (int i = 1; i < PACKETS_COUNT_IN_MESSAGE && packets.Count>i; i++)
            {
                if (packets[i].id != i)
                {
                    for (int j = 0; j < i; j++)
                    {
                        packets.RemoveAt(j);
                    }
                    i = 0;
                }
            }
            if (packets.Count < PACKETS_COUNT_IN_MESSAGE)
            {
                return null;
            }
            return data;
        }

        public Packet PacketHandle(byte[] message)
        {
            try
            {
                Packet packet_inf = null;
                //проверка заголовка пакета
                ushort head = BitConverter.ToUInt16(new byte[] { message[HEAD_BEGIN], message[HEAD_BEGIN + 1] }, 0);
                if (head != 0xFACE)
                {
                    return packet_inf;
                }               
                Byte[] inf_part = new byte[40];
                Array.Copy(message, 2, inf_part, 0, PACKET_SIZE - 2);
                int checkSum = CRC16.crc16(inf_part);
                //проверка контрольной суммы
                Console.WriteLine("контрольня сумма " + BitConverter.ToUInt16(new byte[2] { message[CHECK_BEGIN], message[CHECK_BEGIN + 1] }, 0));
                Console.WriteLine( "расчитаная контр сумма "+checkSum);
                if (BitConverter.ToUInt16(new byte[2] { message[CHECK_BEGIN], message[CHECK_BEGIN + 1] }, 0) != checkSum)
                {
                    Console.WriteLine("Контрольная сумма не верна");
                    return packet_inf;
                }
                //номер пакета 
                int packetId = BitConverter.ToInt32(new byte[] { message[ID_BEGIN], message[ID_BEGIN + 1] }, 0);
                Console.WriteLine("id" + packetId);
                //данные с гироскопов
                int w_x = BitConverter.ToInt32(new byte[] { message[W_X_BEGIN], message[W_X_BEGIN + 1], message[W_X_BEGIN + 2], message[W_X_BEGIN + 3] }, 0);
                int w_y = BitConverter.ToInt32(new byte[] { message[W_Y_BEGIN], message[W_Y_BEGIN + 1], message[W_Y_BEGIN + 2], message[W_Y_BEGIN + 3] }, 0);
                int w_z = BitConverter.ToInt32(new byte[] { message[W_Z_BEGIN], message[W_Z_BEGIN + 1], message[W_Z_BEGIN + 2], message[W_Z_BEGIN + 3] }, 0);
                int[] w = new int[] { w_x, w_y, w_z };
                Console.WriteLine(w_x + " " + w_y + " " + w_z);
                // данные с акселерометров
                int a_x = BitConverter.ToInt32(new byte[] { message[A_X_BEGIN], message[A_X_BEGIN + 1], message[A_X_BEGIN + 2], message[A_X_BEGIN + 3] }, 0);
                int a_y = BitConverter.ToInt32(new byte[] { message[A_Y_BEGIN], message[A_Y_BEGIN + 1], message[A_Y_BEGIN + 2], message[A_Y_BEGIN + 3] }, 0);
                int a_z = BitConverter.ToInt32(new byte[] { message[A_Z_BEGIN], message[A_Z_BEGIN + 1], message[A_Z_BEGIN + 2], message[A_Z_BEGIN + 3] }, 0);
                int[] a = new int[] { a_x, a_y, a_z };
                Console.WriteLine(a_x + " " + a_y + " " + a_z);
                //данные о температуре
                if (packetId < 2)
                {
                    int u_x = BitConverter.ToInt32(new byte[] { message[U_X_BEGIN], message[U_X_BEGIN + 1], message[U_X_BEGIN + 2], message[U_X_BEGIN + 3] }, 0);
                    int u_y = BitConverter.ToInt32(new byte[] { message[U_Y_BEGIN], message[U_Y_BEGIN + 1], message[U_Y_BEGIN + 2], message[U_Y_BEGIN + 3] }, 0);
                    int u_z = BitConverter.ToInt32(new byte[] { message[U_Z_BEGIN], message[U_Z_BEGIN + 1], message[U_Z_BEGIN + 2], message[U_Z_BEGIN + 3] }, 0);
                    int[] u = new int[] { u_x, u_y, u_z };
                    Console.WriteLine(u_x + " " + u_y + " " + u_z);
                    packet_inf = new Packet(w, a, u, packetId);
                    Console.WriteLine(packet_inf);
                }             
                return packet_inf;
            }
            catch (Exception)
            {
                Log.WriteLog("Ошибка обработки пакета");
            }
            return null;
        }

        public void NewPacketHandler()
        {
            lock(bufferMessage)
            {
                bufferMessage.AddRange(ReadBuffer());
            }
            if (bufferMessage.Count != 0)
            {
                List<byte> temp = new List<byte>();
                lock (bufferMessage)
                {
                    bool isBeginFinded=FindPacketBegin(ref bufferMessage);

                    if (!isBeginFinded && bufferMessage.Count < PACKET_SIZE)
                    {
                        return;
                    }
                    for (int i = 0; i < PACKET_SIZE; i++)
                    {
                        temp[i] = bufferMessage[i];
                    }
                    bufferMessage.RemoveRange(0, PACKET_SIZE);
                }
                Packet recognazedPacket = PacketHandle(temp.ToArray());
                if (recognazedPacket == null)
                {
                    return;
                }
                lock (bufferPacket)
                {
                    bufferPacket.Enqueue(recognazedPacket);
                }
                if (bufferPacket.Count == PACKETS_COUNT_IN_MESSAGE)
                {
                    EventHandlersListForController();
                }
            }          
        }

        static bool FindPacketBegin(ref List<byte> buffer)
        {
            if (buffer.Count < 2)
            {
                return false;
            }
            byte first = buffer[0];
            byte second = buffer[1];
            int i = 2;
            while (BitConverter.ToUInt16(new byte[] { first, second }, 0) != 0xFACE && buffer.Count > i)
            {
                first = second;
                second = buffer[i];
                i++;
            }
            if (i == buffer.Count)
            {
                buffer = new List<byte>();
                //Начало пакета не найдено
                return false;
            }
            else
            {
                i -= 2;
                buffer.RemoveRange(0, i);
                return true;
            }
        }

    }
}
