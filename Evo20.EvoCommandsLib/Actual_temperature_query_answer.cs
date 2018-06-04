using System;
using System.Collections.Generic;
using System.Text;

namespace Evo20.EvoCommandsLib
{
    public class Actual_temperature_query_answer:Command
    {
        public double temperature;
        public Actual_temperature_query_answer()
        {
            is_answer = true;
            have_answer = false;
            can_send = false;
        }
        public Actual_temperature_query_answer(String temperature)
        {
            temperature = temperature.Replace(',', '.');
            this.temperature = Convert.ToDouble(temperature);        
            is_answer = true;
            have_answer = false;
            can_send = false;
        }
    }
}
