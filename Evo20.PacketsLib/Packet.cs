using Evo20.Utils;
using System;
using System.Collections.Generic;

namespace Evo20.Packets
{
    public class Packet
    {
        #region Constants    
        const int WStringNumber = 1;
        const int AStringNumber = 4;
        const int UStringNumber = 7;

        #endregion
        public byte[] Data;
        private int[] _w;
        private int[] _a;
        private int[] _u;
        public int[] W
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
        public int[] A
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
        public int[] U
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
       
        public Packet(int id,byte[] array)
        {
           Id = id;
           Data = array;
        }


        public override string ToString()
        {
            string buffer = $"{Id} {W[0]} {W[1]} {W[2]} {A[0]} {A[1]} {A[2]} {U[0]} {U[1]} {U[2]}";
            return buffer;
        }

        public Packet(string input)
        {
            string[] tmp = input.Split(' ');
            try
            {
                A = new int[Config.Instance.ParamsCount];
                W = new int[Config.Instance.ParamsCount];
                U = new int[Config.Instance.ParamsCount];
                Id = Convert.ToInt32(tmp[0]);
                for (int i = 0; i < Config.Instance.ParamsCount; i++)
                {
                    W[i] = Convert.ToInt32(tmp[WStringNumber + i]);
                    A[i] = Convert.ToInt32(tmp[AStringNumber + i]);
                    U[i] = Convert.ToInt32(tmp[UStringNumber + i]);
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
                int wX = ConvertParam(new[] { Data[Config.Instance.WxBegin], Data[Config.Instance.WxBegin + 1], Data[Config.Instance.WxBegin + 2], Data[Config.Instance.WxBegin + 3] }, ref rangeFlag, ref dataFlag);
                int wY = ConvertParam(new[] { Data[Config.Instance.WyBegin], Data[Config.Instance.WyBegin + 1], Data[Config.Instance.WyBegin + 2], Data[Config.Instance.WyBegin + 3] }, ref rangeFlag, ref dataFlag);
                int wZ = ConvertParam(new[] { Data[Config.Instance.WzBegin], Data[Config.Instance.WzBegin + 1], Data[Config.Instance.WzBegin + 2], Data[Config.Instance.WzBegin + 3] }, ref rangeFlag, ref dataFlag);
                W = new[] { wX, wY, wZ };
                int aX = ConvertParam(new[] { Data[Config.Instance.AxBegin], Data[Config.Instance.AxBegin + 1], Data[Config.Instance.AxBegin + 2], Data[Config.Instance.AxBegin + 3] }, ref rangeFlag, ref dataFlag);
                int aY = ConvertParam(new[] { Data[Config.Instance.AyBegin], Data[Config.Instance.AyBegin + 1], Data[Config.Instance.AyBegin + 2], Data[Config.Instance.AyBegin + 3] }, ref rangeFlag, ref dataFlag);
                int aZ = ConvertParam(new[] { Data[Config.Instance.AzBegin], Data[Config.Instance.AzBegin + 1], Data[Config.Instance.AzBegin + 2], Data[Config.Instance.AzBegin + 3] }, ref rangeFlag, ref dataFlag);
                A = new[] { aX, aY, aZ };
                if (Id < 3)
                {
                    int uX = ConvertParam(new[] { Data[Config.Instance.UxBegin], Data[Config.Instance.UxBegin + 1], Data[Config.Instance.UxBegin + 2], Data[Config.Instance.UxBegin + 3] }, ref rangeFlag, ref dataFlag);
                    int uY = ConvertParam(new[] { Data[Config.Instance.UyBegin], Data[Config.Instance.UyBegin + 1], Data[Config.Instance.UyBegin + 2], Data[Config.Instance.UyBegin + 3] }, ref rangeFlag, ref dataFlag);
                    int uZ = ConvertParam(new[] { Data[Config.Instance.UzBegin], Data[Config.Instance.UzBegin + 1], Data[Config.Instance.UzBegin + 2], Data[Config.Instance.UzBegin + 3] }, ref rangeFlag, ref dataFlag);
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
#if !DEBUG
                int checkSum = Crc16.ComputeChecksum(message, Config.Instance.PacketSize);
                if (BitConverter.ToUInt16(new[] { message[Config.Instance.CheckBegin], message[Config.Instance.CheckBegin + 1] }, 0) != checkSum)
                    return null;
#endif
                int packetId = BitConverter.ToUInt16(new[] { message[Config.Instance.IdBegin], message[Config.Instance.IdBegin + 1] }, 0);
                return new Packet(packetId, message);
            }
            catch (Exception)
            {
                return null;
            }

        }

        private static int ConvertParam(byte[] bytes, ref bool rangeFlag, ref bool dataFlag)
        {
            //bool sign = true;          
            int res = BitConverter.ToInt32(bytes, 0);
            //if ((res & int.MinValue) == int.MinValue)
            //    sign = false;
            if ((res & 3) == 2 || (res & 3) == 1)
                rangeFlag = true;
            if ((res & 3) == 3)
                dataFlag = true;
            return res;
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
