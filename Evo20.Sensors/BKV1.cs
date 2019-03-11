﻿using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;
using System.IO;
using Evo20.Packets;
using Evo20.Utils;

namespace Evo20.Sensors
{
    public abstract class BKV1 : ISensor
    {
        protected const int RawCount = 6;

        #region Private Fields

        private ProfilePart[] _calibrationProfile;

        private ProfilePart[] _checkProfile;

        #endregion

        #region Properties

        public AutoResetEvent PacketsCollectedEvent
        {
            set;
            get;
        }

        public ProfilePart[] CalibrationProfile => _calibrationProfile ?? (_calibrationProfile = GetCalibrationProfile());

        public ProfilePart[] CheckProfile => _checkProfile ?? (_checkProfile = GetCheckProfile());

        public List<PacketsCollection> CalibrationPacketsCollection {set;get;}

        public List<PacketsCollection> CheckPacketsCollection {set;get;}

        public abstract string Name {get;}

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

        public List<double> СalculateCalibrationAverage(int index, int numberOfPosition)
        {
            List<double> result;
            if (index == -1)
                return null;

            lock (CalibrationPacketsCollection)
            {
                result = CalibrationPacketsCollection[index].MeanParams(numberOfPosition);
                if (result == null)
                    return null;
            }         
            return result;
        }

        public List<double> СalculateCheckAverage(int index, int numberOfPosition)
        {
            List<double> result;
            if (index == -1)
                return null;

            lock (CheckPacketsCollection)
            {
                result = CheckPacketsCollection[index].MeanParams(numberOfPosition);               
            }
            return result;
        }

        public bool AddCalibrationPacketData(PacketsData packetData,
           int index,
           int currentPositionNumber)
        {
            if (index == -1)
                return false;
            if (!CalibrationPacketsCollection[index].AddPacketData(currentPositionNumber, packetData))
            {
                PacketsCollectedEvent.Set();
            }
            return true;
        }

        public bool AddCheckPacketData(PacketsData packetData,
           int index,
           int currentPositionNumber)
        {
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

        public int PacketsArivedCountCalibration(int index, int numberOfPosition)
        {         
            if (index == -1)
                return 0;

            return CalibrationPacketsCollection[index].PositionCount > numberOfPosition ? CalibrationPacketsCollection[index][numberOfPosition]==null?0: CalibrationPacketsCollection[index][numberOfPosition].Count: 0;
          
        }

        public int PacketsArivedCountCheck(int index, int numberOfPosition)
        {
            if (index == -1)
                return 0;
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
            var filename = $"{Config.Instance.ProfileFolder}{Name}.txt";
            if(!File.Exists(filename))
            {
                return null;
            }
            var file = new StreamReader($"{Config.Instance.ProfileFolder}{Name}.txt");
            var str = file.ReadToEnd();
            var profile = JsonConvert.DeserializeObject<Profile>(str);
            Log.Instance.Info(str);
            return profile.ProfilePartArray;
        }

#endregion

        #region Abstract Methods

        protected abstract ProfilePart[] GetCheckProfile();

        public abstract double[][,] GetCalibrationAdcCodes();

        public abstract double[][,] GetCheckAdcCodes();

        #endregion
    }
}
