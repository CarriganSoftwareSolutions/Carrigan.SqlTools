namespace Carrigan.SqlTools.Base.Tests.TestEntities.NarrowTypes;

internal class NullableFloatTest
{
    public float? Value { get; set; }

    public NullableFloatTest() { }

    internal NullableFloatTest(float? value) => Value = value;
}
