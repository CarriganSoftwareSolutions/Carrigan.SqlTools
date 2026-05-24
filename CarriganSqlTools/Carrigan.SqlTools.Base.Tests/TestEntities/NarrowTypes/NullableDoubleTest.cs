namespace Carrigan.SqlTools.Base.Tests.TestEntities.NarrowTypes;

internal class NullableDoubleTest
{
    public double? Value { get; set; }

    public NullableDoubleTest() { }

    internal NullableDoubleTest(double? value) => Value = value;
}
