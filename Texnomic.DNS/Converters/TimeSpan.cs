using BinarySerialization;
using System;

namespace Texnomic.DNS.Converters
{
    public class TimeSpanConverter : FieldValueAttributeBase
    {
        public TimeSpanConverter(string Field) : base(Field) { }

        public object Convert(object Value, object Parameter, BinarySerializationContext Context)
        {
            return TimeSpan.FromSeconds((uint)Value);
        }

        public object ConvertBack(object value, object parameter, BinarySerializationContext context)
        {
            return (int)((TimeSpan)value).TotalSeconds;
        }

        protected override object GetFinalValue(object state)
        {
            throw new NotImplementedException();
        }

        protected override object GetInitialState(BinarySerializationContext context)
        {
            throw new NotImplementedException();
        }

        protected override object GetUpdatedState(object state, byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
