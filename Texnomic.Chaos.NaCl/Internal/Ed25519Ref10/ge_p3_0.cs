namespace Texnomic.Chaos.NaCl.Internal.Ed25519Ref10
{
	internal static partial class GroupOperations
	{
		public static void ge_p3_0(out GroupElementP3 H)
		{
			FieldOperations.fe_0(out H.X);
			FieldOperations.fe_1(out H.Y);
			FieldOperations.fe_1(out H.Z);
			FieldOperations.fe_0(out  H.T);
		}
	}
}