using Carrigan.SqlTools.Invocation;
using Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

namespace Carrigan.SqlTools.Tests.InvocationTests.TypeTests;

public class SqlTimeTests
{
    private static Dictionary<string, object?> GetTestData(object? value)
    {
        Dictionary<string, object?> data = [];
        data["Value"] = value;
        return data;
    }
    [Fact]
    public void TimeOnly_Min()
    {
        Dictionary<string, object?> data = GetTestData(TimeOnly.MinValue);
        TimeOnlyTest actual = Invoker<TimeOnlyTest>.Invoke(data);
        Assert.Equal(TimeOnly.MinValue, actual.Value);
    }

    [Fact]
    public void TimeOnly_Max()
    {
        Dictionary<string, object?> data = GetTestData(TimeOnly.MaxValue);
        TimeOnlyTest actual = Invoker<TimeOnlyTest>.Invoke(data);
        Assert.Equal(TimeOnly.MaxValue, actual.Value);
    }

    [Fact]
    public void TimeOnly_Reasonable()
    {
        TimeOnly value = new(13, 37, 42, 123);
        Dictionary<string, object?> data = GetTestData(value);
        TimeOnlyTest actual = Invoker<TimeOnlyTest>.Invoke(data);
        Assert.Equal(value, actual.Value);
    }

    [Fact]
    public void NullableTimeOnly_Min()
    {
        Dictionary<string, object?> data = GetTestData(TimeOnly.MinValue);
        NullableTimeOnlyTest actual = Invoker<NullableTimeOnlyTest>.Invoke(data);
        Assert.Equal(TimeOnly.MinValue, actual.Value);
    }

    [Fact]
    public void NullableTimeOnly_Max()
    {
        Dictionary<string, object?> data = GetTestData(TimeOnly.MaxValue);
        NullableTimeOnlyTest actual = Invoker<NullableTimeOnlyTest>.Invoke(data);
        Assert.Equal(TimeOnly.MaxValue, actual.Value);
    }

    [Fact]
    public void NullableTimeOnly_Null()
    {
        Dictionary<string, object?> data = GetTestData(null);
        NullableTimeOnlyTest actual = Invoker<NullableTimeOnlyTest>.Invoke(data);
        Assert.Null(actual.Value);
    }

    [Fact]
    public void TimeSpan_Min()
    {
        Dictionary<string, object?> data = GetTestData(TimeSpan.MinValue);
        TimeSpanTest actual = Invoker<TimeSpanTest>.Invoke(data);
        Assert.Equal(TimeSpan.MinValue, actual.Value);
    }

    [Fact]
    public void TimeSpan_Max()
    {
        Dictionary<string, object?> data = GetTestData(TimeSpan.MaxValue);
        TimeSpanTest actual = Invoker<TimeSpanTest>.Invoke(data);
        Assert.Equal(TimeSpan.MaxValue, actual.Value);
    }

    [Fact]
    public void TimeSpan_Reasonable()
    {
        TimeSpan time = new(1, 2, 3, 4, 500);
        Dictionary<string, object?> data = GetTestData(time);
        TimeSpanTest actual = Invoker<TimeSpanTest>.Invoke(data);
        Assert.Equal(time, actual.Value);
    }

    [Fact]
    public void NullableTimeSpan_Min()
    {
        Dictionary<string, object?> data = GetTestData(TimeSpan.MinValue);
        NullableTimeSpanTest actual = Invoker<NullableTimeSpanTest>.Invoke(data);
        Assert.Equal(TimeSpan.MinValue, actual.Value);
    }

    [Fact]
    public void NullableTimeSpan_Max()
    {
        Dictionary<string, object?> data = GetTestData(TimeSpan.MaxValue);
        NullableTimeSpanTest actual = Invoker<NullableTimeSpanTest>.Invoke(data);
        Assert.Equal(TimeSpan.MaxValue, actual.Value);
    }

    [Fact]
    public void NullableTimeSpan_Null()
    {
        Dictionary<string, object?> data = GetTestData(null);
        NullableTimeSpanTest actual = Invoker<NullableTimeSpanTest>.Invoke(data);
        Assert.Null(actual.Value);
    }
}
