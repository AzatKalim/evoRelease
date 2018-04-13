using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PacketsLib
{
    public class PacketsData : IPacketData
    {
        const int AXIS_COUNT=3;
        public const int PACKETS_COUNT = 4;

        public PacketsData(StreamReader file)
        {
            List<Packet> packets = new List<Packet>();
            try
            {
                for (int i = 0; i < PACKETS_COUNT; i++)
                {
                    string packetLine = file.ReadLine();
                    Packet packet = new Packet(packetLine);
                }
            }
            catch (FormatException exception)
            {
                packets = null;
                throw exception;
            }
        }

        public List<Packet> packets
        {
            get;
            private set;
        }

        public double[] MeanW
        {
            get
            {
                double[] w = new double[AXIS_COUNT];
                for (int i = 0; i < w.Length; i++)
                {
                    double sum = 0;
                    sum += FirstPacket.w[i];
                    sum += SecondPacket.w[i];
                    sum += ThirdPacket.w[i];
                    sum += FourthPacket.w[i];
                    w[i] += sum / packets.Count;
                }
                return w;
            }
        }

        public double[] MeanA
        {
            get
            {
                double[] a = new double[AXIS_COUNT];
                for (int i = 0; i < a.Length; i++)
                {
                    double sum = 0;
                    sum += FirstPacket.a[i];
                    sum += SecondPacket.a[i];
                    sum += ThirdPacket.a[i];
                    sum += FourthPacket.a[i];
                    a[i] += sum / packets.Count;
                }
                return a;
            }
        }

        public double[] MeanUW
        {
            get
            {
                double[] uw = new double[AXIS_COUNT];
                for (int i = 0; i < uw.Length; i++)
                {
                    double sum = 0;
                    sum += FirstPacket.u[i];
                    sum += ThirdPacket.u[i];
                    uw[i] += sum / 2;
                }
                return uw;
            }
        }

        public double[] MeanUA
        {
            get
            {
                double[] ua = new double[AXIS_COUNT];
                for (int i = 0; i < ua.Length; i++)
                {
                    double sum = 0;
                    sum += SecondPacket.u[i];
                    sum += FourthPacket.u[i];
                    ua[i] += sum / 2;
                }
                return ua;
            }
        }

        public Packet FirstPacket
        {
            get
            {
                return packets[0];
            }
        }

        public Packet SecondPacket
        {
            get
            {
                return packets[1];
            }
        }

        public Packet ThirdPacket
        {
            get
            {
                return packets[2];
            }
        }

        public Packet FourthPacket
        {
            get
            {
                return packets[3];
            }
        }

        public int Length
        {
            get
            {
                return packets.Count;
            }
        }

        public PacketsData(List<Packet> packets)
        {
            this.packets = packets;
            packets[2].u = packets[0].u;
            packets[3].u = packets[1].u;
        }

        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder();
            for (int i = 0; i < packets.Count; i++)
            {
                buffer.Append(packets[i].ToString() + Environment.NewLine);
            }
            return buffer.ToString();
        }

        /// <summary>
        /// Метод сбора пакетов в PacketsData
        /// </summary>
        /// <param name="packets"></param>
        /// <returns></returns>
        public static PacketsData CollectPackages(ref List<Packet> packets)
        {
            PacketsData data = null;
            if (packets == null || packets.Count == 0 || packets[0] == null)
            {
                return null;
            }
            // проверка id пакетов 
            for (int i = 0; i < PacketsData.PACKETS_COUNT && packets.Count > i; i++)
            {
                if (packets[i].id != i + 1)
                {
                    for (int j = 0; j < i; j++)
                    {
                        packets.RemoveAt(j);
                    }
                    i = 0;
                }
            }
            if (packets.Count != PacketsData.PACKETS_COUNT)
            {
                return null;
            }
            return data = new PacketsData(packets);
        }
    }
}
