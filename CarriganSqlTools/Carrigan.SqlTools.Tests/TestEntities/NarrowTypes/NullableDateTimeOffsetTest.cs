namespace Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

internal class NullableDateTimeOffsetTest
{
    public DateTimeOffset? Value { get; set; }

    public NullableDateTimeOffsetTest() { }

    internal NullableDateTimeOffsetTest(DateTimeOffset? value) => Value = value;
}
