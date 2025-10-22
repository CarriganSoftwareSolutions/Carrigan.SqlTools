namespace Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

internal class NullableCharTest
{
    public char? Value { get; set; }

    public NullableCharTest() { }

    internal NullableCharTest(char? value) => Value = value;
}
