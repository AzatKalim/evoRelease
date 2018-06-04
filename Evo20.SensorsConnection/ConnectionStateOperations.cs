
namespace Evo20.SensorsConnection
{
    /// <summary>
    /// Состояния соединения 
    /// </summary>
    public enum ConnectionStatus
    {
        CONNECTED,
        DISCONNECTED,
        PAUSE,
        ERROR
    }

    public static class ConnectionStateOperations
    {
        public static string ToText(this ConnectionStatus state)
        {
            switch (state)
            {
                case (ConnectionStatus.CONNECTED):
                    return "Соединен";
                case (ConnectionStatus.DISCONNECTED):
                    return "Разьединен";
                case (ConnectionStatus.PAUSE):
                    return "Пауза";
                case (ConnectionStatus.ERROR):
                    return "Ошибка";
            }
            return null;
        }
    }
}
