namespace Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

internal class IntTest
{
    public int Value { get; set; }

    public IntTest() { }

    internal IntTest(int value) => Value = value;
}
