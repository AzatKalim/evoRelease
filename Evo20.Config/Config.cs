using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Evo20
{
    public partial class Config
    {
        private static Config instance;

        public static Config Instance
        {
            get
            {
                if (instance == null)
                    instance = new Config();
                return instance;
            }
        }

        private int evoType = -1;
        public int EvoType
        {
            get
            {
                if(evoType==-1)
                    evoType = Convert.ToInt32(ConfigurationManager.AppSettings.Get("EvoType"));
                return evoType;
            }
        }

        private int localPortNumber = -1;
        //номер порта приходящих сообщений 
        public int LocalPortNumber
        {
            get
            {
                if (localPortNumber == -1)
                    localPortNumber = Convert.ToInt32(ConfigurationManager.AppSettings.Get("LocalPortNumber"));
                return localPortNumber;
            }
        }

        private int remotePortNumber = -1;
        //номер удаленного порта 
        public int RemotePortNumber
        {
            get
            {
                if (remotePortNumber == -1)
                    remotePortNumber = Convert.ToInt32(ConfigurationManager.AppSettings.Get("RemotePortNumber"));
                return remotePortNumber;
            }
        }

        private string remoteIPAdress;
        //ip адресс удаленного компьютера
        public string RemoteIPAdress
        {
            get
            {
                if (remoteIPAdress == null)
                    remoteIPAdress = ConfigurationManager.AppSettings.Get("RemoteIPAdress");
                return remoteIPAdress;
            }
        }

        private int speedOfTemperatureChange = -1;
        //номер удаленного порта 
        public int SpeedOfTemperatureChange
        {
            get
            {
                if (speedOfTemperatureChange == -1)
                    speedOfTemperatureChange = Convert.ToInt32(ConfigurationManager.AppSettings.Get("SPEED_OF_TEMPERATURE_CHANGE"));
                return speedOfTemperatureChange;
            }
        }

        private int baseMoveSpeed = -1;
        //номер удаленного порта 
        public int BaseMoveSpeed
        {
            get
            {
                if (baseMoveSpeed == -1)
                    baseMoveSpeed = Convert.ToInt32(ConfigurationManager.AppSettings.Get("BASE_MOVE_SPEED"));
                return baseMoveSpeed;
            }
        }

        private string profileFolder;
        public string ProfileFolder
        {
            get
            {
                if (profileFolder == null)
                    profileFolder = ConfigurationManager.AppSettings.Get("ProfileFolder");
                return profileFolder;
            }
        }

        private string defaultSettingsFileName;
        public string DefaultSettingsFileName
        {
            get
            {
                if(defaultSettingsFileName==null)
                {
                    defaultSettingsFileName=ConfigurationManager.AppSettings.Get("DefaultSettingsFileName");
                }
                return defaultSettingsFileName;
            }
        }

        public static bool IsFakeEvo = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("FakeEvo"));

    }
}
