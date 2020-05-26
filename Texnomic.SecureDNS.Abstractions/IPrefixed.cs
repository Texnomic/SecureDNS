namespace Texnomic.SecureDNS.Abstractions
{
    public interface IPrefixed<T>
    {
        byte Length { get; set; }
        T Value { get; set; }
    }
}
