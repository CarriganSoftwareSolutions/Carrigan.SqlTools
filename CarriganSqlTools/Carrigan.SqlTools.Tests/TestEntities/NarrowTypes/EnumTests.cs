namespace Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

internal class EnumTests
{
    public Enum Value { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public EnumTests() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    internal EnumTests(Enum value) => Value = value;
}
