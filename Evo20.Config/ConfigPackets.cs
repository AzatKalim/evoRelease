using System;
using System.Configuration;

namespace Evo20.Utils
{
    public partial class Config
    {
        //позиция служебных  байт в пакете
        public readonly int HeadBegin = Convert.ToInt32(ConfigurationManager.AppSettings.Get("HeadBegin"));
        public readonly int IdBegin = Convert.ToInt32(ConfigurationManager.AppSettings.Get("IdBegin"));
        public readonly int CheckBegin = Convert.ToInt32(ConfigurationManager.AppSettings.Get("CheckBegin"));
        //позиция информационных байт в пакете
        public int WxBegin = Convert.ToInt32(ConfigurationManager.AppSettings.Get("W_X_Begin"));
        public int WyBegin = Convert.ToInt32(ConfigurationManager.AppSettings.Get("W_Y_Begin"));
        public int WzBegin = Convert.ToInt32(ConfigurationManager.AppSettings.Get("W_Z_Begin"));

        public int AxBegin = Convert.ToInt32(ConfigurationManager.AppSettings.Get("A_X_Begin"));
        public int AyBegin = Convert.ToInt32(ConfigurationManager.AppSettings.Get("A_Y_Begin"));
        public int AzBegin = Convert.ToInt32(ConfigurationManager.AppSettings.Get("A_Z_Begin"));

        public int UxBegin = Convert.ToInt32(ConfigurationManager.AppSettings.Get("U_X_Begin"));
        public int UyBegin = Convert.ToInt32(ConfigurationManager.AppSettings.Get("U_Y_Begin"));
        public int UzBegin = Convert.ToInt32(ConfigurationManager.AppSettings.Get("U_Z_Begin"));

        public int ParamsCount = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ParamsCount"));
        public int PacketSize = Convert.ToInt32(ConfigurationManager.AppSettings.Get("PacketSize"));

        public int XAxisNumber = Convert.ToInt32(ConfigurationManager.AppSettings.Get("X_AXIS_NUMBER"));
        public int YAxisNumber = Convert.ToInt32(ConfigurationManager.AppSettings.Get("Y_AXIS_NUMBER"));
        public int ZAxisNumber = Convert.ToInt32(ConfigurationManager.AppSettings.Get("Z_AXIS_NUMBER"));
        public int AllAxisNumber = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ALL_AXIS_NUMBER"));

        public int PacketReceiveTimeout = Convert.ToInt32(ConfigurationManager.AppSettings.Get("PacketReceiveTimeout"));
    }
}
