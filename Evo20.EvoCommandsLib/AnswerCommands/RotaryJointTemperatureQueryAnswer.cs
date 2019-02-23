using Evo20.Commands.Abstract;

namespace Evo20.Commands.AnswerCommands
{
    public class RotaryJointTemperatureQueryAnswer : AnswerCommand
    {
        public double Temperture;
        public Axis Axis
        {
            get;
        }

        public RotaryJointTemperatureQueryAnswer(string temper,Axis ax)
        {
            temper = temper.Replace(',', '.');
            if (!double.TryParse(temper, out Temperture))
            {
                temper = temper.Replace('.', ',');
                double.TryParse(temper, out Temperture);
            }
            Axis = ax;
        }
        public override string ToString()
        {
            return "AXE.TELL.TJT";
        }
    }
}
