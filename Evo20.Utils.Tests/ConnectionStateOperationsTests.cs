using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Evo20.Utils.Tests
{
    [TestClass]
    public class ConnectionStateOperationsTests
    {
        [TestMethod]
        public void TestToTextReturnsConnected()
        {
            ConnectionStatus connected = ConnectionStatus.Connected;
            Assert.AreEqual(connected.ToText(), "Соединен");
        }

        [TestMethod]
        public void TestToTextReturnsError()
        {
            ConnectionStatus connected = ConnectionStatus.Error;
            Assert.AreEqual(connected.ToText(), "Ошибка");
        }

        [TestMethod]
        public void TestToTextReturnsDisconnected()
        {
            ConnectionStatus connected = ConnectionStatus.Disconnected;
            Assert.AreEqual(connected.ToText(), "Разьединен");
        }

        [TestMethod]
        public void TestToTextReturnsPause()
        {
            ConnectionStatus connected = ConnectionStatus.Pause;
            Assert.AreEqual(connected.ToText(), "Пауза");
        }
    }
}
