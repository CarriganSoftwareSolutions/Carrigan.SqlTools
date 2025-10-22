namespace Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

internal class NullableByteTest
{
    public byte? Value { get; set; }

    public NullableByteTest() { }

    internal NullableByteTest(byte? value) => Value = value;
}
