﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Evo20.Evo20.Packets;
using Newtonsoft.Json;
using System.IO;

namespace Evo20.Sensors
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

        public AutoResetEvent PacketsCollectedEvent
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
#if !DEBUG
                    calibrationProfile = GetCalibrationProfile();
#else
                    calibrationProfile = this.GetCalibrationProfileTest();
#endif
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

        public List<PacketsCollection> CalibrationPacketsCollection
        {
            set;
            get;
        }

        public List<PacketsCollection> CheckPacketsCollection
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

        protected virtual ProfilePart[] GetCalibrationProfileTest()
        {
            var profile = new List<ProfilePart>();
            for (int i = 0; i < 2; i++)
            {
                profile.Add(new ProfilePart(i * 15, 0));
            }
            return profile.ToArray();
        }
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
                if (result == null)
                    return null;
            }
            double mul = 0.5/System.Math.Pow(2,28);
            for (int i = 0; i < result.Count; i++)
            {
                result[i] *= mul;
            }
            return result;
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
            }
            double mul = 0.5 / System.Math.Pow(2, 28);
            for (int i = 0; i < result.Count; i++)
            {
                result[i] *= mul;
            }
            return result;
        }

        private int FindTemperatureIndex(List<PacketsCollection> list, int temperature)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Temperature == temperature)
                    return i;
            }
            return -1;
        }

        public bool AddCalibrationPacketData(PacketsData packetData,
           int temperatureOfCollect,
           int currentPositionNumber)
        {
            int index = FindTemperatureIndex(CalibrationPacketsCollection, temperatureOfCollect);
            if (index == -1)
                return false;
            if (!CalibrationPacketsCollection[index].AddPacketData(currentPositionNumber, packetData))
            {
                PacketsCollectedEvent.Set();
            }
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
            return CalibrationPacketsCollection[index].PositionCount > numberOfPosition ? CalibrationPacketsCollection[index][numberOfPosition]==null?0: CalibrationPacketsCollection[index][numberOfPosition].Count: 0;
          
        }

        public int PacketsArivedCountCheck(int temperature, int numberOfPosition)
        {
            int index = FindTemperatureIndex(CheckPacketsCollection, temperature);
            if (index == -1)
            {
                return 0;
            }
            if (CheckPacketsCollection[index] == null)
                return 0;

            return CheckPacketsCollection[index].PositionCount > numberOfPosition ? CheckPacketsCollection[index][numberOfPosition].Count : 0;
        }

        public bool WriteRedPackets(string filesPath)
        {
            return true;
        }

        public ProfilePart[] GetCalibrationProfile()
        {
            var file = new StreamReader(string.Format("{0}{1}.txt", Config.Instance.ProfileFolder, this.Name));
            var str = file.ReadToEnd();
            var profile = JsonConvert.DeserializeObject<Profile>(str);
            Log.Instance.Info(str);
            return profile.ProfilePartArray;
        }

        public void WriteCalibrationProfile()
        {
            var Profile = new Profile(GetCalibrationProfile());
            string json = JsonConvert.SerializeObject(Profile);
            using (var file = new StreamWriter(string.Format("{0}{1}.txt", Config.Instance.ProfileFolder, this.Name+"2")))
            {
                file.Write(json);
            }
        }

#endregion

#region Abstract Methods

        protected abstract ProfilePart[] GetCheckProfile();

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
