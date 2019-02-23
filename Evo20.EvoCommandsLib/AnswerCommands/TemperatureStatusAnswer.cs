using System;
using System.Text;
using Evo20.Commands.Abstract;

namespace Evo20.Commands.AnswerCommands
{
    public class TemperatureStatusAnswer : AnswerCommand
    {
        public bool IsMaxReached
        {
            get;
        }
        public bool IsMinReached
        {
            get;
        }
        public bool IsTemperatureReached
        {
            get;
        }
        public bool IsVentilationActive
        {
            get;
        }
        public bool IsCycleActive
        {
            get;
        }
        public bool IsPowerOn
        {
            get;
        }
        public bool HaveDefaultData
        {
            get;
        }

        public TemperatureStatusAnswer(string value)
        {
            var data = new StringBuilder(Convert.ToString(Convert.ToInt32(value, 16), 2));
            while (data.Length < 8)
            {
                data.Append('0');
            }
            IsMaxReached = data[0] == '1';
            IsMinReached = data[1] == '1';
            IsTemperatureReached = data[2] == '1';
            IsVentilationActive = data[3] == '1';
            IsCycleActive = data[4] == '1';
            IsPowerOn = data[5] == '1';
            HaveDefaultData = data[6] == '1';
        }
    }
}
