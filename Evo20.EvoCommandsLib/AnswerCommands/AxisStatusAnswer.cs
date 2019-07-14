using System;
using System.Text;
using Evo20.Commands.Abstract;

namespace Evo20.Commands.AnswerCommands
{
    public class AxisStatusAnswer : AnswerCommand
    {
        public bool IsInitialized
        {
            get;
        }
        public bool IsZero1Found
        {
            get;
        }
        public bool IsZero2Found
        {
            get;
        }
        public bool IsZero3Found
        {
            get;
        }
        public bool Power
        {
            get;
        }
        public bool IsAxis1Move
        {
            get;
        }
        public bool IsAxis2Move
        {
            get;
        }
        public bool IsAxis3Move
        {
            get;
        }
        public bool IsLimit1
        {
            get;
        }
        public bool IsLimit2
        {
            get;
        }
        public bool IsLimit3
        {
            get;
        }
        public bool IsAxis1Stop
        {
            get;
        }
        public bool IsAxis2Stop
        {
            get;
        }
        public bool IsAxis3Stop
        {
            get;
        }
        public bool IsError
        {
            get;
        }
        public AxisStatusAnswer(String value)
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
            IsInitialized = data[0] == '1';

            IsZero1Found = data[1] == '1';
            IsZero2Found = data[2] == '1';
            IsZero3Found = data[3] == '1';  
                  
            Power = data[4] == '1';  
                    
            IsAxis1Move = data[8] == '1';           
            IsAxis2Move = data[9] == '1';
            IsAxis3Move = data[10] == '1';

            IsLimit1 = data[11] == '1';
            IsLimit2 = data[12] == '1';
            IsLimit3 = data[13] == '1';

            IsAxis1Stop = data[14] == '1';
            IsAxis2Stop = data[15] == '1';
            IsAxis3Stop = data[16] == '1';

            IsError = data[17] == '1';
        }
    }
}
