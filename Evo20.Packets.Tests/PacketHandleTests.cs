using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Evo20.Packets.Tests
{
    [TestClass]
    public class PacketHandleTests
    {
        private const int PacketSize = 42; 
        [TestMethod]
        public void TestHandleAllBytes()
        {
            var p = new Packet(1)
            {
                A = new[] {-0.0007554683834, 0.04095339961, 0.0003258381039},
                W = new double[] {0,0,0},
                U = new[] {0.1510306019, 0.1499569397, 0.151255725}
            };
            var p2 = new Packet(2)
            {
                A = new[] {-0.0007338766009, 0.04100187682, 0.0001815538853},
                W = new[] {-0.0001476164907, -7.347203791e-05, 0.0003942903131},
                U = new[] {0.3030270357, 0.3030270357, 0.3030270357}
            };
            var bytes = ReadBytesTest().ToList();
            var packetsList = new List<Packet>();
            while (Packet.FindPacketBegin(ref bytes))
            {
                var temp = new byte[PacketSize];
                bytes.CopyTo(0, temp, 0, PacketSize);
                bytes.RemoveRange(0, PacketSize);
                packetsList.Add(Packet.FirstPacketHandle(temp));
            }
            Assert.AreEqual(packetsList.Count, 4);
            //Assert.IsTrue(packetsList[0].Equals(p));
            Assert.IsTrue(packetsList[1].Equals(p2));

        }

        private byte[] ReadBytesTest()
        {
            var fileName = "sample.bin";
            return File.ReadAllBytes(fileName);
        }

        [TestMethod]
        public void TestConvertParam()
        {
            var number = 100500;
            var bytes = BitConverter.GetBytes(number);
            bool rangeFlag = false;
            bool dataFlag = false;
            var n = Packet.ConvertParam(bytes,ref rangeFlag, ref dataFlag);
            int numbersCount = 15;
            Assert.AreEqual(Math.Round(n, numbersCount), Math.Round(0.00004679895937442779541015625, numbersCount));
        }
    }
}
