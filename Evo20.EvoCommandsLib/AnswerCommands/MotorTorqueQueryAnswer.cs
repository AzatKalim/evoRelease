using System;
using Evo20.Commands.Abstract;

namespace Evo20.Commands.AnswerCommands
{
    internal class MotorTorqueQueryAnswer : AnswerCommand
    {
        public static double GetTorque(String command)
        {
            int beginOfFreq = command.IndexOf('=') + 1;
            double torque = 0;
            string temp = command.Substring(beginOfFreq, command.Length - beginOfFreq);
            try
            {
                torque = Convert.ToDouble(temp);
            }
            catch (FormatException)
            {               
            }
            return torque;
        }
    }
}
