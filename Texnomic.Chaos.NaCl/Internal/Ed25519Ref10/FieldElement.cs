namespace Texnomic.Chaos.NaCl.Internal.Ed25519Ref10
{
    internal struct FieldElement
    {
        internal int x0;
        internal int x1;
        internal int x2;
        internal int x3;
        internal int x4;
        internal int x5;
        internal int x6;
        internal int x7;
        internal int x8;
        internal int x9;

        //public static readonly FieldElement Zero = new FieldElement();
        //public static readonly FieldElement One = new FieldElement() { x0 = 1 };

        internal FieldElement(params int[] Elements)
        {
            InternalAssert.Assert(Elements.Length == 10, "elements.Length != 10");
            x0 = Elements[0];
            x1 = Elements[1];
            x2 = Elements[2];
            x3 = Elements[3];
            x4 = Elements[4];
            x5 = Elements[5];
            x6 = Elements[6];
            x7 = Elements[7];
            x8 = Elements[8];
            x9 = Elements[9];
        }
    }
}
