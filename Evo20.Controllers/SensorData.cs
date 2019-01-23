using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Evo20.Evo20.Packets;
using Evo20.Sensors;

namespace Evo20.Controllers
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

        private static SensorData sensorData;

        public static SensorData Instance
        {
            get
            {
                if (sensorData == null)
                    sensorData = new SensorData();
                return sensorData;
            }
        }
    
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

        public void WritePackets(StreamWriter file, PacketsCollection[] data)
        {
            file.WriteLine(data.Length);

            for (int j = 0; j < data.Length; j++)
            {
                file.WriteLine(data[j].ToString());
            }
        }

        public void WritePacketsForCurrentTemperature(StreamWriter file, PacketsCollection[] data,int temperatureIndex)
        {
            Log.Instance.Info("Запись данных в файл {0} для температуры", temperatureIndex);
            if(data!=null && data.Length>temperatureIndex && data[temperatureIndex]!=null)
            {
                file.WriteLine(data.Length);
                file.WriteLine(data.ToString());
                Log.Instance.Info("Запись данных в файл {0} для температуры завершена", temperatureIndex);
            }
            Log.Instance.Error("Запись данных в файл {0} для температуры невыполнена", temperatureIndex);
        }
        public bool WritePacketsForCurrentTemperture(ISensor[] sensors, StreamWriter file, int temperatureIndex)
        {
            if (sensors[0].CalibrationPacketsCollection == null || sensors[0].CalibrationPacketsCollection.Count == 0)
            {
                Log.Instance.Error("Нет пакетов по калибровке: {0}", sensors[0].Name);
                return false;
            }
            else
            {
                WritePackets(file, sensors[0].CalibrationPacketsCollection.ToArray());
            }
            if (sensors[1].CalibrationPacketsCollection == null || sensors[1].CalibrationPacketsCollection.Count == 0)
            {
                Log.Instance.Error("Нет пакетов по калибровке: {0}", sensors[1].Name);
                return false;
            }
            else
            {
                WritePackets(file, sensors[1].CalibrationPacketsCollection.ToArray());
            }
            return true;
        }

        public bool WriteAllPackets(ISensor[] sensors,StreamWriter file)
        {
            if (sensors[0].CalibrationPacketsCollection == null || sensors[0].CalibrationPacketsCollection.Count == 0)
            {
                Log.Instance.Error("Нет пакетов по калибровке: {0}",sensors[0].Name);
                return false;
            }
            else
            {
                WritePackets(file, sensors[0].CalibrationPacketsCollection.ToArray());
            }
            if (sensors[1].CalibrationPacketsCollection == null || sensors[1].CalibrationPacketsCollection.Count == 0)
            {
                Log.Instance.Error("Нет пакетов по калибровке: {0}", sensors[1].Name);
                return false;
            }
            else
            {
                WritePackets(file, sensors[1].CalibrationPacketsCollection.ToArray());
            }
            //if (sensors[0].CheckPacketsCollection == null || sensors[0].CheckPacketsCollection.Length == 0)
            //{
            //    file.WriteLine("Нет пакетов по Проверка ДЛУ");
            //}
            //else
            //{
            //    WritePackets(file, sensors[0].CheckPacketsCollection);
            //}
            //if (sensors[1].CheckPacketsCollection == null || sensors[1].CheckPacketsCollection.Length == 0)
            //{
            //    file.WriteLine("Нет пакетов по Проверка ДУС");
            //}
            //else
            //{
            //    WritePackets(file, sensors[1].CheckPacketsCollection);
            //}
            return true;
        }

        private bool ReadDataFromFile(StreamReader file, List<PacketsCollection>collection)
        {
            var temp = file.ReadLine();
            int temperaturesCount = Convert.ToInt32(temp);
            collection = new List<PacketsCollection>();
            for (int i = 0; i < temperaturesCount; i++)
            {
                try
                {
                    collection.Add(new PacketsCollection(file));
                }
                catch (Exception exception)
                {
                    Log.Instance.Error("Ошибка: Чтение файла данных пакетов при температуре {0}",i);
                    Log.Instance.Exception(exception);
                    return false;
                }
            }
            return true;
        }

        public bool ReadDataFromFile(List<ISensor> sensors,StreamReader file)
        {
            if (!ReadDataFromFile(file, sensors[0].CalibrationPacketsCollection))
            {
                return false;
            }
            if (!ReadDataFromFile(file, sensors[1].CalibrationPacketsCollection))
            {
                return false;
            }
            //if (!ReadDataFromFile(file, sensors[0].CheckPacketsCollection))
            //{
            //    return false;
            //}
            //if (!ReadDataFromFile(file, sensors[1].CheckPacketsCollection))
            //{
            //    return false;
            //}
            return true;
        }

        #endregion      
    }
}
