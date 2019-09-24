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
            if (other as object == null) return false;
            return (Math.Abs(FirstPosition - other.FirstPosition) <= Config.Instance.AxisDeviation &&
                    Math.Abs(SecondPosition - other.SecondPosition) <= Config.Instance.AxisDeviation &&
                    other.SpeedFirst == 0 && other.SpeedSecond == 0) ||
                   (Math.Abs(SpeedFirst - other.SpeedFirst) <= Config.Instance.SpeedDeviation &&
                    Math.Abs(SpeedSecond - other.SpeedSecond) <= Config.Instance.SpeedDeviation &&
                    (other.SpeedFirst != 0 || other.SpeedSecond != 0));
        }

        public override string ToString()
        {
            return $"{FirstPosition} {SecondPosition} {SpeedFirst} {SpeedSecond}";
        }
    }
}
