namespace Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

internal class DecimalTest
{
    public decimal Value { get; set; }

    public DecimalTest() { }

    internal DecimalTest(decimal value) => Value = value;
}
