using System.Collections.Generic;
using System;
#if DEBUG
    using System.Threading;
#endif
using Evo20.Packets;
using Evo20.Utils;

namespace Evo20.SensorsConnection
{
    public delegate void DataHandler(object sender,EventArgs e);

#if !DEBUG
    public class SensorHandler : SensorConnection
    {
        public long GoodPackets;

        //список полученых пакетов 
        List<Packet> _bufferPacket;
        //буфер полученных байт
        List<byte> _bufferMessage;

        public long DropedPackets { get; set; }

        public event DataHandler PacketDataCollected;

        public SensorHandler()
        {
            _bufferPacket = new List<Packet>();
            _bufferMessage = new List<byte>();
            EventHandlersListForPacket += NewPacketHandler;
        }

        /// <summary>
        /// Метод собирает 4 Packet в PacketsData-контейнер хранящий 4 пакета 
        /// </summary>
        /// <returns></returns>
        public PacketsData DataHandle()
        {
            lock (_bufferPacket)
            {
                while (_bufferPacket.Count > 0 && _bufferPacket[0].Id != 1)
                {
                    _bufferPacket.RemoveAt(0);
                }
            }
            // число пакетов меньше, чем должно быть  
            if (_bufferPacket.Count < PacketsData.PacketsCount)
                return null;
            return PacketsData.CollectPackages(ref _bufferPacket);
        }

        /// <summary>
        /// Обработчик прихода нового пакета 
        /// </summary>
        public void NewPacketHandler(object sender, EventArgs e)
        {
            lock (_bufferMessage)
            {
                // извлекаем полученные байты 
                _bufferMessage.AddRange(ReadBuffer());
                if (_bufferMessage.Count == 0)
                {
                    return;
                }
            }         
            // ReSharper disable once InconsistentlySynchronizedField
            bool isBeginFinded = Packet.FindPacketBegin(buffer: ref _bufferMessage);
            // ReSharper disable once InconsistentlySynchronizedField
            if (!isBeginFinded || _bufferMessage.Count < Config.Instance.PacketSize)
            {
                DropedPackets++;
                return;
            }
            var temp = new byte[Config.Instance.PacketSize];
            lock (_bufferMessage)
            {
                // извлекаем  байты в колличестве размера пакета в список 
                _bufferMessage.CopyTo(0, temp, 0, Config.Instance.PacketSize);
                _bufferMessage.RemoveRange(0, Config.Instance.PacketSize);
            }

            // получаем информацию из массива байт
            var recognazedPacket = Packet.FirstPacketHandle(temp);
            if (recognazedPacket == null)
            {
                DropedPackets++;
                return;
            }
            // добавляем пакет в очередь обработанных
            lock (_bufferPacket)
            {
                GoodPackets++;
                _bufferPacket.Add(recognazedPacket);
            }
            // если собрали колличество равно числу в сообщении 
            if (_bufferPacket.Count == PacketsData.PacketsCount)
            {
                PacketDataCollected?.Invoke(this, null);
            }
        }
    }
#else
    /// <summary>
    /// Класс создающий из массива байт пакеты, обертка класса SensorConnection
    /// </summary>
    public class SensorHandler : SensorConnection
    {
        public int GoodPackets;
        public int DropedPackets;
        //список полученых пакетов 
        List<Packet> _bufferPacket;
        //буфер полученных байт
        List<byte> _bufferMessage;
        public event DataHandler PacketDataCollected;

        readonly AutoResetEvent NewBytesArrived = new AutoResetEvent(false);

        Thread _newBytesHandler;

        public SensorHandler()
        {
            _bufferPacket = new List<Packet>();
            _bufferMessage = new List<byte>();
            //подписка на событие прихода нового пакета
            EventHandlersListForPacket += NewPacketHandler;
        }

        public void HandleNewBytes()
        {
            while(true)
            {
                lock (_bufferMessage)
                {
                    if (_bufferMessage.Count == 0)
                        NewBytesArrived.WaitOne();
                }
                // находим стартовые байты пакета 
                // ReSharper disable once InconsistentlySynchronizedField
                bool isBeginFinded = Packet.FindPacketBegin(ref _bufferMessage);
                if (!isBeginFinded || _bufferMessage.Count < Config.Instance.PacketSize)
                {
                    DropedPackets++;
                    continue;
                }
                var temp = new byte[Config.Instance.PacketSize];
                lock (_bufferMessage)
                {
                    // извлекаем  байты в колличестве размера пакета в список 
                    _bufferMessage.CopyTo(0, temp, 0, Config.Instance.PacketSize);
                    _bufferMessage.RemoveRange(0, Config.Instance.PacketSize);
                }

                var recognazedPacket = Packet.FirstPacketHandle(temp);
                if (recognazedPacket == null)
                {
                    DropedPackets++;
                    continue;
                }
                lock (_bufferPacket)
                {
                    GoodPackets++;
                    _bufferPacket.Add(recognazedPacket);
                }

                if (_bufferPacket.Count == PacketsData.PacketsCount)
                {
                    PacketDataCollected?.Invoke(this, null);
                }
            }
        }

        public PacketsData DataHandle()
        {
            lock (_bufferPacket)
            {
                while (_bufferPacket.Count > 0 && _bufferPacket[0].Id != 1)
                {
                    _bufferPacket.RemoveAt(0);
                }
            }
            // число пакетов меньше, чем должно быть  
            if (_bufferPacket.Count < PacketsData.PacketsCount)
                return null;
            return PacketsData.CollectPackages(ref _bufferPacket);
        }

        /// <summary>
        /// Обработчик прихода нового пакета 
        /// </summary>
        public void NewPacketHandler(object sender, EventArgs e)
        {
            //lock (_bufferMessage)
            //{
                // извлекаем полученные байты 
                _bufferMessage.AddRange(ReadBuffer());
                if (_bufferMessage.Count == 0)
                {
                    return;
                }
            //}           
            NewBytesArrived.Set();
        }

        public override bool StopConnection()
        {
            if (_newBytesHandler!=null && _newBytesHandler.IsAlive)
            {
                _newBytesHandler.Abort();
            }
            return base.StopConnection();
        }
        public override bool StartConnection(string portName)
        {
            var result = base.StartConnection(portName);
            if(result)
            {
                _newBytesHandler = new Thread(HandleNewBytes);
                _newBytesHandler.Start();
            }
            return result;
        }
    }
#endif
}

