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
    public abstract class Command
    {
        protected int AxisToInt(Axis axis)
        {
            switch (axis)
            {
                case Axis.First:
                    return Config.Instance.X_AXIS_NUMBER;
                case Axis.Second:
                    return Config.Instance.Y_AXIS_NUMBER;
                case Axis.Third:
                    return Config.Instance.Z_AXIS_NUMBER;
                case Axis.ALL:
                    return Config.Instance.ALL_AXIS_NUMBER;
                default:
                    return 0;
            }
        }
    }
}
