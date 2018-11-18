using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Evo20.Evo20.Packets
{
    /// <summary>
    /// Класс хранящий 4 пакета
    /// </summary>
    [Serializable]
    public class PacketsData
    {
        #region Constants
        //число осей 
        [NonSerialized]
        const int AXIS_COUNT=3;
        //число пакетов
        [NonSerialized]
        public const int PACKETS_COUNT = 4;

        #endregion

        #region Constructors

        public PacketsData(StreamReader file)
        {
            var packets = new List<Packet>();
            try
            {
                for (int i = 0; i < PACKETS_COUNT; i++)
                {
                    var packetLine = file.ReadLine();
                    var packet = new Packet(packetLine);
                }
            }
            catch (FormatException exception)
            {
                packets = null;
                throw exception;
            }
        }

        public PacketsData(ref List<Packet> newPackets)
        {
            var packetsList=new List<Packet>();
            for (int i = 0; i < PACKETS_COUNT; i++)
            {
                packetsList.Add(newPackets[i]);
            }
            lock (newPackets)
            {
                newPackets.RemoveRange(0, PACKETS_COUNT);
            }
            this.packets = packetsList;
            packets[2].U = packets[0].U;
            packets[3].U = packets[1].U;
        }
        #endregion

        #region Public properties
        //список пакетов 
        public List<Packet> packets
        {
            get;
            private set;
        }
        //среднее значение по гироскопам
        public double[] MeanW
        {
            get
            {
                var w = new double[AXIS_COUNT];
                double sum = 0;
                for (int i = 0; i < w.Length; i++)
                {
                    for (int j = 0; j < packets.Count; j++)
                    {
                        sum += packets[j].W[i];
                    }                    
                    w[i] += sum / packets.Count;
                    sum = 0;
                }
                return w;
            }
        }
        //среднее значение по аксерометрам

        public double[] MeanA
        {
            get
            {
                var a = new double[AXIS_COUNT];
                double sum = 0;
                for (int i = 0; i < a.Length; i++)
                {
                    
                    for (int j = 0; j < packets.Count; j++)
                    {
                        sum += packets[j].A[i];
                    }
                    a[i] += sum / packets.Count;
                    sum = 0;
                }
                return a;
            }
        }
        //среднее значение 
        public double[] MeanUW
        {
            get
            {
                var uw = new double[AXIS_COUNT];
                double sum = 0;
                for (int i = 0; i < uw.Length; i++)
                {                   
                    sum += packets[0].U[i];
                    sum += packets[2].U[i];
                    uw[i] += sum / 2;
                    sum = 0;
                }
                return uw;
            }
        }

        public double[] MeanUA
        {
            get
            {
                var ua = new double[AXIS_COUNT];
                double sum = 0;
                for (int i = 0; i < ua.Length; i++)
                {                    
                    sum += packets[1].U[i];
                    sum += packets[3].U[i];
                    ua[i] += sum / 2;
                    sum = 0;
                }
                return ua;
            }
        }

        public Packet this[int index]
        {
            get 
            {
                return packets[index];
            }
        }

        public int Length
        {
            get
            {
                return packets.Count;
            }
        }

        #endregion  

        public override string ToString()
        {
            var buffer = new StringBuilder();
            for (int i = 0; i < packets.Count; i++)
            {
                buffer.Append(packets[i].ToString() + Environment.NewLine);
            }
            return buffer.ToString();
        }

        /// <summary>
        /// Метод сбора пакетов в PacketsData
        /// </summary>
        /// <param name="packets">список пакетов </param>
        /// <returns></returns>
        public static PacketsData CollectPackages(ref List<Packet> packets)
        {
            if (packets == null || packets.Count == 0)
                return null;
            // проверка id пакетов 
            for (int i = 0; i < PACKETS_COUNT && packets.Count > i; i++)
            {
                if (packets[i].id != i + 1)
                {
                    for (int j = 0; j < i && j<packets.Count; j++)
                    {
                        lock (packets)
                        {
                            packets.RemoveAt(j);
                        }
                    }
                    i = 0;
                }
            }
            if (packets.Count < PacketsData.PACKETS_COUNT)
                return null;

            return new PacketsData(ref packets);
        }
    }
}
