using System;
using System.Collections.Generic;
using System.Text;

namespace Evo20.EvoCommandsLib
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
                    return 2;
                case Axis.Y:
                    return 1;
                case Axis.Z:
                    return 3;
                case Axis.ALL:
                    return 0;
                default:
                    return 0;
            }
        }
    }
}
