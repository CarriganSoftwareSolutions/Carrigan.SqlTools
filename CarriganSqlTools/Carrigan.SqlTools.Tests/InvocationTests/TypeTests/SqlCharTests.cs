using Carrigan.SqlTools.Invocation;
using Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

namespace Carrigan.SqlTools.Tests.InvocationTests.TypeTests;

public class SqlCharTests
{
    private static Dictionary<string, object?> GetTestData(object? value)
    {
        Dictionary<string, object?> data = [];
        data["Value"] = value;
        return data;
    }
    [Fact]
    public void Char_Min()
    {
        Dictionary<string, object?> data = GetTestData(char.MinValue);
        CharTest actual = Invoker<CharTest>.Invoke(data);
        Assert.Equal(char.MinValue, actual.Value);
    }

    [Fact]
    public void Char_Max()
    {
        Dictionary<string, object?> data = GetTestData(char.MaxValue);
        CharTest actual = Invoker<CharTest>.Invoke(data);
        Assert.Equal(char.MaxValue, actual.Value);
    }

    [Fact]
    public void Char_Reasonable()
    {
        Dictionary<string, object?> data = GetTestData('Y');
        CharTest actual = Invoker<CharTest>.Invoke(data);
        Assert.Equal('Y', actual.Value);
    }

    [Fact]
    public void NullableChar_Min()
    {
        Dictionary<string, object?> data = GetTestData(char.MinValue);
        NullableCharTest actual = Invoker<NullableCharTest>.Invoke(data);
        Assert.Equal(char.MinValue, actual.Value);
    }

    [Fact]
    public void NullableChar_Max()
    {
        Dictionary<string, object?> data = GetTestData(char.MaxValue);
        NullableCharTest actual = Invoker<NullableCharTest>.Invoke(data);
        Assert.Equal(char.MaxValue, actual.Value);
    }

    [Fact]
    public void NullableChar_Null()
    {
        Dictionary<string, object?> data = GetTestData(null);
        NullableCharTest actual = Invoker<NullableCharTest>.Invoke(data);
        Assert.Null(actual.Value);
    }

    [Fact]
    public void Char_FromString_FirstCharacter()
    {
        Dictionary<string, object?> data = GetTestData("Y");
        CharTest actual = Invoker<CharTest>.Invoke(data);
        Assert.Equal('Y', actual.Value);
    }

    [Fact]
    public void NullableChar_FromString_FirstCharacter()
    {
        Dictionary<string, object?> data = GetTestData("Y");
        NullableCharTest actual = Invoker<NullableCharTest>.Invoke(data);
        Assert.Equal('Y', actual.Value);
    }
}
