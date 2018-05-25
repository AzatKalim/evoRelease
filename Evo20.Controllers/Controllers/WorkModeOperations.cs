using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Evo20.Controllers;

namespace Evo20.Controllers
{
    public enum WorkMode
    {
        NoMode,
        CheckMode,
        CalibrationMode,
        Stop,
        Pause,
        Error
    }
    static class WorkModeOperations
    {
        public static string ToView(this WorkMode mode)
        {
            switch (mode)
            {
                case(WorkMode.NoMode):
                        return "Режим не задан";
                case(WorkMode.CheckMode):
                        return "Режим проверки";
                case(WorkMode.CalibrationMode):
                        return "Режим калибровки";
                case(WorkMode.Pause):
                        return "Пауза";
                case (WorkMode.Stop):
                        return "Остановлен";
                case (WorkMode.Error):
                        return "Ошибка!";
            }
            return null;
        }
    }
}
