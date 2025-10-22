using Carrigan.SqlTools.Invocation;
using Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

namespace Carrigan.SqlTools.Tests.InvocationTests.TypeTests;

public class SqlBigIntTests
{
    private static Dictionary<string, object?> GetTestData(object? value)
    {
        Dictionary<string, object?> data = [];
        data["Value"] = value;
        return data;
    }

    [Fact]
    public void Long_Min()
    {
        Dictionary<string, object?> data = GetTestData(long.MinValue);
        LongTest actual = Invoker<LongTest>.Invoke(data);
        Assert.Equal(long.MinValue, actual.Value);
    }

    [Fact]
    public void Long_Max()
    {
        Dictionary<string, object?> data = GetTestData(long.MaxValue);
        LongTest actual = Invoker<LongTest>.Invoke(data);
        Assert.Equal(long.MaxValue, actual.Value);
    }

    [Fact]
    public void Long_Reasonable()
    {
        Dictionary<string, object?> data = GetTestData(123456789L);
        LongTest actual = Invoker<LongTest>.Invoke(data);
        Assert.Equal(123456789L, actual.Value);
    }

    [Fact]
    public void NullableLong_Min()
    {
        Dictionary<string, object?> data = GetTestData(long.MinValue);
        NullableLongTest actual = Invoker<NullableLongTest>.Invoke(data);
        Assert.Equal(long.MinValue, actual.Value);
    }

    [Fact]
    public void NullableLong_Max()
    {
        Dictionary<string, object?> data = GetTestData(long.MaxValue);
        NullableLongTest actual = Invoker<NullableLongTest>.Invoke(data);
        Assert.Equal(long.MaxValue, actual.Value);
    }

    [Fact]
    public void NullableLong_Null()
    {
        Dictionary<string, object?> data = GetTestData(null);
        NullableLongTest actual = Invoker<NullableLongTest>.Invoke(data);
        Assert.Null(actual.Value);
    }
}
