namespace Texnomic.Sodium;

public static class Random
{
    public static byte[] Generate(int Size)
    {
        var Buffer = new byte[Size];

        SodiumLibrary.randombytes_buf(Buffer, Size);

        return Buffer;
    }
}