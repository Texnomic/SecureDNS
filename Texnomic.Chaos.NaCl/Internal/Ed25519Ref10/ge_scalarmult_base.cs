using System;

namespace Texnomic.Chaos.NaCl.Internal.Ed25519Ref10
{
    internal static partial class GroupOperations
    {
        static byte Equal(byte B, byte C)
        {

            var ub = B;
            var uc = C;
            var x = (byte)(ub ^ uc); /* 0: yes; 1..255: no */
            UInt32 y = x; /* 0: yes; 1..255: no */
            unchecked { y -= 1; } /* 4294967295: yes; 0..254: no */
            y >>= 31; /* 1: yes; 0: no */
            return (byte)y;
        }

        static byte Negative(sbyte B)
        {
            var x = unchecked((ulong)(long)B); /* 18446744073709551361..18446744073709551615: yes; 0..255: no */
            x >>= 63; /* 1: yes; 0: no */
            return (byte)x;
        }

        static void Cmov(ref GroupElementPreComp T, ref GroupElementPreComp U, byte B)
        {
            FieldOperations.fe_cmov(ref T.Yplusx, ref U.Yplusx, B);
            FieldOperations.fe_cmov(ref T.Yminusx, ref U.Yminusx, B);
            FieldOperations.fe_cmov(ref T.Xy2d, ref U.Xy2d, B);
        }

        static void Select(out GroupElementPreComp T, int Pos, sbyte B)
        {
            GroupElementPreComp minust;
            var bnegative = Negative(B);
            var babs = (byte)(B - (((-bnegative) & B) << 1));

            ge_precomp_0(out T);
            var table = LookupTables.Base[Pos];
            Cmov(ref T, ref table[0], Equal(babs, 1));
            Cmov(ref T, ref table[1], Equal(babs, 2));
            Cmov(ref T, ref table[2], Equal(babs, 3));
            Cmov(ref T, ref table[3], Equal(babs, 4));
            Cmov(ref T, ref table[4], Equal(babs, 5));
            Cmov(ref T, ref table[5], Equal(babs, 6));
            Cmov(ref T, ref table[6], Equal(babs, 7));
            Cmov(ref T, ref table[7], Equal(babs, 8));
            minust.Yplusx = T.Yminusx;
            minust.Yminusx = T.Yplusx;
            FieldOperations.fe_neg(out minust.Xy2d, ref T.Xy2d);
            Cmov(ref T, ref minust, bnegative);
        }

        /*
        h = a * B
        where a = a[0]+256*a[1]+...+256^31 a[31]
        B is the Ed25519 base point (x,4/5) with x positive.

        Preconditions:
          a[31] <= 127
        */

        public static void ge_scalarmult_base(out GroupElementP3 H, byte[] A, int Offset)
        {
            // todo: Perhaps remove this allocation
            var e = new sbyte[64];
            sbyte carry;
            GroupElementP1P1 r;
            GroupElementP2 s;
            GroupElementPreComp t;
            int i;

            for (i = 0; i < 32; ++i)
            {
                e[2 * i + 0] = (sbyte)((A[Offset + i] >> 0) & 15);
                e[2 * i + 1] = (sbyte)((A[Offset + i] >> 4) & 15);
            }
            /* each e[i] is between 0 and 15 */
            /* e[63] is between 0 and 7 */

            carry = 0;
            for (i = 0; i < 63; ++i)
            {
                e[i] += carry;
                carry = (sbyte)(e[i] + 8);
                carry >>= 4;
                e[i] -= (sbyte)(carry << 4);
            }
            e[63] += carry;
            /* each e[i] is between -8 and 8 */

            ge_p3_0(out H);
            for (i = 1; i < 64; i += 2)
            {
                Select(out t, i / 2, e[i]);
                ge_madd(out r, ref H, ref t); ge_p1p1_to_p3(out H, ref r);
            }

            ge_p3_dbl(out r, ref H); ge_p1p1_to_p2(out s, ref r);
            ge_p2_dbl(out r, ref s); ge_p1p1_to_p2(out s, ref r);
            ge_p2_dbl(out r, ref s); ge_p1p1_to_p2(out s, ref r);
            ge_p2_dbl(out r, ref s); ge_p1p1_to_p3(out H, ref r);

            for (i = 0; i < 64; i += 2)
            {
                Select(out t, i / 2, e[i]);
                ge_madd(out r, ref H, ref t); ge_p1p1_to_p3(out H, ref r);
            }
        }

    }
}