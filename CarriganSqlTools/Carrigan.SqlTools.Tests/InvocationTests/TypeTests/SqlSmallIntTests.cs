using Carrigan.SqlTools.Invocation;
using Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

namespace Carrigan.SqlTools.Tests.InvocationTests.TypeTests;

public class SqlSmallIntTests
{
    private static Dictionary<string, object?> GetTestData(object? value)
    {
        Dictionary<string, object?> data = [];
        data["Value"] = value;
        return data;
    }
    [Fact]
    public void Short_Min()
    {
        Dictionary<string, object?> data = GetTestData(short.MinValue);
        ShortTest actual = Invoker<ShortTest>.Invoke(data);
        Assert.Equal(short.MinValue, actual.Value);
    }

    [Fact]
    public void Short_Max()
    {
        Dictionary<string, object?> data = GetTestData(short.MaxValue);
        ShortTest actual = Invoker<ShortTest>.Invoke(data);
        Assert.Equal(short.MaxValue, actual.Value);
    }

    [Fact]
    public void Short_Reasonable()
    {
        Dictionary<string, object?> data = GetTestData((short)42);
        ShortTest actual = Invoker<ShortTest>.Invoke(data);
        Assert.Equal((short)42, actual.Value);
    }

    [Fact]
    public void NullableShort_Min()
    {
        Dictionary<string, object?> data = GetTestData(short.MinValue);
        NullableShortTest actual = Invoker<NullableShortTest>.Invoke(data);
        Assert.Equal(short.MinValue, actual.Value);
    }

    [Fact]
    public void NullableShort_Max()
    {
        Dictionary<string, object?> data = GetTestData(short.MaxValue);
        NullableShortTest actual = Invoker<NullableShortTest>.Invoke(data);
        Assert.Equal(short.MaxValue, actual.Value);
    }

    [Fact]
    public void NullableShort_Null()
    {
        Dictionary<string, object?> data = GetTestData(null);
        NullableShortTest actual = Invoker<NullableShortTest>.Invoke(data);
        Assert.Null(actual.Value);
    }
}
