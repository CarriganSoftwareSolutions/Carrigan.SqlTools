using Carrigan.SqlTools.Invocation;
using Carrigan.SqlTools.Tests.InvocationTests;
using Carrigan.SqlTools.Tests.TestEntities;

namespace Carrigan.SqlTools.Tests.InvocationTests;

public class SpecialInvokerTests
{
    // Arrange: Build a dictionary simulating ADO.Net values.
    // Note: SQL Server returns a DateTime for a date column and a TimeSpan for a time column.
    // Also, sometimes the DB might return a DateTime for a DateTimeOffset column.
    Dictionary<string, object?> specialInvocation = new()
    {
            // DateOnly property: simulate a SQL date returned as DateTime
            { "DateOnlyValue", new DateTime(2025, 2, 19, 0, 0, 0) },
            
            // TimeOnly property: simulate a SQL time returned as TimeSpan
            { "TimeOnlyValue",  new TimeSpan(14, 30, 0) },
            
            // DateTimeOffset property: simulate a DateTime returned from the database that needs conversion
            { "DateTimeOffsetValue", new DateTime(2025, 2, 19, 14, 30, 0) },
            
            // Enum conversion: simulate a string value that should be converted to TestEnum
            { "EnumValueString", "ValueB" },
            
            // Enum conversion: simulate a string value that should be converted to TestEnum
            { "EnumValueInt", 2 },
            
            // Nullable enum: simulate a DBNull value, which should convert to null
            { "NullableEnumValue", DBNull.Value }
        };
    // Act: Invoke the conversion/invocation process.
    Invoker<SpecialEntity> invocator;
    SpecialEntity entity;
    public SpecialInvokerTests()
    {
        // Act: Invoke the conversion/invocation process.
        invocator = new Invoker<SpecialEntity>();
        entity = invocator.Invoke(specialInvocation);
    }

    [Fact]
    public void Special_Invocation_Test1()
    {
        DateOnly dateOnlyValue = new(2025, 2, 19);
        // DateOnly conversion: Compare with DateOnly converted from the original DateTime.
        Assert.Equal(dateOnlyValue, entity.DateOnlyValue);
    }
    [Fact]
    public void Special_Invocation_Test2()
    {
        TimeOnly timeOnlyValue = new(14, 30, 0);
        // TimeOnly conversion: Compare with TimeOnly created from the original TimeSpan.
        Assert.Equal(timeOnlyValue, entity.TimeOnlyValue);
    }
    [Fact]
    public void Special_Invocation_Test3()
    {            
        // DateTimeOffset conversion: Expect a conversion from DateTime.
        DateTimeOffset expectedDTO = new((DateTime)specialInvocation["DateTimeOffsetValue"]!);
        Assert.Equal(expectedDTO, entity.DateTimeOffsetValue);
    }
    [Fact]
    public void Special_Invocation_Test4()
    {
        // Enum conversion: The string "ValueB" should map to TestEnum.ValueB.
        Assert.Equal(TestEnum.ValueB, entity.EnumValueString);
    }
    [Fact]
    public void Special_Invocation_Test5()
    {
        // Enum conversion: The int "ValueB" should map to TestEnum.ValueB.
        Assert.Equal(TestEnum.ValueB, entity.EnumValueInt);
    }
    [Fact]
    public void Special_Invocation_Test6()
    {

        // Nullable enum: Since the input was DBNull, the resulting property should be null.
        Assert.Null(entity.NullableEnumValue);
    }
}
