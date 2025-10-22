namespace Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

internal class TimeOnlyTest
{
    public TimeOnly Value { get; set; }

    public TimeOnlyTest() { }

    internal TimeOnlyTest(TimeOnly value) => Value = value;
}
