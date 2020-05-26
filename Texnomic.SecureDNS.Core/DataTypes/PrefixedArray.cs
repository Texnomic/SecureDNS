namespace Texnomic.SecureDNS.Core.DataTypes
{
    /// <summary>
    /// <see cref="https://github.com/DNSCrypt/dnscrypt-proxy/wiki/stamps#common-definitions"/>
    /// </summary>
    public class PrefixedArray<T>
    {
        public byte Count { get; set; }

        public T[] Value { get; set; }

        public T this[int Index]
        {
            get => Value[Index];
            set => Value[Index] = value;
        }
    }
}
