namespace Texnomic.Chaos.NaCl.Internal.Ed25519Ref10
{
	internal static partial class GroupOperations
	{
		/*
		r = p
		*/
		public static void ge_p3_to_p2(out GroupElementP2 R, ref GroupElementP3 P)
		{
			R.X = P.X;
			R.Y = P.Y;
			R.Z = P.Z;
		}
	}
}