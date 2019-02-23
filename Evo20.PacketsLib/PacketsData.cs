using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
// ReSharper disable InconsistentlySynchronizedField

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

        #region Constructors

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
            catch (FormatException)
            {
                Packets = null;
                throw;
            }
        }

        public PacketsData(ref List<Packet> newPackets)
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
        #endregion

        #region Public properties

        public List<Packet> Packets
        {
            get;
        }
        private double[] _meanW;
        public double[] MeanW
        {
            get
            {
                if (_meanW == null)
                {
                    _meanW = new double[AxisCount];
                    double sum = 0;
                    for (int i = 0; i < _meanW.Length; i++)
                    {
                        foreach (var packet in Packets)
                        {
                            sum += packet.W[i];
                        }
                        _meanW[i] += sum / Packets.Count;
                        sum = 0;
                    }
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

                        foreach (var packet in Packets)
                        {
                            sum += packet.A[i];
                        }
                        _meanA[i] += sum / Packets.Count;
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
                    sum += Packets[0].U[i];
                    sum += Packets[2].U[i];
                    _meanUw[i] += sum / 2;
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
                for (int i = 0; i < _meanUa.Length; i++)
                {
                    sum += Packets[1].U[i];
                    sum += Packets[3].U[i];
                    _meanUa[i] += sum / 2;
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
            foreach (var packet in Packets)
            {
                buffer.Append(packet + Environment.NewLine);
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
            for (int i = 0; i < PacketsCount && packets.Count > i; i++)
            {
                if (packets[i].Id != i + 1)
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
            if (packets.Count < PacketsCount)
                return null;

            return new PacketsData(ref packets);
        }
    }
}
