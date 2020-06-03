using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using Texnomic.SecureDNS.Extensions;
using Texnomic.SecureDNS.Serialization;
using Texnomic.Socks5.Enum;
using Texnomic.Socks5.Options;

namespace Texnomic.Socks5
{
    public class Socks5
    {
        private const byte SubNegotiationVersion = 0x01;
        private const byte SocksVersion = 0x05;
        private readonly IOptionsMonitor<Socks5Options> Options;
        private Socket Socket;
        private bool Initialized;

        public Socks5(IOptionsMonitor<Socks5Options> Options)
        {
            this.Options = Options;
            Options.OnChange(async (ChangedOptions) => await Initialize(ChangedOptions));
        }

        private async Task Initialize(Socks5Options Options)
        {
            Socket = new Socket(Options.SocketType, Options.ProtocolType);

            Socket = Options.Authentication == Authentication.NoAuthentication
                        ? await Connect(Options.IPEndPoint)
                        : await Connect(Options.IPEndPoint, Options.Username, Options.Password);

            Initialized = true;
        }

        private async Task<Socket> Connect(IPEndPoint IPEndPoint)
        {
            await Socket.ConnectAsync(IPEndPoint);

            await Socket.SendAsync(BuildHelloMessage(Authentication.NoAuthentication), SocketFlags.None);

            var Buffer = new byte[2];

            var Size = await Socket.ReceiveAsync(Buffer, SocketFlags.None);

            if (Size != 2) throw new Exception();

            if (Buffer[0] != SocksVersion) throw new Exception();

            if (Buffer[1] != (byte)Authentication.NoAuthentication) throw new Exception();

            return Socket;
        }

        private async Task<Socket> Connect(IPEndPoint IPEndPoint, string Username, string Password)
        {
            await Socket.ConnectAsync(IPEndPoint);

            await Socket.SendAsync(BuildHelloMessage(Authentication.UsernamePassword), SocketFlags.None);

            var Buffer = new byte[2];

            var Size = await Socket.ReceiveAsync(Buffer, SocketFlags.None);

            if (Size != 2) throw new Exception();

            if (Buffer[0] != SocksVersion) throw new Exception();

            if (Buffer[1] != (byte)Authentication.UsernamePassword) throw new Exception();

            await Socket.SendAsync(BuildAuthenticationMessage(Username, Password), SocketFlags.None);

            Buffer = new byte[2];

            Size = await Socket.ReceiveAsync(Buffer, SocketFlags.None);

            if (Size != 2) throw new Exception();

            if (Buffer[0] != SubNegotiationVersion) throw new Exception();

            if (Buffer[1] != 1) throw new Exception();

            return Socket;
        }


        public async Task<Socket> Tunnel(string Domain, int Port)
        {
            if (!Initialized) await Initialize(Options.CurrentValue);

            var Message = BuildRequestMessage(Command.Connect, Domain, Port);

            await Socket.SendAsync(Message, SocketFlags.None);

            var Buffer = new byte[7 + Domain.Length];

            var Size = await Socket.ReceiveAsync(Buffer, SocketFlags.None);

            if (Buffer[0] != SocksVersion) throw new Exception();
            if (Buffer[1] > 8) throw new Exception();
            if (Buffer[1] != 0) throw new Exception();
            if (Buffer[2] != 0) throw new Exception();
            if (Buffer[3] != 1 && Buffer[3] != 3 && Buffer[3] != 4) throw new Exception();


            var Address = Buffer[3].AsEnum<Address>();

            switch (Address)
            {
                case Address.Domain:
                    if (Size < 7 + Domain.Length) throw new Exception();
                    break;
                case Address.IPv4:
                    if (Size < 10) throw new Exception();
                    break;
                case Address.IPv6:
                    if (Size < 22) throw new Exception();
                    break;
                default:
                    throw new Exception();
            }


            return Socket;
        }

        public async Task<Socket> Tunnel(IPEndPoint IPEndPoint)
        {
            if (!Initialized) await Initialize(Options.CurrentValue);

            var Message = BuildRequestMessage(Command.Connect, IPEndPoint);

            await Socket.SendAsync(Message, SocketFlags.None);

            var Buffer = new byte[10];

            var Size = await Socket.ReceiveAsync(Buffer, SocketFlags.None);

            var IPAddress = IPEndPoint.AddressFamily == AddressFamily.InterNetwork ? Address.IPv4 : Address.IPv6;

            if (Buffer[0] != SocksVersion) throw new Exception();
            if (Buffer[1] > 8) throw new Exception();
            if (Buffer[1] != 0) throw new Exception();
            if (Buffer[2] != 0) throw new Exception();
            if (Buffer[3] != 1 && Buffer[3] != 3 && Buffer[3] != 4) throw new Exception();
            if(IPAddress != Buffer[3].AsEnum<Address>()) throw new Exception();

            switch (IPAddress)
            {
                case Address.IPv4:
                    if (Size < 10) throw new Exception();
                    break;
                case Address.IPv6:
                    if (Size < 22) throw new Exception();
                    break;
                default:
                    throw new Exception();
            }

            return Socket;
        }


        private static byte[] BuildHelloMessage(Authentication Authentication)
        {
            var Authenticated = Authentication == Authentication.UsernamePassword;
            var Size = Authentication == Authentication.UsernamePassword ? 4 : 3;
            var DnStream = new DnStream((ushort)Size);
            DnStream.WriteByte(SocksVersion);
            DnStream.WriteByte((byte)(Authenticated ? 2 : 1));
            DnStream.WriteByte(0);
            if (Authenticated) DnStream.WriteByte((byte)Authentication);
            return DnStream.ToArray();
        }

        private static byte[] BuildRequestMessage(Command Command, IPEndPoint IPEndPoint)
        {
            var IPAddress = IPEndPoint.Address.GetAddressBytes();
            var IPAddressType = IPEndPoint.AddressFamily == AddressFamily.InterNetwork ? Address.IPv4 : Address.IPv6;
            var DnStream = new DnStream((ushort)(6 + IPAddress.Length));
            DnStream.WriteByte(SocksVersion);
            DnStream.WriteByte((byte)Command);
            DnStream.WriteByte(0);
            DnStream.WriteByte((byte)IPAddressType);
            DnStream.WriteBytes(IPAddress);
            DnStream.WriteUShort((ushort)IPEndPoint.Port);
            return DnStream.ToArray();
        }

        private static byte[] BuildRequestMessage(Command Command, string Domain, int Port)
        {
            var DnStream = new DnStream((ushort)(7 + Domain.Length));
            DnStream.WriteByte(SocksVersion);
            DnStream.WriteByte((byte)Command);
            DnStream.WriteByte(0);
            DnStream.WriteByte((byte)Address.Domain);
            DnStream.WriteByte((byte)Domain.Length);
            DnStream.WriteString(Domain);
            DnStream.WriteUShort((ushort)Port);
            return DnStream.ToArray();
        }

        private static byte[] BuildAuthenticationMessage(string Username, string Password)
        {
            var DnStream = new DnStream((ushort)(3 + Username.Length + Password.Length));
            DnStream.WriteByte(SubNegotiationVersion);
            DnStream.WriteByte((byte)Username.Length);
            DnStream.WriteString(Username);
            DnStream.WriteByte((byte)Password.Length);
            DnStream.WriteString(Password);
            return DnStream.ToArray();
        }
    }
}
