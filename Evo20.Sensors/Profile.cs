using System;
using System.Text;

namespace Evo20.Sensors
{
    public class Profile
    {
        public Position[] PositionArray { get; set; }

        public Profile(Position[] profile)
        {
            PositionArray = profile;
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            for (var i = 0; i < PositionArray.Length; i++)
            {
                buffer.Append($"{i}: {PositionArray[i]} {Environment.NewLine}");
            }
            return buffer.ToString();
        }
    }
}
