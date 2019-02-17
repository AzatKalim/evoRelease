using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Evo20.EvoConnections;
using Evo20.Commands;

namespace Evo20.Connections.Tests
{
    [TestClass]
    public class CommandHandlerTests
    {
        [TestMethod]
        public void TestRecognazeCommandReturnsNull()
        {
            string test = null;
            Assert.IsNull(CommandHandler.RecognizeCommand(test));
            test = string.Empty;
            Assert.IsNull(CommandHandler.RecognizeCommand(test));
            test = "test";
            Assert.IsNull(CommandHandler.RecognizeCommand(test));
            test = "test=test=test";
            Assert.IsNull(CommandHandler.RecognizeCommand(test));
        }
       
        [TestMethod]
        public void TestRecognazeCommandReturnsAxis_Status_answer()
        {
            string test = Axis_Status.Command + "=" + "F2C22A3B\0";
            Assert.IsInstanceOfType(CommandHandler.RecognizeCommand(test), typeof(Axis_Status_answer));
        }

        [TestMethod]
        public void TestRecognazeCommandReturnsTemperature_status_answer()
        {
            string test = Temperature_status.Command + "=" + "F2C22A3B\0";
            Assert.IsInstanceOfType(CommandHandler.RecognizeCommand(test), typeof(Temperature_status_answer));
        }

        [TestMethod]
        public void TestRecognazeCommandReturnsRotary_joint_temperature_Query_answer()
        {
            string test = new Rotary_joint_temperature_Query(Axis.First).ToString() + "=" + "F2C22A3B\0";
            Assert.IsInstanceOfType(CommandHandler.RecognizeCommand(test), typeof(Rotary_joint_temperature_Query_answer));
            test = new Rotary_joint_temperature_Query(Axis.Second).ToString() + "=" + "F2C22A3B\0";
            Assert.IsInstanceOfType(CommandHandler.RecognizeCommand(test), typeof(Rotary_joint_temperature_Query_answer));
        }

        [TestMethod]
        public void TestRecognazeCommandReturnsAxis_Position_Query_answer()
        {
            string test = new Axis_Position_Query(Axis.First).ToString() + "=" + "F2C22A3B\0";
            Assert.IsInstanceOfType(CommandHandler.RecognizeCommand(test), typeof(Axis_Position_Query_answer));
            test = new Axis_Position_Query(Axis.Second).ToString() + "=" + "F2C22A3B\0";
            Assert.IsInstanceOfType(CommandHandler.RecognizeCommand(test), typeof(Axis_Position_Query_answer));
        }

        [TestMethod]
        public void TestRecognazeCommandReturnsAxis_Rate_Query_answer()
        {
            string test = new Axis_Rate_Query(Axis.First).ToString() + "=" + "F2C22A3B\0";
            Assert.IsInstanceOfType(CommandHandler.RecognizeCommand(test), typeof(Axis_Rate_Query_answer));
            test = new Axis_Rate_Query(Axis.Second).ToString() + "=" + "F2C22A3B\0";
            Assert.IsInstanceOfType(CommandHandler.RecognizeCommand(test), typeof(Axis_Rate_Query_answer));
        }
    }
}
