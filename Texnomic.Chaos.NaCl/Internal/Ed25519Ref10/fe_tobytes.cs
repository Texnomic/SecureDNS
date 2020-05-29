using System;

namespace Texnomic.Chaos.NaCl.Internal.Ed25519Ref10
{
    internal static partial class FieldOperations
    {
        /*
        Preconditions:
          |h| bounded by 1.1*2^26,1.1*2^25,1.1*2^26,1.1*2^25,etc.

        Write p=2^255-19; q=floor(h/p).
        Basic claim: q = floor(2^(-255)(h + 19 2^(-25)h9 + 2^(-1))).

        Proof:
          Have |h|<=p so |q|<=1 so |19^2 2^(-255) q|<1/4.
          Also have |h-2^230 h9|<2^231 so |19 2^(-255)(h-2^230 h9)|<1/4.

          Write y=2^(-1)-19^2 2^(-255)q-19 2^(-255)(h-2^230 h9).
          Then 0<y<1.

          Write r=h-pq.
          Have 0<=r<=p-1=2^255-20.
          Thus 0<=r+19(2^-255)r<r+19(2^-255)2^255<=2^255-1.

          Write x=r+19(2^-255)r+y.
          Then 0<x<2^255 so floor(2^(-255)x) = 0 so floor(q+2^(-255)x) = q.

          Have q+2^(-255)x = 2^(-255)(h + 19 2^(-25) h9 + 2^(-1))
          so floor(2^(-255)(h + 19 2^(-25) h9 + 2^(-1))) = q.
        */
        internal static void fe_tobytes(byte[] S, int Offset, ref FieldElement H)
        {
            FieldElement hr;
            fe_reduce(out hr, ref H);

            var h0 = hr.x0;
            var h1 = hr.x1;
            var h2 = hr.x2;
            var h3 = hr.x3;
            var h4 = hr.x4;
            var h5 = hr.x5;
            var h6 = hr.x6;
            var h7 = hr.x7;
            var h8 = hr.x8;
            var h9 = hr.x9;

            /*
            Goal: Output h0+...+2^255 h10-2^255 q, which is between 0 and 2^255-20.
            Have h0+...+2^230 h9 between 0 and 2^255-1;
            evidently 2^255 h10-2^255 q = 0.
            Goal: Output h0+...+2^230 h9.
            */
            unchecked
            {
                S[Offset + 0] = (byte) (h0 >> 0);
                S[Offset + 1] = (byte) (h0 >> 8);
                S[Offset + 2] = (byte) (h0 >> 16);
                S[Offset + 3] = (byte) ((h0 >> 24) | (h1 << 2));
                S[Offset + 4] = (byte) (h1 >> 6);
                S[Offset + 5] = (byte) (h1 >> 14);
                S[Offset + 6] = (byte) ((h1 >> 22) | (h2 << 3));
                S[Offset + 7] = (byte) (h2 >> 5);
                S[Offset + 8] = (byte) (h2 >> 13);
                S[Offset + 9] = (byte) ((h2 >> 21) | (h3 << 5));
                S[Offset + 10] = (byte) (h3 >> 3);
                S[Offset + 11] = (byte) (h3 >> 11);
                S[Offset + 12] = (byte) ((h3 >> 19) | (h4 << 6));
                S[Offset + 13] = (byte) (h4 >> 2);
                S[Offset + 14] = (byte) (h4 >> 10);
                S[Offset + 15] = (byte) (h4 >> 18);
                S[Offset + 16] = (byte) (h5 >> 0);
                S[Offset + 17] = (byte) (h5 >> 8);
                S[Offset + 18] = (byte) (h5 >> 16);
                S[Offset + 19] = (byte) ((h5 >> 24) | (h6 << 1));
                S[Offset + 20] = (byte) (h6 >> 7);
                S[Offset + 21] = (byte) (h6 >> 15);
                S[Offset + 22] = (byte) ((h6 >> 23) | (h7 << 3));
                S[Offset + 23] = (byte) (h7 >> 5);
                S[Offset + 24] = (byte) (h7 >> 13);
                S[Offset + 25] = (byte) ((h7 >> 21) | (h8 << 4));
                S[Offset + 26] = (byte) (h8 >> 4);
                S[Offset + 27] = (byte) (h8 >> 12);
                S[Offset + 28] = (byte) ((h8 >> 20) | (h9 << 6));
                S[Offset + 29] = (byte) (h9 >> 2);
                S[Offset + 30] = (byte) (h9 >> 10);
                S[Offset + 31] = (byte) (h9 >> 18);
            }
        }

        internal static void fe_reduce(out FieldElement Hr, ref FieldElement H)
        {
            var h0 = H.x0;
            var h1 = H.x1;
            var h2 = H.x2;
            var h3 = H.x3;
            var h4 = H.x4;
            var h5 = H.x5;
            var h6 = H.x6;
            var h7 = H.x7;
            var h8 = H.x8;
            var h9 = H.x9;
            Int32 q;
            Int32 carry0;
            Int32 carry1;
            Int32 carry2;
            Int32 carry3;
            Int32 carry4;
            Int32 carry5;
            Int32 carry6;
            Int32 carry7;
            Int32 carry8;
            Int32 carry9;

            q = (19 * h9 + (((Int32)1) << 24)) >> 25;
            q = (h0 + q) >> 26;
            q = (h1 + q) >> 25;
            q = (h2 + q) >> 26;
            q = (h3 + q) >> 25;
            q = (h4 + q) >> 26;
            q = (h5 + q) >> 25;
            q = (h6 + q) >> 26;
            q = (h7 + q) >> 25;
            q = (h8 + q) >> 26;
            q = (h9 + q) >> 25;

            /* Goal: Output h-(2^255-19)q, which is between 0 and 2^255-20. */
            h0 += 19 * q;
            /* Goal: Output h-2^255 q, which is between 0 and 2^255-20. */

            carry0 = h0 >> 26; h1 += carry0; h0 -= carry0 << 26;
            carry1 = h1 >> 25; h2 += carry1; h1 -= carry1 << 25;
            carry2 = h2 >> 26; h3 += carry2; h2 -= carry2 << 26;
            carry3 = h3 >> 25; h4 += carry3; h3 -= carry3 << 25;
            carry4 = h4 >> 26; h5 += carry4; h4 -= carry4 << 26;
            carry5 = h5 >> 25; h6 += carry5; h5 -= carry5 << 25;
            carry6 = h6 >> 26; h7 += carry6; h6 -= carry6 << 26;
            carry7 = h7 >> 25; h8 += carry7; h7 -= carry7 << 25;
            carry8 = h8 >> 26; h9 += carry8; h8 -= carry8 << 26;
            carry9 = h9 >> 25; h9 -= carry9 << 25;
            /* h10 = carry9 */

            Hr.x0 = h0;
            Hr.x1 = h1;
            Hr.x2 = h2;
            Hr.x3 = h3;
            Hr.x4 = h4;
            Hr.x5 = h5;
            Hr.x6 = h6;
            Hr.x7 = h7;
            Hr.x8 = h8;
            Hr.x9 = h9;
        }
    }
}