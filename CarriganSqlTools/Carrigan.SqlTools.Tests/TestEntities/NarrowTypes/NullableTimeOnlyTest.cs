namespace Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

internal class NullableTimeOnlyTest
{
    public TimeOnly? Value { get; set; }

    public NullableTimeOnlyTest() { }

    internal NullableTimeOnlyTest(TimeOnly? value) => Value = value;
}
