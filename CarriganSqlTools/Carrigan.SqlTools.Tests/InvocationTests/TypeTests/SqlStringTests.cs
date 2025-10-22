using Carrigan.SqlTools.Invocation;
using Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

namespace Carrigan.SqlTools.Tests.InvocationTests.TypeTests;

public class SqlStringTests
{
    private static Dictionary<string, object?> GetTestData(object? value)
    {
        Dictionary<string, object?> data = [];
        data["Value"] = value;
        return data;
    }
    [Fact]
    public void String_Empty()
    {
        Dictionary<string, object?> data = GetTestData(string.Empty);
        StringTest actual = Invoker<StringTest>.Invoke(data);
        Assert.Equal(string.Empty, actual.Value);
    }

    [Fact]
    public void String_Null()
    {
        Dictionary<string, object?> data = GetTestData(null);
        StringTest actual = Invoker<StringTest>.Invoke(data);
        Assert.Equal(string.Empty, actual.Value);
    }

    [Fact]
    public void String_Reasonable()
    {
        Dictionary<string, object?> data = GetTestData("Hello World!");
        StringTest actual = Invoker<StringTest>.Invoke(data);
        Assert.Equal("Hello World!", actual.Value);
    }


    [Fact]
    public void NullableString_Empty()
    {
        Dictionary<string, object?> data = GetTestData(string.Empty);
        NullableStringTest actual = Invoker<NullableStringTest>.Invoke(data);
        Assert.Equal(string.Empty, actual.Value);
    }

    [Fact]
    public void NullableString_Null()
    {
        Dictionary<string, object?> data = GetTestData(null);
        NullableStringTest actual = Invoker<NullableStringTest>.Invoke(data);
        Assert.Null(actual.Value);
    }

    [Fact]
    public void NullableString_Reasonable()
    {
        Dictionary<string, object?> data = GetTestData("Hello World!");
        NullableStringTest actual = Invoker<NullableStringTest>.Invoke(data);
        Assert.Equal("Hello World!", actual.Value);
    }
}
