using System;

namespace Texnomic.Chaos.NaCl
{
    public abstract class OneTimeAuth
    {
        private static readonly Poly1305 _Poly1305 = new Poly1305();

        public abstract int KeySizeInBytes { get; }
        public abstract int SignatureSizeInBytes { get; }

        public abstract byte[] Sign(byte[] Message, byte[] Key);
        public abstract void Sign(ArraySegment<byte> Signature, ArraySegment<byte> Message, ArraySegment<byte> Key);
        public abstract bool Verify(byte[] Signature, byte[] Message, byte[] Key);
        public abstract bool Verify(ArraySegment<byte> Signature, ArraySegment<byte> Message, ArraySegment<byte> Key);

        public static OneTimeAuth Poly1305 => _Poly1305;
    }
}
