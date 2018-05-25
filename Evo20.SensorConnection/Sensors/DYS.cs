using System.Collections.Generic;
using System.Threading;
using Evo20.PacketsLib;
using Evo20.Log;

namespace Evo20.SensorsConnection
{

    public class DYS : BKV1
    {
        static int[] INDEXES = { 0, 1, 7, 13, 14, 20, 26, 27, 33, 39 };
        static int[] COLL_SPEED = { 2, 8, 16, 64, 112, 128 };
        static int[] CHECK_SPEED = { 1, 5, 21, 43, 75, 115 };
        public const int COUNT_OF_POSITIONS = 39;


        //private ProfilePart[] calibrationProfile;

        //private ProfilePart[] checkProfile;

        #region Properties 
     
        public override string Name
        {
            get
            {
                return "DYS";
            }
        }

        #endregion

        public DYS(List<int>colibrationTemperatures,List<int> checkTemperatures,int calibrationMaxPacketsCount,int checkMaxPacketsCount)
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

        protected override ProfilePart[] GetCalibrationProfile()
        {
            ProfilePart[] profile = new ProfilePart[INDEXES[INDEXES.Length - 1]];

            profile[0] = new ProfilePart(0, 0, 0, 0);

            for (int i = INDEXES[1]; i < INDEXES[2]; i++)
            {
                profile[i] = new ProfilePart(0, 0, 0, COLL_SPEED[i - INDEXES[1]]);
            }
            for (int i = INDEXES[2]; i < INDEXES[3]; i++)
            {
                profile[i] = new ProfilePart(0, 0, 0, -COLL_SPEED[i - INDEXES[2]]);
            }

            profile[INDEXES[3]] = new ProfilePart(0, 0, 0, 0);

            for (int i = INDEXES[4]; i < INDEXES[5]; i++)
            {
                profile[i] = new ProfilePart(0, 0, -COLL_SPEED[i - INDEXES[4]], 0);
            }

            for (int i = INDEXES[5]; i < INDEXES[6]; i++)
            {
                profile[i] = new ProfilePart(0, 0, COLL_SPEED[i - INDEXES[5]], 0);
            }

            profile[INDEXES[6]] = new ProfilePart(0, 90, 0, 0);

            for (int i = INDEXES[7]; i < INDEXES[8]; i++)
            {
                profile[i] = new ProfilePart(0, 0, COLL_SPEED[i - INDEXES[7]], 0);
            }
            for (int i = INDEXES[8]; i < INDEXES[9]; i++)
            {
                profile[i] = new ProfilePart(0, 0, -COLL_SPEED[i - INDEXES[8]], 0);
            }

            return profile;
        }

        protected override ProfilePart[] GetCheckProfile()
        {
            ProfilePart[] profile = new ProfilePart[INDEXES[INDEXES.Length - 1]];

            profile[0] = new ProfilePart(0, 0, 0, 0);

            for (int i = INDEXES[1]; i < INDEXES[2]; i++)
            {
                profile[i] = new ProfilePart(0, 0, 0, CHECK_SPEED[i - INDEXES[1]]);
            }
            for (int i = INDEXES[2]; i < INDEXES[3]; i++)
            {
                profile[i] = new ProfilePart(0, 0, 0, -CHECK_SPEED[i - INDEXES[2]]);
            }

            profile[INDEXES[3]] = new ProfilePart(0, 0, 0, 0);

            for (int i = INDEXES[4]; i < INDEXES[5]; i++)
            {
                profile[i] = new ProfilePart(0, 0, -CHECK_SPEED[i - INDEXES[4]], 0);
            }

            for (int i = INDEXES[5]; i < INDEXES[6]; i++)
            {
                profile[i] = new ProfilePart(0, 0, CHECK_SPEED[i - INDEXES[5]], 0);
            }

            profile[INDEXES[6]] = new ProfilePart(0, 90, 0, 0);

            for (int i = INDEXES[7]; i < INDEXES[8]; i++)
            {
                profile[i] = new ProfilePart(0, 0, CHECK_SPEED[i - INDEXES[7]], 0);
            }
            for (int i = INDEXES[8]; i < INDEXES[9]; i++)
            {
                profile[i] = new ProfilePart(0, 0, -CHECK_SPEED[i - INDEXES[8]], 0);
            }
            return profile;
        }

    }
}
