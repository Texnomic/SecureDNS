using System;

namespace Texnomic.Chaos.NaCl.Internal.Ed25519Ref10
{
	internal static partial class FieldOperations
	{
		/*
		Replace (f,g) with (g,g) if b == 1;
		replace (f,g) with (f,g) if b == 0.

		Preconditions: b in {0,1}.
		*/

		//void fe_cmov(fe f,const fe g,unsigned int b)
		internal static void fe_cmov(ref FieldElement F, ref FieldElement G, int B)
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
			var x0 = f0 ^ g0;
			var x1 = f1 ^ g1;
			var x2 = f2 ^ g2;
			var x3 = f3 ^ g3;
			var x4 = f4 ^ g4;
			var x5 = f5 ^ g5;
			var x6 = f6 ^ g6;
			var x7 = f7 ^ g7;
			var x8 = f8 ^ g8;
			var x9 = f9 ^ g9;
			B = -B;
			x0 &= B;
			x1 &= B;
			x2 &= B;
			x3 &= B;
			x4 &= B;
			x5 &= B;
			x6 &= B;
			x7 &= B;
			x8 &= B;
			x9 &= B;
			F.x0 = f0 ^ x0;
			F.x1 = f1 ^ x1;
			F.x2 = f2 ^ x2;
			F.x3 = f3 ^ x3;
			F.x4 = f4 ^ x4;
			F.x5 = f5 ^ x5;
			F.x6 = f6 ^ x6;
			F.x7 = f7 ^ x7;
			F.x8 = f8 ^ x8;
			F.x9 = f9 ^ x9;
		}
	}
}