using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Evo20.Utils;

namespace Evo20.Packets
{
    public class PacketsData
    {
        #region Constants
        //число осей 
        const int AxisCount = 3;
        //число пакетов
        public const int PacketsCount = 4;

        #endregion

        public PacketsData(StreamReader file)
        {
            Packets= new List<Packet>();
            try
            {
                for (var i = 0; i < PacketsCount; i++)
                {
                    var packetLine = file.ReadLine();
                    Packets.Add(new Packet(packetLine));
                }
            }
            catch (FormatException ex)
            {
                Packets = null;
                Log.Instance.Warning($"Format ex {ex}");
                throw;
            }
        }

        private PacketsData(ref List<Packet> newPackets)
        {
            var packetsList=new List<Packet>();
            for (int i = 0; i < PacketsCount; i++)
            {
                packetsList.Add(newPackets[i]);
            }
            lock (newPackets)
            {
                newPackets.RemoveRange(0, PacketsCount);
            }
            Packets = packetsList;
            Packets[2].U = Packets[0].U;
            Packets[3].U = Packets[1].U;
        }

        #region Public properties

        private List<Packet> Packets
        {
            get;
        }
        private double[] _meanW;
        public double[] MeanW
        {
            get
            {
                if (_meanW != null)
                    return _meanW;
                _meanW = new double[AxisCount];
                double sum = 0;
                for (var i = 0; i < _meanW.Length; i++)
                {
                    var goodPacketsCount = 0;
                    foreach (var packet in Packets)
                    {
                        if (packet.BadPacket) continue;
                        sum += packet.W[i];
                        goodPacketsCount++;
                    }

                    _meanW[i] += sum / goodPacketsCount;
                    sum = 0;
                }
                return _meanW;
            }
        }

        private double[] _meanA;
        public double[] MeanA
        {
            get
            {
                if (_meanA == null)
                {
                    _meanA = new double[AxisCount];
                    double sum = 0;
                    for (int i = 0; i < _meanA.Length; i++)
                    {
                        var goodPacketsCount = 0;
                        foreach (var packet in Packets)
                        {
                            if (packet.BadPacket) continue;
                            sum += packet.A[i];
                            goodPacketsCount++;
                        }
                        _meanA[i] += sum / goodPacketsCount;
                        sum = 0;
                    }
                }
                return _meanA;
            }
        }

        private double[] _meanUw;

        public double[] MeanUw
        {
            get
            {
                if (_meanUw != null) return _meanUw;
                _meanUw = new double[AxisCount];
                double sum = 0;
                for (var i = 0; i < _meanUw.Length; i++)
                {
                    var count = 0;
                    if (!Packets[0].BadPacket)
                    {
                        sum += Packets[0].U[i];
                        count++;
                    }
                    if (!Packets[2].BadPacket)
                    {
                        sum += Packets[2].U[i];
                        count++;
                    }
                    _meanUw[i] += sum / count;
                    sum = 0;
                }
                return _meanUw;
            }
        }

        private double [] _meanUa;

        public double[] MeanUa
        {
            get
            {
                if (_meanUa != null) return _meanUa;
                _meanUa = new double[AxisCount];
                double sum = 0;
                for (var i = 0; i < _meanUw.Length; i++)
                {
                    var count = 0;
                    if (!Packets[1].BadPacket)
                    {
                        sum += Packets[1].U[i];
                        count++;
                    }
                    if (!Packets[3].BadPacket)
                    {
                        sum += Packets[3].U[i];
                        count++;
                    }
                    _meanUa[i] += sum / count;
                    sum = 0;
                }
                return _meanUa;
            }
        }

        public Packet this[int index] => Packets[index];

        #endregion  

        public override string ToString()
        {
            var buffer = new StringBuilder();
            for (var i = 0; i < Packets.Count-1; i++)
            {
                buffer.Append(Packets[i] + Environment.NewLine);
            }
            buffer.Append(Packets[Packets.Count - 1]);           
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
            for (var i = 0; i < PacketsCount && packets.Count > i; i++)
            {
                if (packets[i].Id == i + 1) continue;
                for (var j = 0; j < i && j<packets.Count; j++)
                {
                    lock (packets)
                    {
                        packets.RemoveAt(j);
                    }
                }
                i = 0;
            }
            return packets.Count < PacketsCount ? null : new PacketsData(ref packets);
        }
    }
}
