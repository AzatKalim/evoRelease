using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Evo_20form
{
    class Cycle
    {
        List<int> calibration_temperatures;
        List<int> check_temperatures;
        int calibration_stab_time;
        int check_stab_time;

        public Cycle()
        {
            calibration_temperatures = new List<int>();
            check_temperatures = new List<int>();
        }
        public bool EnterTemperature(StreamReader file)
        {
            string templine = file.ReadLine();
            //string temperature_count_line= file.ReadLine();
            string[] temp = templine.Split(':');
            int calibration_temperature_count = 0;
            try
            {
                if (temp[0] != "Число температур калибровки")
                {
                    Log.WriteLog("Не верна строка файла настроек:"+0);
                    return false;
                }
                else
                {
                    calibration_temperature_count = Convert.ToInt32(temp[1]);
                }
            }
            catch (Exception)
            {
                Log.WriteLog("Ошибка: Чтение из файла настроек.Строка номер:"+0);
                return false;
            }
            for (int i = 0; i < calibration_temperature_count; i++)
            {
                try
                {
                    string temperature_line = file.ReadLine();
                    temp = temperature_line.Split(':');
                    if (temp[0] != ("Температура калибровки №" +( i + 1)))
                    {
                        Log.WriteLog("Не верна строка файла настроек:" + i + 1);
                        return false;
                    }
                    else
                    {
                        calibration_temperatures.Add(Convert.ToInt32(temp[1]));
                    }
                }
                catch (Exception)
                {
                    Log.WriteLog("Ошибка: Чтение из файла настроек.Строка номер:" + i + 1);
                    return false;
                }
            }
            templine = file.ReadLine();
            temp = templine.Split(':');
            if (temp[0] != "Время стабилизации температуры в режиме Калибровка")
            {
                Log.WriteLog("Не верна строка файла настроек:" + calibration_temperature_count + 1);
                return false;
            }
            else
            {
                calibration_stab_time = Convert.ToInt32(temp[1]);
            }
            templine = file.ReadLine();
            temp = templine.Split(':');
            int check_temperature_count = 0;
            try
            {
                if (temp[0] != "Число температур проверки")
                {
                    Log.WriteLog("Не верна строка файла настроек:" + 0);
                    return false;
                }
                else
                {
                    check_temperature_count = Convert.ToInt32(temp[1]);
                }
            }
            catch (Exception)
            {
                Log.WriteLog("Ошибка: Чтение из файла настроек.Строка номер:" + calibration_temperature_count+3);
                return false;
            }
            for (int i = 0; i < check_temperature_count; i++)
            {
                try
                {
                    string temperature_line = file.ReadLine();
                    temp = temperature_line.Split(':');
                    if (temp[0] != ("Температура проверки №" + (i + 1)))
                    {
                        Log.WriteLog("Не верна строка файла настроек:" + i  + calibration_temperature_count + 4);
                        return false;
                    }
                    else
                    {
                        check_temperatures.Add(Convert.ToInt32(temp[1]));
                    }
                }
                catch (Exception)
                {
                    Log.WriteLog("Ошибка: Чтение из файла настроек.Строка номер:" + i + calibration_temperature_count + 4);
                    return false;
                }
            }
            return true;

        }

    }
}
