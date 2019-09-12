using BinarySerialization;
using Texnomic.DNS.Abstractions;
using Texnomic.DNS.Abstractions.Enums;

namespace Texnomic.DNS.Records
{
    //                                 1  1  1  1  1  1
    //   0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
    // +------------+--------------+------------------------------+
    // | Field Name | Field Type   | Description                  |
    // +------------+--------------+------------------------------+
    // | NAME       | domain name  | MUST be 0 (root domain)      |
    // | TYPE       | u_int16_t    | OPT (41)                     |
    // | CLASS      | u_int16_t    | requestor's UDP payload size |
    // | TTL        | u_int32_t    | extended RCODE and flags     |
    // | RDLEN      | u_int16_t    | length of all RDATA          |
    // | RDATA      | octet stream | {attribute,value} pairs      |
    // +------------+--------------+------------------------------+
    //
    // +0 (MSB)                            +1 (LSB)
    // +---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+
    // 0: |                          OPTION-CODE                          |
    // +---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+
    // 2: |                         OPTION-LENGTH                         |
    // +---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+
    // 4: |                                                               |
    //    /                          OPTION-DATA                          /
    //    /                                                               /
    //    +---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+

    /// <summary>
    /// Pseudo or Meta Resource Record <see href="https://tools.ietf.org/html/rfc6891#section-6.1.2">(OPT)</see>
    /// </summary>
    public class OPT : IRecord
    {
        [FieldOrder(0)]
        public OptionCode OptionCode { get; set; }

        [FieldOrder(1)] 
        public ushort Length { get; set; }

        [FieldOrder(2)]
        [FieldCount(nameof(Length))]
        public byte[] Data { get; set; }
    }
}
