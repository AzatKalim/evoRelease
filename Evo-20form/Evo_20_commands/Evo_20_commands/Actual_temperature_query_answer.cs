using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Evo_20_commands
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
            this.temperature= Convert.ToDouble(temperature);
            is_answer = true;
            have_answer = false;
            can_send = false;
        }
    }
}
