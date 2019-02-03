using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using Evo20.Math;
using Evo20.Sensors;

namespace Evo20.Controllers
{
    public class FileController
    {
        public AutoResetEvent PacketsWritedEvent
        {
            set;
            get;
        }
        private static FileController fileController;

        public static string filesPath;

        public static FileController Instance
        {
            get
            {
                if (fileController == null)
                    fileController = new FileController();
                return fileController;
            }
        }

        /// <summary>
        /// Вычислить калибровочные коэфииценты
        /// </summary>
        /// <param name="file">файл для записи результатов</param>
        /// <returns>true- выполнено успешно,false-возникла ошибка </returns>
        public bool ComputeCoefficents(List<ISensor> sensorsList, StreamWriter file)
        {
            bool result = CalculatorCoefficients.CalculateCoefficients(sensorsList[0].GetCalibrationADCCodes(),
                sensorsList[1].GetCalibrationADCCodes(),
                file);
            if (!result)
            {
                Log.Instance.Error("Вычисление коэфицентов не выполнено!");
                return result;
            }
            Log.Instance.Error("Вычисление коэфицентов выполнено!");
            return result;
        }

        /// <summary>
        /// Запись всех пакетов в файл( не работает пока)
        /// </summary>
        /// <param name="file">файл для записи</param>
        /// <returns></returns>
        public bool WritePackets(List<ISensor> sensorsList, StreamWriter file)
        {
            return SensorData.Instance.WriteAllPackets(sensorsList.ToArray(), file);
        }

        /// <summary>
        /// Чтение пакетов их файла
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool ReadDataFromFile(ref List<ISensor> sensorsList)
        {
            for (int temperature = 0; temperature < CycleData.Instance.CalibrationTemperatures.Count; temperature++)
            {
                var fileName= Path.Combine(FileController.filesPath,temperature+".txt");
                if(!File.Exists(fileName))
                {
                    Log.Instance.Warning(@"Файл {0} не существует!");
                    return true;
                }
                using (var reader = new StreamReader(fileName))
                {
                    var result = SensorData.Instance.ReadDataFromFile(sensorsList,reader,temperature);
                    if (result)
                        CycleData.Instance.StartTemperatureIndex = sensorsList[0].CalibrationPacketsCollection.Count;
                    else
                        return result;
                }
            }
            return true;
        }

        /// <summary>
        /// Чтение настроек из файла
        /// </summary>
        /// <returns> результат чтения </returns>
        public bool ReadSettings(string fileName)
        {
            //Попытка чтения из файла
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
                Log.Instance.Error("Возникла ошибка чтения файла настроек" + exception.ToString());
                Log.Instance.Exception(exception);
                throw;
            }
            return true;
        }

        public bool WriteRedPackets(List<ISensor> sensorsList,int temperatureNumber)
        {
            Log.Instance.Info("Запись уже считанных пакетов в файл");
            bool result;
            using( var file = new StreamWriter(Path.Combine(filesPath,temperatureNumber+".txt")))
            {
                file.WriteLine(temperatureNumber);
                result= SensorData.Instance.WritePacketsForCurrentTemperture(sensorsList.ToArray(),
                    file,temperatureNumber);
            }
            return result;
            //foreach (var sensor in sensorsList)
            //{
            //    if (!sensor.WriteRedPackets(filesPath))
            //    {
            //        Log.Instance.Error("Запись прервана на датчике {0}", sensor.Name);
            //        return false;
            //    }
            //}
            //return true;
        }
    }
}
