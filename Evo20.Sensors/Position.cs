using System;
using Evo20.Utils;
using Newtonsoft.Json;

namespace Evo20.Sensors
{
    public class Position
    {
        public double FirstPosition { set; get; }

        public double SecondPosition { set; get; }

        public double SpeedFirst { set; get; }

        public double SpeedSecond { set; get; }
        [JsonConstructor]
        public Position(double firstPosition = 0, double secondPosition = 0, double speedFirst = 0, double speedSecond = 0)
        {
           FirstPosition = firstPosition;
           SecondPosition = secondPosition;
           SpeedFirst = speedFirst;
           SpeedSecond = speedSecond;
        }

        public bool Equals(Position other)
        {
            if (other as object == null)
                return false;

            bool firstAxisEqual = (Math.Abs(FirstPosition - other.FirstPosition) <= Config.Instance.AxisDeviation &&
                SpeedFirst == 0 && other.SpeedFirst == 0) ||
                (Math.Abs(SpeedFirst - other.SpeedFirst) <= Config.Instance.SpeedDeviation
                && SpeedFirst != 0 && other.SpeedFirst != 0);

            bool secondAxisEqual = (Math.Abs(SecondPosition - other.SecondPosition) <= Config.Instance.AxisDeviation &&
                    SpeedSecond == 0 && other.SpeedSecond == 0) ||
                    (Math.Abs(SpeedSecond - other.SpeedSecond) <= Config.Instance.SpeedDeviation
                    && SpeedSecond != 0 && other.SpeedSecond != 0);

            return firstAxisEqual && secondAxisEqual;          
        }

        public override string ToString()
        {
            return $"{FirstPosition} {SecondPosition} {SpeedFirst} {SpeedSecond}";
        }
    }
}
