using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Evo20.PacketsLib
{
    /// <summary>
    /// Структура данных, которая хранит в себе список PacketData(4 пакета)и  тепературу с позициями, когда они были собраны 
    /// </summary>
    public class PacketsCollection
    {
        #region Private Fields
        /// <summary>
        /// Максимальное число пакетов
        /// </summary>
        readonly int maxPacketsCount;

        /// <summary>
        /// температура сбора пакетов 
        /// </summary>
        readonly int temperature;

        /// <summary>
        /// Массив, где индекс- номер позиции, а значение -список собраных PacketData в этой позиции
        /// </summary>
        List<PacketsData>[] positionPackets;

        bool isCollected = false;

        double[] meanUa=new double[Packet.PARAMS_COUNT];
        double[] meanUw=new double[Packet.PARAMS_COUNT];
        double[] meanA=new double[Packet.PARAMS_COUNT];
        double[] meanW=new double[Packet.PARAMS_COUNT];

        #endregion 

        #region Constructors

        public PacketsCollection(int temperature, int positionCount, int packetsCount)
        {
            this.temperature = temperature;
            positionPackets = new List<PacketsData>[positionCount];
            for (int i = 0; i < positionCount; i++)
            {
                positionPackets[i] = new List<PacketsData>();
            }
            maxPacketsCount = packetsCount;
        }

        public PacketsCollection(StreamReader file)
        {
            string tmp = file.ReadLine();
            int positionCount = 0;
            try
            {
                string[] temp = tmp.Split(' ');
                temperature = Convert.ToInt32(temp[0]);
                positionCount = Convert.ToInt32(temp[1]);
            }
            catch (Exception ex)
            {
                temperature = 0;
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
                temperature = 0;
                maxPacketsCount = 0;
                positionPackets = null;
                throw exception;
            }
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
                return  temperature;
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
            double[] w = new double[Packet.PARAMS_COUNT];
            double[] sum = new double[Packet.PARAMS_COUNT];

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
            double[] a = new double[Packet.PARAMS_COUNT];
            double[] sum = new double[Packet.PARAMS_COUNT];

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
            meanUw = new double[Packet.PARAMS_COUNT];
            double[] sum = new double[Packet.PARAMS_COUNT];

            for (int j = 0; j < packetsList.Count; j++)
            {
                double[] tmp = packetsList[j].MeanUW;
                for (int i = 0; i < sum.Length; i++)
                {
                    sum[i]+= tmp[i];
                }
            }
            for (int i = 0; i < meanUw.Length; i++)
            {
                meanUw[i] = sum[i] / packetsList.Count;
            }
            return meanUw;
        }

        /// <summary>
        /// Возвращает среднее значение температуры аксерометра по всем PacketData в данной позиции
        /// </summary>
        /// <param name="positionNumber">номер позиции</param>
        /// <returns> массив средних значений </returns>
        public double[] MeanUA(int positionNumber)
        {
            List<PacketsData> packetsList = positionPackets[positionNumber];
            meanUa = new double[Packet.PARAMS_COUNT];
            double[] sum = new double[Packet.PARAMS_COUNT];

            for (int j = 0; j < packetsList.Count; j++)
            {
                double[] tmp = packetsList[j].MeanUA;
                for (int i = 0; i < sum.Length; i++)
                {
                    sum[i]+= tmp[i];
                }
            }
            for (int i = 0; i < meanUa.Length; i++)
            {
                meanUa[i] = sum[i] / packetsList.Count;
            }
            return meanUa;
        }

        /// <summary>
        /// Возвращает средние значения параметров в текущей позиции 
        /// </summary>
        /// <param name="positionNumber">номер позиции</param>
        /// <returns></returns>
        public List<double> MeanParams(int positionNumber)
        {
            var result = new List<double>();
            if (!isCollected)
            {
                var packetsList = positionPackets[positionNumber];
                if (packetsList.Count == 0)
                {
                    return null;
                }
                for (int i = 0; i < Packet.PARAMS_COUNT; i++)
                {
                    meanUa[i] = 0;
                    meanUw[i] = 0;
                    meanW[i] = 0;
                    meanA[i] = 0;
                }
                for (int j = 0; j < packetsList.Count; j++)
                {
                    double[] ua = packetsList[j].MeanUA;
                    double[] w = packetsList[j].MeanW;
                    double[] a = packetsList[j].MeanA;
                    double[] uw = packetsList[j].MeanUW;
                    for (int i = 0; i < Packet.PARAMS_COUNT; i++)
                    {
                        meanUa[i] += ua[i];
                        meanUw[i] += uw[i];
                        meanW[i] += w[i];
                        meanA[i] += a[i];
                    }
                }
                for (int i = 0; i < Packet.PARAMS_COUNT; i++)
                {
                    meanUa[i] /= packetsList.Count;
                    meanUw[i] /= packetsList.Count;
                    meanA[i] /= packetsList.Count;
                    meanW[i] /= packetsList.Count;
                }         
            }
            result.AddRange(meanA);
            result.AddRange(meanW);
            result.AddRange(meanUa);
            result.AddRange(meanUw);
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
            if (positionPackets[positionNumber].Count < maxPacketsCount)
            {
                positionPackets[positionNumber].Add(packetData);
                return true;
            }
            else
            {
                isCollected = true;
                return false;
            }
        }


        public void WrieToFile(StreamWriter file)
        {
            file.WriteLine(temperature + " " + positionPackets.Length);
            for (int i = 0; i < positionPackets.Length; i++)
            {
                file.WriteLine(positionPackets[i].Count+Environment.NewLine);
                for (int j = 0; j <positionPackets[i].Count; j++)
                {
                    file.WriteLine(positionPackets[i][j]);
                }
            }
        }
        /// <summary>
        /// Переопределяет метод ToString 
        /// </summary>
        /// <returns>Читабельную строку </returns>
        public override string ToString()
        {
            var buff = new StringBuilder(temperature + " " + positionPackets.Length+Environment.NewLine);
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
