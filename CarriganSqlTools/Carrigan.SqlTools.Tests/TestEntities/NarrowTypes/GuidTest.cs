namespace Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

internal class GuidTest
{
    public Guid Value { get; set; }

    public GuidTest() { }

    internal GuidTest(Guid value) => Value = value;
}
