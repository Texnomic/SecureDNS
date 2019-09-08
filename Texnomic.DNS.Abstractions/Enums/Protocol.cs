namespace Texnomic.DNS.Abstractions.Enums
{
    /// <summary>
    /// Protocol Numbers <see cref="https://tools.ietf.org/html/rfc1010">RFC1010 Page 3</see>
    /// </summary>
    public enum Protocol : byte
    {
        Reserved = 0,

        /// <summary>
        /// Internet Control Message
        /// </summary>
        ICMP = 1,

        /// <summary>
        /// Internet Group Management
        /// </summary>
        IGMP = 2,

        /// <summary>
        /// Gateway-to-Gateway
        /// </summary>
        GGP = 3,

        Unassigned = 4,

        /// <summary>
        /// Stream
        /// </summary>
        ST = 5,

        /// <summary>
        /// Transmission Control
        /// </summary>
        TCP = 6,

        /// <summary>
        /// UCL
        /// </summary>
        UCL = 6,

        /// <summary>
        /// Exterior Gateway Protocol
        /// </summary>
        EGP = 8,
    }
}
