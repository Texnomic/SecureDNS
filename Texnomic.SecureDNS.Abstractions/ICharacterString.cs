namespace Texnomic.SecureDNS.Abstractions
{
    public interface ICharacterString
    {
        byte Length { get; set; }

        string Value { get; set; }
    }
}
