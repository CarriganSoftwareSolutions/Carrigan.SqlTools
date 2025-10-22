namespace Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

internal class NullableBoolTest
{
    public bool? Value { get; set; }

    public NullableBoolTest() { }

    internal NullableBoolTest(bool? value) => Value = value;
}
