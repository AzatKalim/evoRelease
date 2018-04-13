using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace PacketsLib
{
    public class Packet
    {
        //позиция служебных  байт в пакете
        public const int HEAD_BEGIN = 0;
        public const int ID_BEGIN = 38;
        public const int CHECK_BEGIN = 40;
        //позиция информационных байт в пакете
        public const int W_X_BEGIN = 2;
        public const int W_Y_BEGIN = 6;
        public const int W_Z_BEGIN = 10;

        public const int A_X_BEGIN = 14;
        public const int A_Y_BEGIN = 18;
        public const int A_Z_BEGIN = 22;

        public const int U_X_BEGIN = 26;
        public const int U_Y_BEGIN = 30;
        public const int U_Z_BEGIN = 34;

        public const int PARAMS_COUNT = 3;
        public const int PACKET_SIZE = 42;

        const int W_STRING_NUMBER = 1;
        const int A_STRING_NUMBER = 4;
        const int U_STRING_NUMBER = 7;

        public int[] w
        {
            get;
            private set;
        }
        public int[] a
        {
            get;
            private set;
        }
        public int[] u
        {
            get;
            internal set;
        }
        public int id
        {
            get;
            private set;
        }

        public Packet(int[] w, int[] a, int[] u, int id)
        {
            this.w = w;
            this.a = a;
            this.u = u;
            this.id = id;
        }

        public Packet(int[] w, int[] a, int id)
        {
            this.id = id;
            this.w = w;
            this.a = a;
        }

        public override string ToString()
        {
            string buffer = id.ToString() + " " + w[0].ToString() + " " + w[1].ToString() + " " + w[2].ToString() + " "
            + a[0].ToString() + " " + a[1].ToString() + " " + a[2].ToString() + " "
            + u[0].ToString() + " " + u[1].ToString() + " " + u[2].ToString() + " ";
            return buffer;
        }

        public Packet(string input)
        {
            string[] tmp = input.Split(' ');
            try
            {
                a = new int[PARAMS_COUNT];
                w = new int[PARAMS_COUNT];
                u = new int[PARAMS_COUNT];
                id = Convert.ToInt32(tmp[0]);
                for (int i = 0; i < PARAMS_COUNT; i++)
                {
                    w[i] = Convert.ToInt32(tmp[W_STRING_NUMBER + i]);
                    a[i] = Convert.ToInt32(tmp[A_STRING_NUMBER + i]);
                    u[i] = Convert.ToInt32(tmp[U_STRING_NUMBER + i]);
                }
            }
            catch (FormatException exception)
            {
                a = null;
                w = null;
                u = null;
                throw exception;
            }
        }

        /// <summary>
        /// Метод извлекающий Packet из массива байт
        /// </summary>
        /// <param name="message"> массив байт</param>
        /// <returns>пакет</returns>
        public static Packet PacketHandle(byte[] message)
        {
            try
            {
                Packet packet_inf = null;
                //проверка заголовка пакета
                ushort head = BitConverter.ToUInt16(new byte[] { message[Packet.HEAD_BEGIN], message[HEAD_BEGIN + 1] }, 0);
                if (head != 0xFACE)
                {
                    return packet_inf;
                }
                Byte[] inf_part = new byte[40];
                Array.Copy(message, 0, inf_part, 0, Packet.PACKET_SIZE - 2);
                int checkSum = CRC16.ComputeChecksum(inf_part);
                //проверка контрольной суммы
                if (BitConverter.ToUInt16(new byte[2] { message[CHECK_BEGIN], message[CHECK_BEGIN + 1] }, 0) != checkSum)
                {
                    return packet_inf;
                }
                //номер пакета 
                int packetId = BitConverter.ToUInt16(new byte[] { message[ID_BEGIN], message[ID_BEGIN + 1] }, 0);
                //данные с гироскопов
                bool rangeFlag = false;
                bool dataFlag = false;
                int w_x = ConvertParam(new byte[] { message[W_X_BEGIN], message[W_X_BEGIN + 1], message[W_X_BEGIN + 2], message[W_X_BEGIN + 3] }, ref rangeFlag, ref dataFlag);
                int w_y = ConvertParam(new byte[] { message[W_Y_BEGIN], message[W_Y_BEGIN + 1], message[W_Y_BEGIN + 2], message[W_Y_BEGIN + 3] }, ref rangeFlag, ref dataFlag);
                int w_z = ConvertParam(new byte[] { message[W_Z_BEGIN], message[W_Z_BEGIN + 1], message[W_Z_BEGIN + 2], message[W_Z_BEGIN + 3] }, ref rangeFlag, ref dataFlag);
                int[] w = new int[] { w_x, w_y, w_z };
                // данные с акселерометров
                int a_x = ConvertParam(new byte[] { message[A_X_BEGIN], message[A_X_BEGIN + 1], message[A_X_BEGIN + 2], message[A_X_BEGIN + 3] }, ref rangeFlag, ref dataFlag);
                int a_y = ConvertParam(new byte[] { message[A_Y_BEGIN], message[A_Y_BEGIN + 1], message[A_Y_BEGIN + 2], message[A_Y_BEGIN + 3] }, ref rangeFlag, ref dataFlag);
                int a_z = ConvertParam(new byte[] { message[A_Z_BEGIN], message[A_Z_BEGIN + 1], message[A_Z_BEGIN + 2], message[A_Z_BEGIN + 3] }, ref rangeFlag, ref dataFlag);
                int[] a = new int[] { a_x, a_y, a_z };
                //данные о температуре
                if (packetId < 3)
                {
                    int u_x = ConvertParam(new byte[] { message[U_X_BEGIN], message[U_X_BEGIN + 1], message[U_X_BEGIN + 2], message[U_X_BEGIN + 3] }, ref rangeFlag, ref dataFlag);
                    int u_y = ConvertParam(new byte[] { message[U_Y_BEGIN], message[U_Y_BEGIN + 1], message[U_Y_BEGIN + 2], message[U_Y_BEGIN + 3] }, ref rangeFlag, ref dataFlag);
                    int u_z = ConvertParam(new byte[] { message[U_Z_BEGIN], message[U_Z_BEGIN + 1], message[U_Z_BEGIN + 2], message[U_Z_BEGIN + 3] }, ref rangeFlag, ref dataFlag);
                    int[] u = new int[] { u_x, u_y, u_z };
                    packet_inf = new Packet(w, a, u, packetId);
                }
                else
                {
                    packet_inf = new Packet(w, a, packetId);
                }
                return packet_inf;
            }
            catch (Exception)
            {                
            }
            return null;
        }

        private static int ConvertParam(byte[] bytes, ref bool rangeFlag, ref bool dataFlag)
        {
            bool sign = true;
            //byte[] reversBits = ReversBits(bytes);
            int res = BitConverter.ToInt32(bytes, 0);
            if ((res & int.MinValue) == int.MinValue)
                sign = false;
            if ((res & 3) == 2 || (res & 3) == 1)
                rangeFlag = true;
            if ((res & 3) == 3)
                dataFlag = true;
            res = res >> 3;
            if (sign)
                return -res;
            else
                return res;
        }

        private static byte[] ReversBits(byte[] bytes)
        {
            var array = new BitArray(bytes);
            for (int i = 0; i < array.Length / 2; i++)
            {
                bool temp = array[i];
                array[i] = array[array.Length - 1 - i];
                array[array.Length - 1 - i] = temp;
            }
            var result = new byte[4];
            array.CopyTo(result, 0);
            return result;
        }

         /// <summary>
        /// Метод поиска байтов, соотвествующих началу пакета 
        /// </summary>
        /// <param name="buffer"> полученные байты</param>
        /// <returns> найдено ли начало </returns>
        public static bool FindPacketBegin(ref List<byte> buffer)
        {
            if (buffer.Count < 2)
            {
                return false;
            }
            byte first = buffer[0];
            byte second = buffer[1];
            int i = 2;
            while (BitConverter.ToUInt16(new byte[] { first, second }, 0) != 0xFACE && buffer.Count > i)
            {
                first = second;
                second = buffer[i];
                i++;
            }
            if (i == buffer.Count)
            {
                buffer.RemoveRange(0, i - 1);
                //Начало пакета не найдено
                return false;
            }
            else
            {
                i -= 2;
                buffer.RemoveRange(0, i);
                return true;
            }
        }
    }
}
