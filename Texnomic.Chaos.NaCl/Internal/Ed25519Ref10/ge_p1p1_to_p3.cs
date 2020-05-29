namespace Texnomic.Chaos.NaCl.Internal.Ed25519Ref10
{
	internal static partial class GroupOperations
	{
		/*
		r = p
		*/
		public static void ge_p1p1_to_p3(out GroupElementP3 R, ref  GroupElementP1P1 P)
		{
			FieldOperations.fe_mul(out R.X, ref P.X, ref P.T);
			FieldOperations.fe_mul(out R.Y, ref P.Y, ref P.Z);
			FieldOperations.fe_mul(out R.Z, ref P.Z, ref P.T);
			FieldOperations.fe_mul(out R.T, ref P.X, ref P.Y);
		}
	}
}