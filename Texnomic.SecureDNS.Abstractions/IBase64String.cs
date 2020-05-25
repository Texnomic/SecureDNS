namespace Texnomic.SecureDNS.Abstractions
{
    public interface IBase64String
    {
        byte[] Bytes { get; set; }

        string Base64 { get; set; }
    }
}
