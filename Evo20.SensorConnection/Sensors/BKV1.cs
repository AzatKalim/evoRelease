using System;
using System.Collections.Generic;
using System.Threading;
using Evo20.PacketsLib;

namespace Evo20.SensorsConnection
{
    /// <summary>
    /// Класс, реализующий общие методы и свойства для датчиков блока БКВ-1
    /// </summary>
    public abstract class BKV1 : ISensor
    {
        protected const int RAW_COUNT = 6;

        #region Private Fields

        private ProfilePart[] calibrationProfile;

        private ProfilePart[] checkProfile;

        #endregion

        #region Properties

        public ManualResetEvent PacketsCollectedEvent
        {
            set;
            get;
        }

        public ProfilePart[] CalibrationProfile
        {
            get
            {
                if (calibrationProfile == null)
                {
                    calibrationProfile = GetCalibrationProfile();
                }
                return calibrationProfile;
            }
        }

        public ProfilePart[] CheckProfile
        {
            get
            {
                if (checkProfile == null)
                {
                    checkProfile = GetCheckProfile();
                }
                return checkProfile;
            }
        }

        public PacketsCollection[] CalibrationPacketsCollection
        {
            set;
            get;
        }

        public PacketsCollection[] CheckPacketsCollection
        {
            set;
            get;
        }

        public abstract string Name
        {
            get;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Вычисляет средниее значения по параметров
        /// </summary>
        /// <param name="temperature"></param>
        /// <param name="numberOfPosition"></param>
        /// <returns></returns>
        public List<double> СalculateCalibrationAverage(int temperature, int numberOfPosition)
        {
            List<double> result = new List<double>();

            int index = FindTemperatureIndex(CalibrationPacketsCollection, temperature);
            if (index == -1)
            {
                return null;
            }
            lock (CalibrationPacketsCollection)
            {
                result = CalibrationPacketsCollection[index].MeanParams(numberOfPosition);
                return result;
            }
        }

        public List<double> СalculateCheckAverage(int temperature, int numberOfPosition)
        {
            List<double> result = new List<double>();

            int index = FindTemperatureIndex(CheckPacketsCollection, temperature);
            if (index == -1)
            {
                return null;
            }
            lock (CheckPacketsCollection)
            {
                result = CheckPacketsCollection[index].MeanParams(numberOfPosition);
                return result;
            }
        }

        private int FindTemperatureIndex(PacketsCollection[] array, int temperature)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Temperature == temperature)
                {
                    return i;
                }
            }
            return -1;
        }

        public bool AddCalibrationPacketData(PacketsData packetData,
           int temperatureOfCollect,
           int currentPositionNumber)
        {
            int index = FindTemperatureIndex(CalibrationPacketsCollection, temperatureOfCollect);
            if (index == -1)
            {
                return false;
            }
            bool isSuccess = CalibrationPacketsCollection[index].AddPacketData(currentPositionNumber, packetData);
            if (!isSuccess)
                PacketsCollectedEvent.Set();
            return true;
        }

        public bool AddCheckPacketData(PacketsData packetData,
           int temperatureOfCollect,
           int currentPositionNumber)
        {
            int index = FindTemperatureIndex(CheckPacketsCollection, temperatureOfCollect);
            if (index == -1)
            {
                return false;
            }
            bool isSuccess = CheckPacketsCollection[index].AddPacketData(currentPositionNumber, packetData);
            if (!isSuccess)
            {
                PacketsCollectedEvent.Set();
            }
            return true;
        }

        public int PacketsArivedCountCalibration(int temperature, int numberOfPosition)
        {
            
            int index = FindTemperatureIndex(CalibrationPacketsCollection, temperature);
            if (index == -1)
            {
                return 0;
            }
            return CalibrationPacketsCollection[index][numberOfPosition].Count;
          
        }

        public int PacketsArivedCountCheck(int temperature, int numberOfPosition)
        {
            int index = FindTemperatureIndex(CheckPacketsCollection, temperature);
            if (index == -1)
            {
                return 0;
            }
            return CheckPacketsCollection[temperature][numberOfPosition].Count;
        }

        #endregion

        #region Abstract Methods

        protected abstract ProfilePart[] GetCheckProfile();

        protected abstract ProfilePart[] GetCalibrationProfile();

        public virtual double[][,] GetCalibrationADCCodes()
        {
            throw new NotImplementedException();
        }

        public virtual double[][,] GetCheckADCCodes()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
