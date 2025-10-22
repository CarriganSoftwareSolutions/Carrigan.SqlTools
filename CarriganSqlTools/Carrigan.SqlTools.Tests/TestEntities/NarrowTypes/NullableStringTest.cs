namespace Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

internal class NullableStringTest
{
    public string? Value { get; set; }

    public NullableStringTest() { }

    internal NullableStringTest(string? value) => Value = value;
}
