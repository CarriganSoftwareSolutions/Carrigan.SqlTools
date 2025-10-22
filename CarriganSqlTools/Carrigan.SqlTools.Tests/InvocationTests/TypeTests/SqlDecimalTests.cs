using Carrigan.SqlTools.Invocation;
using Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

namespace Carrigan.SqlTools.Tests.InvocationTests.TypeTests;

public class SqlDecimalTests
{
    private static Dictionary<string, object?> GetTestData(object? value)
    {
        Dictionary<string, object?> data = [];
        data["Value"] = value;
        return data;
    }
    [Fact]
    public void Decimal_Min()
    {
        Dictionary<string, object?> data = GetTestData(decimal.MinValue);
        DecimalTest actual = Invoker<DecimalTest>.Invoke(data);
        Assert.Equal(decimal.MinValue, actual.Value);
    }

    [Fact]
    public void Decimal_Max()
    {
        Dictionary<string, object?> data = GetTestData(decimal.MaxValue);
        DecimalTest actual = Invoker<DecimalTest>.Invoke(data);
        Assert.Equal(decimal.MaxValue, actual.Value);
    }

    [Fact]
    public void Decimal_Reasonable()
    {
        Dictionary<string, object?> data = GetTestData(12345.6789m);
        DecimalTest actual = Invoker<DecimalTest>.Invoke(data);
        Assert.Equal(12345.6789m, actual.Value);
    }

    [Fact]
    public void NullableDecimal_Min()
    {
        Dictionary<string, object?> data = GetTestData(decimal.MinValue);
        NullableDecimalTest actual = Invoker<NullableDecimalTest>.Invoke(data);
        Assert.Equal(decimal.MinValue, actual.Value);
    }

    [Fact]
    public void NullableDecimal_Max()
    {
        Dictionary<string, object?> data = GetTestData(decimal.MaxValue);
        NullableDecimalTest actual = Invoker<NullableDecimalTest>.Invoke(data);
        Assert.Equal(decimal.MaxValue, actual.Value);
    }

    [Fact]
    public void NullableDecimal_Null()
    {
        Dictionary<string, object?> data = GetTestData(null);
        NullableDecimalTest actual = Invoker<NullableDecimalTest>.Invoke(data);
        Assert.Null(actual.Value);
    }
}
