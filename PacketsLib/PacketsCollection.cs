using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PacketsLib
{
    public class PacketsCollection
    {
        readonly int maxPacketsCount;

        readonly int temperature;

        List<PacketsData>[] positionPackets;

        #region Properties 

        public int  Temperature
        {
            get
            {
                return  temperature;
            }
        }

        public int PositionCount
        {
            get
            {
                return positionPackets.Length;
            }
        }

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

        public double[] MeanUW(int positionNumber)
        {
            List<PacketsData> packetsList = positionPackets[positionNumber];
            double[] uw = new double[Packet.PARAMS_COUNT];
            double[] sum = new double[Packet.PARAMS_COUNT];

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

        public double[] MeanUA(int positionNumber)
        {
            List<PacketsData> packetsList = positionPackets[positionNumber];
            double[] ua = new double[Packet.PARAMS_COUNT];
            double[] sum = new double[Packet.PARAMS_COUNT];

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

        public List<double> MeanParams(int positionNumber)
        {
            List<PacketsData> packetsList = positionPackets[positionNumber];
            if (packetsList.Count == 0)
            {
                return null;
            }
            double[] sumUa = new double[Packet.PARAMS_COUNT];
            double[] sumUw = new double[Packet.PARAMS_COUNT];
            double[] sumA =  new double[Packet.PARAMS_COUNT];
            double[] sumW = new double[Packet.PARAMS_COUNT];


            for (int j = 0; j < packetsList.Count; j++)
            {
                double[] ua = packetsList[j].MeanUA;
                double[] w = packetsList[j].MeanW;
                double[] a = packetsList[j].MeanA;
                double[] uw = packetsList[j].MeanUW;
                for (int i = 0; i < Packet.PARAMS_COUNT; i++)
                {
                    sumUa[i] +=ua[i];
                    sumUw[i] += uw[i];
                    sumW[i] += w[i];
                    sumA[i] += a[i];
                }
            }
            for (int i = 0; i < Packet.PARAMS_COUNT; i++)
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

        public List<PacketsData> this[int index]
        {
            get { return positionPackets[index]; }
        }  

        #endregion 

        public PacketsCollection(int temperature, int positionCount, int packetsCount)
        {
            this.temperature = temperature;
            positionPackets = new List<PacketsData>[positionCount];
            for (int i = 0; i<positionCount; i++)
            {
                positionPackets[i]= new List<PacketsData>();
            }
            maxPacketsCount = packetsCount;
        }

        public PacketsCollection(StreamReader file)
        {
            string tmp=file.ReadLine();
            int positionCount=0;
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
            catch(Exception exception)
            {
                temperature = 0;
                maxPacketsCount = 0;
                positionPackets = null;
                throw exception;
            }
        }

        public bool AddPacketData(int positionNumber,PacketsData packetData)
        {
            if (positionPackets[positionNumber].Count < maxPacketsCount)
            {
                positionPackets[positionNumber].Add(packetData);
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            StringBuilder buff = new StringBuilder(temperature + " " + positionPackets.Length+Environment.NewLine);
            for (int i = 0; i < positionPackets.Length; i++)
            {
                buff.Append(positionPackets[i].Count+Environment.NewLine);
                for (int j = 0; j <positionPackets[i].Count; j++)
                {
                    buff.Append(positionPackets[i][j]);
                }
            }
            return buff.ToString();
        }
     
    }
}
