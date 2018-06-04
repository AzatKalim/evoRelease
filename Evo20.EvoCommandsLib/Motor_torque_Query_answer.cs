using System;
using System.Collections.Generic;
using System.Text;

namespace Evo20.EvoCommandsLib
{
    class Motor_torque_Query_answer:Command
    {
        public Motor_torque_Query_answer()
        {
            is_answer = true;
            have_answer = false;
            can_send = false;
        }
        public static double GetTorque(String command)
        {
            int begin_of_freq = command.IndexOf('=') + 1;
            double torque = 0;
            string temp = command.Substring(begin_of_freq, command.Length - begin_of_freq);
            try
            {
                torque = Convert.ToDouble(temp);
            }
            catch (FormatException)
            {
                Console.WriteLine("Ошибка формата");
            }
            return torque;
        }
    }
}
