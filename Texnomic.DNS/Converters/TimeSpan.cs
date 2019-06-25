using BinarySerialization;
using System;
using System.IO;

namespace Texnomic.DNS.Converters
{
    public class TimeSpanConverter : IValueConverter, IBinarySerializable
    {
        public object Convert(object value, object parameter, BinarySerializationContext context)
        {
            return new TimeSpan(0, 0, (int)value);
        }

        public object ConvertBack(object value, object parameter, BinarySerializationContext context)
        {
            return (uint)((TimeSpan)value).TotalSeconds;
        }

        public void Deserialize(Stream stream, Endianness endianness, BinarySerializationContext serializationContext)
        {
            throw new NotImplementedException();
        }

        public void Serialize(Stream stream, Endianness endianness, BinarySerializationContext serializationContext)
        {
            throw new NotImplementedException();
        }
    }
}
