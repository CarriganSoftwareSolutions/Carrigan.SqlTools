using Carrigan.SqlTools.Invocation;
using Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

namespace Carrigan.SqlTools.Tests.InvocationTests.TypeTests;

public class SqlBitTests
{
    private static Dictionary<string, object?> GetTestData(object? value)
    {
        Dictionary<string, object?> data = [];
        data["Value"] = value;
        return data;
    }
    [Fact]
    public void Bool_False()
    {
        Dictionary<string, object?> data = GetTestData(false);
        BoolTest actual = Invoker<BoolTest>.Invoke(data);
        Assert.False(actual.Value);
    }

    [Fact]
    public void Bool_True()
    {
        Dictionary<string, object?> data = GetTestData(true);
        BoolTest actual = Invoker<BoolTest>.Invoke(data);
        Assert.True(actual.Value);
    }

    [Fact]
    public void NullableBool_False()
    {
        Dictionary<string, object?> data = GetTestData(false);
        NullableBoolTest actual = Invoker<NullableBoolTest>.Invoke(data);
        Assert.False(actual.Value);
    }

    [Fact]
    public void NullableBool_True()
    {
        Dictionary<string, object?> data = GetTestData(true);
        NullableBoolTest actual = Invoker<NullableBoolTest>.Invoke(data);
        Assert.True(actual.Value);
    }

    [Fact]
    public void NullableBool_Null()
    {
        Dictionary<string, object?> data = GetTestData(null);
        NullableBoolTest actual = Invoker<NullableBoolTest>.Invoke(data);
        Assert.Null(actual.Value);
    }
}
