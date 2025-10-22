using Carrigan.SqlTools.Invocation;
using Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

//IGNORE SPELLING: cdef
namespace Carrigan.SqlTools.Tests.InvocationTests.TypeTests;

public class SqlUniqueIdentifierTests
{
    private static Dictionary<string, object?> GetTestData(object? value)
    {
        Dictionary<string, object?> data = [];
        data["Value"] = value;
        return data;
    }
    [Fact]
    public void Guid_Empty()
    {
        Dictionary<string, object?> data = GetTestData(Guid.Empty);
        GuidTest actual = Invoker<GuidTest>.Invoke(data);
        Assert.Equal(Guid.Empty, actual.Value);
    }

    [Fact]
    public void Guid_Reasonable()
    {
        Guid value = Guid.Parse("01234567-89ab-cdef-0123-456789abcdef");
        Dictionary<string, object?> data = GetTestData(value);
        GuidTest actual = Invoker<GuidTest>.Invoke(data);
        Assert.Equal(value, actual.Value);
    }

    [Fact]
    public void Guid_Random()
    {
        Guid value = Guid.NewGuid();
        Dictionary<string, object?> data = GetTestData(value);
        GuidTest actual = Invoker<GuidTest>.Invoke(data);
        Assert.Equal(value, actual.Value);
    }

    [Fact]
    public void Guid_Max()
    {
        Guid value = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF");
        Dictionary<string, object?> data = GetTestData(value);
        GuidTest actual = Invoker<GuidTest>.Invoke(data);
        Assert.Equal(value, actual.Value);
    }

    [Fact]
    public void NullableGuid_Empty()
    {
        Dictionary<string, object?> data = GetTestData(Guid.Empty);
        NullableGuidTest actual = Invoker<NullableGuidTest>.Invoke(data);
        Assert.Equal(Guid.Empty, actual.Value);
    }

    [Fact]
    public void NullableGuid_Null()
    {
        Dictionary<string, object?> data = GetTestData(null);
        NullableGuidTest actual = Invoker<NullableGuidTest>.Invoke(data);
        Assert.Null(actual.Value);
    }

    [Fact]
    public void NullableGuid_Random()
    {
        Guid value = Guid.NewGuid();
        Dictionary<string, object?> data = GetTestData(value);
        NullableGuidTest actual = Invoker<NullableGuidTest>.Invoke(data);
        Assert.Equal(value, actual.Value);
    }

    [Fact]
    public void NullableGuid_Max()
    {
        Guid value = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF");
        Dictionary<string, object?> data = GetTestData(value);
        NullableGuidTest actual = Invoker<NullableGuidTest>.Invoke(data);
        Assert.Equal(value, actual.Value);
    }
}
