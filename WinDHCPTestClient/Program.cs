using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using WinDHCP.Library;
using NetworkInterface = System.Net.NetworkInformation.NetworkInterface;

namespace WinDHCPTestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Int32 sessionId = (Int32)DateTime.Now.Ticks;

            using (Socket dhcpClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                dhcpClientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                dhcpClientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                dhcpClientSocket.Bind(new IPEndPoint(IPAddress.Parse("0.0.0.0"), 68));

                DhcpMessage discoverMessage = new DhcpMessage();

                discoverMessage.SessionId = sessionId;
                discoverMessage.Operation = DhcpOperation.BootRequest;
                discoverMessage.Hardware = HardwareType.Ethernet;
                discoverMessage.Flags = 128;

                Byte[] physicalAddr = NetworkInterface.GetAllNetworkInterfaces()[0].GetPhysicalAddress().GetAddressBytes();
                discoverMessage.ClientHardwareAddress = physicalAddr;

                discoverMessage.AddOption(DhcpOption.DhcpMessageType, (Byte)DhcpMessageType.Discover);

                Byte[] clientId = new Byte[physicalAddr.Length + 1];
                clientId[0] = (Byte)1;
                physicalAddr.CopyTo(clientId, 1);
                discoverMessage.AddOption(DhcpOption.AutoConfig, 1);
                discoverMessage.AddOption(DhcpOption.Hostname, Encoding.ASCII.GetBytes(Environment.MachineName));
                discoverMessage.AddOption(DhcpOption.ClassId, 77, 83, 70, 84, 32, 53, 46, 48);
                discoverMessage.AddOption(DhcpOption.ClientId, clientId);
                discoverMessage.AddOption(DhcpOption.ParameterList, 1, 15, 3, 6, 44, 46, 47, 31, 33, 121, 249, 43);

                dhcpClientSocket.SendTo(discoverMessage.ToArray(), new IPEndPoint(IPAddress.Broadcast, 67));

                Byte[] buffer = new Byte[1024];
                Int32 len = dhcpClientSocket.Receive(buffer);
                Byte[] messageData = new Byte[len];
                Array.Copy(buffer, messageData, Math.Min(len, buffer.Length));
                DhcpMessage responseMessage = new DhcpMessage(messageData);

                Console.ReadLine();
            }
        }
    }
}
