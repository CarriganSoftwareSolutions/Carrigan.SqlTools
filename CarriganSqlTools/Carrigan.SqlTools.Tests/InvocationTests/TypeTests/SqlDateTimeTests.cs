using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Invocation;
using Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

namespace Carrigan.SqlTools.Tests.InvocationTests.TypeTests;

public class SqlDateTimeTests
{
    private static Dictionary<string, object?> GetTestData(object? value)
    {
        Dictionary<string, object?> data = [];
        data["Value"] = value;
        return data;
    }
    [Fact]
    public void DateTime_Min()
    {
        Dictionary<string, object?> data = GetTestData(DateTime.MinValue);
        DateTimeTest actual = Invoker<DateTimeTest>.Invoke(data);
        Assert.Equal(DateTime.MinValue, actual.Value);
    }

    [Fact]
    public void DateTime_Max()
    {
        Dictionary<string, object?> data = GetTestData(DateTime.MaxValue);
        DateTimeTest actual = Invoker<DateTimeTest>.Invoke(data);
        Assert.Equal(DateTime.MaxValue, actual.Value);
    }

    [Fact]
    public void DateTime_Reasonable()
    {
        DateTime value = new(2020, 1, 2, 3, 4, 5, DateTimeKind.Utc);
        Dictionary<string, object?> data = GetTestData(value);
        DateTimeTest actual = Invoker<DateTimeTest>.Invoke(data);
        Assert.Equal(value, actual.Value);
    }

    [Fact]
    public void NullableDateTime_Min()
    {
        Dictionary<string, object?> data = GetTestData(DateTime.MinValue);
        NullableDateTimeTest actual = Invoker<NullableDateTimeTest>.Invoke(data);
        Assert.Equal(DateTime.MinValue, actual.Value);
    }

    [Fact]
    public void NullableDateTime_Max()
    {
        Dictionary<string, object?> data = GetTestData(DateTime.MaxValue);
        NullableDateTimeTest actual = Invoker<NullableDateTimeTest>.Invoke(data);
        Assert.Equal(DateTime.MaxValue, actual.Value);
    }

    [Fact]
    public void NullableDateTime_Null()
    {
        Dictionary<string, object?> data = GetTestData(null);
        NullableDateTimeTest actual = Invoker<NullableDateTimeTest>.Invoke(data);
        Assert.Null(actual.Value);
    }

    [Fact]
    public void ToDateOnly()
    {
        DateTime value = new(1971, 11, 15, 11, 35, 12);
        Dictionary<string, object?> data = GetTestData(value);
        DateOnlyTest actual = Invoker<DateOnlyTest>.Invoke(data);
        Assert.Equal(value.ToDateOnly(), actual.Value);
    }

    [Fact]
    public void ToTimeOnly()
    {
        DateTime value = new(1971, 11, 15, 11, 35, 12);
        Dictionary<string, object?> data = GetTestData(value);
        TimeOnlyTest actual = Invoker<TimeOnlyTest>.Invoke(data);
        Assert.Equal(value.ToTimeOnly(), actual.Value);
    }

    [Fact]
    public void ToDateTimeOffSet()
    {
        DateTime value = new(1971, 11, 15, 11, 35, 12);
        DateTimeOffset expected = new(value);
        Dictionary<string, object?> data = GetTestData(value);
        DateTimeOffsetTest actual = Invoker<DateTimeOffsetTest>.Invoke(data);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public void NullableToDateOnly()
    {
        DateTime value = new(1971, 11, 15, 11, 35, 12);
        Dictionary<string, object?> data = GetTestData(value);
        NullableDateOnlyTest actual = Invoker<NullableDateOnlyTest>.Invoke(data);
        Assert.Equal(value.ToDateOnly(), actual.Value);
    }

    [Fact]
    public void NullableToTimeOnly()
    {
        DateTime value = new(1971, 11, 15, 11, 35, 12);
        Dictionary<string, object?> data = GetTestData(value);
        NullableTimeOnlyTest actual = Invoker<NullableTimeOnlyTest>.Invoke(data);
        Assert.Equal(value.ToTimeOnly(), actual.Value);
    }

    [Fact]
    public void NullableToDateTimeOffSet()
    {
        DateTime value = new(1971, 11, 15, 11, 35, 12);
        DateTimeOffset expected = new(value);
        Dictionary<string, object?> data = GetTestData(value);
        NullableDateTimeOffsetTest actual = Invoker<NullableDateTimeOffsetTest>.Invoke(data);
        Assert.Equal(expected, actual.Value);
    }


    [Fact]
    public void NullToDateOnly()
    {
        DateTime? value = null;
        Dictionary<string, object?> data = GetTestData(value);
        NullableDateOnlyTest actual = Invoker<NullableDateOnlyTest>.Invoke(data);
        Assert.Null(actual.Value);
    }

    [Fact]
    public void NullTimeOnly()
    {
        DateTime? value = null;
        Dictionary<string, object?> data = GetTestData(value);
        NullableTimeOnlyTest actual = Invoker<NullableTimeOnlyTest>.Invoke(data);
        Assert.Null(actual.Value);
    }

    [Fact]
    public void NullToDateTimeOffSet()
    {
        DateTime? value = null;
        Dictionary<string, object?> data = GetTestData(value);
        NullableDateTimeOffsetTest actual = Invoker<NullableDateTimeOffsetTest>.Invoke(data);
        Assert.Null(actual.Value);
    }
}
