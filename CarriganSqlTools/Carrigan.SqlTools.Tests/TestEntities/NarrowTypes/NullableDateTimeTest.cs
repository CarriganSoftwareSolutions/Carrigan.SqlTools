namespace Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

internal class NullableDateTimeTest
{
    public DateTime? Value { get; set; }

    public NullableDateTimeTest() { }

    internal NullableDateTimeTest(DateTime? value) => Value = value;
}
