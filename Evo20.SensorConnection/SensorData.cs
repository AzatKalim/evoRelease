using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Evo20.SensorsConnection
{
    /// <summary>
    /// Класс хранящий информацию о датчиках
    /// </summary>
    public class SensorData : AbstractData
    {
        #region Properties

        public int CalibrationDLYMaxPacketsCount;

        public int CalibrationDYSMaxPacketsCount;

        public int CheckDLYMaxPacketsCount;

        public int CheckDYSMaxPacketsCount;
    
        #endregion 
     
        #region File work

        /// <summary>
        /// Чтение настроек из файла
        /// </summary>
        /// <param name="file">файл</param>
        /// <returns>true- чтени заверешено, false- возникла ошибка чтения</returns>
        public override bool ReadSettings(ref StreamReader file)
        {
            bool isSuccess = ReadParamFromFile(ref file,
                "Количество пакетов для расчета средних кодов АЦП ДЛУ в режиме калибровка",
                ref  CalibrationDLYMaxPacketsCount);
            if (!isSuccess)
                return false;
            isSuccess = ReadParamFromFile(ref file,
                "Количество пакетов для расчета средних кодов АЦП ДУС в режиме калибровка",
                ref  CalibrationDYSMaxPacketsCount);
            if (!isSuccess)
                return false;
            isSuccess = ReadParamFromFile(ref file,
                "Количество пакетов для расчета средних кодов АЦП ДЛУ в режиме проверка",
                ref  CheckDLYMaxPacketsCount);
            if (!isSuccess)
                return false;
            isSuccess = ReadParamFromFile(ref file,
                "Количество пакетов для расчета средних кодов АЦП ДУС в режиме проверка",
                ref  CheckDYSMaxPacketsCount);
            if (!isSuccess)
                return false;
            return true;
        }

        //public void WritePackets(StreamWriter file, PacketsCollection[] data)
        //{
        //    file.WriteLine(data.Length);

        //    for (int j = 0; j < data.Length; j++)
        //    {
        //        file.WriteLine(data[j].ToString());                
        //    }
        //}

        //public bool WriteAllPackets(StreamWriter file)
        //{
        //    if (calibrationDLYPacketsCollection==null || calibrationDLYPacketsCollection.Length == 0)
        //    {
        //        file.WriteLine("Нет пакетов по калибровке ДЛУ");
        //    }
        //    else
        //    {
        //        WritePackets(file, calibrationDLYPacketsCollection);
        //    }
        //    if (calibrationDYSPacketsCollection==null || calibrationDYSPacketsCollection.Length == 0)
        //    {
        //        file.WriteLine("Нет пакетов по калибровке ДУС");
        //    }
        //    else
        //    {
        //        WritePackets(file, calibrationDYSPacketsCollection);
        //    }
        //    if (checkDLYPacketsCollection==null || checkDLYPacketsCollection.Length == 0)
        //    {
        //        file.WriteLine("Нет пакетов по Проверка ДЛУ");
        //    }
        //    else
        //    {
        //        WritePackets(file, checkDLYPacketsCollection);
        //    }
        //    if (checkDYSPacketsCollection==null || checkDYSPacketsCollection.Length == 0)
        //    {
        //        file.WriteLine("Нет пакетов по Проверка ДУС");
        //    }
        //    else
        //    {
        //        WritePackets(file, checkDYSPacketsCollection);
        //    }
        //    return true;
        //}

        //public bool WriteAllPackets()
        //{
        //    StreamWriter file = new StreamWriter("allpackets.txt");
        //    return WriteAllPackets(file);
        //}

        //private bool ReadDataFromFile(StreamReader file,PacketsCollection[] collection)
        //{
        //    int temperaturesCount = Convert.ToInt32(file.ReadLine());
        //    collection = new PacketsCollection[temperaturesCount];
        //    for (int i = 0; i < temperaturesCount; i++)
        //    {
        //        try
        //        {
        //            collection[i] = new PacketsCollection(file);
        //        }
        //        catch (Exception exception)
        //        {
        //            Log.WriteLog("Ошибка: Чтение файла данных пакетов при температуре" + i + " " + exception);
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        //public bool ReadDataFromFile(StreamReader file)
        //{
        //    if (!ReadDataFromFile(file, calibrationDLYPacketsCollection))
        //    {
        //        return false;
        //    }
        //    if(!ReadDataFromFile(file,calibrationDYSPacketsCollection))
        //    {
        //        return false;
        //    }
        //    if (!ReadDataFromFile(file, checkDLYPacketsCollection))
        //    {
        //        return false;
        //    }
        //    if (!ReadDataFromFile(file, checkDYSPacketsCollection))
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        #endregion
    }
}
