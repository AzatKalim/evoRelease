using System;
using System.Collections.Generic;
using System.Text;
using Evo20;

namespace Evo20.Commands
{
    public enum Axis
    {
        First,
        Second,
        Third,
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
                case Axis.First:
                    return Config.X_AXIS_NUMBER;
                case Axis.Second:
                    return Config.Y_AXIS_NUMBER;
                case Axis.Third:
                    return Config.Z_AXIS_NUMBER;
                case Axis.ALL:
                    return Config.ALL_AXIS_NUMBER;
                default:
                    return 0;
            }
        }
    }
}
