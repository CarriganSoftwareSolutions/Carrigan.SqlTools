using Carrigan.SqlTools.Invocation;
using Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

namespace Carrigan.SqlTools.Tests.InvocationTests.TypeTests;

public class SqlDateTimeOffsetTests
{
    private static Dictionary<string, object?> GetTestData(object? value)
    {
        Dictionary<string, object?> data = [];
        data["Value"] = value;
        return data;
    }
    [Fact]
    public void DateTimeOffset_Min()
    {
        Dictionary<string, object?> data = GetTestData(DateTimeOffset.MinValue);
        DateTimeOffsetTest actual = Invoker<DateTimeOffsetTest>.Invoke(data);
        Assert.Equal(DateTimeOffset.MinValue, actual.Value);
    }

    [Fact]
    public void DateTimeOffset_Max()
    {
        Dictionary<string, object?> data = GetTestData(DateTimeOffset.MaxValue);
        DateTimeOffsetTest actual = Invoker<DateTimeOffsetTest>.Invoke(data);
        Assert.Equal(DateTimeOffset.MaxValue, actual.Value);
    }

    [Fact]
    public void DateTimeOffset_Reasonable()
    {
        DateTimeOffset value = new(2020, 1, 2, 3, 4, 5, TimeSpan.FromHours(-5));
        Dictionary<string, object?> data = GetTestData(value);
        DateTimeOffsetTest actual = Invoker<DateTimeOffsetTest>.Invoke(data);
        Assert.Equal(value, actual.Value);
    }

    [Fact]
    public void NullableDateTimeOffset_Min()
    {
        Dictionary<string, object?> data = GetTestData(DateTimeOffset.MinValue);
        NullableDateTimeOffsetTest actual = Invoker<NullableDateTimeOffsetTest>.Invoke(data);
        Assert.Equal(DateTimeOffset.MinValue, actual.Value);
    }

    [Fact]
    public void NullableDateTimeOffset_Max()
    {
        Dictionary<string, object?> data = GetTestData(DateTimeOffset.MaxValue);
        NullableDateTimeOffsetTest actual = Invoker<NullableDateTimeOffsetTest>.Invoke(data);
        Assert.Equal(DateTimeOffset.MaxValue, actual.Value);
    }

    [Fact]
    public void NullableDateTimeOffset_Null()
    {
        Dictionary<string, object?> data = GetTestData(null);
        NullableDateTimeOffsetTest actual = Invoker<NullableDateTimeOffsetTest>.Invoke(data);
        Assert.Null(actual.Value);
    }
}
