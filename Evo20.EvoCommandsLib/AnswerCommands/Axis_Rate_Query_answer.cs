using System;
using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    public class Axis_Rate_Query_answer : AnswerCommand
    {
        public double speedOfRate;
        public Axis axis
        {
            get;
            private set;
        }
        public Axis_Rate_Query_answer(String speedOfRate, Axis axis)
        {
            this.axis = axis;
            speedOfRate = speedOfRate.Replace(',', '.');
            if (!double.TryParse(speedOfRate, out this.speedOfRate))
            {
                speedOfRate = speedOfRate.Replace('.', ',');
                double.TryParse(speedOfRate, out this.speedOfRate);
            }    
        }
    }
}
