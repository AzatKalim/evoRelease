using Evo20.Commands.Abstract;

namespace Evo20.Commands.AnswerCommands
{
    public class AxisRateQueryAnswer : AnswerCommand
    {
        public readonly double SpeedOfRate;
        public Axis Axis
        {
            get;
        }
        public AxisRateQueryAnswer(string speedOfRate, Axis axis)
        {
            Axis = axis;
            speedOfRate = speedOfRate.Replace(',', '.');
            if (double.TryParse(speedOfRate, out SpeedOfRate))
                return;
            speedOfRate = speedOfRate.Replace('.', ',');
            double.TryParse(speedOfRate, out SpeedOfRate);
        }
    }
}
