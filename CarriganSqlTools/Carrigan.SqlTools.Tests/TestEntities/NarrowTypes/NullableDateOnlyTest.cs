namespace Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

internal class NullableDateOnlyTest
{
    public DateOnly? Value { get; set; }

    public NullableDateOnlyTest() { }

    internal NullableDateOnlyTest(DateOnly? value) => Value = value;
}
