using Evo20.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Evo20.Packets
{
    public class Packet
    {
        #region Constants    
        const int WStringNumber = 1;
        const int AStringNumber = 4;
        const int UStringNumber = 7;

        static readonly double mul = 0.5 / Math.Pow(2, 28);
        #endregion

        private byte[] Data;
        private double[] _w;
        private double[] _a;
        private double[] _u;
        public double[] W
        {
            get
            {
                if (_w == null)
                    PacketHandle();
                return _w;
            }
            private set
            {
                _w = value;
            }
        }
        public double[] A
        {
            get
            {
                if (_a == null)
                    PacketHandle();
                return _a;
            }
            private set
            {
                _a = value;
            }
        }
        public double[] U
        {
            get
            {
                if (_u == null)
                    PacketHandle();
                return _u;
            }
            internal set
            {
                _u = value;
            }
        }

        public int Id
        {
            get;
        }

        private Packet(int id,byte[] array)
        {
           Id = id;
           Data = array;
        }

        public override string ToString()
        {
            return $"{Id} " +
                $"{W[0].ToString(CultureInfo.InvariantCulture)} {W[1].ToString(CultureInfo.InvariantCulture)} {W[2].ToString(CultureInfo.InvariantCulture)} " +
                $"{A[0].ToString(CultureInfo.InvariantCulture)} {A[1].ToString(CultureInfo.InvariantCulture)} {A[2].ToString(CultureInfo.InvariantCulture)} " +
                $"{U[0].ToString(CultureInfo.InvariantCulture)} {U[1].ToString(CultureInfo.InvariantCulture)} {U[2].ToString(CultureInfo.InvariantCulture)}";
        }

        public Packet(string input)
        {
            string[] tmp = input.Split(' ');
            try
            {
                A = new double[Config.Instance.ParamsCount];
                W = new double[Config.Instance.ParamsCount];
                U = new double[Config.Instance.ParamsCount];
                Id = Convert.ToInt32(tmp[0]);
                for (var i = 0; i < Config.Instance.ParamsCount; i++)
                {
                    W[i] = double.Parse(tmp[WStringNumber + i], CultureInfo.InvariantCulture);
                    A[i] = double.Parse(tmp[AStringNumber + i], CultureInfo.InvariantCulture);
                    U[i] = double.Parse(tmp[UStringNumber + i], CultureInfo.InvariantCulture);
                }
            }
            catch (FormatException)
            {
                A = null;
                W = null;
                U = null;
                throw;
            }
        }

        private void PacketHandle()
        {
            try
            {       
                bool rangeFlag = false;
                bool dataFlag = false;
                var wX = ConvertParam(new[] { Data[Config.Instance.WxBegin], Data[Config.Instance.WxBegin + 1], Data[Config.Instance.WxBegin + 2], Data[Config.Instance.WxBegin + 3] }, ref rangeFlag, ref dataFlag);
                var wY = ConvertParam(new[] { Data[Config.Instance.WyBegin], Data[Config.Instance.WyBegin + 1], Data[Config.Instance.WyBegin + 2], Data[Config.Instance.WyBegin + 3] }, ref rangeFlag, ref dataFlag);
                var wZ = ConvertParam(new[] { Data[Config.Instance.WzBegin], Data[Config.Instance.WzBegin + 1], Data[Config.Instance.WzBegin + 2], Data[Config.Instance.WzBegin + 3] }, ref rangeFlag, ref dataFlag);
                W = new[] { wX, wY, wZ };
                var aX = ConvertParam(new[] { Data[Config.Instance.AxBegin], Data[Config.Instance.AxBegin + 1], Data[Config.Instance.AxBegin + 2], Data[Config.Instance.AxBegin + 3] }, ref rangeFlag, ref dataFlag);
                var aY = ConvertParam(new[] { Data[Config.Instance.AyBegin], Data[Config.Instance.AyBegin + 1], Data[Config.Instance.AyBegin + 2], Data[Config.Instance.AyBegin + 3] }, ref rangeFlag, ref dataFlag);
                var aZ = ConvertParam(new[] { Data[Config.Instance.AzBegin], Data[Config.Instance.AzBegin + 1], Data[Config.Instance.AzBegin + 2], Data[Config.Instance.AzBegin + 3] }, ref rangeFlag, ref dataFlag);
                A = new[] { aX, aY, aZ };
                if (Id < 3)
                {
                    var uX = ConvertParam(new[] { Data[Config.Instance.UxBegin], Data[Config.Instance.UxBegin + 1], Data[Config.Instance.UxBegin + 2], Data[Config.Instance.UxBegin + 3] }, ref rangeFlag, ref dataFlag);
                    var uY = ConvertParam(new[] { Data[Config.Instance.UyBegin], Data[Config.Instance.UyBegin + 1], Data[Config.Instance.UyBegin + 2], Data[Config.Instance.UyBegin + 3] }, ref rangeFlag, ref dataFlag);
                    var uZ = ConvertParam(new[] { Data[Config.Instance.UzBegin], Data[Config.Instance.UzBegin + 1], Data[Config.Instance.UzBegin + 2], Data[Config.Instance.UzBegin + 3] }, ref rangeFlag, ref dataFlag);
                    U = new[] { uX, uY, uZ };
                }
                Data = null;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static Packet FirstPacketHandle(byte[] message)
        {
            try
            {
                ushort head = BitConverter.ToUInt16(new[] { message[Config.Instance.HeadBegin], message[Config.Instance.HeadBegin + 1] }, 0);
                if (head != 0xFACE)
                    return null;
//#if !DEBUG
                int checkSum = Crc16.ComputeChecksum(message, Config.Instance.PacketSize);
                if (BitConverter.ToUInt16(new[] { message[Config.Instance.CheckBegin], message[Config.Instance.CheckBegin + 1] }, 0) != checkSum)
                    return null;
//#endif
                int packetId = BitConverter.ToUInt16(new[] { message[Config.Instance.IdBegin], message[Config.Instance.IdBegin + 1] }, 0);
                return new Packet(packetId, message);
            }
            catch (Exception)
            {
                return null;
            }

        }

        private static double ConvertParam(byte[] bytes, ref bool rangeFlag, ref bool dataFlag)
        {
            //bool sign = true;          
            var res = BitConverter.ToInt32(bytes, 0);
            //if ((res & int.MinValue) == int.MinValue)
            //    sign = false;
            if ((res & 3) == 2 || (res & 3) == 1)
                rangeFlag = true;
            if ((res & 3) == 3)
                dataFlag = true;

            return (res << 2) * mul;
        }

        public static bool FindPacketBegin(ref List<byte> buffer)
        {
            if (buffer.Count < 2)
            {
                return false;
            }
            byte first = buffer[0];
            byte second = buffer[1];
            int i = 2;
            while ((first!=206 || second != 250) && buffer.Count > i)
            {
                first = second;
                second = buffer[i];
                i++;
            }
            if (i == buffer.Count)
            {
                lock (buffer)
                {
                    buffer.RemoveRange(0, i - 1);
                }
                return false;
            }
            else
            {
                i -= 2;
                lock (buffer)
                {
                    buffer.RemoveRange(0, i);
                }
                return true;
            }
        }
    }
}
