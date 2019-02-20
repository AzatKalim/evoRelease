using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Evo20.Evo20.Packets
{
    /// <summary>
    /// Структура данных, которая хранит в себе список PacketData(4 пакета)и  тепературу с позициями, когда они были собраны 
    /// </summary>
    public class PacketsCollection
    {
        private sealed class MeanParametres
        {
            public double[] MeanW;
            public double[] MeanA;
            public double[] MeanUA;
            public double[] MeanUW;
            public int packetsCount;
        }
        #region Private Fields
        /// <summary>
        /// Максимальное число пакетов
        /// </summary>
        [NonSerialized]
        readonly int MAX_PACKETS_COUNT;

        /// <summary>
        /// Массив, где индекс- номер позиции, а значение -список собраных PacketData в этой позиции
        /// </summary>
        List<PacketsData>[] positionPackets;

        MeanParametres[] meanParams;

        private MeanParametres[] ComputedMeanParams
        {
            get
            {
                if (meanParams == null)
                {
                    meanParams = new MeanParametres[PositionCount];
                }
                return meanParams;
            }
        }

        #endregion 

        #region Constructors

        public PacketsCollection(int temperature, int positionCount, int packetsCount)
        {
            this.Temperature = temperature;
            positionPackets = new List<PacketsData>[positionCount];
            for (int i = 0; i < positionCount; i++)
                positionPackets[i] = new List<PacketsData>();

            MAX_PACKETS_COUNT = packetsCount;
            IsCollected = false;
        }

        public PacketsCollection(StreamReader file,int temperature)
        {
            string tmp = file.ReadLine();
            int positionCount = 0;
            try
            {
                string[] temp = tmp.Split(' ');
                Temperature = Convert.ToInt32(temp[0]);
                positionCount = Convert.ToInt32(temp[1]);
            }
            catch (Exception)
            {
                Temperature = 0;
                positionCount = 0;
                throw;
            }
            positionPackets = new List<PacketsData>[positionCount];
            try
            {
                for (int i = 0; i < positionCount; i++)
                {
                    positionPackets[i] = new List<PacketsData>();
                    int packetsCount = Convert.ToInt32(file.ReadLine());
                    for (int j = 0; j < packetsCount; j++)
                    {
                        positionPackets[i].Add(new PacketsData(file));
                    }
                }
            }
            catch (Exception )
            {
                Temperature = 0;
                MAX_PACKETS_COUNT = 0;
                positionPackets = null;
                IsCollected = false;
                throw;
            }
            IsCollected = true;
        }

        #endregion 

        #region Properties

        /// <summary>
        /// Возвращает температуру 
        /// </summary>
        public int Temperature{ get; set; }

        /// <summary>
        /// Возвращает число позиций 
        /// </summary>
        public int PositionCount
        {
            get { return positionPackets==null ? 0 : positionPackets.Length; }
        }

        public List<PacketsData> this[int index]
        {
            get { return positionPackets[index]; }
        }

        public bool IsCollected
        {
            get;
            private set;
        } 

        #endregion 

        #region Compute Mean Methods

        /// <summary>
        /// Возвращает среднее значение гироскопа по всем PacketData в данной позиции
        /// </summary>
        /// <param name="positionNumber">номер позиции</param>
        /// <returns> массив средних значений </returns>
        public double[] MeanW(int positionNumber)
        {
            if (ComputedMeanParams[positionNumber] != null &&
                 ComputedMeanParams[positionNumber].MeanW != null && ComputedMeanParams[positionNumber].packetsCount != positionPackets[positionNumber].Count)
                return ComputedMeanParams[positionNumber].MeanW;
            if (ComputedMeanParams[positionNumber] == null)
                ComputedMeanParams[positionNumber] = new MeanParametres();

            var packetsList = positionPackets[positionNumber];
            var w = new double[Config.Instance.PARAMS_COUNT];
            var sum = new double[Config.Instance.PARAMS_COUNT];

            for (int j = 0; j < packetsList.Count; j++)
            {
                var tmp = packetsList[j].MeanW;
                for (int i = 0; i < sum.Length; i++)
                    sum[i]+= tmp[i];
            }
            for (int i = 0; i < w.Length; i++)
                w[i] = sum[i] / packetsList.Count;
            ComputedMeanParams[positionNumber].MeanW = w;
            ComputedMeanParams[positionNumber].packetsCount = packetsList.Count;
            return w;
        }

        /// <summary>
        /// Возвращает среднее значение акселерометра по всем PacketData в данной позиции
        /// </summary>
        /// <param name="positionNumber">номер позиции</param>
        /// <returns> массив средних значений </returns>
        public double[] MeanA(int positionNumber)
        {
            if (ComputedMeanParams[positionNumber] != null &&
                 ComputedMeanParams[positionNumber].MeanA != null && ComputedMeanParams[positionNumber].packetsCount != positionPackets[positionNumber].Count)
                return ComputedMeanParams[positionNumber].MeanA;
            if (ComputedMeanParams[positionNumber] == null)
                ComputedMeanParams[positionNumber] = new MeanParametres();

            var packetsList = positionPackets[positionNumber];
            var a = new double[Config.Instance.PARAMS_COUNT];
            var sum = new double[Config.Instance.PARAMS_COUNT];

            for (int j = 0; j < packetsList.Count; j++)
            {
                var tmp = packetsList[j].MeanA;
                for (int i = 0; i < sum.Length; i++)
                    sum[i]+= tmp[i];
            }
            for (int i = 0; i < a.Length; i++)
                a[i] = sum[i] / packetsList.Count;

            ComputedMeanParams[positionNumber].MeanA = a;
            ComputedMeanParams[positionNumber].packetsCount = packetsList.Count;
            return a;
        }

        /// <summary>
        /// Возвращает среднее значение температуры гироскопа по всем PacketData в данной позиции
        /// </summary>
        /// <param name="positionNumber">номер позиции</param>
        /// <returns> массив средних значений </returns>
        public double[] MeanUW(int positionNumber)
        {
            if (ComputedMeanParams[positionNumber] != null &&
                 ComputedMeanParams[positionNumber].MeanUW != null && ComputedMeanParams[positionNumber].packetsCount != positionPackets[positionNumber].Count)
                return ComputedMeanParams[positionNumber].MeanUW;
            if (ComputedMeanParams[positionNumber] == null)
                ComputedMeanParams[positionNumber] = new MeanParametres();

            var packetsList = positionPackets[positionNumber];
            var uw = new double[Config.Instance.PARAMS_COUNT];
            var sum = new double[Config.Instance.PARAMS_COUNT];

            for (int j = 0; j < packetsList.Count; j++)
            {
                var tmp = packetsList[j].MeanUW;
                for (int i = 0; i < sum.Length; i++)
                    sum[i]+= tmp[i];
            }
            for (int i = 0; i < uw.Length; i++)
                uw[i] = sum[i] / packetsList.Count;
            ComputedMeanParams[positionNumber].MeanUW = uw;
            ComputedMeanParams[positionNumber].packetsCount = packetsList.Count;
            return uw;
        }
        /// <summary>
        /// Возвращает среднее значение температуры аксерометра по всем PacketData в данной позиции
        /// </summary>
        /// <param name="positionNumber">номер позиции</param>
        /// <returns> массив средних значений </returns>
        public double[] MeanUA(int positionNumber)
        {
            if (ComputedMeanParams[positionNumber] != null &&
                 ComputedMeanParams[positionNumber].MeanUA != null && ComputedMeanParams[positionNumber].packetsCount != positionPackets[positionNumber].Count)
                return ComputedMeanParams[positionNumber].MeanUA;
            if (ComputedMeanParams[positionNumber] == null)
                ComputedMeanParams[positionNumber] = new MeanParametres();
            var packetsList = positionPackets[positionNumber];
            var ua = new double[Config.Instance.PARAMS_COUNT];
            var sum = new double[Config.Instance.PARAMS_COUNT];

            for (int j = 0; j < packetsList.Count; j++)
            {
                var tmp = packetsList[j].MeanUA;
                for (int i = 0; i < sum.Length; i++)
                    sum[i]+= tmp[i];
            }
            for (int i = 0; i < ua.Length; i++)
                ua[i] = sum[i] / packetsList.Count;
            ComputedMeanParams[positionNumber].MeanUA = ua;
            ComputedMeanParams[positionNumber].packetsCount = packetsList.Count;
            return ua;
        }

        /// <summary>
        /// Возвращает средние значения параметров в текущей позиции 
        /// </summary>
        /// <param name="positionNumber">номер позиции</param>
        /// <returns></returns>
        public List<double> MeanParams(int positionNumber)
        {
            if (positionPackets ==null || positionPackets.Length < positionNumber)
                return null;
            var packetsList = positionPackets[positionNumber];
            if (packetsList==null || packetsList.Count == 0)
                return null;
            var sumUa = new double[Config.Instance.PARAMS_COUNT];
            var sumUw = new double[Config.Instance.PARAMS_COUNT];
            var sumA = new double[Config.Instance.PARAMS_COUNT];
            var sumW = new double[Config.Instance.PARAMS_COUNT];

            for (int j = 0; j < packetsList.Count; j++)
            {
                var ua = packetsList[j].MeanUA;
                var w = packetsList[j].MeanW;
                var a = packetsList[j].MeanA;
                var uw = packetsList[j].MeanUW;
                for (int i = 0; i < Config.Instance.PARAMS_COUNT; i++)
                {
                    sumUa[i] +=ua[i];
                    sumUw[i] += uw[i];
                    sumW[i] += w[i];
                    sumA[i] += a[i];
                }
            }
            for (int i = 0; i < Config.Instance.PARAMS_COUNT; i++)
            {
                sumUa[i] /= packetsList.Count;
                sumUw[i] /= packetsList.Count;
                sumA[i] /= packetsList.Count;
                sumW[i] /= packetsList.Count;
            }
            var result = new List<double>();
            result.AddRange(sumA);
            result.AddRange(sumW);
            result.AddRange(sumUa);
            result.AddRange(sumUw);
            return result;
        }

        #endregion

        public bool Cleaned { get; set; }
        /// <summary>
        /// Добавление PacketData 
        /// </summary>
        /// <param name="positionNumber">номер позиции</param>
        /// <param name="packetData">данные</param>
        /// <returns>true- пакет добавлен, false- необходимое число пакетов собрано</returns>
        public bool AddPacketData(int positionNumber,PacketsData packetData)
        {
            if (positionPackets[positionNumber].Count < MAX_PACKETS_COUNT)
            {
                positionPackets[positionNumber].Add(packetData);
                return true;
            }
            else
            {
                IsCollected = true;
                return false;
            }
        }

        /// <summary>
        /// Переопределяет метод ToString 
        /// </summary>
        /// <returns>Читабельную строку </returns>
        public override string ToString()
        {
            var buff = new StringBuilder(Temperature + " " + positionPackets.Length + Environment.NewLine);
            for (int i = 0; i < positionPackets.Length; i++)
            {
                buff.Append(positionPackets[i].Count+Environment.NewLine);
                for (int j = 0; j <positionPackets[i].Count; j++)
                {
                    buff.Append(positionPackets[i][j]);
                    //buff.Append(" ");
                }
            }
            return buff.ToString();
        }
        
        public void ClearUnneedInfo()
        {
            if (Cleaned)
                Log.Instance.Error("Уже очищено");
            Log.Instance.Debug("ClearUnneedInfo");
            for (int i = 0; i < positionPackets.Length; i++)
            {
                bool FindAll = false;
                if (ComputedMeanParams[i] == null)
                    FindAll = true;
                if (FindAll||ComputedMeanParams[i].MeanA==null )
                    MeanA(i);
                if (FindAll || ComputedMeanParams[i].MeanW == null)
                    MeanW(i);
                if (FindAll || ComputedMeanParams[i].MeanUA == null)
                    MeanUA(i);
                if (FindAll || ComputedMeanParams[i].MeanUW == null)
                    MeanUW(i);
                positionPackets[i] = null;
            }
            Cleaned = true;
            GC.Collect();
        }
        private bool MeanComputed()
        {
            return true;
        }

    }
}
