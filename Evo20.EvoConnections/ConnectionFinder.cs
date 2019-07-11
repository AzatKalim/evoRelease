using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Evo20.Utils;
using Evo20.Commands.CommndsWithAnswer;

namespace Evo20.EvoConnections
{
    class ConnectionFinder
    {
        struct UdpState
        {
            public UdpClient client;
            public IPEndPoint point;
        }

        public bool MessageReceived { get; set; }

        public IPEndPoint ReceivedEndPoint { get; set;}

        public int SendedPort { get; set; }

        public bool TryConnect(int portNumber)
        {
            try
            {
                var endPoint = new IPEndPoint(IPAddress.Parse(Config.Instance.RemoteIpAdress),
                    portNumber);

                using (var sender = new UdpClient(endPoint))
                using (var receivingUdpClient = new UdpClient(portNumber))
                {
                    Log.Instance.Info($"Отравка сообщения на тестовый порт {portNumber}");
                    sender.BeginReceive(ReceiveCallback, new UdpState
                    {
                        point = endPoint,
                        client = sender
                    });
                    receivingUdpClient.BeginReceive(ReceiveCallback, new UdpState
                    {
                        point = endPoint,
                        client = receivingUdpClient
                    });
                    byte[] bytes = Encoding.UTF8.GetBytes(new AxisStatus().ToString());
                    if (bytes.Length == sender.Send(bytes, bytes.Length))
                    {
                        for (int i = 0; i < 5 || !MessageReceived; i++)
                        {
                            Thread.Sleep(100);
                        }
                        return MessageReceived;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (SocketException)
            {
                return false;
            }
        }


        public int FindPort(out int type)
        {
            for (int i = 500; i < 15000; i++)
            {
                if (!TryConnect(i)) continue;
                type = ReceivedEndPoint != null ? 1 : 0;
                return i;
            }
            type = -1;
            return -1;
            
        }

        public void ReceiveCallback(IAsyncResult ar)
        {
            UdpClient u = ((UdpState)(ar.AsyncState)).client;
            var e = ((UdpState)(ar.AsyncState)).point;

            byte[] receiveBytes = u.EndReceive(ar, ref e);
            string receiveString = Encoding.ASCII.GetString(receiveBytes);

            Log.Instance.Debug($"Received: {receiveString} {e.Address} {e.Port}");
            MessageReceived = true;
            ReceivedEndPoint = e;
        }
    }
}
