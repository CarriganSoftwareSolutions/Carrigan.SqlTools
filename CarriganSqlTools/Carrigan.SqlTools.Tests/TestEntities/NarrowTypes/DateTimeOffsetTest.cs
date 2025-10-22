namespace Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

internal class DateTimeOffsetTest
{
    public DateTimeOffset Value { get; set; }

    public DateTimeOffsetTest() { }

    internal DateTimeOffsetTest(DateTimeOffset value) => Value = value;
}
