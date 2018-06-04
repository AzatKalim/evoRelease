using System;
using System.Collections.Generic;
using System.Text;

namespace Evo20.EvoCommandsLib
{
    public class Axis_Position_Query_answer:Command
    {
        public double position
        {
            get;
            private set;
        }
        public Axis axis
        {
            get;
            private set;
        }
        public Axis_Position_Query_answer()
        {
            is_answer = true;
            have_answer = false;
            can_send = false;
        }
        public Axis_Position_Query_answer(String position,Axis axis)
        {
            position = position.Replace(',', '.');
           this.position= Convert.ToDouble(position);
           this.axis= axis;
           is_answer = true;
           have_answer = false;
           can_send = false;
        }
        public override string ToString()
        {
            return "AXE.TELL.POS";
        }

    }
}
