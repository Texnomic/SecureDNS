namespace Texnomic.Chaos.NaCl.Internal.Ed25519Ref10
{
	internal static partial class GroupOperations
	{
		public static void ge_tobytes(byte[] S, int Offset, ref  GroupElementP2 H)
		{
			FieldElement recip;
			FieldElement x;
			FieldElement y;

			FieldOperations.fe_invert(out recip, ref H.Z);
			FieldOperations.fe_mul(out x, ref H.X, ref recip);
			FieldOperations.fe_mul(out y, ref H.Y, ref recip);
			FieldOperations.fe_tobytes(S, Offset, ref y);
			S[Offset + 31] ^= (byte)(FieldOperations.fe_isnegative(ref x) << 7);
		}
	}
}