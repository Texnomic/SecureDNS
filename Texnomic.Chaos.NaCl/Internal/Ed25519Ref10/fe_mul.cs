using System;

namespace Texnomic.Chaos.NaCl.Internal.Ed25519Ref10
{
	internal static partial class FieldOperations
	{
		/*
		h = f * g
		Can overlap h with f or g.

		Preconditions:
		   |f| bounded by 1.65*2^26,1.65*2^25,1.65*2^26,1.65*2^25,etc.
		   |g| bounded by 1.65*2^26,1.65*2^25,1.65*2^26,1.65*2^25,etc.

		Postconditions:
		   |h| bounded by 1.01*2^25,1.01*2^24,1.01*2^25,1.01*2^24,etc.
		*/

		/*
		Notes on implementation strategy:

		Using schoolbook multiplication.
		Karatsuba would save a little in some cost models.

		Most multiplications by 2 and 19 are 32-bit precomputations;
		cheaper than 64-bit postcomputations.

		There is one remaining multiplication by 19 in the carry chain;
		one *19 precomputation can be merged into this,
		but the resulting data flow is considerably less clean.

		There are 12 carries below.
		10 of them are 2-way parallelizable and vectorizable.
		Can get away with 11 carries, but then data flow is much deeper.

		With tighter constraints on inputs can squeeze carries into int32.
		*/

		internal static void fe_mul(out FieldElement H, ref FieldElement F, ref FieldElement G)
		{
			var f0 = F.x0;
			var f1 = F.x1;
			var f2 = F.x2;
			var f3 = F.x3;
			var f4 = F.x4;
			var f5 = F.x5;
			var f6 = F.x6;
			var f7 = F.x7;
			var f8 = F.x8;
			var f9 = F.x9;
			var g0 = G.x0;
			var g1 = G.x1;
			var g2 = G.x2;
			var g3 = G.x3;
			var g4 = G.x4;
			var g5 = G.x5;
			var g6 = G.x6;
			var g7 = G.x7;
			var g8 = G.x8;
			var g9 = G.x9;
			var G119 = 19 * g1; /* 1.959375*2^29 */
			var G219 = 19 * g2; /* 1.959375*2^30; still ok */
			var G319 = 19 * g3;
			var G419 = 19 * g4;
			var G519 = 19 * g5;
			var G619 = 19 * g6;
			var G719 = 19 * g7;
			var G819 = 19 * g8;
			var G919 = 19 * g9;
			var F12 = 2 * f1;
			var F32 = 2 * f3;
			var F52 = 2 * f5;
			var F72 = 2 * f7;
			var F92 = 2 * f9;
			var F0G0 = f0 * (Int64)g0;
			var F0G1 = f0 * (Int64)g1;
			var F0G2 = f0 * (Int64)g2;
			var F0G3 = f0 * (Int64)g3;
			var F0G4 = f0 * (Int64)g4;
			var F0G5 = f0 * (Int64)g5;
			var F0G6 = f0 * (Int64)g6;
			var F0G7 = f0 * (Int64)g7;
			var F0G8 = f0 * (Int64)g8;
			var F0G9 = f0 * (Int64)g9;
			var F1G0 = f1 * (Int64)g0;
			var F1G12 = F12 * (Int64)g1;
			var F1G2 = f1 * (Int64)g2;
			var F1G32 = F12 * (Int64)g3;
			var F1G4 = f1 * (Int64)g4;
			var F1G52 = F12 * (Int64)g5;
			var F1G6 = f1 * (Int64)g6;
			var F1G72 = F12 * (Int64)g7;
			var F1G8 = f1 * (Int64)g8;
			var F1G938 = F12 * (Int64)G919;
			var F2G0 = f2 * (Int64)g0;
			var F2G1 = f2 * (Int64)g1;
			var F2G2 = f2 * (Int64)g2;
			var F2G3 = f2 * (Int64)g3;
			var F2G4 = f2 * (Int64)g4;
			var F2G5 = f2 * (Int64)g5;
			var F2G6 = f2 * (Int64)g6;
			var F2G7 = f2 * (Int64)g7;
			var F2G819 = f2 * (Int64)G819;
			var F2G919 = f2 * (Int64)G919;
			var F3G0 = f3 * (Int64)g0;
			var F3G12 = F32 * (Int64)g1;
			var F3G2 = f3 * (Int64)g2;
			var F3G32 = F32 * (Int64)g3;
			var F3G4 = f3 * (Int64)g4;
			var F3G52 = F32 * (Int64)g5;
			var F3G6 = f3 * (Int64)g6;
			var F3G738 = F32 * (Int64)G719;
			var F3G819 = f3 * (Int64)G819;
			var F3G938 = F32 * (Int64)G919;
			var F4G0 = f4 * (Int64)g0;
			var F4G1 = f4 * (Int64)g1;
			var F4G2 = f4 * (Int64)g2;
			var F4G3 = f4 * (Int64)g3;
			var F4G4 = f4 * (Int64)g4;
			var F4G5 = f4 * (Int64)g5;
			var F4G619 = f4 * (Int64)G619;
			var F4G719 = f4 * (Int64)G719;
			var F4G819 = f4 * (Int64)G819;
			var F4G919 = f4 * (Int64)G919;
			var F5G0 = f5 * (Int64)g0;
			var F5G12 = F52 * (Int64)g1;
			var F5G2 = f5 * (Int64)g2;
			var F5G32 = F52 * (Int64)g3;
			var F5G4 = f5 * (Int64)g4;
			var F5G538 = F52 * (Int64)G519;
			var F5G619 = f5 * (Int64)G619;
			var F5G738 = F52 * (Int64)G719;
			var F5G819 = f5 * (Int64)G819;
			var F5G938 = F52 * (Int64)G919;
			var F6G0 = f6 * (Int64)g0;
			var F6G1 = f6 * (Int64)g1;
			var F6G2 = f6 * (Int64)g2;
			var F6G3 = f6 * (Int64)g3;
			var F6G419 = f6 * (Int64)G419;
			var F6G519 = f6 * (Int64)G519;
			var F6G619 = f6 * (Int64)G619;
			var F6G719 = f6 * (Int64)G719;
			var F6G819 = f6 * (Int64)G819;
			var F6G919 = f6 * (Int64)G919;
			var F7G0 = f7 * (Int64)g0;
			var F7G12 = F72 * (Int64)g1;
			var F7G2 = f7 * (Int64)g2;
			var F7G338 = F72 * (Int64)G319;
			var F7G419 = f7 * (Int64)G419;
			var F7G538 = F72 * (Int64)G519;
			var F7G619 = f7 * (Int64)G619;
			var F7G738 = F72 * (Int64)G719;
			var F7G819 = f7 * (Int64)G819;
			var F7G938 = F72 * (Int64)G919;
			var F8G0 = f8 * (Int64)g0;
			var F8G1 = f8 * (Int64)g1;
			var F8G219 = f8 * (Int64)G219;
			var F8G319 = f8 * (Int64)G319;
			var F8G419 = f8 * (Int64)G419;
			var F8G519 = f8 * (Int64)G519;
			var F8G619 = f8 * (Int64)G619;
			var F8G719 = f8 * (Int64)G719;
			var F8G819 = f8 * (Int64)G819;
			var F8G919 = f8 * (Int64)G919;
			var F9G0 = f9 * (Int64)g0;
			var F9G138 = F92 * (Int64)G119;
			var F9G219 = f9 * (Int64)G219;
			var F9G338 = F92 * (Int64)G319;
			var F9G419 = f9 * (Int64)G419;
			var F9G538 = F92 * (Int64)G519;
			var F9G619 = f9 * (Int64)G619;
			var F9G738 = F92 * (Int64)G719;
			var F9G819 = f9 * (Int64)G819;
			var F9G938 = F92 * (Int64)G919;
			var h0 = F0G0 + F1G938 + F2G819 + F3G738 + F4G619 + F5G538 + F6G419 + F7G338 + F8G219 + F9G138;
			var h1 = F0G1 + F1G0 + F2G919 + F3G819 + F4G719 + F5G619 + F6G519 + F7G419 + F8G319 + F9G219;
			var h2 = F0G2 + F1G12 + F2G0 + F3G938 + F4G819 + F5G738 + F6G619 + F7G538 + F8G419 + F9G338;
			var h3 = F0G3 + F1G2 + F2G1 + F3G0 + F4G919 + F5G819 + F6G719 + F7G619 + F8G519 + F9G419;
			var h4 = F0G4 + F1G32 + F2G2 + F3G12 + F4G0 + F5G938 + F6G819 + F7G738 + F8G619 + F9G538;
			var h5 = F0G5 + F1G4 + F2G3 + F3G2 + F4G1 + F5G0 + F6G919 + F7G819 + F8G719 + F9G619;
			var h6 = F0G6 + F1G52 + F2G4 + F3G32 + F4G2 + F5G12 + F6G0 + F7G938 + F8G819 + F9G738;
			var h7 = F0G7 + F1G6 + F2G5 + F3G4 + F4G3 + F5G2 + F6G1 + F7G0 + F8G919 + F9G819;
			var h8 = F0G8 + F1G72 + F2G6 + F3G52 + F4G4 + F5G32 + F6G2 + F7G12 + F8G0 + F9G938;
			var h9 = F0G9 + F1G8 + F2G7 + F3G6 + F4G5 + F5G4 + F6G3 + F7G2 + F8G1 + F9G0;
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

			/*
			|h0| <= (1.65*1.65*2^52*(1+19+19+19+19)+1.65*1.65*2^50*(38+38+38+38+38))
			  i.e. |h0| <= 1.4*2^60; narrower ranges for h2, h4, h6, h8
			|h1| <= (1.65*1.65*2^51*(1+1+19+19+19+19+19+19+19+19))
			  i.e. |h1| <= 1.7*2^59; narrower ranges for h3, h5, h7, h9
			*/

			carry0 = (h0 + (Int64)(1 << 25)) >> 26; h1 += carry0; h0 -= carry0 << 26;
			carry4 = (h4 + (Int64)(1 << 25)) >> 26; h5 += carry4; h4 -= carry4 << 26;
			/* |h0| <= 2^25 */
			/* |h4| <= 2^25 */
			/* |h1| <= 1.71*2^59 */
			/* |h5| <= 1.71*2^59 */

			carry1 = (h1 + (Int64)(1 << 24)) >> 25; h2 += carry1; h1 -= carry1 << 25;
			carry5 = (h5 + (Int64)(1 << 24)) >> 25; h6 += carry5; h5 -= carry5 << 25;
			/* |h1| <= 2^24; from now on fits into int32 */
			/* |h5| <= 2^24; from now on fits into int32 */
			/* |h2| <= 1.41*2^60 */
			/* |h6| <= 1.41*2^60 */

			carry2 = (h2 + (Int64)(1 << 25)) >> 26; h3 += carry2; h2 -= carry2 << 26;
			carry6 = (h6 + (Int64)(1 << 25)) >> 26; h7 += carry6; h6 -= carry6 << 26;
			/* |h2| <= 2^25; from now on fits into int32 unchanged */
			/* |h6| <= 2^25; from now on fits into int32 unchanged */
			/* |h3| <= 1.71*2^59 */
			/* |h7| <= 1.71*2^59 */

			carry3 = (h3 + (Int64)(1 << 24)) >> 25; h4 += carry3; h3 -= carry3 << 25;
			carry7 = (h7 + (Int64)(1 << 24)) >> 25; h8 += carry7; h7 -= carry7 << 25;
			/* |h3| <= 2^24; from now on fits into int32 unchanged */
			/* |h7| <= 2^24; from now on fits into int32 unchanged */
			/* |h4| <= 1.72*2^34 */
			/* |h8| <= 1.41*2^60 */

			carry4 = (h4 + (Int64)(1 << 25)) >> 26; h5 += carry4; h4 -= carry4 << 26;
			carry8 = (h8 + (Int64)(1 << 25)) >> 26; h9 += carry8; h8 -= carry8 << 26;
			/* |h4| <= 2^25; from now on fits into int32 unchanged */
			/* |h8| <= 2^25; from now on fits into int32 unchanged */
			/* |h5| <= 1.01*2^24 */
			/* |h9| <= 1.71*2^59 */

			carry9 = (h9 + (Int64)(1 << 24)) >> 25; h0 += carry9 * 19; h9 -= carry9 << 25;
			/* |h9| <= 2^24; from now on fits into int32 unchanged */
			/* |h0| <= 1.1*2^39 */

			carry0 = (h0 + (Int64)(1 << 25)) >> 26; h1 += carry0; h0 -= carry0 << 26;
			/* |h0| <= 2^25; from now on fits into int32 unchanged */
			/* |h1| <= 1.01*2^24 */

			H.x0 = (Int32)h0;
			H.x1 = (Int32)h1;
			H.x2 = (Int32)h2;
			H.x3 = (Int32)h3;
			H.x4 = (Int32)h4;
			H.x5 = (Int32)h5;
			H.x6 = (Int32)h6;
			H.x7 = (Int32)h7;
			H.x8 = (Int32)h8;
			H.x9 = (Int32)h9;
		}
	}
}