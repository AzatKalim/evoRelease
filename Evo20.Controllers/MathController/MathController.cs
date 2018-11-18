using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Evo20.Math;
using Evo20.Sensors;
using System.IO;

namespace Evo20.Controllers
{
    public class MathController
    {
        private static MathController mathController;

        public static MathController Current
        {
            get
            {
                if (mathController == null)
                    mathController = new MathController();
                return mathController;
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
    }
}
