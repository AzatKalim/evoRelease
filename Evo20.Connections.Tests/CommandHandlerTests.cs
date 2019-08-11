using Microsoft.VisualStudio.TestTools.UnitTesting;
using Evo20.EvoConnections;
using Evo20.Commands.Abstract;
using Evo20.Commands.AnswerCommands;
using Evo20.Commands.CommndsWithAnswer;

namespace Evo20.Connections.Tests
{
    [TestClass]
    public class CommandHandlerTests
    { 
        [TestMethod]
        public void TestRecognazeCommandReturnsNull()
        {
            Assert.IsNull(CommandHandler.RecognizeCommand(null));
            var test = string.Empty;
            Assert.IsNull(CommandHandler.RecognizeCommand(test));
            test = "test";
            Assert.IsNull(CommandHandler.RecognizeCommand(test));
            test = "test=test=test";
            Assert.IsNull(CommandHandler.RecognizeCommand(test));
        }
       
        [TestMethod]
        public void TestRecognazeCommandReturnsAxis_Status_answer()
        {
            string test = AxisStatus.Command + "=" + "F2C22A3B\0";
            Assert.IsInstanceOfType(CommandHandler.RecognizeCommand(test), typeof(AxisStatusAnswer));
        }

        [TestMethod]
        public void TestRecognazeCommandReturnsTemperature_status_answer()
        {
            string test = TemperatureStatus.Command + "=" + "F2C22A3B\0";
            Assert.IsInstanceOfType(CommandHandler.RecognizeCommand(test), typeof(TemperatureStatusAnswer));
        }

        [TestMethod]
        public void TestRecognazeCommandReturnsRotary_joint_temperature_Query_answer()
        {
            string test = new RotaryJointTemperatureQuery(Axis.First) + "=" + "F2C22A3B\0";
            Assert.IsInstanceOfType(CommandHandler.RecognizeCommand(test), typeof(RotaryJointTemperatureQueryAnswer));
            test = new RotaryJointTemperatureQuery(Axis.Second) + "=" + "F2C22A3B\0";
            Assert.IsInstanceOfType(CommandHandler.RecognizeCommand(test), typeof(RotaryJointTemperatureQueryAnswer));
        }

        [TestMethod]
        public void TestRecognazeCommandReturnsAxis_Position_Query_answer()
        {
            string test = new AxisPositionQuery(Axis.First) + "=" + "F2C22A3B\0";
            Assert.IsInstanceOfType(CommandHandler.RecognizeCommand(test), typeof(AxisPositionQueryAnswer));
            test = new AxisPositionQuery(Axis.Second) + "=" + "F2C22A3B\0";
            Assert.IsInstanceOfType(CommandHandler.RecognizeCommand(test), typeof(AxisPositionQueryAnswer));
        }

        [TestMethod]
        public void TestRecognazeCommandReturnsAxis_Rate_Query_answer()
        {
            string test = new AxisRateQuery(Axis.First) + "=" + "F2C22A3B\0";
            Assert.IsInstanceOfType(CommandHandler.RecognizeCommand(test), typeof(AxisRateQueryAnswer));
            test = new AxisRateQuery(Axis.Second) + "=" + "F2C22A3B\0";
            Assert.IsInstanceOfType(CommandHandler.RecognizeCommand(test), typeof(AxisRateQueryAnswer));
        }
        [TestMethod]
        public void TestRecognazeCommandReturnsAxisRateQueryAnswer2()
        {
            string test = "AXE.TELL.VIT 1=+064.001\0";
            var command = CommandHandler.RecognizeCommand(test);
            Assert.IsInstanceOfType(command, typeof(AxisRateQueryAnswer));          
            Assert.AreEqual((int)(command as AxisRateQueryAnswer).SpeedOfRate,64);
        }
    }
}
