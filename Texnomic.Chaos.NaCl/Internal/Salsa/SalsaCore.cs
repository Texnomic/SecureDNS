using System;

namespace Texnomic.Chaos.NaCl.Internal.Salsa
{
    internal static class SalsaCore
    {
        public static void HSalsa(out Array16<UInt32> Output, ref Array16<UInt32> Input, int Rounds)
        {
            InternalAssert.Assert(Rounds % 2 == 0, "Number of salsa rounds must be even");

            var doubleRounds = Rounds / 2;

            var x0 = Input.x0;
            var x1 = Input.x1;
            var x2 = Input.x2;
            var x3 = Input.x3;
            var x4 = Input.x4;
            var x5 = Input.x5;
            var x6 = Input.x6;
            var x7 = Input.x7;
            var x8 = Input.x8;
            var x9 = Input.x9;
            var x10 = Input.x10;
            var x11 = Input.x11;
            var x12 = Input.x12;
            var x13 = Input.x13;
            var x14 = Input.x14;
            var x15 = Input.x15;

            unchecked
            {
                for (var i = 0; i < doubleRounds; i++)
                {
                    UInt32 y;

                    // row 0
                    y = x0 + x12;
                    x4 ^= (y << 7) | (y >> (32 - 7));
                    y = x4 + x0;
                    x8 ^= (y << 9) | (y >> (32 - 9));
                    y = x8 + x4;
                    x12 ^= (y << 13) | (y >> (32 - 13));
                    y = x12 + x8;
                    x0 ^= (y << 18) | (y >> (32 - 18));

                    // row 1
                    y = x5 + x1;
                    x9 ^= (y << 7) | (y >> (32 - 7));
                    y = x9 + x5;
                    x13 ^= (y << 9) | (y >> (32 - 9));
                    y = x13 + x9;
                    x1 ^= (y << 13) | (y >> (32 - 13));
                    y = x1 + x13;
                    x5 ^= (y << 18) | (y >> (32 - 18));

                    // row 2
                    y = x10 + x6;
                    x14 ^= (y << 7) | (y >> (32 - 7));
                    y = x14 + x10;
                    x2 ^= (y << 9) | (y >> (32 - 9));
                    y = x2 + x14;
                    x6 ^= (y << 13) | (y >> (32 - 13));
                    y = x6 + x2;
                    x10 ^= (y << 18) | (y >> (32 - 18));

                    // row 3
                    y = x15 + x11;
                    x3 ^= (y << 7) | (y >> (32 - 7));
                    y = x3 + x15;
                    x7 ^= (y << 9) | (y >> (32 - 9));
                    y = x7 + x3;
                    x11 ^= (y << 13) | (y >> (32 - 13));
                    y = x11 + x7;
                    x15 ^= (y << 18) | (y >> (32 - 18));

                    // column 0
                    y = x0 + x3;
                    x1 ^= (y << 7) | (y >> (32 - 7));
                    y = x1 + x0;
                    x2 ^= (y << 9) | (y >> (32 - 9));
                    y = x2 + x1;
                    x3 ^= (y << 13) | (y >> (32 - 13));
                    y = x3 + x2;
                    x0 ^= (y << 18) | (y >> (32 - 18));

                    // column 1
                    y = x5 + x4;
                    x6 ^= (y << 7) | (y >> (32 - 7));
                    y = x6 + x5;
                    x7 ^= (y << 9) | (y >> (32 - 9));
                    y = x7 + x6;
                    x4 ^= (y << 13) | (y >> (32 - 13));
                    y = x4 + x7;
                    x5 ^= (y << 18) | (y >> (32 - 18));

                    // column 2
                    y = x10 + x9;
                    x11 ^= (y << 7) | (y >> (32 - 7));
                    y = x11 + x10;
                    x8 ^= (y << 9) | (y >> (32 - 9));
                    y = x8 + x11;
                    x9 ^= (y << 13) | (y >> (32 - 13));
                    y = x9 + x8;
                    x10 ^= (y << 18) | (y >> (32 - 18));

                    // column 3
                    y = x15 + x14;
                    x12 ^= (y << 7) | (y >> (32 - 7));
                    y = x12 + x15;
                    x13 ^= (y << 9) | (y >> (32 - 9));
                    y = x13 + x12;
                    x14 ^= (y << 13) | (y >> (32 - 13));
                    y = x14 + x13;
                    x15 ^= (y << 18) | (y >> (32 - 18));
                }
            }

            Output.x0 = x0;
            Output.x1 = x1;
            Output.x2 = x2;
            Output.x3 = x3;
            Output.x4 = x4;
            Output.x5 = x5;
            Output.x6 = x6;
            Output.x7 = x7;
            Output.x8 = x8;
            Output.x9 = x9;
            Output.x10 = x10;
            Output.x11 = x11;
            Output.x12 = x12;
            Output.x13 = x13;
            Output.x14 = x14;
            Output.x15 = x15;
        }

        public static void Salsa(out Array16<UInt32> Output, ref Array16<UInt32> Input, int Rounds)
        {
            Array16<UInt32> temp;
            HSalsa(out temp, ref Input, Rounds);
            unchecked
            {
                Output.x0 = temp.x0 + Input.x0;
                Output.x1 = temp.x1 + Input.x1;
                Output.x2 = temp.x2 + Input.x2;
                Output.x3 = temp.x3 + Input.x3;
                Output.x4 = temp.x4 + Input.x4;
                Output.x5 = temp.x5 + Input.x5;
                Output.x6 = temp.x6 + Input.x6;
                Output.x7 = temp.x7 + Input.x7;
                Output.x8 = temp.x8 + Input.x8;
                Output.x9 = temp.x9 + Input.x9;
                Output.x10 = temp.x10 + Input.x10;
                Output.x11 = temp.x11 + Input.x11;
                Output.x12 = temp.x12 + Input.x12;
                Output.x13 = temp.x13 + Input.x13;
                Output.x14 = temp.x14 + Input.x14;
                Output.x15 = temp.x15 + Input.x15;
            }
        }

        /*public static void SalsaCore(int[] output, int outputOffset, int[] input, int inputOffset, int rounds)
        {
            if (rounds % 2 != 0)
                throw new ArgumentException("rounds must be even");
        }


static void store_littleendian(unsigned char *x,uint32 u)
{
  x[0] = u; u >>= 8;
  x[1] = u; u >>= 8;
  x[2] = u; u >>= 8;
  x[3] = u;
}

        public static void HSalsaCore(int[] output, int outputOffset, int[] input, int inputOffset, int rounds)
        {
            if (rounds % 2 != 0)
                throw new ArgumentException("rounds must be even");
            static uint32 rotate(uint32 u,int c)
{
  return (u << c) | (u >> (32 - c));
}



int crypto_core(
        unsigned char *out,
  const unsigned char *in,
  const unsigned char *k,
  const unsigned char *c
)
{
  uint32 x0, x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15;
  int i;

  x0 = load_littleendian(c + 0);
  x1 = load_littleendian(k + 0);
  x2 = load_littleendian(k + 4);
  x3 = load_littleendian(k + 8);
  x4 = load_littleendian(k + 12);
  x5 = load_littleendian(c + 4);
  x6 = load_littleendian(in + 0);
  x7 = load_littleendian(in + 4);
  x8 = load_littleendian(in + 8);
  x9 = load_littleendian(in + 12);
  x10 = load_littleendian(c + 8);
  x11 = load_littleendian(k + 16);
  x12 = load_littleendian(k + 20);
  x13 = load_littleendian(k + 24);
  x14 = load_littleendian(k + 28);
  x15 = load_littleendian(c + 12);

  for (i = ROUNDS;i > 0;i -= 2) {
     x4 ^= rotate( x0+x12, 7);
     x8 ^= rotate( x4+ x0, 9);
    x12 ^= rotate( x8+ x4,13);
     x0 ^= rotate(x12+ x8,18);
     x9 ^= rotate( x5+ x1, 7);
    x13 ^= rotate( x9+ x5, 9);
     x1 ^= rotate(x13+ x9,13);
     x5 ^= rotate( x1+x13,18);
    x14 ^= rotate(x10+ x6, 7);
     x2 ^= rotate(x14+x10, 9);
     x6 ^= rotate( x2+x14,13);
    x10 ^= rotate( x6+ x2,18);
     x3 ^= rotate(x15+x11, 7);
     x7 ^= rotate( x3+x15, 9);
    x11 ^= rotate( x7+ x3,13);
    x15 ^= rotate(x11+ x7,18);
     x1 ^= rotate( x0+ x3, 7);
     x2 ^= rotate( x1+ x0, 9);
     x3 ^= rotate( x2+ x1,13);
     x0 ^= rotate( x3+ x2,18);
     x6 ^= rotate( x5+ x4, 7);
     x7 ^= rotate( x6+ x5, 9);
     x4 ^= rotate( x7+ x6,13);
     x5 ^= rotate( x4+ x7,18);
    x11 ^= rotate(x10+ x9, 7);
     x8 ^= rotate(x11+x10, 9);
     x9 ^= rotate( x8+x11,13);
    x10 ^= rotate( x9+ x8,18);
    x12 ^= rotate(x15+x14, 7);
    x13 ^= rotate(x12+x15, 9);
    x14 ^= rotate(x13+x12,13);
    x15 ^= rotate(x14+x13,18);
  }

  store_littleendian(out + 0,x0);
  store_littleendian(out + 4,x5);
  store_littleendian(out + 8,x10);
  store_littleendian(out + 12,x15);
  store_littleendian(out + 16,x6);
  store_littleendian(out + 20,x7);
  store_littleendian(out + 24,x8);
  store_littleendian(out + 28,x9);

  return 0;
}*/

    }
}
