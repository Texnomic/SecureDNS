using System;

namespace Texnomic.Chaos.NaCl.Internal.Ed25519Ref10
{
	internal static partial class FieldOperations
	{
		/*
		h = f * f
		Can overlap h with f.

		Preconditions:
		   |f| bounded by 1.65*2^26,1.65*2^25,1.65*2^26,1.65*2^25,etc.

		Postconditions:
		   |h| bounded by 1.01*2^25,1.01*2^24,1.01*2^25,1.01*2^24,etc.
		*/

		/*
		See fe_mul.c for discussion of implementation strategy.
		*/
		internal static void fe_sq(out FieldElement H, ref FieldElement F)
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
			var F02 = 2 * f0;
			var F12 = 2 * f1;
			var F22 = 2 * f2;
			var F32 = 2 * f3;
			var F42 = 2 * f4;
			var F52 = 2 * f5;
			var F62 = 2 * f6;
			var F72 = 2 * f7;
			var F538 = 38 * f5; /* 1.959375*2^30 */
			var F619 = 19 * f6; /* 1.959375*2^30 */
			var F738 = 38 * f7; /* 1.959375*2^30 */
			var F819 = 19 * f8; /* 1.959375*2^30 */
			var F938 = 38 * f9; /* 1.959375*2^30 */
			var F0F0 = f0 * (Int64)f0;
			var F0F12 = F02 * (Int64)f1;
			var F0F22 = F02 * (Int64)f2;
			var F0F32 = F02 * (Int64)f3;
			var F0F42 = F02 * (Int64)f4;
			var F0F52 = F02 * (Int64)f5;
			var F0F62 = F02 * (Int64)f6;
			var F0F72 = F02 * (Int64)f7;
			var F0F82 = F02 * (Int64)f8;
			var F0F92 = F02 * (Int64)f9;
			var F1F12 = F12 * (Int64)f1;
			var F1F22 = F12 * (Int64)f2;
			var F1F34 = F12 * (Int64)F32;
			var F1F42 = F12 * (Int64)f4;
			var F1F54 = F12 * (Int64)F52;
			var F1F62 = F12 * (Int64)f6;
			var F1F74 = F12 * (Int64)F72;
			var F1F82 = F12 * (Int64)f8;
			var F1F976 = F12 * (Int64)F938;
			var F2F2 = f2 * (Int64)f2;
			var F2F32 = F22 * (Int64)f3;
			var F2F42 = F22 * (Int64)f4;
			var F2F52 = F22 * (Int64)f5;
			var F2F62 = F22 * (Int64)f6;
			var F2F72 = F22 * (Int64)f7;
			var F2F838 = F22 * (Int64)F819;
			var F2F938 = f2 * (Int64)F938;
			var F3F32 = F32 * (Int64)f3;
			var F3F42 = F32 * (Int64)f4;
			var F3F54 = F32 * (Int64)F52;
			var F3F62 = F32 * (Int64)f6;
			var F3F776 = F32 * (Int64)F738;
			var F3F838 = F32 * (Int64)F819;
			var F3F976 = F32 * (Int64)F938;
			var F4F4 = f4 * (Int64)f4;
			var F4F52 = F42 * (Int64)f5;
			var F4F638 = F42 * (Int64)F619;
			var F4F738 = f4 * (Int64)F738;
			var F4F838 = F42 * (Int64)F819;
			var F4F938 = f4 * (Int64)F938;
			var F5F538 = f5 * (Int64)F538;
			var F5F638 = F52 * (Int64)F619;
			var F5F776 = F52 * (Int64)F738;
			var F5F838 = F52 * (Int64)F819;
			var F5F976 = F52 * (Int64)F938;
			var F6F619 = f6 * (Int64)F619;
			var F6F738 = f6 * (Int64)F738;
			var F6F838 = F62 * (Int64)F819;
			var F6F938 = f6 * (Int64)F938;
			var F7F738 = f7 * (Int64)F738;
			var F7F838 = F72 * (Int64)F819;
			var F7F976 = F72 * (Int64)F938;
			var F8F819 = f8 * (Int64)F819;
			var F8F938 = f8 * (Int64)F938;
			var F9F938 = f9 * (Int64)F938;
			var h0 = F0F0 + F1F976 + F2F838 + F3F776 + F4F638 + F5F538;
			var h1 = F0F12 + F2F938 + F3F838 + F4F738 + F5F638;
			var h2 = F0F22 + F1F12 + F3F976 + F4F838 + F5F776 + F6F619;
			var h3 = F0F32 + F1F22 + F4F938 + F5F838 + F6F738;
			var h4 = F0F42 + F1F34 + F2F2 + F5F976 + F6F838 + F7F738;
			var h5 = F0F52 + F1F42 + F2F32 + F6F938 + F7F838;
			var h6 = F0F62 + F1F54 + F2F42 + F3F32 + F7F976 + F8F819;
			var h7 = F0F72 + F1F62 + F2F52 + F3F42 + F8F938;
			var h8 = F0F82 + F1F74 + F2F62 + F3F54 + F4F4 + F9F938;
			var h9 = F0F92 + F1F82 + F2F72 + F3F62 + F4F52;
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

			carry0 = (h0 + (Int64)(1 << 25)) >> 26; h1 += carry0; h0 -= carry0 << 26;
			carry4 = (h4 + (Int64)(1 << 25)) >> 26; h5 += carry4; h4 -= carry4 << 26;

			carry1 = (h1 + (Int64)(1 << 24)) >> 25; h2 += carry1; h1 -= carry1 << 25;
			carry5 = (h5 + (Int64)(1 << 24)) >> 25; h6 += carry5; h5 -= carry5 << 25;

			carry2 = (h2 + (Int64)(1 << 25)) >> 26; h3 += carry2; h2 -= carry2 << 26;
			carry6 = (h6 + (Int64)(1 << 25)) >> 26; h7 += carry6; h6 -= carry6 << 26;

			carry3 = (h3 + (Int64)(1 << 24)) >> 25; h4 += carry3; h3 -= carry3 << 25;
			carry7 = (h7 + (Int64)(1 << 24)) >> 25; h8 += carry7; h7 -= carry7 << 25;

			carry4 = (h4 + (Int64)(1 << 25)) >> 26; h5 += carry4; h4 -= carry4 << 26;
			carry8 = (h8 + (Int64)(1 << 25)) >> 26; h9 += carry8; h8 -= carry8 << 26;

			carry9 = (h9 + (Int64)(1 << 24)) >> 25; h0 += carry9 * 19; h9 -= carry9 << 25;

			carry0 = (h0 + (Int64)(1 << 25)) >> 26; h1 += carry0; h0 -= carry0 << 26;

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