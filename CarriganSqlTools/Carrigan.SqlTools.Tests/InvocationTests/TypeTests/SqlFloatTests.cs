using Carrigan.SqlTools.Invocation;
using Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

namespace Carrigan.SqlTools.Tests.InvocationTests.TypeTests;

public class SqlFloatTests
{
    private static Dictionary<string, object?> GetTestData(object? value)
    {
        Dictionary<string, object?> data = [];
        data["Value"] = value;
        return data;
    }
    [Fact]
    public void Double_Min()
    {
        Dictionary<string, object?> data = GetTestData(double.MinValue);
        DoubleTest actual = Invoker<DoubleTest>.Invoke(data);
        Assert.Equal(double.MinValue, actual.Value);
    }

    [Fact]
    public void Double_Max()
    {
        Dictionary<string, object?> data = GetTestData(double.MaxValue);
        DoubleTest actual = Invoker<DoubleTest>.Invoke(data);
        Assert.Equal(double.MaxValue, actual.Value);
    }

    [Fact]
    public void Double_Reasonable()
    {
        Dictionary<string, object?> data = GetTestData(0.5772d);
        DoubleTest actual = Invoker<DoubleTest>.Invoke(data);
        Assert.Equal(0.5772d, actual.Value);
    }

    [Fact]
    public void NullableDouble_Min()
    {
        Dictionary<string, object?> data = GetTestData(double.MinValue);
        NullableDoubleTest actual = Invoker<NullableDoubleTest>.Invoke(data);
        Assert.Equal(double.MinValue, actual.Value);
    }

    [Fact]
    public void NullableDouble_Max()
    {
        Dictionary<string, object?> data = GetTestData(double.MaxValue);
        NullableDoubleTest actual = Invoker<NullableDoubleTest>.Invoke(data);
        Assert.Equal(double.MaxValue, actual.Value);
    }

    [Fact]
    public void NullableDouble_Null()
    {
        Dictionary<string, object?> data = GetTestData(null);
        NullableDoubleTest actual = Invoker<NullableDoubleTest>.Invoke(data);
        Assert.Null(actual.Value);
    }
}
