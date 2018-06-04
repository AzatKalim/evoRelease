using System;
using System.Collections.Generic;
using System.Text;

namespace Evo20.EvoCommandsLib
{
    public enum ModeParam
    {
        Position,
        Speed,
        Sinus,
        TrapezePosition,
        TrapezeSpeed,
        TrapezeAcceleration,
        EnterAnalogPosition,
        EnterAnalogSpeed,
        EnterTimeRealPosition,
        EnterTimeRealSpeed
    }
    public class Mode:Command
    {
        ModeParam param;
        Axis axis;
        public Mode(ModeParam param, Axis axis)
        {
            this.param = param;
            this.axis = axis;
            is_answer = false;
            have_answer = true;
            can_send = true;
        }
        private int ModeParamToInt(ModeParam param)
        {
            switch(param)
            {
                case ModeParam.Position:
                    return 0;
                case ModeParam.Speed:
                    return 1;
                case ModeParam.Sinus:
                    return 2;
                case ModeParam.TrapezePosition:
                    return 3;
                case ModeParam.TrapezeSpeed:
                    return 4;
                case ModeParam.TrapezeAcceleration:
                    return 5;
                case ModeParam.EnterAnalogPosition:
                    return 6;
                case ModeParam.EnterAnalogSpeed:
                    return 8;
                case ModeParam.EnterTimeRealPosition:
                    return 9;
                case ModeParam.EnterTimeRealSpeed:
                    return 10;
                default: 
                    return 0;
            }
        }
        public override string ToString()
        {
            string buffer = "AXE.CONS.MOD ";
            return buffer + ModeParamToInt(param) + " " + AxisToInt(axis);
        }
    }
}
