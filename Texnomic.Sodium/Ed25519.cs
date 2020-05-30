namespace Texnomic.Sodium
{
    public static class Ed25519
    {
        public static bool Verify(byte[] Signature, byte[] Message, byte[] Key)
        {
            return SodiumLibrary.crypto_sign_verify_detached(Signature, Message, Message.Length, Key) == 0;
        }
    }
}
