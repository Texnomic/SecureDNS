﻿using Random = Texnomic.Sodium.Random;

namespace Texnomic.SecureDNS.Protocols;

/// <summary>
/// <see cref="https://github.com/DNSCrypt/dnscrypt-protocol/blob/master/DNSCRYPT-V2-PROTOCOL.txt"/>
/// </summary>
public sealed class DNSCrypt : Protocol
{
    private readonly IOptionsMonitor<DNSCryptOptions> Options;

    private IPEndPoint IPEndPoint;

    private ICertificate Certificate;

    private byte[] PublicKey;

    private byte[] SecretKey;

    private byte[] SharedKey;

    public DNSCrypt(IOptionsMonitor<DNSCryptOptions> DNSCryptOptions)
    {
        Options = DNSCryptOptions;

        Options.OnChange(async (ChangedOptions) => await InitializeAsync());

        IsInitialized = false;
    }

    protected override async ValueTask InitializeAsync()
    {
        IPEndPoint = IPEndPoint.Parse(Options.CurrentValue.DNSCryptStamp.Address);

        SecretKey = Random.Generate(32);

        PublicKey = Curve25519.ScalarMultiplicationBase(SecretKey);

        var Query = new Message()
        {
            ID = BitConverter.ToUInt16(Random.Generate(2)),
            MessageType = MessageType.Query,
            Truncated = false,
            CheckingDisabled = true,
            RecursionDesired = true,
            Questions =
            [
                new Question()
                {
                    Type = RecordType.TXT,
                    Class = RecordClass.Internet,
                    Domain = Domain.FromString(Options.CurrentValue.DNSCryptStamp.ProviderName)
                }
            ]
        };

        var RawQuery = DnSerializer.Serialize(Query);

        using var Socket = new Socket(SocketType.Dgram, ProtocolType.Udp);

        Socket.ReceiveTimeout = (int)Options.CurrentValue.Timeout.TotalMilliseconds;
        Socket.SendTimeout = (int)Options.CurrentValue.Timeout.TotalMilliseconds;

        await Socket.ConnectAsync(IPEndPoint);

        await Socket.SendAsync(RawQuery, SocketFlags.None);

        var RawAnswer = new byte[1024];

        var Size = await Socket.ReceiveAsync(RawAnswer, SocketFlags.None);

        RawAnswer = RawAnswer[..Size];

        var AnswerMessage = DnSerializer.Deserialize(RawAnswer);

        var IsValid = VerifyServer(AnswerMessage);

        if (!IsValid)
            throw new CryptographicUnexpectedOperationException("Invalid Server Certificate.");

        SharedKey = PreComputeSharedKey();

        IsInitialized = true;
    }

    private bool VerifyServer(IMessage Message)
    {
        foreach (var Answer in Message.Answers)
        {
            var Record = Answer.Record as TXT;

            Certificate = Record.Certificate;

            var Bytes = DnSerializer.Serialize(Certificate);

            var Result = Ed25519.Verify(Certificate.Signature, Bytes[73..], Options.CurrentValue.DNSCryptStamp.PublicKey);

            if (Result) return true;
        }

        return false;
    }

    private static byte[] GenerateQueryPad(int QueryLength)
    {
        var Pad = Array.Empty<byte>();

        switch (QueryLength)
        {
            case < 256:
                {
                    Pad = new byte[256 - QueryLength];

                    break;
                }
            case > 256:
                {
                    var PaddingLength = 256 + 64;

                    while (PaddingLength < QueryLength)
                    {
                        PaddingLength += 64;
                    }

                    Pad = new byte[PaddingLength - QueryLength];
                    break;
                }
        }

        Pad[0] = 0x80;

        return Pad;
    }


    public override async ValueTask<byte[]> ResolveAsync(byte[] Query)
    {
        if (!IsInitialized) await InitializeAsync();

        var ClientNonce = Random.Generate(12);

        var ClientNoncePad = new byte[12];

        var PaddedClientNonce = ArrayExtensions.Concat(ClientNonce, ClientNoncePad);

        var QueryPad = GenerateQueryPad(Query.Length);

        var PaddedQuery = ArrayExtensions.Concat(Query, QueryPad);

        var EncryptedQuery = Encrypt(ref PaddedQuery, ref PaddedClientNonce);

        var QueryPacket = ArrayExtensions.Concat(Certificate.ClientMagic, PublicKey, ClientNonce, EncryptedQuery);

        using var Socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

        Socket.ReceiveTimeout = (int)Options.CurrentValue.Timeout.TotalMilliseconds;
        Socket.SendTimeout = (int)Options.CurrentValue.Timeout.TotalMilliseconds;

        await Socket.ConnectAsync(IPEndPoint);

        var Prefix = new byte[2];

        BinaryPrimitives.WriteUInt16BigEndian(Prefix, (ushort)QueryPacket.Length);

        await Socket.SendAsync(Prefix, SocketFlags.None);

        await Socket.SendAsync(QueryPacket, SocketFlags.None);

        await Socket.ReceiveAsync(Prefix, SocketFlags.None);

        var Size = BinaryPrimitives.ReadUInt16BigEndian(Prefix);

        var AnswerPacket = new byte[Size];

        await Socket.ReliableReceiveAsync(AnswerPacket);

        var ClientMagic = Encoding.ASCII.GetString(AnswerPacket[..8]);

        if (ClientMagic != "r6fnvWj8")
            throw new FormatException("Invalid DNSCrypt Client Magic Received.");

        if (!ClientNonce.SequenceEqual(AnswerPacket[8..20]))
            throw new FormatException("Invalid DNSCrypt Client Nonce Received.");

        var ServerNonce = AnswerPacket[20..32];

        var Nonce = ArrayExtensions.Concat(ClientNonce, ServerNonce);

        var EncryptedAnswer = AnswerPacket[32..];

        var DecryptedAnswer = Decrypt(ref EncryptedAnswer, ref Nonce);

        //if (DecryptedAnswer == null)
        //    throw new CryptographicUnexpectedOperationException("DNSCrypt Decryption Failed.");

        //if (DecryptedAnswer.Length <= Query.Length)
        //    throw new CryptographicUnexpectedOperationException("DNSCrypt Decryption Failed.");

        return DecryptedAnswer;
    }

    private byte[] PreComputeSharedKey()
    {
        return Certificate.Version switch
        {
            ESVersion.X25519_XSalsa20Poly1305 => NaCl.KeyExchange(Certificate.PublicKey, SecretKey),
            ESVersion.X25519_XChacha20Poly1305 => Curve25519XChaCha20Poly1305.KeyExchange(Certificate.PublicKey, SecretKey),
            _ => throw new ArgumentOutOfRangeException(nameof(ESVersion)),
        };
    }

    private byte[] Encrypt(ref byte[] PaddedQuery, ref byte[] ClientNonce)
    {
        return Certificate.Version switch
        {
            ESVersion.X25519_XSalsa20Poly1305 => Curve25519XSalsa20Poly1305.Encrypt(PaddedQuery, ClientNonce, SharedKey),
            ESVersion.X25519_XChacha20Poly1305 => Curve25519XChaCha20Poly1305.Encrypt(PaddedQuery, ClientNonce, SharedKey),
            _ => throw new ArgumentOutOfRangeException(nameof(ESVersion)),
        };
    }

    private byte[] Decrypt(ref byte[] EncryptedAnswer, ref byte[] ServerNonce)
    {
        return Certificate.Version switch
        {
            ESVersion.X25519_XSalsa20Poly1305 => Curve25519XSalsa20Poly1305.Decrypt(EncryptedAnswer, ServerNonce, SharedKey),
            ESVersion.X25519_XChacha20Poly1305 => Curve25519XChaCha20Poly1305.Decrypt(EncryptedAnswer, ServerNonce, SharedKey),
            _ => throw new ArgumentOutOfRangeException(nameof(ESVersion)),
        };
    }

    protected override void Dispose(bool Disposing)
    {
        if (IsDisposed) 
            return;

        IsDisposed = true;
    }
}