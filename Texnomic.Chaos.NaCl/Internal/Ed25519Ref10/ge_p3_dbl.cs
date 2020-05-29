namespace Texnomic.Chaos.NaCl.Internal.Ed25519Ref10
{
	internal static partial class GroupOperations
	{
		/*
		r = 2 * p
		*/
		public static void ge_p3_dbl(out GroupElementP1P1 R, ref GroupElementP3 P)
		{
			GroupElementP2 q;
			ge_p3_to_p2(out q, ref P);
			ge_p2_dbl(out R, ref q);
		}
	}
}