using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Evo_20_commands
{
    public class Axis_Status_answer:Command
    {
        public bool is_initialized
        {
            private set;
            get;
        }
        public bool is_zero_1_found
        {
            private set;
            get;
        }
        public bool is_zero_2_found
        {
            private set;
            get;
        }
        public bool is_zero_3_found
        {
            private set;
            get;
        }
        public bool power
        {
            private set;
            get;
        }
        public bool is_axis_1_move
        {
            private set;
            get;
        }
        public bool is_axis_2_move
        {
            private set;
            get;
        }
        public bool is_axis_3_move
        {
            private set;
            get;
        }
        public bool is_limit_1
        {
            private set;
            get;
        }
        public bool is_limit_2
        {
            private set;
            get;
        }
        public bool is_limit_3
        {
            private set;
            get;
        }
        public bool is_axis_1_stop
        {
            private set;
            get;
        }
        public bool is_axis_2_stop
        {
            private set;
            get;
        }
        public bool is_axis_3_stop
        {
            private set;
            get;
        }
        public bool is_error
        {
            private set;
            get;
        }
        public bool is_trigger_1_active
        {
            private set;
            get;
        }
        public bool is_trigger_2_active
        {
            private set;
            get;
        }
        public bool is_trigger_3_active
        {
            private set;
            get;
        }
        public bool is_trigger_active
        {
            private set;
            get;
        }

        public Axis_Status_answer(String value)
        {
            StringBuilder data = new StringBuilder(Convert.ToString(Convert.ToInt32(value, 16), 2));
            while (data.Length < 22)
            {
                data.Append('0');
            }
            is_answer = true;
            have_answer = false;
            can_send = false;

            if (data[0] == '1') { is_initialized = true; } 
            else{is_initialized = false;}

            if (data[1] == '1'){is_zero_1_found = true;}
            else {is_zero_1_found = false;}

            if (data[2] == '1') { is_zero_2_found = true; }
            else { is_zero_2_found = false; }

            if (data[3] == '1') { is_zero_3_found = true; }
            else { is_zero_3_found = false; }

            if (data[4] == '1') { power = true; }
            else { power = false; }

            if (data[8] == '1') { is_axis_1_move = true; }
            else { is_axis_1_move = false; }

            if (data[9] == '1') { is_axis_2_move = true; }
            else { is_axis_2_move = false; }

            if (data[10] == '1') { is_axis_3_move = true; }
            else { is_axis_3_move = false; }

            if (data[11] == '1') { is_limit_1 = true; }
            else { is_limit_1 = false; }

            if (data[12] == '1') { is_limit_2 = true; }
            else { is_limit_2 = false; }

            if (data[13] == '1') { is_limit_3 = true; }
            else { is_limit_3 = false; }

            if (data[14] == '1') { is_axis_1_stop = true; }
            else { is_axis_1_stop = false; }

            if (data[15] == '1') { is_axis_2_stop = true; }
            else { is_axis_2_stop = false; }

            if (data[16] == '1') { is_axis_3_stop = true; }
            else { is_axis_3_stop = false; }

            if (data[17] == '1') { is_error = true; }
            else { is_error = false; }

            if (data[18] == '1') { is_trigger_1_active = true; }
            else { is_trigger_1_active = false; }

            if (data[19] == '1') { is_trigger_2_active = true; }
            else { is_trigger_2_active = false; }

            if (data[20] == '1') { is_trigger_3_active = true; }
            else { is_trigger_3_active = false; }

            if (data[21] == '1') { is_trigger_active = true; }
            else { is_trigger_active = false; }

        }
    }
}
