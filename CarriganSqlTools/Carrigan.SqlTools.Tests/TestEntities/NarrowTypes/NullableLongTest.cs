namespace Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

internal class NullableLongTest
{
    public long? Value { get; set; }

    public NullableLongTest() { }

    internal NullableLongTest(long? value) => Value = value;
}
