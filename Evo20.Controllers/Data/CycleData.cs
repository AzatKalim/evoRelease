using System.Collections.Generic;
using System.IO;

namespace Evo20.Controllers.Data
{
    public class CycleData : AbstractData
    {
        public List<int> CalibrationTemperatures {get; private set;}

        public List<int> CheckTemperatures {get; private set;}

        public int TemperutureIndex { get; set; }

        public int CalibrationStabTime;

        public int CheckStabTime;

        private int MaxTemperatureFromSettings;

        public int StartTemperatureIndex = 0;

        private static CycleData _cycleData;

        public static CycleData Instance => _cycleData ?? (_cycleData = new CycleData());

        private CycleData()
        {
            CalibrationTemperatures = new List<int>();
            CheckTemperatures = new List<int>(); 
        }

        public override bool ReadSettings(ref StreamReader file)
        {            
            int calibrationTemperatureCount = 0;
            bool isSuccess = ReadParamFromFile(ref file, "Число температур калибровки", ref calibrationTemperatureCount);
            if(!isSuccess)
                return false;
            MaxTemperatureFromSettings = calibrationTemperatureCount;
            for (int i = 0; i < calibrationTemperatureCount; i++)
            {
                int temperature=0;
                isSuccess = ReadParamFromFile(ref file, "Температура калибровки №" + (i + 1), ref temperature);
                if (!isSuccess)
                    return false;
                CalibrationTemperatures.Add(temperature);             
            }
            double temp = 0;
            isSuccess = ReadParamFromFile(ref file, "Время стабилизации температуры в режиме калибровка", ref temp);
            if (!isSuccess)
                return false;
            //Перевод минут в миллисекунды 
            CalibrationStabTime = (int)temp*60 * 1000;
            temp = 0;
            isSuccess = ReadParamFromFile(ref file, "Время стабилизации температуры в режиме проверка", ref temp);
            if (!isSuccess)
                return false;

            //Перевод минут в миллисекунды 
            CheckStabTime=(int)temp*60 * 1000;
            return true;

        }
     
        public bool IsFullCycle
        {
            get
            {
                if (MaxTemperatureFromSettings == CalibrationTemperatures.Count)
                    return true;
                return false;
            }
        }

        public void SetTemperatures(List<int> list)
        {
            CalibrationTemperatures = list;
            CheckTemperatures = list;
        }
    }
}
