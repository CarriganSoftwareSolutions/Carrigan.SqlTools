namespace Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

internal class NullableTimeSpanTest
{
    public TimeSpan? Value { get; set; }

    public NullableTimeSpanTest() { }

    internal NullableTimeSpanTest(TimeSpan? value) => Value = value;
}
