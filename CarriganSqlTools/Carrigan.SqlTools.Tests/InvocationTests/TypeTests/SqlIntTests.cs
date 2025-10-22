using Carrigan.SqlTools.Invocation;
using Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

namespace Carrigan.SqlTools.Tests.InvocationTests.TypeTests;

public class SqlIntTests
{
    private static Dictionary<string, object?> GetTestData(object? value)
    {
        Dictionary<string, object?> data = [];
        data["Value"] = value;
        return data;
    }
    [Fact]
    public void Int_Min()
    {
        Dictionary<string, object?> data = GetTestData(int.MinValue);
        IntTest actual = Invoker<IntTest>.Invoke(data);
        Assert.Equal(int.MinValue, actual.Value);
    }

    [Fact]
    public void Int_Max()
    {
        Dictionary<string, object?> data = GetTestData(int.MaxValue);
        IntTest actual = Invoker<IntTest>.Invoke(data);
        Assert.Equal(int.MaxValue, actual.Value);
    }

    [Fact]
    public void Int_Reasonable()
    {
        Dictionary<string, object?> data = GetTestData(1776);
        IntTest actual = Invoker<IntTest>.Invoke(data);
        Assert.Equal(1776, actual.Value);
    }

    [Fact]
    public void NullableInt_Min()
    {
        Dictionary<string, object?> data = GetTestData(int.MinValue);
        NullableIntTest actual = Invoker<NullableIntTest>.Invoke(data);
        Assert.Equal(int.MinValue, actual.Value);
    }

    [Fact]
    public void NullableInt_Max()
    {
        Dictionary<string, object?> data = GetTestData(int.MaxValue);
        NullableIntTest actual = Invoker<NullableIntTest>.Invoke(data);
        Assert.Equal(int.MaxValue, actual.Value);
    }

    [Fact]
    public void NullableInt_Null()
    {
        Dictionary<string, object?> data = GetTestData(null);
        NullableIntTest actual = Invoker<NullableIntTest>.Invoke(data);
        Assert.Null(actual.Value);
    }
}
