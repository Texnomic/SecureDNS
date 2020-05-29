using System;

namespace Texnomic.Chaos.NaCl.Internal.Ed25519Ref10
{
    internal static partial class ScalarOperations
    {
        /*
        Input:
          s[0]+256*s[1]+...+256^63*s[63] = s

        Output:
          s[0]+256*s[1]+...+256^31*s[31] = s mod l
          where l = 2^252 + 27742317777372353535851937790883648493.
          Overwrites s in place.
        */

        public static void sc_reduce(byte[] S)
        {
            var s0 = 2097151 & load_3(S, 0);
            var s1 = 2097151 & (load_4(S, 2) >> 5);
            var s2 = 2097151 & (load_3(S, 5) >> 2);
            var s3 = 2097151 & (load_4(S, 7) >> 7);
            var s4 = 2097151 & (load_4(S, 10) >> 4);
            var s5 = 2097151 & (load_3(S, 13) >> 1);
            var s6 = 2097151 & (load_4(S, 15) >> 6);
            var s7 = 2097151 & (load_3(S, 18) >> 3);
            var s8 = 2097151 & load_3(S, 21);
            var s9 = 2097151 & (load_4(S, 23) >> 5);
            var s10 = 2097151 & (load_3(S, 26) >> 2);
            var s11 = 2097151 & (load_4(S, 28) >> 7);
            var s12 = 2097151 & (load_4(S, 31) >> 4);
            var s13 = 2097151 & (load_3(S, 34) >> 1);
            var s14 = 2097151 & (load_4(S, 36) >> 6);
            var s15 = 2097151 & (load_3(S, 39) >> 3);
            var s16 = 2097151 & load_3(S, 42);
            var s17 = 2097151 & (load_4(S, 44) >> 5);
            var s18 = 2097151 & (load_3(S, 47) >> 2);
            var s19 = 2097151 & (load_4(S, 49) >> 7);
            var s20 = 2097151 & (load_4(S, 52) >> 4);
            var s21 = 2097151 & (load_3(S, 55) >> 1);
            var s22 = 2097151 & (load_4(S, 57) >> 6);
            var s23 = (load_4(S, 60) >> 3);
            Int64 carry0;
            Int64 carry1;
            Int64 carry2;
            Int64 carry3;
            Int64 carry4;
            Int64 carry5;
            Int64 carry6;
            Int64 carry7;
            Int64 carry8;
            Int64 carry9;
            Int64 carry10;
            Int64 carry11;
            Int64 carry12;
            Int64 carry13;
            Int64 carry14;
            Int64 carry15;
            Int64 carry16;

            s11 += s23 * 666643;
            s12 += s23 * 470296;
            s13 += s23 * 654183;
            s14 -= s23 * 997805;
            s15 += s23 * 136657;
            s16 -= s23 * 683901;
            s23 = 0;

            s10 += s22 * 666643;
            s11 += s22 * 470296;
            s12 += s22 * 654183;
            s13 -= s22 * 997805;
            s14 += s22 * 136657;
            s15 -= s22 * 683901;
            s22 = 0;

            s9 += s21 * 666643;
            s10 += s21 * 470296;
            s11 += s21 * 654183;
            s12 -= s21 * 997805;
            s13 += s21 * 136657;
            s14 -= s21 * 683901;
            s21 = 0;

            s8 += s20 * 666643;
            s9 += s20 * 470296;
            s10 += s20 * 654183;
            s11 -= s20 * 997805;
            s12 += s20 * 136657;
            s13 -= s20 * 683901;
            s20 = 0;

            s7 += s19 * 666643;
            s8 += s19 * 470296;
            s9 += s19 * 654183;
            s10 -= s19 * 997805;
            s11 += s19 * 136657;
            s12 -= s19 * 683901;
            s19 = 0;

            s6 += s18 * 666643;
            s7 += s18 * 470296;
            s8 += s18 * 654183;
            s9 -= s18 * 997805;
            s10 += s18 * 136657;
            s11 -= s18 * 683901;
            s18 = 0;

            carry6 = (s6 + (1 << 20)) >> 21; s7 += carry6; s6 -= carry6 << 21;
            carry8 = (s8 + (1 << 20)) >> 21; s9 += carry8; s8 -= carry8 << 21;
            carry10 = (s10 + (1 << 20)) >> 21; s11 += carry10; s10 -= carry10 << 21;
            carry12 = (s12 + (1 << 20)) >> 21; s13 += carry12; s12 -= carry12 << 21;
            carry14 = (s14 + (1 << 20)) >> 21; s15 += carry14; s14 -= carry14 << 21;
            carry16 = (s16 + (1 << 20)) >> 21; s17 += carry16; s16 -= carry16 << 21;

            carry7 = (s7 + (1 << 20)) >> 21; s8 += carry7; s7 -= carry7 << 21;
            carry9 = (s9 + (1 << 20)) >> 21; s10 += carry9; s9 -= carry9 << 21;
            carry11 = (s11 + (1 << 20)) >> 21; s12 += carry11; s11 -= carry11 << 21;
            carry13 = (s13 + (1 << 20)) >> 21; s14 += carry13; s13 -= carry13 << 21;
            carry15 = (s15 + (1 << 20)) >> 21; s16 += carry15; s15 -= carry15 << 21;

            s5 += s17 * 666643;
            s6 += s17 * 470296;
            s7 += s17 * 654183;
            s8 -= s17 * 997805;
            s9 += s17 * 136657;
            s10 -= s17 * 683901;
            s17 = 0;

            s4 += s16 * 666643;
            s5 += s16 * 470296;
            s6 += s16 * 654183;
            s7 -= s16 * 997805;
            s8 += s16 * 136657;
            s9 -= s16 * 683901;
            s16 = 0;

            s3 += s15 * 666643;
            s4 += s15 * 470296;
            s5 += s15 * 654183;
            s6 -= s15 * 997805;
            s7 += s15 * 136657;
            s8 -= s15 * 683901;
            s15 = 0;

            s2 += s14 * 666643;
            s3 += s14 * 470296;
            s4 += s14 * 654183;
            s5 -= s14 * 997805;
            s6 += s14 * 136657;
            s7 -= s14 * 683901;
            s14 = 0;

            s1 += s13 * 666643;
            s2 += s13 * 470296;
            s3 += s13 * 654183;
            s4 -= s13 * 997805;
            s5 += s13 * 136657;
            s6 -= s13 * 683901;
            s13 = 0;

            s0 += s12 * 666643;
            s1 += s12 * 470296;
            s2 += s12 * 654183;
            s3 -= s12 * 997805;
            s4 += s12 * 136657;
            s5 -= s12 * 683901;
            s12 = 0;

            carry0 = (s0 + (1 << 20)) >> 21; s1 += carry0; s0 -= carry0 << 21;
            carry2 = (s2 + (1 << 20)) >> 21; s3 += carry2; s2 -= carry2 << 21;
            carry4 = (s4 + (1 << 20)) >> 21; s5 += carry4; s4 -= carry4 << 21;
            carry6 = (s6 + (1 << 20)) >> 21; s7 += carry6; s6 -= carry6 << 21;
            carry8 = (s8 + (1 << 20)) >> 21; s9 += carry8; s8 -= carry8 << 21;
            carry10 = (s10 + (1 << 20)) >> 21; s11 += carry10; s10 -= carry10 << 21;

            carry1 = (s1 + (1 << 20)) >> 21; s2 += carry1; s1 -= carry1 << 21;
            carry3 = (s3 + (1 << 20)) >> 21; s4 += carry3; s3 -= carry3 << 21;
            carry5 = (s5 + (1 << 20)) >> 21; s6 += carry5; s5 -= carry5 << 21;
            carry7 = (s7 + (1 << 20)) >> 21; s8 += carry7; s7 -= carry7 << 21;
            carry9 = (s9 + (1 << 20)) >> 21; s10 += carry9; s9 -= carry9 << 21;
            carry11 = (s11 + (1 << 20)) >> 21; s12 += carry11; s11 -= carry11 << 21;

            s0 += s12 * 666643;
            s1 += s12 * 470296;
            s2 += s12 * 654183;
            s3 -= s12 * 997805;
            s4 += s12 * 136657;
            s5 -= s12 * 683901;
            s12 = 0;

            carry0 = s0 >> 21; s1 += carry0; s0 -= carry0 << 21;
            carry1 = s1 >> 21; s2 += carry1; s1 -= carry1 << 21;
            carry2 = s2 >> 21; s3 += carry2; s2 -= carry2 << 21;
            carry3 = s3 >> 21; s4 += carry3; s3 -= carry3 << 21;
            carry4 = s4 >> 21; s5 += carry4; s4 -= carry4 << 21;
            carry5 = s5 >> 21; s6 += carry5; s5 -= carry5 << 21;
            carry6 = s6 >> 21; s7 += carry6; s6 -= carry6 << 21;
            carry7 = s7 >> 21; s8 += carry7; s7 -= carry7 << 21;
            carry8 = s8 >> 21; s9 += carry8; s8 -= carry8 << 21;
            carry9 = s9 >> 21; s10 += carry9; s9 -= carry9 << 21;
            carry10 = s10 >> 21; s11 += carry10; s10 -= carry10 << 21;
            carry11 = s11 >> 21; s12 += carry11; s11 -= carry11 << 21;

            s0 += s12 * 666643;
            s1 += s12 * 470296;
            s2 += s12 * 654183;
            s3 -= s12 * 997805;
            s4 += s12 * 136657;
            s5 -= s12 * 683901;
            s12 = 0;

            carry0 = s0 >> 21; s1 += carry0; s0 -= carry0 << 21;
            carry1 = s1 >> 21; s2 += carry1; s1 -= carry1 << 21;
            carry2 = s2 >> 21; s3 += carry2; s2 -= carry2 << 21;
            carry3 = s3 >> 21; s4 += carry3; s3 -= carry3 << 21;
            carry4 = s4 >> 21; s5 += carry4; s4 -= carry4 << 21;
            carry5 = s5 >> 21; s6 += carry5; s5 -= carry5 << 21;
            carry6 = s6 >> 21; s7 += carry6; s6 -= carry6 << 21;
            carry7 = s7 >> 21; s8 += carry7; s7 -= carry7 << 21;
            carry8 = s8 >> 21; s9 += carry8; s8 -= carry8 << 21;
            carry9 = s9 >> 21; s10 += carry9; s9 -= carry9 << 21;
            carry10 = s10 >> 21; s11 += carry10; s10 -= carry10 << 21;

            unchecked
            {
                S[0] = (byte)(s0 >> 0);
                S[1] = (byte)(s0 >> 8);
                S[2] = (byte)((s0 >> 16) | (s1 << 5));
                S[3] = (byte)(s1 >> 3);
                S[4] = (byte)(s1 >> 11);
                S[5] = (byte)((s1 >> 19) | (s2 << 2));
                S[6] = (byte)(s2 >> 6);
                S[7] = (byte)((s2 >> 14) | (s3 << 7));
                S[8] = (byte)(s3 >> 1);
                S[9] = (byte)(s3 >> 9);
                S[10] = (byte)((s3 >> 17) | (s4 << 4));
                S[11] = (byte)(s4 >> 4);
                S[12] = (byte)(s4 >> 12);
                S[13] = (byte)((s4 >> 20) | (s5 << 1));
                S[14] = (byte)(s5 >> 7);
                S[15] = (byte)((s5 >> 15) | (s6 << 6));
                S[16] = (byte)(s6 >> 2);
                S[17] = (byte)(s6 >> 10);
                S[18] = (byte)((s6 >> 18) | (s7 << 3));
                S[19] = (byte)(s7 >> 5);
                S[20] = (byte)(s7 >> 13);
                S[21] = (byte)(s8 >> 0);
                S[22] = (byte)(s8 >> 8);
                S[23] = (byte)((s8 >> 16) | (s9 << 5));
                S[24] = (byte)(s9 >> 3);
                S[25] = (byte)(s9 >> 11);
                S[26] = (byte)((s9 >> 19) | (s10 << 2));
                S[27] = (byte)(s10 >> 6);
                S[28] = (byte)((s10 >> 14) | (s11 << 7));
                S[29] = (byte)(s11 >> 1);
                S[30] = (byte)(s11 >> 9);
                S[31] = (byte)(s11 >> 17);
            }
        }

    }
}