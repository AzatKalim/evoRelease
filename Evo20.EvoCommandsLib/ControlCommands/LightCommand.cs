﻿using Evo20.Commands.Abstract;

namespace Evo20.Commands.ControlCommands
{
    public class LightCommand : ControlCommand
    {
        readonly bool _lightMode;
        public LightCommand(bool lightMode)
        {
            _lightMode = lightMode;
        }
        public override string ToString()
        {
            return $"CLIM.ECL {_lightMode}";
        }
    }
}
