using System;
using System.Collections.Generic;
using System.Text;
using Evo20.Config;

namespace Evo20.Commands
{
    public enum Axis
    {
        X,
        Y,
        Z,
        ALL
    }
    public class Command
    {
        public bool is_answer;
        public bool have_answer;
        public bool can_send;

        protected int AxisToInt(Axis axis)
        {
            switch (axis)
            {
                case Axis.X:
                    return Config.Config.X_AXIS_NUMBER;
                case Axis.Y:
                    return Config.Config.Y_AXIS_NUMBER;
                case Axis.Z:
                    return Config.Config.Z_AXIS_NUMBER;
                case Axis.ALL:
                    return Config.Config.ALL_AXIS_NUMBER;
                default:
                    return 0;
            }
        }
    }
}
