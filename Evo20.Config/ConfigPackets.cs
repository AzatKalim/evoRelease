using System;
using System.Configuration;

namespace Evo20
{
    public partial class Config
    {
        //позиция служебных  байт в пакете
        public int HEAD_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("HeadBegin"));
        public int ID_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("IdBegin"));
        public int CHECK_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("CheckBegin"));
        //позиция информационных байт в пакете
        public int W_X_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("W_X_Begin"));
        public int W_Y_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("W_Y_Begin"));
        public int W_Z_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("W_Z_Begin"));

        public int A_X_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("A_X_Begin"));
        public int A_Y_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("A_Y_Begin"));
        public int A_Z_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("A_Z_Begin"));

        public int U_X_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("U_X_Begin"));
        public int U_Y_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("U_Y_Begin"));
        public int U_Z_BEGIN = Convert.ToInt32(ConfigurationManager.AppSettings.Get("U_Z_Begin"));

        public int PARAMS_COUNT = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ParamsCount"));
        public int PACKET_SIZE = Convert.ToInt32(ConfigurationManager.AppSettings.Get("PacketSize"));

        public int X_AXIS_NUMBER = Convert.ToInt32(ConfigurationManager.AppSettings.Get("X_AXIS_NUMBER"));
        public int Y_AXIS_NUMBER = Convert.ToInt32(ConfigurationManager.AppSettings.Get("Y_AXIS_NUMBER"));
        public int Z_AXIS_NUMBER = Convert.ToInt32(ConfigurationManager.AppSettings.Get("Z_AXIS_NUMBER"));
        public int ALL_AXIS_NUMBER = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ALL_AXIS_NUMBER"));
    }
}
