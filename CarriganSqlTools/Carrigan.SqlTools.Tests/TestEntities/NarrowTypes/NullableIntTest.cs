namespace Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

internal class NullableIntTest
{
    public int? Value { get; set; }

    public NullableIntTest() { }

    internal NullableIntTest(int? value) => Value = value;
}
