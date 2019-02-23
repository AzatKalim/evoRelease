using Evo20.Utils;

namespace Evo20.Commands.Abstract
{
    public enum Axis
    {
        First,
        Second,
        Third,
        All
    }
    public abstract class Command
    {
        protected int AxisToInt(Axis axis)
        {
            switch (axis)
            {
                case Axis.First:
                    return Config.Instance.XAxisNumber;
                case Axis.Second:
                    return Config.Instance.YAxisNumber;
                case Axis.Third:
                    return Config.Instance.ZAxisNumber;
                case Axis.All:
                    return Config.Instance.AllAxisNumber;
                default:
                    return 0;
            }
        }
    }
}
