using Carrigan.SqlTools.Invocation;
using Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

namespace Carrigan.SqlTools.Tests.InvocationTests.TypeTests;

public class SqlRealTests
{
    private static Dictionary<string, object?> GetTestData(object? value)
    {
        Dictionary<string, object?> data = [];
        data["Value"] = value;
        return data;
    }
    [Fact]
    public void Float_Min()
    {
        Dictionary<string, object?> data = GetTestData(float.MinValue);
        FloatTest actual = Invoker<FloatTest>.Invoke(data);
        Assert.Equal(float.MinValue, actual.Value);
    }

    [Fact]
    public void Float_Max()
    {
        Dictionary<string, object?> data = GetTestData(float.MaxValue);
        FloatTest actual = Invoker<FloatTest>.Invoke(data);
        Assert.Equal(float.MaxValue, actual.Value);
    }

    [Fact]
    public void Float_Reasonable()
    {
        Dictionary<string, object?> data = GetTestData(2.7182f);
        FloatTest actual = Invoker<FloatTest>.Invoke(data);
        Assert.Equal(2.7182f, actual.Value);
    }

    [Fact]
    public void NullableFloat_Min()
    {
        Dictionary<string, object?> data = GetTestData(float.MinValue);
        NullableFloatTest actual = Invoker<NullableFloatTest>.Invoke(data);
        Assert.Equal(float.MinValue, actual.Value);
    }

    [Fact]
    public void NullableFloat_Max()
    {
        Dictionary<string, object?> data = GetTestData(float.MaxValue);
        NullableFloatTest actual = Invoker<NullableFloatTest>.Invoke(data);
        Assert.Equal(float.MaxValue, actual.Value);
    }

    [Fact]
    public void NullableFloat_Null()
    {
        Dictionary<string, object?> data = GetTestData(null);
        NullableFloatTest actual = Invoker<NullableFloatTest>.Invoke(data);
        Assert.Null(actual.Value);
    }
}
