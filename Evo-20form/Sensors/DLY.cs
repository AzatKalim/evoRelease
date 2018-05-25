using System.Collections.Generic;
using System.Threading;
using PacketsLib;

namespace Evo_20form.Sensors
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
    }
}
