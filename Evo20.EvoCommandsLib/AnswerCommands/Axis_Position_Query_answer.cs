using System;
using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    public class Axis_Position_Query_answer : AnswerCommand
    {
        public double position;

        public Axis axis
        {
            get;
            private set;
        }

        public Axis_Position_Query_answer(String position,Axis axis)
        {
           position = position.Replace(',', '.');

           if (!double.TryParse(position, out this.position))
           {
               position = position.Replace('.', ',');
               double.TryParse(position, out this.position);
           }    
           this.axis= axis;
        }
        public override string ToString()
        {
            return "AXE.TELL.POS";
        }

    }
}
