using System;

namespace Texnomic.Chaos.NaCl.Internal.Ed25519Ref10
{
	internal static partial class FieldOperations
	{
		/*
		h = f - g
		Can overlap h with f or g.

		Preconditions:
		   |f| bounded by 1.1*2^25,1.1*2^24,1.1*2^25,1.1*2^24,etc.
		   |g| bounded by 1.1*2^25,1.1*2^24,1.1*2^25,1.1*2^24,etc.

		Postconditions:
		   |h| bounded by 1.1*2^26,1.1*2^25,1.1*2^26,1.1*2^25,etc.
		*/

		internal static void fe_sub(out FieldElement H, ref FieldElement F, ref FieldElement G)
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
			var h0 = f0 - g0;
			var h1 = f1 - g1;
			var h2 = f2 - g2;
			var h3 = f3 - g3;
			var h4 = f4 - g4;
			var h5 = f5 - g5;
			var h6 = f6 - g6;
			var h7 = f7 - g7;
			var h8 = f8 - g8;
			var h9 = f9 - g9;
			H.x0 = h0;
			H.x1 = h1;
			H.x2 = h2;
			H.x3 = h3;
			H.x4 = h4;
			H.x5 = h5;
			H.x6 = h6;
			H.x7 = h7;
			H.x8 = h8;
			H.x9 = h9;
		}
	}
}