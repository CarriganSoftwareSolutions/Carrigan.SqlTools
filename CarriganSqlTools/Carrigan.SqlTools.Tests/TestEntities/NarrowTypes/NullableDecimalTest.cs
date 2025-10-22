namespace Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

internal class NullableDecimalTest
{
    public decimal? Value { get; set; }

    public NullableDecimalTest() { }

    internal NullableDecimalTest(decimal? value) => Value = value;
}
