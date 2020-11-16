using System.Timers;

namespace Evo20.Utils.Extensions
{
    public static class TimerExtensions
    {
        public static void Reset(this Timer timer)
        {
            timer.Stop();
            timer.Start();
        }

    }
}
