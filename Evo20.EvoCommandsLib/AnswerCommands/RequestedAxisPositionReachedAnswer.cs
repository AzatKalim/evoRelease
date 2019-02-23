using Evo20.Commands.Abstract;

namespace Evo20.Commands.AnswerCommands
{
    public class RequestedAxisPositionReachedAnswer : AnswerCommand 
    {
        public bool AxisXReached
        {
            get;
        }
        public bool AxisYReached
        {
            get;
        }
        public RequestedAxisPositionReachedAnswer(string text)
        {
            string[] temp = text.Split(',');
            AxisXReached = temp[0][0]=='1';
            AxisYReached = temp[1][0] == '1';                      
        }
    }
}
