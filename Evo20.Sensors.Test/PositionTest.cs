using System;
using System.Security.Cryptography.X509Certificates;
using Evo20.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Evo20.Sensors.Test
{
    [TestClass]
    public class PositionTest
    {
        [TestMethod]
        public void PositionEqulas_xy00()
        {
            var x = 10;
            var y = -128;
            var etalonPosition = new Position(x, y, 0, 0);
            var currentPosition = new Position(x, y, 0, 0);
            Assert.IsTrue(currentPosition.Equals(etalonPosition));
            Assert.IsTrue(etalonPosition.Equals(currentPosition));
        }

        [TestMethod]
        public void PositionNotEqulas_xyt0()
        {
            var x = 10;
            var y = -128;
            var etalonPosition = new Position(x, y, 0, 0);
            var currentPosition = new Position(x, y, Config.Instance.SpeedDeviation + 1, 0);
            Assert.IsFalse(currentPosition.Equals(etalonPosition));
            Assert.IsFalse(etalonPosition.Equals(currentPosition));
        }
        [TestMethod]
        public void PositionNotEqulas_xy0t()
        {
            var x = 10;
            var y = -128;
            var etalonPosition = new Position(x, y, 0, 0);
            var currentPosition = new Position(x, y, 0, Config.Instance.SpeedDeviation + 1);
            Assert.IsFalse(currentPosition.Equals(etalonPosition));
            Assert.IsFalse(etalonPosition.Equals(currentPosition));
        }

        [TestMethod]
        public void PositionEqulas_x00s()
        {
            var x = 10;
            var s = 15;
            var etalonPosition = new Position(x, 0, 0, s);
            var currentPosition = new Position(x, 0, 0, s);
            Assert.IsTrue(currentPosition.Equals(etalonPosition));
            Assert.IsTrue(etalonPosition.Equals(currentPosition));
        }

        [TestMethod]
        public void PositionNotEqulas_x0ts()
        {
            var x = 10;
            var s = 15;
            var etalonPosition = new Position(x, 0, 0, s);
            var currentPosition = new Position(x, 0, Config.Instance.SpeedDeviation + 1, s);
            Assert.IsFalse(currentPosition.Equals(etalonPosition));
            Assert.IsFalse(etalonPosition.Equals(currentPosition));
        }

        [TestMethod]
        public void PositionEqulas_xt0s()
        {
            var x = 10;
            var s = 15;
            var etalonPosition = new Position(x, Config.Instance.AxisDeviation +1, 0, s);
            var currentPosition = new Position(x, 0, 0, s);
            Assert.IsTrue(currentPosition.Equals(etalonPosition));
            Assert.IsTrue(etalonPosition.Equals(currentPosition));
        }

        [TestMethod]
        public void PositionNotEqulas_t00s()
        {
            var s = 15;
            var etalonPosition = new Position(Config.Instance.AxisDeviation + 1, 0, s);
            var currentPosition = new Position(0, 0, 0, s);
            Assert.IsFalse(currentPosition.Equals(etalonPosition));
            Assert.IsFalse(etalonPosition.Equals(currentPosition));
        }

        [TestMethod]
        public void PositionEqulas_0ys0()
        {
            var y = 10;
            var s = 15;
            var etalonPosition = new Position(0, y, 0, s);
            var currentPosition = new Position(0, 0, 0, s);
            Assert.IsTrue(currentPosition.Equals(etalonPosition));
            Assert.IsTrue(etalonPosition.Equals(currentPosition));
        }

        [TestMethod]
        public void PositionEqulas_00sh()
        {
            var s = 15;
            var h = -128;
            var etalonPosition = new Position(1, -50, h, s);
            var currentPosition = new Position(-66, 1, h, s);
            Assert.IsTrue(currentPosition.Equals(etalonPosition));
            Assert.IsTrue(etalonPosition.Equals(currentPosition));
        }
    }
}
