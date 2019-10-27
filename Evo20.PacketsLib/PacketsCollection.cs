using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Evo20.Utils;

namespace Evo20.Packets
{
    public class PacketsCollection
    {
        private sealed class MeanParametres
        {
            public double[] MeanW;
            public double[] MeanA;
            public double[] MeanUa;
            public double[] MeanUw;
            public int PacketsCount;

            public MeanParametres(StreamReader file)
            {
                MeanA = ReadParams(file.ReadLine());
                MeanUa = ReadParams(file.ReadLine());
                MeanW = ReadParams(file.ReadLine());
                MeanUw = ReadParams(file.ReadLine());
            }

            private double[] ReadParams(string line)
            {
                return line.Split(':', ' ').Skip(1).Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToArray();
            }
            public MeanParametres()
            {
            }
        }
        #region Private Fields

        readonly int _maxPacketsCount;

        readonly List<PacketsData>[] _positionPackets;

        MeanParametres[] _meanParams;

        private MeanParametres[] ComputedMeanParams => _meanParams ?? (_meanParams = new MeanParametres[PositionCount]);

        #endregion 

        #region Constructors

        public PacketsCollection(int temperature, int positionCount, int packetsCount)
        {
            Temperature = temperature;
            _positionPackets = new List<PacketsData>[positionCount];
            for (int i = 0; i < positionCount; i++)
                _positionPackets[i] = new List<PacketsData>();

            _maxPacketsCount = packetsCount;
        }

        public PacketsCollection(StreamReader file,int temperature, bool IsMeanFile = false)
        {
            Temperature = temperature;
            try
            {
                Temperature = Convert.ToInt32(file.ReadLine());               
            }
            catch (Exception)
            {
                Temperature = 0;
                throw;
            }

            if (!IsMeanFile)
            {
                try
                {
                     int positionCount = Convert.ToInt32(file.ReadLine());
                    _positionPackets = new List<PacketsData>[positionCount];
                    for (int i = 0; i < positionCount; i++)
                    {
                        _positionPackets[i] = new List<PacketsData>();
                        int packetsCount = Convert.ToInt32(file.ReadLine());
                        for (int j = 0; j < packetsCount; j++)
                        {
                            _positionPackets[i].Add(new PacketsData(file));
                        }
                    }
                }
                catch (Exception)
                {
                    Temperature = 0;
                    _maxPacketsCount = 0;
                    _positionPackets = null;
                    throw;
                }
            }
            else
            {
                try
                {
                    var means = new List<MeanParametres>();
                    while (file.Peek() == 'П')
                    {
                        file.ReadLine();
                        means.Add(new MeanParametres(file));
                    }
                    _meanParams = means.ToArray();                 
                }
                catch (Exception)
                {
                    Temperature = 0;
                    _maxPacketsCount = 0;
                    _positionPackets = null;
                    throw;
                }
            }
        }

        #endregion 

        #region Properties

        public int Temperature{ get; }

        public int PositionCount => _positionPackets?.Length ?? 0;

        public List<PacketsData> this[int index] => _positionPackets[index];

        #endregion 

        #region Compute Mean Methods

        public double[] MeanW(int positionNumber)
        {
            if (ComputedMeanParams[positionNumber] != null &&
                ComputedMeanParams[positionNumber].MeanA != null &&
                (_positionPackets?[positionNumber] == null ||
                 ComputedMeanParams[positionNumber].PacketsCount != _positionPackets[positionNumber].Count))
                return ComputedMeanParams[positionNumber].MeanW;
            if (ComputedMeanParams[positionNumber] == null)
                ComputedMeanParams[positionNumber] = new MeanParametres();

            var packetsList = _positionPackets[positionNumber];
            var w = new double[Config.Instance.ParamsCount];
            var sum = new double[Config.Instance.ParamsCount];

            foreach (var packet in packetsList)
            {
                var tmp = packet.MeanW;
                for (int i = 0; i < sum.Length; i++)
                    sum[i]+= tmp[i];
            }
            for (int i = 0; i < w.Length; i++)
                w[i] = sum[i] / packetsList.Count;
            ComputedMeanParams[positionNumber].MeanW = w;
            ComputedMeanParams[positionNumber].PacketsCount = packetsList.Count;
            return w;
        }

        public double[] MeanA(int positionNumber)
        {
            if (ComputedMeanParams[positionNumber] != null &&
                 ComputedMeanParams[positionNumber].MeanA != null && 
                 (_positionPackets?[positionNumber] == null ||
                  ComputedMeanParams[positionNumber].PacketsCount != _positionPackets[positionNumber].Count))
                return ComputedMeanParams[positionNumber].MeanA;
            if (ComputedMeanParams[positionNumber] == null)
                ComputedMeanParams[positionNumber] = new MeanParametres();

            var packetsList = _positionPackets[positionNumber];
            var a = new double[Config.Instance.ParamsCount];
            var sum = new double[Config.Instance.ParamsCount];

            foreach (var packet in packetsList)
            {
                var tmp = packet.MeanA;
                for (int i = 0; i < sum.Length; i++)
                    sum[i]+= tmp[i];
            }
            for (int i = 0; i < a.Length; i++)
                a[i] = sum[i] / packetsList.Count;

            ComputedMeanParams[positionNumber].MeanA = a;
            ComputedMeanParams[positionNumber].PacketsCount = packetsList.Count;
            return a;
        }

        public double[] MeanUw(int positionNumber)
        {
            if (ComputedMeanParams[positionNumber] != null &&
                ComputedMeanParams[positionNumber].MeanA != null &&
                (_positionPackets?[positionNumber] == null ||
                 ComputedMeanParams[positionNumber].PacketsCount != _positionPackets[positionNumber].Count))
                return ComputedMeanParams[positionNumber].MeanUw;
            if (ComputedMeanParams[positionNumber] == null)
                ComputedMeanParams[positionNumber] = new MeanParametres();

            var packetsList = _positionPackets[positionNumber];
            var uw = new double[Config.Instance.ParamsCount];
            var sum = new double[Config.Instance.ParamsCount];

            foreach (var packet in packetsList)
            {
                var tmp = packet.MeanUw;
                for (int i = 0; i < sum.Length; i++)
                    sum[i]+= tmp[i];
            }
            for (int i = 0; i < uw.Length; i++)
                uw[i] = sum[i] / packetsList.Count;
            ComputedMeanParams[positionNumber].MeanUw = uw;
            ComputedMeanParams[positionNumber].PacketsCount = packetsList.Count;
            return uw;
        }

        public double[] MeanUa(int positionNumber)
        {
            if (ComputedMeanParams[positionNumber] != null &&
                ComputedMeanParams[positionNumber].MeanA != null && 
                (_positionPackets?[positionNumber] == null ||
                 ComputedMeanParams[positionNumber].PacketsCount != _positionPackets[positionNumber].Count))
                return ComputedMeanParams[positionNumber].MeanUa;
            if (ComputedMeanParams[positionNumber] == null)
                ComputedMeanParams[positionNumber] = new MeanParametres();
            var packetsList = _positionPackets[positionNumber];
            var ua = new double[Config.Instance.ParamsCount];
            var sum = new double[Config.Instance.ParamsCount];

            foreach (var packet in packetsList)
            {
                var tmp = packet.MeanUa;
                for (int i = 0; i < sum.Length; i++)
                    sum[i]+= tmp[i];
            }
            for (int i = 0; i < ua.Length; i++)
                ua[i] = sum[i] / packetsList.Count;
            ComputedMeanParams[positionNumber].MeanUa = ua;
            ComputedMeanParams[positionNumber].PacketsCount = packetsList.Count;
            return ua;
        }

        public List<double> MeanParams(int positionNumber)
        {
            if (_positionPackets ==null || _positionPackets.Length < positionNumber)
                return null;
            var packetsList = _positionPackets[positionNumber];
            if (packetsList==null || packetsList.Count == 0)
                return null;
            var sumUa = new double[Config.Instance.ParamsCount];
            var sumUw = new double[Config.Instance.ParamsCount];
            var sumA = new double[Config.Instance.ParamsCount];
            var sumW = new double[Config.Instance.ParamsCount];
            int count = packetsList.Count;
            for (var i = 0; i < count; i++)
            {
                var ua = packetsList[i].MeanUa;
                var w = packetsList[i].MeanW;
                var a = packetsList[i].MeanA;
                var uw = packetsList[i].MeanUw;
                for (var j = 0; j < Config.Instance.ParamsCount; j++)
                {
                    sumUa[j] += ua[j];
                    sumUw[j] += uw[j];
                    sumW[j] += w[j];
                    sumA[j] += a[j];
                }
            }
            for (int i = 0; i < Config.Instance.ParamsCount; i++)
            {
                sumUa[i] /= packetsList.Count;
                sumUw[i] /= packetsList.Count;
                sumA[i] /= packetsList.Count;
                sumW[i] /= packetsList.Count;
            }
            var result = new List<double>();
            result.AddRange(sumW);
            result.AddRange(sumUw);
            result.AddRange(sumA);
            result.AddRange(sumUa);
            return result;
        }

        #endregion

        private bool Cleaned { get; set; }

        public bool AddPacketData(int positionNumber,PacketsData packetData)
        {
            if (_positionPackets[positionNumber].Count < _maxPacketsCount)
            {
                _positionPackets[positionNumber].Add(packetData);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ToFile(StreamWriter file)
        {
            file.WriteLine(Temperature);
            file.WriteLine(_positionPackets.Length);
            foreach (var positionPacket in _positionPackets)
            {
                file.WriteLine(positionPacket.Count);
                foreach (var packet in positionPacket)
                    file.WriteLine(packet);
            }
        }

        public void MeanToFile(StreamWriter file)
        {
            file.WriteLine(Temperature);
            for (int i = 0; i <  _positionPackets.Length; i++)
            {
                file.WriteLine($"Позиция {i}");
                var a = MeanA(i);
                file.WriteLine($"A:{a[0]} {a[1]} {a[2]}");
                var u = MeanUa(i);
                file.WriteLine($"UA:{u[0]} {u[1]} {u[2]}");
                var w = MeanW(i);
                file.WriteLine($"W:{w[0]} {w[1]} {w[2]}");
                u = MeanUw(i);
                file.WriteLine($"UW:{u[0]} {u[1]} {u[2]}");
            }
        }

        public override string ToString()
        {
            var buff = new StringBuilder(Temperature + " " + _positionPackets.Length + Environment.NewLine);
            foreach (var positionPacket in _positionPackets)
            {
                buff.Append(positionPacket.Count+Environment.NewLine);
                foreach (var packet in positionPacket)
                {
                    buff.Append(packet);
                }
            }
            return buff.ToString();
        }
        
        public void ClearData()
        {
            if (Cleaned)
            {
                Log.Instance.Warning("Уже очищено");
                return;
            }
            Log.Instance.Debug("ClearData температура {0}",Temperature);
            for (var i = 0; i < _positionPackets.Length; i++)
            {
                var findAll = ComputedMeanParams[i] == null;
                if (findAll||ComputedMeanParams[i].MeanA==null )
                    MeanA(i);
                if (findAll || ComputedMeanParams[i].MeanW == null)
                    MeanW(i);
                if (findAll || ComputedMeanParams[i].MeanUa == null)
                    MeanUa(i);
                if (findAll || ComputedMeanParams[i].MeanUw == null)
                    MeanUw(i);
                _positionPackets[i] = null;
            }
            Cleaned = true;
            GC.Collect();
        }
    }
}
