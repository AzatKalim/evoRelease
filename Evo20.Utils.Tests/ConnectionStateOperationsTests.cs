using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Evo20.Utils.Tests
{
    [TestClass]
    public class ConnectionStateOperationsTests
    {
        [TestMethod]
        public void ToTextReturnsCONNECTED()
        {
            ConnectionStatus connected = ConnectionStatus.CONNECTED;
            Assert.AreEqual(connected.ToText(), "Соединен");
        }

        [TestMethod]
        public void ToTextReturnsERROR()
        {
            ConnectionStatus connected = ConnectionStatus.ERROR;
            Assert.AreEqual(connected.ToText(), "Ошибка");
        }

        [TestMethod]
        public void ToTextReturnsDISCONNECTED()
        {
            ConnectionStatus connected = ConnectionStatus.DISCONNECTED;
            Assert.AreEqual(connected.ToText(), "Разьединен");
        }

        [TestMethod]
        public void ToTextReturnsPAUSE()
        {
            ConnectionStatus connected = ConnectionStatus.PAUSE;
            Assert.AreEqual(connected.ToText(), "Пауза");
        }
    }
}
