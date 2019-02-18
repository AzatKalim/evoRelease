using System.Collections.Generic;
using Evo20.Evo20.Packets;
using Evo20;
using System.Threading;

namespace Evo20.SensorsConnection
{
    public delegate void DataHandler();

    /// <summary>
    /// Класс создающий из массива байт пакеты, обертка класса SensorConnection
    /// </summary>
    public class SensorHandler : SensorConnection
    {
        public int goodPackets = 0;
        public int dropedPackets = 0;
        //список полученых пакетов 
        List<Packet> bufferPacket;
        //буфер полученных байт
        List<byte> bufferMessage;
        public event DataHandler EventHandlersListForController;

        AutoResetEvent NewBytesArrived = new AutoResetEvent(false);

        Thread newBytesHandler;

        public SensorHandler() : base()
        {
            bufferPacket = new List<Packet>();
            bufferMessage = new List<byte>();
            //подписка на событие рихода нового пакета
            EventHandlersListForPacket += NewPacketHandler;
        }

        public void HandleNewBytes()
        {
            while(true)
            {
                if (bufferMessage.Count == 0)
                    NewBytesArrived.WaitOne();
                // находим стартовые байты пакета 
                bool isBeginFinded = Packet.FindPacketBegin(ref bufferMessage);
                if (!isBeginFinded || bufferMessage.Count < Config.Instance.PACKET_SIZE)
                {
                    dropedPackets++;
                    continue;
                }
                var temp = new byte[Config.Instance.PACKET_SIZE];
                lock (bufferMessage)
                {
                    // извлекаем  байты в колличестве размера пакета в список 
                    bufferMessage.CopyTo(0, temp, 0, Config.Instance.PACKET_SIZE);
                    bufferMessage.RemoveRange(0, Config.Instance.PACKET_SIZE);
                }

                // получаем информацию из массива байт
                var recognazedPacket = Packet.FirstPacketHandle(temp);
                if (recognazedPacket == null)
                {
                    dropedPackets++;
                    continue;
                }
                // добавляем пакет в очередь обработанных
                lock (bufferPacket)
                {
                    goodPackets++;
                    bufferPacket.Add(recognazedPacket);
                }
                // если собрали колличество равно числу в сообщении 
                if (bufferPacket.Count == PacketsData.PACKETS_COUNT)
                {
                    EventHandlersListForController();
                }
            }
        }
        /// <summary>
        /// Метод собирает 4 Packet в PacketsData-контейнер хранящий 4 пакета 
        /// </summary>
        /// <returns></returns>
        public PacketsData DataHandle()
        {
            lock (bufferPacket)
            {
                // извлекаем пакеты, пока id пакета  не равно 1
                while (bufferPacket.Count > 0 && bufferPacket[0].id != 1)
                {
                    bufferPacket.RemoveAt(0);
                }
            }
            // число пакетов меньше, чем должно быть  
            if (bufferPacket.Count < PacketsData.PACKETS_COUNT)
                return null;
            return PacketsData.CollectPackages(ref bufferPacket);
        }

        /// <summary>
        /// Обработчик прихода нового пакета 
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
            NewBytesArrived.Set();
        }

        public override bool StopConnection()
        {
            if (newBytesHandler.IsAlive)
            {
                newBytesHandler.Abort();
            }
            return base.StopConnection();
        }
        public override bool StartConnection(string portName)
        {
            var result = base.StartConnection(portName);
            if(result)
            {
                newBytesHandler = new Thread(HandleNewBytes);
                newBytesHandler.Start();
            }
            return result;
        }
    }
}

