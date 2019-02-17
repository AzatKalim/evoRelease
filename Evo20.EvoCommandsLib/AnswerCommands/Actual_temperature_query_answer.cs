using System;
using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    public class Actual_temperature_query_answer : AnswerCommand
    {
        public double temperature;
              
        public Actual_temperature_query_answer(String temperature)
        {
            temperature = temperature.Replace(',', '.');
            if (!double.TryParse(temperature,out this.temperature))
            {
                temperature = temperature.Replace('.', ',');
                double.TryParse(temperature, out this.temperature);
            }         
        }
    }
}
