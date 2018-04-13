using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Evo_20form
{
    class ControllerData
    {
        struct DLYProfilePart
        {
            public static int[] INDEXES = { 24, 48, 72 };
            public int axisX;
            public int axisY;
            public DLYProfilePart(int x, int y)
            {
                axisX = x;
                axisY = y;
            }
        }
 
        struct DYSProfilePart
        {
            public static  int[] INDEXES = {0,1,7,13,14,20,26,27,33,39};
            public static  int[] COLL_SPEED = {2,8,16,64,112,128};
            public static int[] CHECK_SPEED ={1,5,21,43,75,115};
            public int axisX;
            public int axisY;
            public int speedX;
            public int speedY;
            public DYSProfilePart(int x, int y, int speedX, int speedY)
            {
                axisX = x;
                axisY = y;
                this.speedX = speedX;
                this.speedY = speedY;
            }
        }

        Dictionary<int, PacketsData> colibrationTemperatureAndPacketDataDLY;

        Dictionary<int, PacketsData> checkTemperatureAndPacketDataDLY;

        Dictionary<int, PacketsData> colibrationTemperatureAndPacketDataDYS;

        Dictionary<int, PacketsData> checkTemperatureAndPacketDataDYS;

        int colibrationRequiredAmountOfPacketsDLY;

        int colibrationRequiredAmountOfPacketsDYS;

        int checkRequiredAmountOfPacketsDLY;

        int checkRequiredAmountOfPacketsDYS;

        int stabilizationTime;

        int axisXCorrection;

        int axisYCorrection;

        DLYProfilePart[] profileDLYCollibration;

        DYSProfilePart[] profileDYSCollibration;

        
        public ControllerData()
        {
            colibrationTemperatureAndPacketDataDLY = new Dictionary<int, PacketsData>();
            checkTemperatureAndPacketDataDLY = new Dictionary<int, PacketsData>();
            colibrationTemperatureAndPacketDataDYS = new Dictionary<int, PacketsData>();
            checkTemperatureAndPacketDataDYS = new Dictionary<int, PacketsData>();
            profileDLYCollibration = GetDLYCollibrationProfile();
            profileDYSCollibration = GetDYSCollibrationProfile();
        }
        public bool ReadSettings(StreamReader file)
        {
            bool isSuccess = ReadParamFromFile(file, "Время стабилизации температуры в режиме проверка", ref stabilizationTime);
            if (!isSuccess)
                return false;
            isSuccess = ReadParamFromFile(file, 
                "Количество пакетов для расчета средних кодов АЦП ДЛУ в режиме калибровка", 
                ref  colibrationRequiredAmountOfPacketsDLY);
            if (!isSuccess)
                return false;
            isSuccess = ReadParamFromFile(file,
                "Количество пакетов для расчета средних кодов АЦП ДУС в режиме калибровка",
                ref  colibrationRequiredAmountOfPacketsDYS);
            if (!isSuccess)
                return false;
            isSuccess = ReadParamFromFile(file,
                "Количество пакетов для расчета средних кодов АЦП ДЛУ в режиме проверка",
                ref  checkRequiredAmountOfPacketsDLY);
            if (!isSuccess)
                return false;
            isSuccess = ReadParamFromFile(file,
                "Количество пакетов для расчета средних кодов АЦП ДУС в режиме проверка",
                ref  checkRequiredAmountOfPacketsDYS);
            if (!isSuccess)
                return false;
            isSuccess = ReadParamFromFile(file,
                "Поправка по оси стола №1",
                ref  axisXCorrection);
            if (!isSuccess)
                return false;
            isSuccess = ReadParamFromFile(file,
                "Поправка по оси стола №2",
                ref  axisYCorrection);
            if (!isSuccess)
                return false;
            return true;
        }

        private DLYProfilePart[] GetDLYCollibrationProfile()
        {
            DLYProfilePart[] profile = new DLYProfilePart[DLYProfilePart.INDEXES[DLYProfilePart.INDEXES.Length-1]];
            for (int i = 0; i < DLYProfilePart.INDEXES[0]; i++)
			{
                profile[i] = new DLYProfilePart(i * 15, 0);
			}
            for (int i = DLYProfilePart.INDEXES[0]; i < DLYProfilePart.INDEXES[1]; i++)
			{
                profile[i] = new DLYProfilePart((i - 24)*15, 90);
			}
            for (int i = DLYProfilePart.INDEXES[1]; i < profile.Length; i++)
			{
                profile[i] = new DLYProfilePart(-90, (i - 54) * 15);
			}
            return profile;
        }

        private DYSProfilePart[] GetDYSCollibrationProfile()
        {
            DYSProfilePart[] profile = new DYSProfilePart[DYSProfilePart.INDEXES[DYSProfilePart.INDEXES.Length-1]];

            profile[0] = new DYSProfilePart(0, 0, 0, 0);

            for (int i = DYSProfilePart.INDEXES[1]; i < DYSProfilePart.INDEXES[2]; i++)
            {
                profile[i] = new DYSProfilePart(0, 0, 0, DYSProfilePart.COLL_SPEED[i - DYSProfilePart.INDEXES[1]]);
            }
            for (int i = DYSProfilePart.INDEXES[2]; i < DYSProfilePart.INDEXES[3]; i++)
            {
                profile[i] = new DYSProfilePart(0, 0, 0, -DYSProfilePart.COLL_SPEED[i - DYSProfilePart.INDEXES[2]]);
            }

            profile[DYSProfilePart.INDEXES[3]] = new DYSProfilePart(0, 0, 0, 0);

            for (int i = DYSProfilePart.INDEXES[4]; i < DYSProfilePart.INDEXES[5]; i++)
            {
                profile[i] = new DYSProfilePart(0, 0, -DYSProfilePart.COLL_SPEED[i - DYSProfilePart.INDEXES[4]], 0);
            }

            for (int i = DYSProfilePart.INDEXES[5]; i < DYSProfilePart.INDEXES[6]; i++)
            {
                profile[i] = new DYSProfilePart(0, 0, DYSProfilePart.COLL_SPEED[i - DYSProfilePart.INDEXES[5]], 0);
            }

            profile[DYSProfilePart.INDEXES[6]] = new DYSProfilePart(0, 90, 0, 0);

            for (int i = DYSProfilePart.INDEXES[7]; i < DYSProfilePart.INDEXES[8]; i++)
            {
                profile[i] = new DYSProfilePart(0, 0, DYSProfilePart.COLL_SPEED[i - DYSProfilePart.INDEXES[7]], 0);
            }
            for (int i = DYSProfilePart.INDEXES[8]; i < DYSProfilePart.INDEXES[9]; i++)
            {
                profile[i] = new DYSProfilePart(0, 0, -DYSProfilePart.COLL_SPEED[i - DYSProfilePart.INDEXES[8]], 0);
            }

            return profile;
        }

        private DLYProfilePart[] GetDLYCheckProfile()
        {
            DLYProfilePart[] profile = new DLYProfilePart[DLYProfilePart.INDEXES[DLYProfilePart.INDEXES.Length-1]];
            for (int i = 0; i < DLYProfilePart.INDEXES[0]; i++)
			{
                profile[i] = new DLYProfilePart(i * 15, 45);
			}
            for (int i = DLYProfilePart.INDEXES[0]; i < DLYProfilePart.INDEXES[1]; i++)
			{
                profile[i] = new DLYProfilePart((i - 24)*15, -45);
			}
            for (int i = DLYProfilePart.INDEXES[1]; i < profile.Length; i++)
			{
                profile[i] = new DLYProfilePart(-45, (i - 54) * 15);
			}
            return profile;       
        }

        private DYSProfilePart[] GetDYSCheckProfile()
        {
            DYSProfilePart[] profile = new DYSProfilePart[DYSProfilePart.INDEXES[DYSProfilePart.INDEXES.Length - 1]];

            profile[0] = new DYSProfilePart(0, 0, 0, 0);

            for (int i = DYSProfilePart.INDEXES[1]; i < DYSProfilePart.INDEXES[2]; i++)
            {
                profile[i] = new DYSProfilePart(0, 0, 0, DYSProfilePart.CHECK_SPEED[i - DYSProfilePart.INDEXES[1]]);
            }
            for (int i = DYSProfilePart.INDEXES[2]; i < DYSProfilePart.INDEXES[3]; i++)
            {
                profile[i] = new DYSProfilePart(0, 0, 0, -DYSProfilePart.CHECK_SPEED[i - DYSProfilePart.INDEXES[2]]);
            }

            profile[DYSProfilePart.INDEXES[3]] = new DYSProfilePart(0, 0, 0, 0);

            for (int i = DYSProfilePart.INDEXES[4]; i < DYSProfilePart.INDEXES[5]; i++)
            {
                profile[i] = new DYSProfilePart(0, 0, -DYSProfilePart.CHECK_SPEED[i - DYSProfilePart.INDEXES[4]], 0);
            }

            for (int i = DYSProfilePart.INDEXES[5]; i < DYSProfilePart.INDEXES[6]; i++)
            {
                profile[i] = new DYSProfilePart(0, 0, DYSProfilePart.CHECK_SPEED[i - DYSProfilePart.INDEXES[5]], 0);
            }

            profile[DYSProfilePart.INDEXES[6]] = new DYSProfilePart(0, 90, 0, 0);

            for (int i = DYSProfilePart.INDEXES[7]; i < DYSProfilePart.INDEXES[8]; i++)
            {
                profile[i] = new DYSProfilePart(0, 0, DYSProfilePart.CHECK_SPEED[i - DYSProfilePart.INDEXES[7]], 0);
            }
            for (int i = DYSProfilePart.INDEXES[8]; i < DYSProfilePart.INDEXES[9]; i++)
            {
                profile[i] = new DYSProfilePart(0, 0, -DYSProfilePart.CHECK_SPEED[i - DYSProfilePart.INDEXES[8]], 0);
            }
            return profile;
        }

        public bool ReadParamFromFile(StreamReader file, string checkString,ref int param)
        {
            String templine = file.ReadLine();
            string [] temp = templine.Split(':');
            if (temp[0] !=checkString)
            {
                Log.WriteLog("Не верна строка файла настроек:" + checkString);
                return false;
            }
            else
            {
                try
                {
                    param = Convert.ToInt32(temp[1]);
                }
                catch (Exception)
                {
                    Log.WriteLog("Не верна строка файла настроек:" + checkString);
                    return false;
                }
            }
            return true;
        }
        /*
        public bool AddNewCollibrationDataDLY(int temperature,PacketsData data)
        {
            if (colibrationTemperatureAndPacketData.Count < colibrationRequiredAmountOfPacketsDLY)
            {
                colibrationTemperatureAndPacketData.Add(temperature, data);
                return true;
            }
            else
            {
                return false;
            }            
        }

        public bool AddNewCollibrationDataDYS(int temperature, PacketsData data)
        {
            if (colibrationTemperatureAndPacketData.Count < colibrationRequiredAmountOfPacketsDLY)
            {
                colibrationTemperatureAndPacketData.Add(temperature, data);
                return true;
            }
            else
            {
                return false;
            }
        }
         */
    }
}
