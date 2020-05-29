using System;

namespace Texnomic.Chaos.NaCl.Internal.Ed25519Ref10
{
    internal static partial class FieldOperations
    {
        /*
        Replace (f,g) with (g,f) if b == 1;
        replace (f,g) with (f,g) if b == 0.

        Preconditions: b in {0,1}.
        */
        public static void fe_cswap(ref FieldElement F, ref FieldElement G, uint B)
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
            var negb = unchecked((int)-B);
            x0 &= negb;
            x1 &= negb;
            x2 &= negb;
            x3 &= negb;
            x4 &= negb;
            x5 &= negb;
            x6 &= negb;
            x7 &= negb;
            x8 &= negb;
            x9 &= negb;
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
            G.x0 = g0 ^ x0;
            G.x1 = g1 ^ x1;
            G.x2 = g2 ^ x2;
            G.x3 = g3 ^ x3;
            G.x4 = g4 ^ x4;
            G.x5 = g5 ^ x5;
            G.x6 = g6 ^ x6;
            G.x7 = g7 ^ x7;
            G.x8 = g8 ^ x8;
            G.x9 = g9 ^ x9;
        }
    }
}