using Carrigan.SqlTools.Invocation;
using Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

namespace Carrigan.SqlTools.Tests.InvocationTests.TypeTests;

public class SqlBinaryTests
{
    private static Dictionary<string, object?> GetTestData(object? value)
    {
        Dictionary<string, object?> data = [];
        data["Value"] = value;
        return data;
    }
    [Fact]
    public void ByteArray_Empty()
    {
        Dictionary<string, object?> data = GetTestData(Array.Empty<byte>());
        ByteArrayTest actual = Invoker<ByteArrayTest>.Invoke(data);
        Assert.NotNull(actual.Value);
        Assert.Empty(actual.Value);
    }
    [Fact]
    public void ByteArray_Null()
    {
        Dictionary<string, object?> data = GetTestData(null);
        ByteArrayTest actual = Invoker<ByteArrayTest>.Invoke(data);
        Assert.NotNull(actual.Value);
        Assert.Empty(actual.Value);
    }

    [Fact]
    public void ByteArray_Reasonable()
    {
        byte[] bytes = [ 0x00, 0x01, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFE, 0xFF ];
        Dictionary<string, object?> data = GetTestData(bytes);
        ByteArrayTest actual = Invoker<ByteArrayTest>.Invoke(data);
        Assert.Equal(bytes, actual.Value);
    }

    [Fact]
    public void NullableByteArray_Empty()
    {
        Dictionary<string, object?> data = GetTestData(Array.Empty<byte>());
        NullableByteArrayTest actual = Invoker<NullableByteArrayTest>.Invoke(data);
        Assert.NotNull(actual.Value);
        Assert.Empty(actual.Value);
    }
    [Fact]
    public void NullableByteArray_Null()
    {
        Dictionary<string, object?> data = GetTestData(null);
        NullableByteArrayTest actual = Invoker<NullableByteArrayTest>.Invoke(data);
        Assert.Null(actual.Value);
    }

    [Fact]
    public void NullableByteArray_Reasonable()
    {
        byte[] bytes = [0x00, 0x01, 0xFE, 0xFF];
        Dictionary<string, object?> data = GetTestData(bytes);
        NullableByteArrayTest actual = Invoker<NullableByteArrayTest>.Invoke(data);
        Assert.Equal(bytes, actual.Value);
    }
}
