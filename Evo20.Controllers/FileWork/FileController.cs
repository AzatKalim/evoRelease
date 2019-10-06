using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Evo20.Controllers.Data;
using Evo20.Math;
using Evo20.Sensors;
using Evo20.Utils;

namespace Evo20.Controllers.FileWork
{
    public sealed class FileController
    {
        private static FileController _fileController;

        private readonly CycleData _cycleData = CycleData.Instance;

        private readonly SensorData _sensorData = SensorData.Instance;


        public static string FilesPath;

        public static FileController Instance => _fileController ?? (_fileController = new FileController());

        /// <summary>
        /// Вычислить калибровочные коэфииценты
        /// </summary>
        /// <param name="sensorsList"></param>
        /// <param name="file">файл для записи результатов</param>
        /// <returns>true- выполнено успешно,false-возникла ошибка </returns>
        public bool ComputeCoefficent(List<ISensor> sensorsList, StreamWriter file)
        {
            bool result=false;
            try
            {
                result = CalculatorCoefficients.CalculateCoefficients(sensorsList[0],
                    sensorsList[1],
                    file);
            }
            catch (Exception ex)
            {
                Log.Instance.Exception(ex);
            }
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
            foreach (var sensor in sensorsList)
            {
                sensor.CalibrationPacketsCollection.Clear();
            }

            for (var temperature = 0; temperature < _cycleData.CalibrationTemperatures.Count; temperature++)
            {
                var fileName = Path.Combine(FilesPath, $"{temperature}mean.txt");
                if (File.Exists(fileName))
                {
                    Log.Instance.Info($"Чтение файла {fileName}");
                    using (var reader = new StreamReader(fileName))
                    {
                        var result = _sensorData.ReadMeanDataFromFile(sensorsList, reader, temperature);
                        if (result)
                            _cycleData.StartTemperatureIndex = sensorsList[0].CalibrationPacketsCollection.Count;
                        else
                            return false;
                    }
                }
                else
                if (File.Exists(fileName = Path.Combine(FilesPath, $"{temperature}.txt")))
                {
                    Log.Instance.Info($"Чтение файла {fileName}");
                    using (var reader = new StreamReader(fileName))
                    {
                        var result = _sensorData.ReadDataFromFile(sensorsList, reader, temperature);
                        if (result)
                            _cycleData.StartTemperatureIndex = sensorsList[0].CalibrationPacketsCollection.Count;
                        else
                            return false;
                    }
                }
                else
                {
                    Log.Instance.Warning($"Файлы для  {temperature} не существует!");
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
                if (!_cycleData.ReadSettings(ref file))
                    return false;
                if (!_sensorData.ReadSettings(ref file))
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

        public void WriteRedPackets(List<ISensor> sensorsList, int temperatureNumber)
        {
            var fileName = Path.Combine(FilesPath, temperatureNumber + ".txt");
            Log.Instance.Info("Запись всех пакетов в файл {0}", fileName);
            using( var file = new StreamWriter(fileName))
            {
                _sensorData.WriteForCurrentTemperture(sensorsList.ToArray(),
                    file,temperatureNumber);
            }
            Log.Instance.Info("Запись всех пакетов в файл {0} завершена", fileName);
        }

        public void WriteMeanParams(List<ISensor> sensorsList, int temperatureNumber)
        {
            var fileName = Path.Combine(FilesPath, $"{temperatureNumber}mean.txt");
            using (var file = new StreamWriter(fileName))
            {
                Log.Instance.Info("Запись всех пакетов в файл {0}", fileName);
                _sensorData.WriteMeanForCurrentTemperture(sensorsList.ToArray(),
                    file, temperatureNumber);
            }
            Log.Instance.Info("Запись всех пакетов в файл {0} завершена", fileName);
        }

        public bool WritePackets(List<ISensor> sensorsList, StreamWriter file)
        {
            return _sensorData.WriteAllPackets(sensorsList.ToArray(), file);
        }
    }
}
