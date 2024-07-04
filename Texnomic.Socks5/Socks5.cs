namespace Texnomic.Socks5;

public class Socks5(IOptionsMonitor<Socks5Options> Options)
{
    private const byte SubNegotiationVersion = 0x01;
    private const byte SocksVersion = 0x05;
    private Socket Socket;
    private bool Initialized;

    private async Task Initialize()
    {
        Socket = new Socket(Options.CurrentValue.SocketType, Options.CurrentValue.ProtocolType);

        Socket = Options.CurrentValue.Authentication == Authentication.NoAuthentication
            ? await Hello(Options.CurrentValue.IPEndPoint)
            : await Hello(Options.CurrentValue.IPEndPoint, Options.CurrentValue.Username, Options.CurrentValue.Password);

        Initialized = true;
    }

    private async Task<Socket> Hello(IPEndPoint IPEndPoint)
    {
        await Socket.ConnectAsync(IPEndPoint);

        await Socket.SendAsync(CreateHello(Authentication.NoAuthentication), SocketFlags.None);

        var Buffer = new byte[2];

        var Size = await Socket.ReceiveAsync(Buffer, SocketFlags.None);

        if (Size != 2) throw new Exception();

        if (Buffer[0] != SocksVersion) throw new Exception();

        if (Buffer[1] != (byte)Authentication.NoAuthentication) throw new Exception();

        return Socket;
    }

    private async Task<Socket> Hello(IPEndPoint IPEndPoint, string Username, string Password)
    {
        await Socket.ConnectAsync(IPEndPoint);

        await Socket.SendAsync(CreateHello(Authentication.UsernamePassword), SocketFlags.None);

        var Buffer = new byte[2];

        var Size = await Socket.ReceiveAsync(Buffer, SocketFlags.None);

        if (Size != 2) throw new Exception();

        if (Buffer[0] != SocksVersion) throw new Exception();

        if (Buffer[1] != (byte)Authentication.UsernamePassword) throw new Exception();

        await Socket.SendAsync(CreateAuthentication(Username, Password), SocketFlags.None);

        Buffer = new byte[2];

        Size = await Socket.ReceiveAsync(Buffer, SocketFlags.None);

        if (Size != 2) throw new Exception();

        if (Buffer[0] != SubNegotiationVersion) throw new Exception();

        if (Buffer[1] != 1) throw new Exception();

        return Socket;
    }


    public async Task<Socket> Connect(string Domain, int Port)
    {
        if (!Initialized) await Initialize();

        var Message = CreateRequest(Command.Connect, Domain, Port);

        //var Message = CreateRequest(Command.Resolve, Domain, Port);

        await Socket.SendAsync(Message, SocketFlags.None);

        var Buffer = new byte[22];

        var Size = await Socket.ReceiveAsync(Buffer, SocketFlags.None);

        Buffer = Buffer[..Size];

        if (Buffer[0] != SocksVersion) throw new Exception();

        var Reply = Buffer[1].AsEnum<Reply>();

        if (Reply != Reply.Succeeded) throw new Exception();

        if (Buffer[2] != 0) throw new Exception();

        var Address = Buffer[3].AsEnum<Address>();

        var ServerAddress = Address == Address.IPv4 ? new IPAddress(Buffer[4..8]) : new IPAddress(Buffer[4..20]);

        return Socket;
    }

    public async Task<Socket> Connect(IPEndPoint IPEndPoint)
    {
        if (!Initialized) await Initialize();

        var Message = CreateRequest(Command.Connect, IPEndPoint);

        await Socket.SendAsync(Message, SocketFlags.None);

        var Buffer = new byte[22];

        var Size = await Socket.ReceiveAsync(Buffer, SocketFlags.None);

        Buffer = Buffer[..Size];

        if (Buffer[0] != SocksVersion) throw new Exception();

        var Reply = Buffer[1].AsEnum<Reply>();

        if (Reply != Reply.Succeeded) throw new Exception();

        if (Buffer[2] != 0) throw new Exception();

        var Address = Buffer[3].AsEnum<Address>();

        var ServerAddress = Address == Address.IPv4 ? new IPAddress(Buffer[4..8]) : new IPAddress(Buffer[4..20]);

        return Socket;
    }


    private static byte[] CreateHello(Authentication Authentication)
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

    private static byte[] CreateAuthentication(string Username, string Password)
    {
        var DnStream = new DnStream((ushort) (3 + Username.Length + Password.Length));
        DnStream.WriteByte(SubNegotiationVersion);
        DnStream.WriteByte((byte) Username.Length);
        DnStream.WriteString(Username);
        DnStream.WriteByte((byte) Password.Length);
        DnStream.WriteString(Password);
        return DnStream.ToArray();
    }

    private static byte[] CreateRequest(Command Command, IPEndPoint IPEndPoint)
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

    private static byte[] CreateRequest(Command Command, string Domain, int Port)
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
}