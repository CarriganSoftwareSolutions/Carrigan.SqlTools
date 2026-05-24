namespace Carrigan.SqlTools.Base.Tests.TestEntities.NarrowTypes;

internal class NullableGuidTest
{
    public Guid? Value { get; set; }

    public NullableGuidTest() { }

    internal NullableGuidTest(Guid? value) => Value = value;
}
