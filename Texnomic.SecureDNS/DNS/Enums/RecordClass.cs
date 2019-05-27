namespace Texnomic.DNS.Protocol
{
    public enum RecordClass
    {
        /// <summary>
        /// Internet
        /// </summary>
        IN = 1,

        /// <summary>
        /// CSNET
        /// </summary>
        CS = 2,


        /// <summary>
        /// CHAOS
        /// </summary>
        CH = 3,

        /// <summary>
        /// Hesiod
        /// </summary>
        HS = 4,

        /// <summary>
        /// Any
        /// </summary>
        ANY = 255,
    }
}
