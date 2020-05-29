namespace Texnomic.Chaos.NaCl.Internal.Ed25519Ref10
{
	internal static partial class GroupOperations
	{
		/*
		r = p
		*/
		public static void ge_p3_to_cached(out GroupElementCached R, ref GroupElementP3 P)
		{
			FieldOperations.fe_add(out R.YplusX, ref P.Y, ref P.X);
			FieldOperations.fe_sub(out R.YminusX, ref P.Y, ref P.X);
			R.Z = P.Z;
			FieldOperations.fe_mul(out R.T2d, ref P.T, ref LookupTables.D2);
		}
	}
}