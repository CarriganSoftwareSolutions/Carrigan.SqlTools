namespace Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

internal class NullableByteArrayTest
{
    public byte[]? Value { get; set; }

    public NullableByteArrayTest() { }

    internal NullableByteArrayTest(byte[]? value) => Value = value;
}
