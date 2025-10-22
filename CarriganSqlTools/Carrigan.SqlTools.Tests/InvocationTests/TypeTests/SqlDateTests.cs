using Carrigan.SqlTools.Invocation;
using Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

namespace Carrigan.SqlTools.Tests.InvocationTests.TypeTests;

public class SqlDateTests
{
    private static Dictionary<string, object?> GetTestData(object? value)
    {
        Dictionary<string, object?> data = [];
        data["Value"] = value;
        return data;
    }
    [Fact]
    public void DateOnly_Min()
    {
        Dictionary<string, object?> data = GetTestData(DateOnly.MinValue);
        DateOnlyTest actual = Invoker<DateOnlyTest>.Invoke(data);
        Assert.Equal(DateOnly.MinValue, actual.Value);
    }

    [Fact]
    public void DateOnly_Max()
    {
        Dictionary<string, object?> data = GetTestData(DateOnly.MaxValue);
        DateOnlyTest actual = Invoker<DateOnlyTest>.Invoke(data);
        Assert.Equal(DateOnly.MaxValue, actual.Value);
    }

    [Fact]
    public void DateOnly_Reasonable()
    {
        DateOnly date = new(1969, 7, 20);
        Dictionary<string, object?> data = GetTestData(date);
        DateOnlyTest actual = Invoker<DateOnlyTest>.Invoke(data);
        Assert.Equal(date, actual.Value);
    }

    [Fact]
    public void NullableDateOnly_Min()
    {
        Dictionary<string, object?> data = GetTestData(DateOnly.MinValue);
        NullableDateOnlyTest actual = Invoker<NullableDateOnlyTest>.Invoke(data);
        Assert.Equal(DateOnly.MinValue, actual.Value);
    }

    [Fact]
    public void NullableDateOnly_Max()
    {
        Dictionary<string, object?> data = GetTestData(DateOnly.MaxValue);
        NullableDateOnlyTest actual = Invoker<NullableDateOnlyTest>.Invoke(data);
        Assert.Equal(DateOnly.MaxValue, actual.Value);
    }

    [Fact]
    public void NullableDateOnly_Null()
    {
        Dictionary<string, object?> data = GetTestData(null);
        NullableDateOnlyTest actual = Invoker<NullableDateOnlyTest>.Invoke(data);
        Assert.Null(actual.Value);
    }
}
