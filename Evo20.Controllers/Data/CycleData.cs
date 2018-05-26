using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Evo20.EvoCommandsLib;

namespace Evo20.Controllers
{
    /// <summary>
    /// Класс хранящий информацию о цикле 
    /// </summary>
    public class CycleData : AbstractData
    {
        //Список температур калибровки
        public List<int> CalibrationTemperatures
        {
            get;
            set;
        }

        //Список температур проверки
        public List<int> CheckTemperatures
        {
            get;
            set;
        }

        //Время стабилизации температуры в режиме калибровки
        public int calibrationStabTime;

        //Время стабилизации температуры в режиме проверки
        public int checkStabTime;

        public int MaxTemperatureFromSettings;

        public CycleData()
        {
            CalibrationTemperatures = new List<int>();
            CheckTemperatures = new List<int>(); 
        }

        /// <summary>
        /// Чтение информации из файла настроек
        /// </summary>
        /// <param name="file">файл настроек</param>
        /// <returns>true- чтение выполнно успешно, false- чтени не выполнено</returns>
        public override bool ReadSettings(ref StreamReader file)
        {            
            int calibrationTemperatureCount = 0;
            bool isSuccess = ReadParamFromFile(ref file, "Число температур калибровки", ref calibrationTemperatureCount);
            if(!isSuccess)
                return false;
            for (int i = 0; i < calibrationTemperatureCount; i++)
            {
                int temperature=0;
                isSuccess = ReadParamFromFile(ref file, "Температура калибровки №" + (i + 1), ref temperature);
                if (!isSuccess)
                    return false;
                CalibrationTemperatures.Add(temperature);             
            }
            double temp = 0; ;
            isSuccess = ReadParamFromFile(ref file, "Время стабилизации температуры в режиме калибровка", ref temp);
            if (!isSuccess)
                return false;
            //Перевод минут в миллисекунды 
            calibrationStabTime = (int)temp*60 * 1000;
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
            checkStabTime=(int)temp*60 * 1000;
            return true;

        }

        /// <summary>
        /// Поиск индекса текущей температуры в списке температур
        /// </summary>
        /// <param name="temperature">температура</param>
        /// <returns>индекс</returns>
        public int FindCalibrationTemperatureIndex(int temperature)
        {
            for (int i = 0; i < CalibrationTemperatures.Count; i++)
            {
                if (temperature == CalibrationTemperatures[i])
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Поиск индекса текущей температуры в списке температур
        /// </summary>
        /// <param name="temperature">температура</param>
        /// <returns>индекс</returns>
        public int FindCheckTemperatureIndex(int temperature)
        {
            for (int i = 0; i < CheckTemperatures.Count; i++)
            {
                if (temperature == CheckTemperatures[i])
                    return i;
            }
            return -1;
        }
    }
}
