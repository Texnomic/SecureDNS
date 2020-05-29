using System;

namespace Texnomic.Chaos.NaCl.Internal
{
    internal static class InternalAssert
    {
        public static void Assert(bool Condition, string Message)
        {
            if (!Condition)
                throw new InvalidOperationException("An assertion in Chaos.Crypto failed " + Message);
        }
    }
}
