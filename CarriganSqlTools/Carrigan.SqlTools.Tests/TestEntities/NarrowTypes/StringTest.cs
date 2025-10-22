namespace Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

internal class StringTest
{
    public string Value { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public StringTest() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    internal StringTest(string value) => Value = value;
}
