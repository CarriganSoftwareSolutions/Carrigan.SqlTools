namespace Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

internal class NullableShortTest
{
    public short? Value { get; set; }

    public NullableShortTest() { }

    internal NullableShortTest(short? value) => Value = value;
}
