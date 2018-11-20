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
    [Serializable]
    public class PacketsCollection
    {
        #region Private Fields
        /// <summary>
        /// Максимальное число пакетов
        /// </summary>
        [NonSerialized]
        readonly int MAX_PACKETS_COUNT;

        /// <summary>
        /// температура сбора пакетов 
        /// </summary>
        readonly int TEMPERATURE;

        /// <summary>
        /// Массив, где индекс- номер позиции, а значение -список собраных PacketData в этой позиции
        /// </summary>
        List<PacketsData>[] positionPackets;

        #endregion 

        #region Constructors

        public PacketsCollection(int temperature, int positionCount, int packetsCount)
        {
            this.TEMPERATURE = temperature;
            positionPackets = new List<PacketsData>[positionCount];
            for (int i = 0; i < positionCount; i++)
            {
                positionPackets[i] = new List<PacketsData>();
            }
            MAX_PACKETS_COUNT = packetsCount;
            IsCollected = false;
        }

        public PacketsCollection(StreamReader file)
        {
            string tmp = file.ReadLine();
            int positionCount = 0;
            try
            {
                string[] temp = tmp.Split(' ');
                TEMPERATURE = Convert.ToInt32(temp[0]);
                positionCount = Convert.ToInt32(temp[1]);
            }
            catch (Exception ex)
            {
                TEMPERATURE = 0;
                positionCount = 0;
                throw ex;
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
            catch (Exception exception)
            {
                TEMPERATURE = 0;
                MAX_PACKETS_COUNT = 0;
                positionPackets = null;
                IsCollected = false;
                throw exception;
            }
            IsCollected = true;
        }

        #endregion 

        #region Properties

        /// <summary>
        /// Возвращает температуру 
        /// </summary>
        public int  Temperature
        {
            get
            {
                return  TEMPERATURE;
            }
        }

        /// <summary>
        /// Возвращает число позиций 
        /// </summary>
        public int PositionCount
        {
            get
            {
                return positionPackets.Length;
            }
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
            List<PacketsData> packetsList = positionPackets[positionNumber];
            double[] w = new double[Config.Config.PARAMS_COUNT];
            double[] sum = new double[Config.Config.PARAMS_COUNT];

            for (int j = 0; j < packetsList.Count; j++)
            {
                double[] tmp = packetsList[j].MeanW;
                for (int i = 0; i < sum.Length; i++)
                {
                    sum[i]+= tmp[i];
                }
            }
            for (int i = 0; i < w.Length; i++)
            {
                w[i] = sum[i] / packetsList.Count;
            }
            return w;
        }

        /// <summary>
        /// Возвращает среднее значение акселерометра по всем PacketData в данной позиции
        /// </summary>
        /// <param name="positionNumber">номер позиции</param>
        /// <returns> массив средних значений </returns>
        public double[] MeanA(int positionNumber)
        {
            List<PacketsData> packetsList = positionPackets[positionNumber];
            double[] a = new double[Config.Config.PARAMS_COUNT];
            double[] sum = new double[Config.Config.PARAMS_COUNT];

            for (int j = 0; j < packetsList.Count; j++)
            {
                double[] tmp = packetsList[j].MeanA;
                for (int i = 0; i < sum.Length; i++)
                {
                    sum[i]+= tmp[i];
                }
            }
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = sum[i] / packetsList.Count;
            }
            return a;
        }

        /// <summary>
        /// Возвращает среднее значение температуры гироскопа по всем PacketData в данной позиции
        /// </summary>
        /// <param name="positionNumber">номер позиции</param>
        /// <returns> массив средних значений </returns>
        public double[] MeanUW(int positionNumber)
        {
            List<PacketsData> packetsList = positionPackets[positionNumber];
            double[] uw = new double[Config.Config.PARAMS_COUNT];
            double[] sum = new double[Config.Config.PARAMS_COUNT];

            for (int j = 0; j < packetsList.Count; j++)
            {
                double[] tmp = packetsList[j].MeanUW;
                for (int i = 0; i < sum.Length; i++)
                {
                    sum[i]+= tmp[i];
                }
            }
            for (int i = 0; i < uw.Length; i++)
            {
                uw[i] = sum[i] / packetsList.Count;
            }
            return uw;
        }
        /// <summary>
        /// Возвращает среднее значение температуры аксерометра по всем PacketData в данной позиции
        /// </summary>
        /// <param name="positionNumber">номер позиции</param>
        /// <returns> массив средних значений </returns>
        public double[] MeanUA(int positionNumber)
        {
            List<PacketsData> packetsList = positionPackets[positionNumber];
            double[] ua = new double[Config.Config.PARAMS_COUNT];
            double[] sum = new double[Config.Config.PARAMS_COUNT];

            for (int j = 0; j < packetsList.Count; j++)
            {
                double[] tmp = packetsList[j].MeanUA;
                for (int i = 0; i < sum.Length; i++)
                {
                    sum[i]+= tmp[i];
                }
            }
            for (int i = 0; i < ua.Length; i++)
            {
                ua[i] = sum[i] / packetsList.Count;
            }
            return ua;
        }

        /// <summary>
        /// Возвращает средние значения параметров в текущей позиции 
        /// </summary>
        /// <param name="positionNumber">номер позиции</param>
        /// <returns></returns>
        public List<double> MeanParams(int positionNumber)
        {
            if (positionPackets.Length < positionNumber)
                return null;
            List<PacketsData> packetsList = positionPackets[positionNumber];
            if (packetsList.Count == 0)
            {
                return null;
            }
            double[] sumUa = new double[Config.Config.PARAMS_COUNT];
            double[] sumUw = new double[Config.Config.PARAMS_COUNT];
            double[] sumA =  new double[Config.Config.PARAMS_COUNT];
            double[] sumW = new double[Config.Config.PARAMS_COUNT];


            for (int j = 0; j < packetsList.Count; j++)
            {
                double[] ua = packetsList[j].MeanUA;
                double[] w = packetsList[j].MeanW;
                double[] a = packetsList[j].MeanA;
                double[] uw = packetsList[j].MeanUW;
                for (int i = 0; i < Config.Config.PARAMS_COUNT; i++)
                {
                    sumUa[i] +=ua[i];
                    sumUw[i] += uw[i];
                    sumW[i] += w[i];
                    sumA[i] += a[i];
                }
            }
            for (int i = 0; i < Config.Config.PARAMS_COUNT; i++)
            {
                sumUa[i] /= packetsList.Count;
                sumUw[i] /= packetsList.Count;
                sumA[i] /= packetsList.Count;
                sumW[i] /= packetsList.Count;
            }
            List<double> result = new List<double>();
            result.AddRange(sumA);
            result.AddRange(sumW);
            result.AddRange(sumUa);
            result.AddRange(sumUw);
            return result;
        }

        #endregion

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
            var buff = new StringBuilder(TEMPERATURE + " " + positionPackets.Length+Environment.NewLine);
            for (int i = 0; i < positionPackets.Length; i++)
            {
                buff.Append(positionPackets[i].Count+Environment.NewLine);
                for (int j = 0; j <positionPackets[i].Count; j++)
                {
                    buff.Append(positionPackets[i][j]+" ");
                }
            }
            return buff.ToString();
        }    

    }
}
