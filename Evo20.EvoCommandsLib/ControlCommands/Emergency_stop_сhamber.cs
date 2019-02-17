using Evo20.Commands.Abstract;

namespace Evo20.Commands
{
    /// <summary>
    /// Екстренное отключение тепловой камеры
    /// </summary>
    public class Emergency_stop_сhamber : ControlCommand
    {
        public override string ToString()
        {
            string buffer = "CLIM.AU";
            return buffer;
        }
    }
}
