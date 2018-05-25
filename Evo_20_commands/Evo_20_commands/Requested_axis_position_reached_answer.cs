using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Evo_20_commands
{
    public class Requested_axis_position_reached_answer:Command 
    {
        public bool axisXreached
        {
            get;
            private set;
        }
        public bool axisYreached
        {
            get;
            private set;
        }
        public Requested_axis_position_reached_answer()
        {
            is_answer = true;
            have_answer = false;
            can_send = false;
        }
        public Requested_axis_position_reached_answer(string text)
        {
            string[] temp = text.Split(',');
            if(temp[0][0]=='1')
            {
                axisXreached = true;
            }
            else
            {
                axisXreached = false;
            }
            if (temp[1][0] == '1')
            {
                axisYreached = true;
            }
            else
            {
                axisYreached = false;
            }                      
        }

        public static List<int> GetResult(String command)
        {
            int begin = command.IndexOf('=') + 1;
            string temp = command.Substring(begin, command.Length - begin);
            string[] axis_result = temp.Split(',');
            List<int> result = new List<int>();
            foreach (var item in axis_result)
            {
                result.Add(Convert.ToInt32(item));
            }
            return result;
        }
    }
}
