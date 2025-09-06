
namespace Carrigan.SqlTools.InvocationTests;

//IGNORE SPELLING: Enums

public class SpecialEntity
{
    // Although SQL Server’s date type returns a DateTime, our conversion logic should
    // convert it into a DateOnly.
    public DateOnly DateOnlyValue { get; set; }

    // SQL Server’s time type returns a TimeSpan. We’ll convert that to a TimeOnly.
    public TimeOnly TimeOnlyValue { get; set; }

    // Sometimes the database might return a DateTime even for columns that are meant to hold
    // a DateTimeOffset. Our conversion logic should handle that by constructing a DateTimeOffset.
    public DateTimeOffset DateTimeOffsetValue { get; set; }

    // For enums, we might receive a string (or integer) from the database. Our conversion logic
    // should convert it to the corresponding enum value.
    public TestEnum EnumValueString { get; set; }
    // For enums, we might receive a string (or integer) from the database. Our conversion logic
    // should convert it to the corresponding enum value.
    public TestEnum EnumValueInt { get; set; }

    // Nullable enum conversion should handle null or DBNull.Value.
    public TestEnum? NullableEnumValue { get; set; }
}
