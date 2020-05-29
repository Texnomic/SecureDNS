namespace Texnomic.Chaos.NaCl.Internal.Ed25519Ref10
{
	internal static partial class GroupOperations
	{
		public static void ge_precomp_0(out GroupElementPreComp H)
		{
			FieldOperations.fe_1(out H.Yplusx);
			FieldOperations.fe_1(out H.Yminusx);
			FieldOperations.fe_0(out H.Xy2d);
		}
	}
}