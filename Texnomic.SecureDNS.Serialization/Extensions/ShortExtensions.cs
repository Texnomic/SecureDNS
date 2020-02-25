using System;

namespace Texnomic.SecureDNS.Serialization.Extensions
{
    public static class ShortExtensions
    {
        public static TEnum AsEnum<TEnum>(this ushort UShort) where TEnum : Enum
        {
            var EnumType = typeof(TEnum);

            if (!EnumType.IsEnum) throw new ArgumentException("TEnum Must Be Enumerated Type.");

            return (TEnum)(object)UShort;
        }

        public static TEnum AsEnum<TEnum>(this short Short) where TEnum : Enum
        {
            var EnumType = typeof(TEnum);

            if (!EnumType.IsEnum) throw new ArgumentException("TEnum Must Be Enumerated Type.");

            return (TEnum)(object)Short;
        }
    }

}
