using System.Collections.Generic;
using System.IO;
using Evo20.Math;
using Evo20.Sensors;
using Evo20.Utils;

namespace Evo20.Controllers.MathController
{
    public class MathController
    {
        private static MathController _mathController;

        public static MathController Instance => _mathController ?? (_mathController = new MathController());

        /// <summary>
        /// Вычислить калибровочные коэфииценты
        /// </summary>
        /// <param name="sensorsList" />
        /// <param name="file">файл для записи результатов</param>
        /// <returns>true- выполнено успешно,false-возникла ошибка </returns>
        public bool ComputeCoefficents(List<ISensor> sensorsList, StreamWriter file)
        {
            bool result = CalculatorCoefficients.CalculateCoefficients(sensorsList[0],
                sensorsList[1],
                file);
            if (!result)
            {
                Log.Instance.Error("Вычисление коэфицентов не выполнено!");
                return false;
            }
            Log.Instance.Info("Вычисление коэфицентов выполнено!");
            return true;
        }
    }
}
