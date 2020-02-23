using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Texnomic.DNS.Servers.Tests
{
    [TestClass]
    public class INA
    {
        [TestMethod]
        public async Task Try()
        {
            var Packet = new byte[22];

            Packet[00] = 0b01101011;
            Packet[01] = 0b00110001;
            Packet[02] = 0b00000001;
            Packet[03] = 0b00000000;
            Packet[04] = 0b00000000;
            Packet[05] = 0b00000001;
            Packet[06] = 0b00000000;
            Packet[07] = 0b00000000;
            Packet[08] = 0b00000000;
            Packet[09] = 0b00000000;
            Packet[10] = 0b00000000;
            Packet[11] = 0b00000000;

            Packet[12] = 0b00000011;
            Packet[13] = 0b01101001;
            Packet[14] = 0b01101110;
            Packet[15] = 0b01100001;

            Packet[16] = 0b01000000;
            Packet[17] = 0b00001100;

            Packet[18] = 0b00000000;
            Packet[19] = 0b00000001;
            Packet[20] = 0b00000000;
            Packet[21] = 0b00000001;

            var UdpClient = new UdpClient();

            await UdpClient.SendAsync(Packet, Packet.Length, new IPEndPoint(IPAddress.Parse("8.8.8.8"), 53));
        }
    }
}
