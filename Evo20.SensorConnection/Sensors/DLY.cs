using System.Collections.Generic;
using System.Threading;
using Evo20.PacketsLib;
using Evo20.Log;

namespace Evo20.SensorsConnection
{
    /// <summary>
    /// Класс датчика ДЛУ
    /// </summary>
    public class DLY : BKV1
    {
        static int[] INDEXES = { 24, 48, 72 };

        public const int COUNT_OF_POSITIONS = 72;

        public override string Name
        {
            get
            {
                return "DLY";
            }
        }

        public DLY(List<int> colibrationTemperatures,
            List<int> checkTemperatures,
            int calibrationMaxPacketsCount,
            int checkMaxPacketsCount)
        {
            CalibrationPacketsCollection = new PacketsCollection[colibrationTemperatures.Count];
            CheckPacketsCollection = new PacketsCollection[checkTemperatures.Count];
            for (int i = 0; i < colibrationTemperatures.Count; i++)
            {
                CalibrationPacketsCollection[i] = new PacketsCollection(colibrationTemperatures[i], COUNT_OF_POSITIONS, calibrationMaxPacketsCount);
            }
            for (int i = 0; i < checkTemperatures.Count; i++)
            {
                CheckPacketsCollection[i] = new PacketsCollection(checkTemperatures[i], COUNT_OF_POSITIONS, checkMaxPacketsCount);
            }
            PacketsCollectedEvent = new ManualResetEvent(false);
        }

        /// <summary>
        /// Возврашщает профиль колибровки датчика ДЛУ
        /// </summary>
        /// <returns>Профиль</returns>
        protected override ProfilePart[] GetCalibrationProfile()
        {
            ProfilePart[] profile = new ProfilePart[INDEXES[INDEXES.Length - 1]];
            for (int i = 0; i < INDEXES[0]; i++)
            {
                profile[i] = new ProfilePart(i * 15, 0);
            }
            for (int i = INDEXES[0]; i < INDEXES[1]; i++)
            {
                profile[i] = new ProfilePart((i - 24) * 15, 90);
            }
            for (int i = INDEXES[1]; i < profile.Length; i++)
            {
                profile[i] = new ProfilePart(-90, (i - 54) * 15);
            }
            return profile;
        }

        /// <summary>
        /// Возврашщает профиль проверки датчика ДЛУ
        /// </summary>
        /// <returns>Профиль</returns>
        protected override ProfilePart[] GetCheckProfile()
        {
            ProfilePart[] profile = new ProfilePart[INDEXES[INDEXES.Length - 1]];
            for (int i = 0; i < INDEXES[0]; i++)
            {
                profile[i] = new ProfilePart(i * 15, 45,0,0);
            }
            for (int i = INDEXES[0]; i < INDEXES[1]; i++)
            {
                profile[i] = new ProfilePart((i - 24) * 15, -45);
            }
            for (int i = INDEXES[1]; i < profile.Length; i++)
            {
                profile[i] = new ProfilePart(-45, (i - 54) * 15);
            }
            return profile;
        }
        // получить коды АЦП из коллекции пакетов
        public override double[][,] GetCalibrationADCCodes()
        {
            double[][,] adcCodes = new double[RAW_COUNT][,];
            for (int i = 0; i < adcCodes.Length; i++)
            {
                adcCodes[i] = new double[CalibrationPacketsCollection.Length, CalibrationPacketsCollection[0].PositionCount];
            }
            for (int j = 0; j < CalibrationPacketsCollection.Length; j++)
            {
                for (int k = 0; k < CalibrationPacketsCollection[j].PositionCount; k++)
                {
                    double[] meanParam = CalibrationPacketsCollection[j].MeanA(k);
                    for (int i = 0; i < 3; i++)
                    {
                        adcCodes[i][j, k] = meanParam[i];
                    }

                    meanParam = CalibrationPacketsCollection[j].MeanUA(k);
                    for (int i = 3; i < RAW_COUNT; i++)
                    {
                        //переход за границы массива!!!
                        adcCodes[i][j, k] = meanParam[i - 3];
                    }
                }
            }
            return adcCodes;
        }
    }
}
