using Evo20.Commands.Abstract;

namespace Evo20.Commands.AnswerCommands
{
    public class ActualTemperatureQueryAnswer : AnswerCommand
    {
        public double Temperature;
              
        public ActualTemperatureQueryAnswer(string temperature)
        {
            temperature = temperature.Replace(',', '.');
            if (!double.TryParse(temperature,out Temperature))
            {
                temperature = temperature.Replace('.', ',');
                double.TryParse(temperature, out Temperature);
            }         
        }
    }
}
