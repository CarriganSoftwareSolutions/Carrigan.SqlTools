using Carrigan.SqlTools.Invocation;
using Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;
using System;

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
    public void TimeOnly_TimeSpanMin()
    {
        Dictionary<string, object?> data = GetTestData(new TimeSpan(TimeOnly.MinValue.Ticks));
        TimeOnlyTest actual = Invoker<TimeOnlyTest>.Invoke(data);
        Assert.Equal(TimeOnly.FromTimeSpan(new TimeSpan(TimeOnly.MinValue.Ticks)), actual.Value);
    }

    [Fact]
    public void TimeOnly_TimeSpanMax()
    {
        Dictionary<string, object?> data = GetTestData(new TimeSpan(TimeOnly.MaxValue.Ticks));
        TimeOnlyTest actual = Invoker<TimeOnlyTest>.Invoke(data);
        Assert.Equal(TimeOnly.FromTimeSpan(new TimeSpan(TimeOnly.MaxValue.Ticks)), actual.Value);
    }

    [Fact]
    public void NullableTimeOnly_TimeSpanMin()
    {
        Dictionary<string, object?> data = GetTestData(new TimeSpan(TimeOnly.MinValue.Ticks));
        NullableTimeOnlyTest actual = Invoker<NullableTimeOnlyTest>.Invoke(data);
        Assert.Equal(TimeOnly.FromTimeSpan(new TimeSpan(TimeOnly.MinValue.Ticks)), actual.Value);
    }

    [Fact]
    public void NullableTimeOnly_TimeSpanMax()
    {
        Dictionary<string, object?> data = GetTestData(new TimeSpan(TimeOnly.MaxValue.Ticks));
        NullableTimeOnlyTest actual = Invoker<NullableTimeOnlyTest>.Invoke(data);
        Assert.Equal(TimeOnly.FromTimeSpan(new TimeSpan(TimeOnly.MaxValue.Ticks)), actual.Value);
    }

    [Fact]
    public void NullableTimeOnly_TimeSpaceNull()
    {
        TimeSpan? value = null;
        Dictionary<string, object?> data = GetTestData(value);
        NullableTimeOnlyTest actual = Invoker<NullableTimeOnlyTest>.Invoke(data);
        Assert.Null(actual.Value);
    }
}
