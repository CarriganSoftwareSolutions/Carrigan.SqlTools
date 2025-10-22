namespace Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

internal class DateTimeTest
{
    public DateTime Value { get; set; }

    public DateTimeTest() { }

    internal DateTimeTest(DateTime value) => Value = value;
}
