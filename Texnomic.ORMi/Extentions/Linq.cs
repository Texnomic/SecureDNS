using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace System.Linq
{
    public static class LinqExtentions
    {
        public static IEnumerable<TOutput> Convert<T, TOutput>(this IEnumerable<T> Sequence, Converter<T, TOutput> Converter)
        {
            foreach (var Item in Sequence)
            {
                yield return Converter(Item);
            }
        }

        public static IEnumerable<T> Do<T>(this IEnumerable<T> Sequence, Action<T> Action)
        {
            foreach (var Item in Sequence)
            {
                Action(Item);
                yield return Item;
            }
        }
    }
}
