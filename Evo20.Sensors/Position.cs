using System;
using System.CodeDom;
using Evo20.Utils;
using Newtonsoft.Json;

namespace Evo20.Sensors
{

    public class Position
    {
        public int FirstPosition { set; get; }

        public int SecondPosition { set; get; }

        public int SpeedFirst { set; get; }

        public int SpeedSecond { set; get; }
        [JsonConstructor]
        public Position(int firstPosition, int secondPosition, int speedFirst, int speedSecond)
        {
           FirstPosition = firstPosition;
           SecondPosition = secondPosition;
           SpeedFirst = speedFirst;
           SpeedSecond = speedSecond;
        }
        public Position(int firstPosition, int secondPosition)
            : this(firstPosition, secondPosition, 0, 0) { }

        public bool Equals(Position other)
        {
            return Math.Abs(FirstPosition - other.FirstPosition) <= Config.Instance.AxisDeviation &&
                   Math.Abs(SecondPosition - other.SecondPosition) <= Config.Instance.AxisDeviation &&
                   Math.Abs(SpeedFirst - other.SpeedFirst) <= Config.Instance.SpeedDeviation &&
                   Math.Abs(SpeedSecond - other.SpeedSecond) <= Config.Instance.SpeedDeviation;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = FirstPosition;
                hashCode = (hashCode * 397) ^ SecondPosition;
                hashCode = (hashCode * 397) ^ SpeedFirst;
                hashCode = (hashCode * 397) ^ SpeedSecond;
                return hashCode;
            }
        }
    }
}
