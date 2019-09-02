using System;
using BinarySerialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Texnomic.DNS.Extensions;

namespace Texnomic.DNS.Tests.Serialization.Bugs
{
    [TestClass]
    public class RecursionDesired
    {
        public class TestObject
        {
            [FieldOrder(0), FieldBitLength(7)] public byte Value { get; set; }

            [FieldOrder(1), FieldBitLength(1)] public bool Flag { get; set; }
        }


        [TestMethod]
        public void Run()
        {
            var TestObject = new TestObject()
            {
                Flag = true
            };

            var Serializer = new BinarySerializer();

            var Result = Serializer.Serialize(TestObject);

            var Byte = Convert.ToString(Result[0], 2).PadLeft(8, '0');

            Assert.AreEqual("00000001", Byte);
        }
    }
}
