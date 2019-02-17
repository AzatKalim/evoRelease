using System;
using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    class Motor_torque_Query_answer : AnswerCommand
    {
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
