namespace Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

internal class DateOnlyTest
{
    public DateOnly Value { get; set; }

    public DateOnlyTest() { }

    internal DateOnlyTest(DateOnly value) => Value = value;
}
