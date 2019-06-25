using System;
using System.Configuration;

namespace Evo20.Utils
{
    public partial class Config
    {
        private static Config _instance;

        public static Config Instance => _instance ?? (_instance = new Config());

        private int _evoType = -1;
        public int EvoType
        {
            get
            {
                if(_evoType==-1)
                    _evoType = Convert.ToInt32(ConfigurationManager.AppSettings.Get("EvoType"));
                return _evoType;
            }
        }

        private int _localPortNumber = -1;
        //номер порта приходящих сообщений 
        public int LocalPortNumber
        {
            get
            {
                if (_localPortNumber == -1)
                    _localPortNumber = Convert.ToInt32(ConfigurationManager.AppSettings.Get("LocalPortNumber"));
                return _localPortNumber;
            }
        }

        private int _remotePortNumber = -1;
        //номер удаленного порта 
        public int RemotePortNumber
        {
            set { _remotePortNumber = value; }
            get { return _remotePortNumber == -1 ? 531 : _remotePortNumber; }
        }

        private string _remoteIpAdress;
        //ip адресс удаленного компьютера
        public string RemoteIpAdress
        {
            set { _remoteIpAdress = value;}
            get { return _remoteIpAdress ?? "192.168.0.1"; }
        }

        private int _speedOfTemperatureChange = -1;
        //номер удаленного порта 
        public int SpeedOfTemperatureChange
        {
            get
            {
                if (_speedOfTemperatureChange == -1)
                    _speedOfTemperatureChange = Convert.ToInt32(ConfigurationManager.AppSettings.Get("SPEED_OF_TEMPERATURE_CHANGE"));
                return _speedOfTemperatureChange;
            }
        }

        private int _baseMoveSpeed = -1;
        //номер удаленного порта 
        public int BaseMoveSpeed
        {
            get
            {
                if (_baseMoveSpeed == -1)
                    _baseMoveSpeed = Convert.ToInt32(ConfigurationManager.AppSettings.Get("BASE_MOVE_SPEED"));
                return _baseMoveSpeed;
            }
        }

        private string _profileFolder;
        public string ProfileFolder => _profileFolder ?? (_profileFolder = ConfigurationManager.AppSettings.Get("ProfileFolder"));

        private string _defaultSettingsFileName;
        public string DefaultSettingsFileName => _defaultSettingsFileName ?? (_defaultSettingsFileName =
                                                     ConfigurationManager.AppSettings.Get("DefaultSettingsFileName"));

        public static bool IsFakeEvo = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("FakeEvo"));

    }
}
