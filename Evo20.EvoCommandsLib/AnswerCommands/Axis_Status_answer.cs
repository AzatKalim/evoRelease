using System;
using System.Text;
using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    public class Axis_Status_answer : AnswerCommand
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
            var tmp = Convert.ToString(Convert.ToInt32(value, 16), 2);
            StringBuilder data= new StringBuilder();
            for (int i = tmp.Length-1; i >=0; i--)
            {
                data.Append(tmp[i]);
            }
            while (data.Length < 24)
            {
                data.Append('0');
            }            
            is_initialized= data[0] == '1' ? true :false;

            is_zero_1_found = data[1] == '1' ? true : false;
            is_zero_2_found = data[2] == '1' ? true : false;
            is_zero_3_found = data[3] == '1' ? true : false;  
                  
            power = data[4] == '1' ? true : false;  
                    
            is_axis_1_move = data[8] == '1' ? true : false;           
            is_axis_2_move = data[9] == '1' ? true : false;
            is_axis_3_move = data[10] == '1' ? true : false;

            is_limit_1 = data[11] == '1' ? true : false;
            is_limit_2 = data[12] == '1' ? true : false;
            is_limit_3 = data[13] == '1' ? true : false;

            is_axis_1_stop = data[14] == '1' ? true : false;
            is_axis_2_stop = data[15] == '1' ? true : false;
            is_axis_3_stop = data[16] == '1' ? true : false;

            is_error = data[17] == '1' ? true : false;
            is_trigger_1_active = data[18] == '1' ? true : false;
            is_trigger_2_active = data[19] == '1' ? true : false;
            is_trigger_3_active = data[20] == '1' ? true : false;
            is_trigger_active = data[21] == '1' ? true : false;        
        }
    }
}
