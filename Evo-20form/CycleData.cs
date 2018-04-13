using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Evo_20form
{
    public struct DLYProfilePart
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

    public struct DYSProfilePart
    {
        public static int[] INDEXES = { 0, 1, 7, 13, 14, 20, 26, 27, 33, 39 };
        public static int[] COLL_SPEED = { 2, 8, 16, 64, 112, 128 };
        public static int[] CHECK_SPEED = { 1, 5, 21, 43, 75, 115 };
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

    class CycleData : Data
    {
        public const int PROFILE_DLY_LENGHT = 72;
        public const int PROFILE_DYS_LENGHT = 39; 

        public List<int> calibration_temperatures
        {
            get;
            private set;
        }

        public List<int> check_temperatures
        {
            get;
            private set;
        }

        public int calibration_stab_time;

        public int check_stab_time;

        public CycleData()
        {
            calibration_temperatures = new List<int>();
            check_temperatures = new List<int>(); 
        }

        public override bool ReadSettings(ref StreamReader file)
        {            
            int calibration_temperature_count = 0;
            bool isSuccess = ReadParamFromFile(ref file, "Число температур калибровки", ref calibration_temperature_count);
            if(!isSuccess)
                return false;
            for (int i = 0; i < calibration_temperature_count; i++)
            {
                int temperature=0;
                isSuccess = ReadParamFromFile(ref file, "Температура калибровки №" + (i + 1), ref temperature);
                if (!isSuccess)
                    return false;
                calibration_temperatures.Add(temperature);             
            }
            isSuccess = ReadParamFromFile(ref file, "Время стабилизации температуры в режиме калибровка", ref calibration_stab_time);
            if (!isSuccess)
                return false;
            int check_temperature_count = 0;
            isSuccess = ReadParamFromFile(ref file, "Число температур проверки", ref  check_temperature_count);
            if (!isSuccess)
                return false;
            for (int i = 0; i < check_temperature_count; i++)
            {
                int temperature=0;
                isSuccess = ReadParamFromFile(ref file, "Температура проверки №" + (i + 1), ref  temperature);   
                if (!isSuccess)
                    return false;
                check_temperatures.Add(temperature);
 
            }
            isSuccess = ReadParamFromFile(ref file, "Время стабилизации температуры в режиме проверка", ref check_stab_time);
            if (!isSuccess)
                return false;
            return true;

        }

        public int[] GetCollibrationTemperatures()
        {
            int [] result= new int[calibration_temperatures.Count];
            calibration_temperatures.CopyTo(result);
            return result;
        }

        public int[] GetCheckTemperatures()
        {
            int[] result = new int[calibration_temperatures.Count];
            calibration_temperatures.CopyTo(result);
            return result;
        }

        public int FindCalibrationTemperatureIndex(int temperature)
        {
            for (int i = 0; i < calibration_temperatures.Count; i++)
            {
                if (temperature == calibration_temperatures[i])
                    return i;
            }
            return -1;
        }

        public int FindCheckTemperatureIndex(int temperature)
        {
            for (int i = 0; i < check_temperatures.Count; i++)
            {
                if (temperature == check_temperatures[i])
                    return i;
            }
            return -1;
        }

        public static DLYProfilePart[] GetDLYCalibrationProfile()
        {
            DLYProfilePart[] profile = new DLYProfilePart[DLYProfilePart.INDEXES[DLYProfilePart.INDEXES.Length - 1]];
            for (int i = 0; i < DLYProfilePart.INDEXES[0]; i++)
            {
                profile[i] = new DLYProfilePart(i * 15, 0);
            }
            for (int i = DLYProfilePart.INDEXES[0]; i < DLYProfilePart.INDEXES[1]; i++)
            {
                profile[i] = new DLYProfilePart((i - 24) * 15, 90);
            }
            for (int i = DLYProfilePart.INDEXES[1]; i < profile.Length; i++)
            {
                profile[i] = new DLYProfilePart(-90, (i - 54) * 15);
            }
            return profile;
        }

        public static DYSProfilePart[] GetDYSCalibrationProfile()
        {
            DYSProfilePart[] profile = new DYSProfilePart[DYSProfilePart.INDEXES[DYSProfilePart.INDEXES.Length - 1]];

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

        public static DLYProfilePart[] GetDLYCheckProfile()
        {
            DLYProfilePart[] profile = new DLYProfilePart[DLYProfilePart.INDEXES[DLYProfilePart.INDEXES.Length - 1]];
            for (int i = 0; i < DLYProfilePart.INDEXES[0]; i++)
            {
                profile[i] = new DLYProfilePart(i * 15, 45);
            }
            for (int i = DLYProfilePart.INDEXES[0]; i < DLYProfilePart.INDEXES[1]; i++)
            {
                profile[i] = new DLYProfilePart((i - 24) * 15, -45);
            }
            for (int i = DLYProfilePart.INDEXES[1]; i < profile.Length; i++)
            {
                profile[i] = new DLYProfilePart(-45, (i - 54) * 15);
            }
            return profile;
        }

        public static DYSProfilePart[] GetDYSCheckProfile()
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
    }
}
