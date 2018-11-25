using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Evo20
{
    public static class Config
    {
        public static int EvoType = Convert.ToInt32(ConfigurationManager.AppSettings.Get("EvoType"));
            
        //номер порта приходящих сообщений 
        public static int LOCAL_PORT_NUMBER = Convert.ToInt32(ConfigurationManager.AppSettings.Get("LocalPortNumber"));

        //номер удаленного порта 
        public static int REMOTE_PORT_NUMBER = Convert.ToInt32(ConfigurationManager.AppSettings.Get("RemotePortNumber"));

        //ip адресс удаленного компьютера 
        public static string REMOTE_IP_ADRESS = ConfigurationManager.AppSettings.Get("RemoteIPAdress");


        public static int SPEED_OF_TEMPERATURE_CHANGE = Convert.ToInt32(ConfigurationManager.AppSettings.Get("SPEED_OF_TEMPERATURE_CHANGE"));
        
        public static int BASE_MOVE_SPEED = Convert.ToInt32(ConfigurationManager.AppSettings.Get("BASE_MOVE_SPEED"));

        public static string ProfileFolder = ConfigurationManager.AppSettings.Get("ProfileFolder");

        private static string defaultSettingsFileName;

        public static string DefaultSettingsFileName
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

        //позиция служебных  байт в пакете
        public static int HEAD_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("HeadBegin"));
        public static int ID_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("IdBegin"));
        public static int CHECK_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("CheckBegin"));
        //позиция информационных байт в пакете
        public static int W_X_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("W_X_Begin"));
        public static int W_Y_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("W_Y_Begin"));
        public static int W_Z_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("W_Z_Begin"));

        public static int A_X_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("A_X_Begin"));
        public static int A_Y_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("A_Y_Begin"));
        public static int A_Z_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("A_Z_Begin"));

        public static int U_X_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("U_X_Begin"));
        public static int U_Y_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("U_Y_Begin"));
        public static int U_Z_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("U_Z_Begin"));

        public static int PARAMS_COUNT = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ParamsCount"));
        public static int PACKET_SIZE = Convert.ToInt32(ConfigurationManager.AppSettings.Get("PacketSize"));

        public static int X_AXIS_NUMBER = Convert.ToInt32(ConfigurationManager.AppSettings.Get("X_AXIS_NUMBER"));
        public static int Y_AXIS_NUMBER = Convert.ToInt32(ConfigurationManager.AppSettings.Get("Y_AXIS_NUMBER"));
        public static int Z_AXIS_NUMBER = Convert.ToInt32(ConfigurationManager.AppSettings.Get("Z_AXIS_NUMBER"));
        public static int ALL_AXIS_NUMBER = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ALL_AXIS_NUMBER"));
    }
}
