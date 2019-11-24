using System.Collections.Generic;
using System.Threading;
using Evo20.Packets;

namespace Evo20.Sensors
{

    public class Dys : BKV1
    {
        static readonly int[] Indexes = { 0, 1, 7, 13, 14, 20, 26, 27, 33, 39 };
        static readonly int[] CheckSpeed = { 1, 5, 21, 43, 75, 115 };
        private const int CountOfPositions = 39;

        #region Properties 
     
        public override string Name => "DYS";

        #endregion

        public Dys(List<int>colibrationTemperatures,List<int> checkTemperatures,int calibrationMaxPacketsCount,int checkMaxPacketsCount)
        {
            CalibrationPacketsCollection = new List<PacketsCollection>();
            CheckPacketsCollection = new List<PacketsCollection>();
            foreach (var temperature in colibrationTemperatures)
            {
                CalibrationPacketsCollection.Add(new PacketsCollection(temperature, CountOfPositions, calibrationMaxPacketsCount));
            }
            foreach (var temperature in checkTemperatures)
            {
                CheckPacketsCollection.Add(new PacketsCollection(temperature, CountOfPositions, checkMaxPacketsCount));
            }
            PacketsCollectedEvent = new AutoResetEvent(false);
        }

        //private Position[] GetCalibrationProfileOld()
        //{
        //    Position[] profile = new Position[INDEXES[INDEXES.Length - 1]];

        //    profile[0] = new Position(0, 0, 0, 0);

        //    for (int i = INDEXES[1]; i < INDEXES[2]; i++)
        //    {
        //        profile[i] = new Position(0, 0, 0, COLL_SPEED[i - INDEXES[1]]);
        //    }
        //    for (int i = INDEXES[2]; i < INDEXES[3]; i++)
        //    {
        //        profile[i] = new Position(0, 0, 0, -COLL_SPEED[i - INDEXES[2]]);
        //    }

        //    profile[INDEXES[3]] = new Position(0, 0, 0, 0);

        //    for (int i = INDEXES[4]; i < INDEXES[5]; i++)
        //    {
        //        profile[i] = new Position(0, 0, -COLL_SPEED[i - INDEXES[4]], 0);
        //    }

        //    for (int i = INDEXES[5]; i < INDEXES[6]; i++)
        //    {
        //        profile[i] = new Position(0, 0, COLL_SPEED[i - INDEXES[5]], 0);
        //    }

        //    profile[INDEXES[6]] = new Position(0, 90, 0, 0);

        //    for (int i = INDEXES[7]; i < INDEXES[8]; i++)
        //    {
        //        profile[i] = new Position(0, 90, COLL_SPEED[i - INDEXES[7]], 0);
        //    }
        //    for (int i = INDEXES[8]; i < INDEXES[9]; i++)
        //    {
        //        profile[i] = new Position(0, 90, -COLL_SPEED[i - INDEXES[8]], 0);
        //    }

        //    return profile;
        //}

        protected override Position[] GetCheckProfile()
        {
            Position[] profile = new Position[Indexes[Indexes.Length - 1]];

            profile[0] = new Position();

            for (int i = Indexes[1]; i < Indexes[2]; i++)
            {
                profile[i] = new Position(0, 0, 0, CheckSpeed[i - Indexes[1]]);
            }
            for (int i = Indexes[2]; i < Indexes[3]; i++)
            {
                profile[i] = new Position(0, 0, 0, -CheckSpeed[i - Indexes[2]]);
            }

            profile[Indexes[3]] = new Position();

            for (int i = Indexes[4]; i < Indexes[5]; i++)
            {
                profile[i] = new Position(0, 0, -CheckSpeed[i - Indexes[4]]);
            }

            for (int i = Indexes[5]; i < Indexes[6]; i++)
            {
                profile[i] = new Position(0, 0, CheckSpeed[i - Indexes[5]]);
            }

            profile[Indexes[6]] = new Position(0, 90);

            for (int i = Indexes[7]; i < Indexes[8]; i++)
            {
                profile[i] = new Position(0, 0, CheckSpeed[i - Indexes[7]]);
            }
            for (int i = Indexes[8]; i < Indexes[9]; i++)
            {
                profile[i] = new Position(0, 0, -CheckSpeed[i - Indexes[8]]);
            }
            return profile;
        }
     
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
                    double[] meanParam = CalibrationPacketsCollection[j].MeanW(k);
                    for (int i = 0; i < RawCount / 2; i++)
                    {
                        adcCodes[i][j, k] = meanParam[i];
                    }

                    meanParam = CalibrationPacketsCollection[j].MeanUw(k);
                    for (int i = RawCount / 2; i < RawCount; i++)
                    {
                        adcCodes[i][j, k] = meanParam[i - RawCount / 2];
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
