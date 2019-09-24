using System.Collections.Generic;
using System.Threading;
using Evo20.Packets;

namespace Evo20.Sensors
{
    /// <summary>
    /// Класс датчика ДЛУ
    /// </summary>
    public class DLY : BKV1
    {
        static readonly int[] Indexes = { 24, 48, 72 };

        private const int CountOfPositions = 72;

        public override string Name => "DLY";

        public DLY(List<int> colibrationTemperatures,
            List<int> checkTemperatures,
            int calibrationMaxPacketsCount,
            int checkMaxPacketsCount)
        {
            CalibrationPacketsCollection = new List<PacketsCollection>();
            CheckPacketsCollection = new List<PacketsCollection>();
            foreach (var temperture in colibrationTemperatures)
            {
                CalibrationPacketsCollection.Add(new PacketsCollection(temperture, CountOfPositions, calibrationMaxPacketsCount));
            }
            foreach (var temperture in checkTemperatures)
            {
                CheckPacketsCollection.Add(new PacketsCollection(temperture, CountOfPositions, checkMaxPacketsCount));
            }
            PacketsCollectedEvent = new AutoResetEvent(false);
        }

        ///// <summary>
        ///// Возврашщает профиль колибровки датчика ДЛУ
        ///// </summary>
        ///// <returns>Профиль</returns>
        //private Position[] GetCalibrationProfileOld()
        //{
        //    Position[] profile = new Position[INDEXES[INDEXES.Length - 1]];
        //    for (int i = 0; i < INDEXES[0]; i++)
        //    {
        //        profile[i] = new Position(i * 15, 0);
        //    }
        //    for (int i = INDEXES[0]; i < INDEXES[1]; i++)
        //    {
        //        profile[i] = new Position((i - 24) * 15, 90);
        //    }
        //    for (int i = INDEXES[1]; i < profile.Length; i++)
        //    {
        //        profile[i] = new Position(-90, (i - 54) * 15);
        //    }
        //    return profile;
        //}

        /// <summary>
        /// Возврашщает профиль проверки датчика ДЛУ
        /// </summary>
        /// <returns>Профиль</returns>
        protected override Position[] GetCheckProfile()
        {
            Position[] profile = new Position[Indexes[Indexes.Length - 1]];
            for (int i = 0; i < Indexes[0]; i++)
            {
                profile[i] = new Position(i * 15, 45,0,0);
            }
            for (int i = Indexes[0]; i < Indexes[1]; i++)
            {
                profile[i] = new Position((i - 24) * 15, -45);
            }
            for (int i = Indexes[1]; i < profile.Length; i++)
            {
                profile[i] = new Position(-45, (i - 54) * 15);
            }
            return profile;
        }
        // получить коды АЦП из коллекции пакетов
        public override double[][,] GetCalibrationAdcCodes()
        {
            double[][,] adcCodes = new double[RawCount][,];
            for (int i = 0; i < adcCodes.Length; i++)
            {
                adcCodes[i] = new double[CalibrationPacketsCollection.Count, CalibrationPacketsCollection[0].PositionCount];
            }
            for (int j = 0; j < CalibrationPacketsCollection.Count; j++)
            {
                for (int k = 0; k < CalibrationPacketsCollection[j].PositionCount; k++)
                {
                    double[] meanParam = CalibrationPacketsCollection[j].MeanA(k);
                    for (int i = 0; i < 3; i++)
                    {
                        adcCodes[i][j, k] = meanParam[i];
                    }

                    meanParam = CalibrationPacketsCollection[j].MeanUa(k);
                    for (int i = 3; i < RawCount; i++)
                    {
                        //переход за границы массива!!!
                        adcCodes[i][j, k] = meanParam[i - 3];
                    }
                }
            }
            return adcCodes;
        }

        public override double[][,] GetCheckAdcCodes()
        {
            return null;
        }
    }
}
