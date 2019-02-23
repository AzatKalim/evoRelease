
namespace Evo20.Utils
{
    public enum ConnectionStatus
    {
        Connected,
        Disconnected,
        Pause,
        Error
    }

    public static class ConnectionStateOperations
    {
        public static string ToText(this ConnectionStatus state)
        {
            switch (state)
            {
                case (ConnectionStatus.Connected):
                    return "Соединен";
                case (ConnectionStatus.Disconnected):
                    return "Разьединен";
                case (ConnectionStatus.Pause):
                    return "Пауза";
                case (ConnectionStatus.Error):
                    return "Ошибка";
            }
            return null;
        }
    }
}
