using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Collections;
using PacketsLib;

namespace Evo_20form
{
    public delegate void DataHandler();

    /// <summary>
    /// Класс создающий из массива байт пакеты
    /// </summary>
    class SensorHandler : SensorConnection
    {
        public int goodPackets = 0;
        public int dropedPackets = 0;
        //очередь полученныъ пакетов 
        Queue<Packet> bufferPacket;

        List<byte> bufferMessage;
        public event DataHandler EventHandlersListForController;

        public SensorHandler()
            : base()
        {
            bufferPacket = new Queue<Packet>();
            bufferMessage = new List<byte>();
            EventHandlersListForPacket += NewPacketHandler;
        }

        /// <summary>
        /// Метод собирает 4 Packet в PacketsData-контейнер хранящий 4 пакета 
        /// </summary>
        /// <returns></returns>
        public PacketsData DataHandle()
        {
            List<Packet> packets = new List<Packet>();
            lock (bufferPacket)
            {
                // извлекаем пакеты, пока id пакета  не равно 1
                while (bufferPacket.Count > 0 && bufferPacket.Peek().id != 1)
                {
                    bufferPacket.Dequeue();
                }
                // число пакетов меньше, чем должно быть  
                if (bufferPacket.Count < PacketsData.PACKETS_COUNT)
                    return null;
                // извлечение пакетов 
                for (int i = 0; i < PacketsData.PACKETS_COUNT; i++)
                {
                    Packet temp = bufferPacket.Dequeue();
                    packets.Add(temp);
                }
            }

            if (packets.Count == 0)
            {
                return null;
            }
            return PacketsData.CollectPackages(ref packets);
        }

        /// <summary>
        /// Обработчик нового пакета 
        /// </summary>
        public void NewPacketHandler()
        {
            lock (bufferMessage)
            {
                // извлекаем полученные байты 
                bufferMessage.AddRange(ReadBuffer());
            }
            //если полученно 0 байт
            if (bufferMessage.Count == 0)
            {
                return;
            }
            List<byte> temp = new List<byte>();
            lock (bufferMessage)
            {
                // находим стартовые байты пакета 
                bool isBeginFinded = Packet.FindPacketBegin(ref bufferMessage);
                if (!isBeginFinded || bufferMessage.Count < Packet.PACKET_SIZE)
                {
                    dropedPackets++;
                    return;
                }
                // извлекаем  байты в колличестве размера пакета в список 
                for (int i = 0; i < Packet.PACKET_SIZE; i++)
                {
                    temp.Add(bufferMessage[i]);
                }
                bufferMessage.RemoveRange(0, Packet.PACKET_SIZE);
            }
            // получаем информацию из массива байт
            Packet recognazedPacket = Packet.PacketHandle(temp.ToArray());
            if (recognazedPacket == null)
            {
                dropedPackets++;
                return;
            }
            // добавляем пакет в очередь обработанных
            lock (bufferPacket)
            {
                goodPackets++;
                bufferPacket.Enqueue(recognazedPacket);
            }
            // если собрали колличество равно числу в сообщении 
            if (bufferPacket.Count == PacketsData.PACKETS_COUNT)
            {
                EventHandlersListForController();
            }
        }
    }
}

