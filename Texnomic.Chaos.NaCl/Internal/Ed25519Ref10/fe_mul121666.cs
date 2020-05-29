using System;

namespace Texnomic.Chaos.NaCl.Internal.Ed25519Ref10
{
	internal static partial class FieldOperations
	{

		/*
		h = f * 121666
		Can overlap h with f.

		Preconditions:
		   |f| bounded by 1.1*2^26,1.1*2^25,1.1*2^26,1.1*2^25,etc.

		Postconditions:
		   |h| bounded by 1.1*2^25,1.1*2^24,1.1*2^25,1.1*2^24,etc.
		*/

		public static void fe_mul121666(out FieldElement H, ref FieldElement F)
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
			var h0 = f0 * (Int64)121666;
			var h1 = f1 * (Int64)121666;
			var h2 = f2 * (Int64)121666;
			var h3 = f3 * (Int64)121666;
			var h4 = f4 * (Int64)121666;
			var h5 = f5 * (Int64)121666;
			var h6 = f6 * (Int64)121666;
			var h7 = f7 * (Int64)121666;
			var h8 = f8 * (Int64)121666;
			var h9 = f9 * (Int64)121666;
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

			carry9 = (h9 + (Int64)(1 << 24)) >> 25; h0 += carry9 * 19; h9 -= carry9 << 25;
			carry1 = (h1 + (Int64)(1 << 24)) >> 25; h2 += carry1; h1 -= carry1 << 25;
			carry3 = (h3 + (Int64)(1 << 24)) >> 25; h4 += carry3; h3 -= carry3 << 25;
			carry5 = (h5 + (Int64)(1 << 24)) >> 25; h6 += carry5; h5 -= carry5 << 25;
			carry7 = (h7 + (Int64)(1 << 24)) >> 25; h8 += carry7; h7 -= carry7 << 25;

			carry0 = (h0 + (Int64)(1 << 25)) >> 26; h1 += carry0; h0 -= carry0 << 26;
			carry2 = (h2 + (Int64)(1 << 25)) >> 26; h3 += carry2; h2 -= carry2 << 26;
			carry4 = (h4 + (Int64)(1 << 25)) >> 26; h5 += carry4; h4 -= carry4 << 26;
			carry6 = (h6 + (Int64)(1 << 25)) >> 26; h7 += carry6; h6 -= carry6 << 26;
			carry8 = (h8 + (Int64)(1 << 25)) >> 26; h9 += carry8; h8 -= carry8 << 26;

			H.x0 = (int)h0;
			H.x1 = (int)h1;
			H.x2 = (int)h2;
			H.x3 = (int)h3;
			H.x4 = (int)h4;
			H.x5 = (int)h5;
			H.x6 = (int)h6;
			H.x7 = (int)h7;
			H.x8 = (int)h8;
			H.x9 = (int)h9;
		}
	}
}