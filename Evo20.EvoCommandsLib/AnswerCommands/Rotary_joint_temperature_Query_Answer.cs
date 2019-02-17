using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    public class Rotary_joint_temperature_Query_answer : AnswerCommand
    {
        public double temperture;
        public Axis axis
        {
            get;
            private set;
        }
        public Rotary_joint_temperature_Query_answer()
        {
        }
        public Rotary_joint_temperature_Query_answer(string temper,Axis ax)
        {
            temper = temper.Replace(',', '.');
            if (!double.TryParse(temper, out this.temperture))
            {
                temper = temper.Replace('.', ',');
                double.TryParse(temper, out this.temperture);
            }
            axis = ax;
        }
        public override string ToString()
        {
            return "AXE.TELL.TJT";
        }
    }
}
