using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Configuration;

namespace Evo20.PacketsLib
{
    public class Packet
    {
        #region Constants
        //позиция служебных  байт в пакете
        public static int HEAD_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("HeadBegin"));
        public static int ID_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("IdBegin"));
        public static int CHECK_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("CheckBegin"));
        //позиция информационных байт в пакете
        public static int W_X_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("W_X_Begin"));
        public static int W_Y_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("W_Y_Begin"));
        public static int W_Z_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("W_Z_Begin"));

        public static int A_X_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("A_X_Begin"));
        public static int A_Y_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("A_Y_Begin"));
        public static int A_Z_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("A_Z_Begin"));

        public static int U_X_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("U_X_Begin"));
        public static int U_Y_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("U_Y_Begin"));
        public static int U_Z_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("U_Z_Begin"));

        public static int PARAMS_COUNT = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ParamsCount"));
        public static int PACKET_SIZE = Convert.ToInt32(ConfigurationManager.AppSettings.Get("PacketSize"));

        const int W_STRING_NUMBER = 1;
        const int A_STRING_NUMBER = 4;
        const int U_STRING_NUMBER = 7;

        #endregion

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
        /// <summary>
        /// Конструктор класса пакет (id 1 и 2 )
        /// </summary>
        /// <param name="w"> массив w(гироскоп)</param>
        /// <param name="a">массив a(акселерометр)</param>
        /// <param name="u">массив u(термодатчик)</param>
        /// <param name="id">идентификатор пакета </param>
        public Packet(int[] w, int[] a, int[] u, int id)
        {
            this.w = w;
            this.a = a;
            this.u = u;
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
            this.w = w;
            this.a = a;
        }

        /// <summary>
        /// Преобразует пакет в читаемую строку 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string buffer = string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9}",id,w[0],w[1],w[2],a[0],a[1],a[2],u[0],u[1],u[2]);
            return buffer;
        }

        /// <summary>
        /// Не работает пока 
        /// </summary>
        /// <param name="input"></param>
        public Packet(string input)
        {
            while (input[0] == ' ')
                input=input.Remove(0, 1);
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
                if (rangeFlag || dataFlag)
                {
                    return packet_inf;
                }
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
            int res = BitConverter.ToInt32(bytes, 0);
            if ((res & int.MinValue) == int.MinValue)
                sign = false;
            if ((res & 3) == 2 || (res & 3) == 1)
                rangeFlag = true;
            if ((res & 3) == 3)
                dataFlag = true;
            res=res >> 2;
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
