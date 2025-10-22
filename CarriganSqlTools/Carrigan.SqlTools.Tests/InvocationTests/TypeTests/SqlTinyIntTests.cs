using Carrigan.SqlTools.Invocation;
using Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

namespace Carrigan.SqlTools.Tests.InvocationTests.TypeTests;

public class SqlTinyIntTests
{
    private static Dictionary<string, object?> GetTestData(object? value)
    {
        Dictionary<string, object?> data = [];
        data["Value"] = value;
        return data;
    }
    [Fact]
    public void Byte_Min()
    {
        Dictionary<string, object?> data = GetTestData(byte.MinValue);
        ByteTest actual = Invoker<ByteTest>.Invoke(data);
        Assert.Equal(byte.MinValue, actual.Value);
    }

    [Fact]
    public void Byte_Max()
    {
        Dictionary<string, object?> data = GetTestData(byte.MaxValue);
        ByteTest actual = Invoker<ByteTest>.Invoke(data);
        Assert.Equal(byte.MaxValue, actual.Value);
    }

    [Fact]
    public void Byte_Reasonable()
    {
        Dictionary<string, object?> data = GetTestData((byte)200);
        ByteTest actual = Invoker<ByteTest>.Invoke(data);
        Assert.Equal((byte)200, actual.Value);
    }

    [Fact]
    public void NullableByte_Min()
    {
        Dictionary<string, object?> data = GetTestData(byte.MinValue);
        NullableByteTest actual = Invoker<NullableByteTest>.Invoke(data);
        Assert.Equal(byte.MinValue, actual.Value);
    }

    [Fact]
    public void NullableByte_Max()
    {
        Dictionary<string, object?> data = GetTestData(byte.MaxValue);
        NullableByteTest actual = Invoker<NullableByteTest>.Invoke(data);
        Assert.Equal(byte.MaxValue, actual.Value);
    }

    [Fact]
    public void NullableByte_Null()
    {
        Dictionary<string, object?> data = GetTestData(null);
        NullableByteTest actual = Invoker<NullableByteTest>.Invoke(data);
        Assert.Null(actual.Value);
    }
}
