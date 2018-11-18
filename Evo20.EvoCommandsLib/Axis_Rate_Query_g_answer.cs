using System;
using System.Collections.Generic;
using System.Text;

namespace Evo20.Commands
{
    public class Axis_Rate_Query_g_answer: Command
    {
        public Axis_Rate_Query_g_answer()
        {
            is_answer = true;
            have_answer = false;
            can_send = false;
        }
        public static double GetFrequency(String command)
        {
            int begin_of_freq = command.IndexOf('=') + 1;
            double freq = 0;
            string temp = command.Substring(begin_of_freq, command.Length - begin_of_freq);
            try
            {
                freq = Convert.ToDouble(temp);
            }
            catch (FormatException)
            {
                Console.WriteLine("Ошибка формата");
            }
            return freq;
        }
    }
}
