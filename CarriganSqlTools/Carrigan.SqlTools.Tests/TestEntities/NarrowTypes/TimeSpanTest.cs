namespace Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

internal class TimeSpanTest
{
    public TimeSpan Value { get; set; }

    public TimeSpanTest() { }

    internal TimeSpanTest(TimeSpan value) => Value = value;
}
