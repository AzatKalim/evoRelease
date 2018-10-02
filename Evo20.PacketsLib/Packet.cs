﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Configuration;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

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
                A = new int[PARAMS_COUNT];
                W = new int[PARAMS_COUNT];
                U = new int[PARAMS_COUNT];
                id = Convert.ToInt32(tmp[0]);
                for (int i = 0; i < PARAMS_COUNT; i++)
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
                int w_x = ConvertParam(new byte[] { data[W_X_BEGIN], data[W_X_BEGIN + 1], data[W_X_BEGIN + 2], data[W_X_BEGIN + 3] }, ref rangeFlag, ref dataFlag);
                int w_y = ConvertParam(new byte[] { data[W_Y_BEGIN], data[W_Y_BEGIN + 1], data[W_Y_BEGIN + 2], data[W_Y_BEGIN + 3] }, ref rangeFlag, ref dataFlag);
                int w_z = ConvertParam(new byte[] { data[W_Z_BEGIN], data[W_Z_BEGIN + 1], data[W_Z_BEGIN + 2], data[W_Z_BEGIN + 3] }, ref rangeFlag, ref dataFlag);
                this.W = new int[] { w_x, w_y, w_z };
                // данные с акселерометров
                int a_x = ConvertParam(new byte[] { data[A_X_BEGIN], data[A_X_BEGIN + 1], data[A_X_BEGIN + 2], data[A_X_BEGIN + 3] }, ref rangeFlag, ref dataFlag);
                int a_y = ConvertParam(new byte[] { data[A_Y_BEGIN], data[A_Y_BEGIN + 1], data[A_Y_BEGIN + 2], data[A_Y_BEGIN + 3] }, ref rangeFlag, ref dataFlag);
                int a_z = ConvertParam(new byte[] { data[A_Z_BEGIN], data[A_Z_BEGIN + 1], data[A_Z_BEGIN + 2], data[A_Z_BEGIN + 3] }, ref rangeFlag, ref dataFlag);
                this.A = new int[] { a_x, a_y, a_z };
                //данные о температуре
                if (id < 3)
                {
                    int u_x = ConvertParam(new byte[] { data[U_X_BEGIN], data[U_X_BEGIN + 1], data[U_X_BEGIN + 2], data[U_X_BEGIN + 3] }, ref rangeFlag, ref dataFlag);
                    int u_y = ConvertParam(new byte[] { data[U_Y_BEGIN], data[U_Y_BEGIN + 1], data[U_Y_BEGIN + 2], data[U_Y_BEGIN + 3] }, ref rangeFlag, ref dataFlag);
                    int u_z = ConvertParam(new byte[] { data[U_Z_BEGIN], data[U_Z_BEGIN + 1], data[U_Z_BEGIN + 2], data[U_Z_BEGIN + 3] }, ref rangeFlag, ref dataFlag);
                    this.U = new int[] { u_x, u_y, u_z };
                }
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
                ushort head = BitConverter.ToUInt16(new byte[] { message[Packet.HEAD_BEGIN], message[HEAD_BEGIN + 1] }, 0);
                if (head != 0xFACE)
                    return null;
                int checkSum = CRC16.ComputeChecksum(message, Packet.PACKET_SIZE);
                //проверка контрольной суммы
                if (BitConverter.ToUInt16(new byte[2] { message[CHECK_BEGIN], message[CHECK_BEGIN + 1] }, 0) != checkSum)
                    return null;
                //номер пакета 
                int packetId = BitConverter.ToUInt16(new byte[] { message[ID_BEGIN], message[ID_BEGIN + 1] }, 0);
                return new Packet(packetId, message);
            }
            catch (Exception)
            {
                return null;
            }

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
