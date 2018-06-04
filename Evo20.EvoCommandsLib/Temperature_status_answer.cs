using System;
using System.Collections.Generic;
using System.Text;

namespace Evo20.EvoCommandsLib
{
    public class Temperature_status_answer:Command
    {
        public bool is_max_reached
        {
            private set;
            get;
        }
        public bool is_min_reached
        {
            private set;
            get;
        }
        public bool is_temperature_reached
        {
            private set;
            get;
        }
        public bool is_ventilation_active
        {
            private set;
            get;
        }
        public bool is_cycle_active
        {
            private set;
            get;
        }
        public bool is_power_on
        {
            private set;
            get;
        }
        public bool have_default_data
        {
            private set;
            get;
        }
        public Temperature_status_answer(){}
        public Temperature_status_answer(string value)
        {
            StringBuilder data = new StringBuilder(Convert.ToString(Convert.ToInt32(value, 16), 2));
            while (data.Length < 8)
            {
                data.Append('0');
            }
            is_answer = true;
            have_answer = false;
            can_send = false;
            if (data[0] == '1') { is_max_reached = true; }
            else { is_max_reached = false; }

            if (data[1] == '1') { is_min_reached = true; }
            else { is_min_reached = false; }

            if (data[2] == '1') { is_temperature_reached = true; }
            else { is_temperature_reached = false; }

            if (data[3] == '1') { is_ventilation_active= true; }
            else { is_ventilation_active = false; }

            if (data[4] == '1') { is_cycle_active = true; }
            else { is_cycle_active = false; }

            if (data[5] == '1') { is_power_on = true; }
            else { is_power_on = false; }

            if (data[6] == '1') { have_default_data = true; }
            else {have_default_data = false; }
        }
    }
}
