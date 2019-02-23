using System.Collections.Generic;
using System.IO;

namespace Evo20.Controllers.Data
{
    public class CycleData : AbstractData
    {
        public List<int> CalibrationTemperatures {get;set;}

        public List<int> CheckTemperatures {get;set;}

        public int TemperutureIndex { get; set; } = 0;

        public int CalibrationStabTime;

        public int CheckStabTime;

        public int MaxTemperatureFromSettings;

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
            int checkTemperatureCount = 0;
            isSuccess = ReadParamFromFile(ref file, "Число температур проверки", ref  checkTemperatureCount);
            if (!isSuccess)
                return false;
            for (int i = 0; i < checkTemperatureCount; i++)
            {
                int temperature=0;
                isSuccess = ReadParamFromFile(ref file, "Температура проверки №" + (i + 1), ref  temperature);   
                if (!isSuccess)
                    return false;
                CheckTemperatures.Add(temperature);
 
            }
            temp = 0;
            isSuccess = ReadParamFromFile(ref file, "Время стабилизации температуры в режиме проверка", ref temp);
            if (!isSuccess)
                return false;

            //Перевод минут в миллисекунды 
            CheckStabTime=(int)temp*60 * 1000;
            return true;

        }
     
        public bool IsFullCycle()
        {
            if (MaxTemperatureFromSettings == CalibrationTemperatures.Count)
                return true;
            else
            {
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
