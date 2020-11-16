using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;
using System.IO;
using System.Windows.Forms;
using Evo20.Packets;
using Evo20.Utils;

namespace Evo20.Sensors
{
    public abstract class BKV1 : ISensor
    {
        protected const int RawCount = 6;

        #region Private Fields

        private Position[] _calibrationProfile;

        private Position[] _checkProfile;

        #endregion

        #region Properties

        public AutoResetEvent PacketsCollectedEvent
        {
            set;
            get;
        }

        private readonly object locker = new object();

        public Position[] CalibrationProfile
        {
            get
            {
                if (_calibrationProfile == null)
                {
                    lock (locker)
                    {
                        if (_calibrationProfile == null)
                        {
                            _calibrationProfile = GetCalibrationProfile();
                        }
                        if (_calibrationProfile == null)
                        {
                            throw new ApplicationException("Cannot read calibration Profile");
                        }
                    }
                }
                return _calibrationProfile;
            }
        }

        public Position[] CheckProfile
        {
            get
            {
                if (_checkProfile == null)
                {
                    lock (locker)
                    {
                        if (_checkProfile == null)
                        {
                            _checkProfile = GetCheckProfile();
                            if (_checkProfile == null)
                            {
                                throw  new ApplicationException("Cannot read check Profile");
                            }
                        }
                    }
                }
                return _checkProfile;
            }
        } 

        public List<PacketsCollection> CalibrationPacketsCollection {set;get;}

        public List<PacketsCollection> CheckPacketsCollection {set;get;}

        public abstract string Name {get;}

#endregion

        #region Public Methods

        protected Position[] GetCalibrationProfileTest()
        {
            var profile = new List<Position>();
            for (int i = 0; i < 2; i++)
            {
                profile.Add(new Position(i * 15));
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
            if (!CalibrationPacketsCollection[index].AddPacketData(currentPositionNumber, packetData)
                && PacketsArivedCountCalibration(index, currentPositionNumber)!= 0)
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

            return CalibrationPacketsCollection[index].PositionCount > numberOfPosition
                ? CalibrationPacketsCollection[index][numberOfPosition] == null ? 0 :
                CalibrationPacketsCollection[index][numberOfPosition].Count
                : 0;
          
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

        private Position[] GetCalibrationProfile()
        {
            var filename = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), $"{Name}.txt");//$"{Config.Instance.ProfileFolder}{Name}.txt";
            Log.Instance.Info($"Try to get colibration profile from file {filename}");
            if (!File.Exists(filename))
            {
                Log.Instance.Warning($"{filename} not exists !");
                using (var dialog = new OpenFileDialog
                {
                    ValidateNames = false,
                    CheckFileExists = true,
                    Title = $"Файл с профилями не найден! Необходимо выбрать файл с профилем для {Name}"
                })
                {
                    if (dialog.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(dialog.FileName))
                    {
                        filename = dialog.FileName;
                        Log.Instance.Info($"Выбран файл профиля { dialog.FileName}");
                    }
                    else
                    {
                        Log.Instance.Error("Папка не выбрана");
                        MessageBox.Show(@"Ошибка: не выбрана папка ", @"Небходимо выбрать папку для сохранения файлов !", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }
                }
            }
            var file = new StreamReader(filename);
            var str = file.ReadToEnd();
            Log.Instance.Info(str);
            Profile profile;
            try
            {
                profile = JsonConvert.DeserializeObject<Profile>(str);
            }
            catch (Exception e)
            {
                Log.Instance.Error("Deserialization error");
                Log.Instance.Exception(e);
                throw;
            }
            Log.Instance.Info($"Deserialized profile {profile}");
            return profile.PositionArray;
        }

        #endregion

        #region Abstract Methods

        protected abstract Position[] GetCheckProfile();

        public abstract double[][,] GetCalibrationAdcCodes();

        public abstract double[][,] GetCheckAdcCodes();

        #endregion
    }
}
