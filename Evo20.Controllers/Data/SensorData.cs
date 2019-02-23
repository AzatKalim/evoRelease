using System;
using System.Collections.Generic;
using System.IO;
using Evo20.Packets;
using Evo20.Sensors;
using Evo20.Utils;

namespace Evo20.Controllers.Data
{
    public class SensorData : AbstractData
    {
        #region Properties

        public int CalibrationDlyMaxPacketsCount;

        public int CalibrationDysMaxPacketsCount;

        public int CheckDlyMaxPacketsCount;

        public int CheckDysMaxPacketsCount;

        private static SensorData _sensorData;

        public static SensorData Instance => _sensorData ?? (_sensorData = new SensorData());

        #endregion 
     
        #region File work

        public override bool ReadSettings(ref StreamReader file)
        {
            bool isSuccess = ReadParamFromFile(ref file,
                "Количество пакетов для расчета средних кодов АЦП ДЛУ в режиме калибровка",
                ref  CalibrationDlyMaxPacketsCount);
            if (!isSuccess)
                return false;
            isSuccess = ReadParamFromFile(ref file,
                "Количество пакетов для расчета средних кодов АЦП ДУС в режиме калибровка",
                ref  CalibrationDysMaxPacketsCount);
            if (!isSuccess)
                return false;
            isSuccess = ReadParamFromFile(ref file,
                "Количество пакетов для расчета средних кодов АЦП ДЛУ в режиме проверка",
                ref  CheckDlyMaxPacketsCount);
            if (!isSuccess)
                return false;
            isSuccess = ReadParamFromFile(ref file,
                "Количество пакетов для расчета средних кодов АЦП ДУС в режиме проверка",
                ref  CheckDysMaxPacketsCount);
            if (!isSuccess)
                return false;
            return true;
        }

        public void WritePackets(StreamWriter file, PacketsCollection[] data)
        {
            file.WriteLine(data.Length);

            foreach (var packetsCollection in data)
            {
                file.WriteLine(packetsCollection.ToString());
            }
        }

        public void WritePacketsForCurrentTemperature(StreamWriter file, List<PacketsCollection> data,int temperatureIndex)
        {
            if (data != null && data.Count > temperatureIndex && data[temperatureIndex] != null)
            {
                file.WriteLine(data[temperatureIndex].PositionCount);
                file.WriteLine(data[temperatureIndex].ToString());
                Log.Instance.Info("Запись данных в файл {0} для температуры завершена", temperatureIndex);
            }
            else
            {
                Log.Instance.Error("Запись данных в файл {0} для температуры невыполнена", temperatureIndex);
            }
        }

        public bool WriteForCurrentTemperture(ISensor[] sensors, StreamWriter file, int temperatureIndex)
        {
            foreach (var sensor  in sensors)
            {
                if (sensor.CalibrationPacketsCollection == null || sensor.CalibrationPacketsCollection.Count == 0)
                {
                    Log.Instance.Error("Нет пакетов по калибровке: {0}", sensor.Name);
                    return false;
                }
                else
                {
                    Log.Instance.Info("Запись данных в файл датчик {0} для температуры {1}",sensor.Name, temperatureIndex);
                    WritePacketsForCurrentTemperature(file, sensor.CalibrationPacketsCollection, temperatureIndex);
                }
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
            return true;
        }

        private bool ReadDataFromFile(StreamReader file, List<PacketsCollection>collection,int temperature)
        {
            try
            {
                collection.Add(new PacketsCollection(file,temperature));
            }
            catch (Exception exception)
            {
                Log.Instance.Error("Ошибка: Чтение файла данных пакетов");
                Log.Instance.Exception(exception);
                return false;
            }
            return true;
        }

        public bool ReadDataFromFile(List<ISensor> sensors,StreamReader file,int temperature)
        {
            foreach (var sensor in sensors)
                if (!ReadDataFromFile(file, sensor.CalibrationPacketsCollection, temperature))
                    return false;
            //foreach (var sensor in sensors)
            //    if (!ReadDataFromFile(file, sensor.CheckPacketsCollection))
            //        return false; 
            return true;
        }

        #endregion      
    }
}
