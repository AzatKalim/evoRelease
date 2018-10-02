using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Evo20.Log;
using System.IO;

namespace Evo20.SensorsConnection
{
    public abstract class AbstractData
    {
        /// <summary>
        /// Функция чтения параметров из файла
        /// </summary>
        /// <param name="file"> Файл</param>
        /// <returns> результат чтения</returns>
        public abstract bool ReadSettings(ref  StreamReader file);

        /// <summary>
        /// Функция чтения параметра из файла
        /// </summary>
        /// <param name="file">файла </param>
        /// <param name="checkString">Строка, с которой надо сравнить считаную из файла </param>
        /// <param name="param"> параметр, который находится в конце строки </param>
        /// <returns>результат чтения </returns>
        public bool ReadParamFromFile(ref StreamReader file, string checkString, ref double param)
        {
            String templine = file.ReadLine();
            string[] temp = templine.Split(':');
            if (temp[0] != checkString)
            {
                Evo20.Log.Log.WriteLog("Не верна строка файла настроек:" + checkString);
                return false;
            }
            else
            {
                try
                {
                    param = Convert.ToDouble(temp[1]);
                }
                catch (Exception)
                {
                    Evo20.Log.Log.WriteLog("Не верна строка файла настроек:" + checkString);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Функция чтения параметра из файла
        /// </summary>
        /// <param name="file">файла </param>
        /// <param name="checkString">Строка, с которой надо сравнить считаную из файла </param>
        /// <param name="param"> параметр, который находится в конце строки </param>
        /// <returns>результат чтения </returns>
        public bool ReadParamFromFile(ref StreamReader file, string checkString, ref int param)
        {
            String templine = file.ReadLine();
            string[] temp = templine.Split(':');
            if (temp[0] != checkString)
            {
                Evo20.Log.Log.WriteLog("Не верна строка файла настроек:" + checkString);
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
                    Evo20.Log.Log.WriteLog("Не верна строка файла настроек:" + checkString);
                    return false;
                }
            }
            return true;
        }
    }
}
