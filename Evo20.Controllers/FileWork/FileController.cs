using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using Evo20.Controllers.Data;
using Evo20.Math;
using Evo20.Sensors;
using Evo20.Utils;

namespace Evo20.Controllers.FileWork
{
    public class FileController
    {
        private static FileController _fileController;

        public static string FilesPath;

        public static FileController Instance => _fileController ?? (_fileController = new FileController());

        /// <summary>
        /// Вычислить калибровочные коэфииценты
        /// </summary>
        /// <param name="sensorsList"></param>
        /// <param name="file">файл для записи результатов</param>
        /// <returns>true- выполнено успешно,false-возникла ошибка </returns>
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public virtual bool ComputeCoefficent(List<ISensor> sensorsList, StreamWriter file)
        {
            bool result = CalculatorCoefficients.CalculateCoefficients(sensorsList[0].GetCalibrationAdcCodes(),
                sensorsList[1].GetCalibrationAdcCodes(),
                file);
            if (!result)
            {
                Log.Instance.Error("Вычисление коэфицентов не выполнено!");
                return false;
            }
            Log.Instance.Error("Вычисление коэфицентов выполнено!");
            return true;
        }

        /// <summary>
        /// Чтение пакетов их файла
        /// </summary>
        /// <returns />
        public bool ReadDataFromFile(ref List<ISensor> sensorsList)
        {
            for (int temperature = 0; temperature < CycleData.Instance.CalibrationTemperatures.Count; temperature++)
            {
                var fileName= Path.Combine(FilesPath,temperature+".txt");
                if(!File.Exists(fileName))
                {
                    Log.Instance.Warning("Файл {0} не существует!", fileName);
                    return true;
                }
                using (var reader = new StreamReader(fileName))
                {
                    var result = SensorData.Instance.ReadDataFromFile(sensorsList,reader,temperature);
                    if (result)
                        CycleData.Instance.StartTemperatureIndex = sensorsList[0].CalibrationPacketsCollection.Count;
                    else
                        return false;
                }
            }
            return true;
        }

        public bool ReadSettings(string fileName)
        {
            try
            {
                var file = new StreamReader(fileName, Encoding.GetEncoding(1251));
                if (!CycleData.Instance.ReadSettings(ref file))
                    return false;
                if (!SensorData.Instance.ReadSettings(ref file))
                    return false;
                file.Close();
            }
            catch (Exception exception)
            {
                Log.Instance.Error("Возникла ошибка чтения файла настроек" + exception);
                Log.Instance.Exception(exception);
                throw;
            }
            return true;
        }

        public bool WriteRedPackets(List<ISensor> sensorsList,int temperatureNumber)
        {
            Log.Instance.Info("Запись уже считанных пакетов в файл");
            bool result;
            using( var file = new StreamWriter(Path.Combine(FilesPath,temperatureNumber+".txt")))
            {
                file.WriteLine(temperatureNumber);
                result= SensorData.Instance.WriteForCurrentTemperture(sensorsList.ToArray(),
                    file,temperatureNumber);
            }
            return result;
            //foreach (var sensor in _sensorsList)
            //{
            //    if (!sensor.WriteRedPackets(filesPath))
            //    {
            //        Log.Instance.Error("Запись прервана на датчике {0}", sensor.Name);
            //        return false;
            //    }
            //}
            //return true;
        }

        public bool WritePackets(List<ISensor> sensorsList, StreamWriter file)
        {
            return SensorData.Instance.WriteAllPackets(sensorsList.ToArray(), file);
        }
    }
}
