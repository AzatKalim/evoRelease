using System;
using Evo20.Commands.Abstract;

namespace Evo20.Commands.AnswerCommands
{
    public class AxisPositionQueryAnswer : AnswerCommand
    {
        public readonly double Position;

        public Axis Axis { get; }

        public AxisPositionQueryAnswer(String position,Axis axis)
        {
           position = position.Replace(',', '.');

           if (!double.TryParse(position, out Position))
           {
               position = position.Replace('.', ',');
               double.TryParse(position, out Position);
           }    
           Axis= axis;
        }
        public override string ToString()
        {
            return "AXE.TELL.POS";
        }

    }
}
