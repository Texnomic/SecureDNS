using System.Linq;

namespace Texnomic.SecureDNS.Extensions
{
    public static class ArrayExtensions
    {
        public static T[] Concat<T>(params T[][] Arrays)
        {
            var Result = new T[Arrays.Sum(A => A.Length)];

            var Offset = 0;

            foreach (var Array in Arrays)
            {
                Array.CopyTo(Result, Offset);

                Offset += Array.Length;
            }

            return Result;
        }
    }
}
