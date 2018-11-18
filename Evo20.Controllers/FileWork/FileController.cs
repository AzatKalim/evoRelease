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

        public static FileController Current
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
                Log.WriteLog("Вычисление коэфицентов не выполнено!");
                return result;
            }
            Log.WriteLog("Вычисление коэфицентов выполнено!");
            return result;
        }

        /// <summary>
        /// Запись всех пакетов в файл( не работает пока)
        /// </summary>
        /// <param name="file">файл для записи</param>
        /// <returns></returns>
        public bool WritePackets(List<ISensor> sensorsList, StreamWriter file)
        {
            return SensorData.Current.WriteAllPackets(sensorsList.ToArray(), file);
        }

        /// <summary>
        /// Чтение пакетов их файла
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool ReadDataFromFile(ref List<ISensor> sensorsList, StreamReader file)
        {
            if (sensorsList == null || sensorsList.Count == 0)
            {
                sensorsList = SensorData.Current.GetSensors();
            }
            var result = SensorData.Current.ReadDataFromFile(sensorsList.ToArray(), file);
            if (result)
                CycleData.Current.StartTemperatureIndex = sensorsList[0].CalibrationPacketsCollection.Count;
            return result;
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
                if (!CycleData.Current.ReadSettings(ref file))
                    return false;
                if (!SensorData.Current.ReadSettings(ref file))
                    return false;
                file.Close();
            }
            catch (Exception exception)
            {
                Log.WriteLog("Возникла ошибка чтения файла настроек" + exception.ToString());
                throw exception;
            }
            return true;
        }

        public bool WriteRedPackets(List<ISensor> sensorsList)
        {
            Log.WriteLog("Запись уже считанных пакетов в файл");

            foreach (var sensor in sensorsList)
            {
                if (!sensor.WriteRedPackets())
                {
                    Log.WriteLog(string.Format("Запись прервана на датчике {0}", sensor.Name));
                    return false;
                }
            }
            return true;
        }
    }
}
