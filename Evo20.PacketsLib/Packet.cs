using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Evo20.Evo20.Packets
{
    public class Packet
    {
        #region Constants    
        const int W_STRING_NUMBER = 1;
        const int A_STRING_NUMBER = 4;
        const int U_STRING_NUMBER = 7;

        #endregion
        public byte[] data;

        private static byte[] inf_part = new byte[40];

        private int[] w;
        private int[] a;
        private int[] u;
        public int[] W
        {
            get
            {
                if (w == null)
                    PacketHandle();
                return w;
            }
            private set
            {
                w = value;
            }
        }
        public int[] A
        {
            get
            {
                if (a == null)
                    PacketHandle();
                return a;
            }
            private set
            {
                a = value;
            }
        }
        public int[] U
        {
            get
            {
                if (u == null)
                    PacketHandle();
                return u;
            }
            internal set
            {
                u = value;
            }
        }
        public byte[] array;
        public int id
        {
            get;
            private set;
        }
        /// <summary>
        /// Конструктор класса пакет (id 1 и 2 )
        /// </summary>
        /// <param name="w"> массив w(гироскоп)</param>
        /// <param name="a">массив a(акселерометр)</param>
        /// <param name="u">массив u(термодатчик)</param>
        /// <param name="id">идентификатор пакета </param>
        public Packet(int[] w, int[] a, int[] u, int id)
        {
            this.W = w;
            this.A = a;
            this.U = u;
            this.id = id;
        }

        /// <summary>
        /// Конструктор класса пакет(для id 3 и 4) 
        /// </summary>
        /// <param name="w"> массив w(гироскоп)</param>
        /// <param name="a">массив a(акселерометр)</param>
        /// <param name="id">идентификатор пакета </param>
        public Packet(int[] w, int[] a, int id)
        {
            this.id = id;
            this.W = w;
            this.A = a;
        }

        public Packet(int id,byte[] array)
        {
            this.id = id;
            this.data = array;
        }

        /// <summary>
        /// Преобразует пакет в читаемую строку 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string buffer = string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9}",id,W[0],W[1],W[2],A[0],A[1],A[2],U[0],U[1],U[2]);
            return buffer;
        }

        /// <summary>
        /// Не работает пока 
        /// </summary>
        /// <param name="input"></param>
        public Packet(string input)
        {
            string[] tmp = input.Split(' ');
            try
            {
                A = new int[Config.PARAMS_COUNT];
                W = new int[Config.PARAMS_COUNT];
                U = new int[Config.PARAMS_COUNT];
                id = Convert.ToInt32(tmp[0]);
                for (int i = 0; i < Config.PARAMS_COUNT; i++)
                {
                    W[i] = Convert.ToInt32(tmp[W_STRING_NUMBER + i]);
                    A[i] = Convert.ToInt32(tmp[A_STRING_NUMBER + i]);
                    U[i] = Convert.ToInt32(tmp[U_STRING_NUMBER + i]);
                }
            }
            catch (FormatException exception)
            {
                A = null;
                W = null;
                U = null;
                throw exception;
            }
        }

        /// <summary>
        /// Метод извлекающий Packet из массива байт
        /// </summary>
        /// <param name="message"> массив байт</param>
        /// <returns>пакет</returns>
        private void PacketHandle()
        {
            try
            {
                //проверка заголовка пакета        
                //данные с гироскопов
                bool rangeFlag = false;
                bool dataFlag = false;
                int w_x = ConvertParam(new byte[] { data[Config.W_X_BEGIN], data[Config.W_X_BEGIN + 1], data[Config.W_X_BEGIN + 2], data[Config.W_X_BEGIN + 3] }, ref rangeFlag, ref dataFlag);
                int w_y = ConvertParam(new byte[] { data[Config.W_Y_BEGIN], data[Config.W_Y_BEGIN + 1], data[Config.W_Y_BEGIN + 2], data[Config.W_Y_BEGIN + 3] }, ref rangeFlag, ref dataFlag);
                int w_z = ConvertParam(new byte[] { data[Config.W_Z_BEGIN], data[Config.W_Z_BEGIN + 1], data[Config.W_Z_BEGIN + 2], data[Config.W_Z_BEGIN + 3] }, ref rangeFlag, ref dataFlag);
                this.W = new int[] { w_x, w_y, w_z };
                // данные с акселерометров
                int a_x = ConvertParam(new byte[] { data[Config.A_X_BEGIN], data[Config.A_X_BEGIN + 1], data[Config.A_X_BEGIN + 2], data[Config.A_X_BEGIN + 3] }, ref rangeFlag, ref dataFlag);
                int a_y = ConvertParam(new byte[] { data[Config.A_Y_BEGIN], data[Config.A_Y_BEGIN + 1], data[Config.A_Y_BEGIN + 2], data[Config.A_Y_BEGIN + 3] }, ref rangeFlag, ref dataFlag);
                int a_z = ConvertParam(new byte[] { data[Config.A_Z_BEGIN], data[Config.A_Z_BEGIN + 1], data[Config.A_Z_BEGIN + 2], data[Config.A_Z_BEGIN + 3] }, ref rangeFlag, ref dataFlag);
                this.A = new int[] { a_x, a_y, a_z };
                //данные о температуре
                if (id < 3)
                {
                    int u_x = ConvertParam(new byte[] { data[Config.U_X_BEGIN], data[Config.U_X_BEGIN + 1], data[Config.U_X_BEGIN + 2], data[Config.U_X_BEGIN + 3] }, ref rangeFlag, ref dataFlag);
                    int u_y = ConvertParam(new byte[] { data[Config.U_Y_BEGIN], data[Config.U_Y_BEGIN + 1], data[Config.U_Y_BEGIN + 2], data[Config.U_Y_BEGIN + 3] }, ref rangeFlag, ref dataFlag);
                    int u_z = ConvertParam(new byte[] { data[Config.U_Z_BEGIN], data[Config.U_Z_BEGIN + 1], data[Config.U_Z_BEGIN + 2], data[Config.U_Z_BEGIN + 3] }, ref rangeFlag, ref dataFlag);
                    this.U = new int[] { u_x, u_y, u_z };
                }
                data = null;
            }
            catch (Exception)
            {                
            }
        }

        public static Packet FirstPacketHandle(byte[] message)
        {
            try
            {
                //проверка заголовка пакета
                ushort head = BitConverter.ToUInt16(new byte[] { message[Config.HEAD_BEGIN], message[Config.HEAD_BEGIN + 1] }, 0);
                if (head != 0xFACE)
                    return null;
                int checkSum = CRC16.ComputeChecksum(message, Config.PACKET_SIZE);
                //проверка контрольной суммы
                if (BitConverter.ToUInt16(new byte[2] { message[Config.CHECK_BEGIN], message[Config.CHECK_BEGIN + 1] }, 0) != checkSum)
                    return null;
                //номер пакета 
                int packetId = BitConverter.ToUInt16(new byte[] { message[Config.ID_BEGIN], message[Config.ID_BEGIN + 1] }, 0);
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
                //Начало пакета не найдено
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
