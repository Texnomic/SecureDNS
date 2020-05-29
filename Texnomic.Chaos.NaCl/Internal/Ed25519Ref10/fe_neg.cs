using System;

namespace Texnomic.Chaos.NaCl.Internal.Ed25519Ref10
{
	internal static partial class FieldOperations
	{
		/*
		h = -f

		Preconditions:
		   |f| bounded by 1.1*2^25,1.1*2^24,1.1*2^25,1.1*2^24,etc.

		Postconditions:
		   |h| bounded by 1.1*2^25,1.1*2^24,1.1*2^25,1.1*2^24,etc.
		*/
		internal static void fe_neg(out FieldElement H, ref FieldElement F)
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
			var h0 = -f0;
			var h1 = -f1;
			var h2 = -f2;
			var h3 = -f3;
			var h4 = -f4;
			var h5 = -f5;
			var h6 = -f6;
			var h7 = -f7;
			var h8 = -f8;
			var h9 = -f9;
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