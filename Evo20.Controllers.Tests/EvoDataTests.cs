using Evo20.Commands.Abstract;
using Evo20.Commands.AnswerCommands;
using Evo20.Controllers.Data;
using Evo20.Sensors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Evo20.Controllers.Tests
{
    [TestClass]
    public class EvoDataTests
    {
        [TestMethod]
        public void TestGetCommandInfoCheckPositionReachedEvent()
        {
            EvoData.Instance.PositionReachedEvent.Reset();
            EvoData.Instance.CurrentPosition = new Position(12,25);
            EvoData.Instance.NextPosition = new Position(0, 0, 0, 64);
            var command = new AxisRateQueryAnswer("+064.5", Axis.Second);
            EvoData.Instance.GetCommandInfo(command);
            Assert.IsTrue(EvoData.Instance.PositionReachedEvent.WaitOne());
        }

        [TestMethod]
        public void TestGetCommandInfoCheckPositionReachedEvent2()
        {
            EvoData.Instance.PositionReachedEvent.Reset();
            EvoData.Instance.CurrentPosition = new Position(-0.0001, 90, -17.365, 0);
            EvoData.Instance.NextPosition = new Position(0, 90, 0, 0);
            var command = new AxisRateQueryAnswer("+063.001", Axis.First);
            EvoData.Instance.GetCommandInfo(command);
            Assert.IsTrue(EvoData.Instance.PositionReachedEvent.WaitOne(100));
        }

        [TestMethod]
        public void TestGetCommandInfoCheckPositionReachedEvent3()
        {
            EvoData.Instance.PositionReachedEvent.Reset();
            EvoData.Instance.CurrentPosition = new Position(0, 0, 25, 35);
            EvoData.Instance.NextPosition = new Position(0, 64);
            var command = new AxisPositionQueryAnswer("+064.001", Axis.Second);
            EvoData.Instance.GetCommandInfo(command);
            Assert.IsTrue(EvoData.Instance.PositionReachedEvent.WaitOne());
        }
    }
}
