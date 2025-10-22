using Carrigan.SqlTools.Invocation;
using Carrigan.SqlTools.OrderByItems;
using Carrigan.SqlTools.Tests.TestEntities.NarrowTypes;

namespace Carrigan.SqlTools.Tests.InvocationTests.TypeTests;

public class EnumInvokerTests
{
    private static Dictionary<string, object?> GetTestData(object? value)
    {
        Dictionary<string, object?> data = [];
        data["Value"] = value;
        return data;
    }
    [Fact]
    public void Bool_Zero()
    {
        Dictionary<string, object?> data = GetTestData(SortDirectionEnum.Ascending);
        EnumTests actual = Invoker<EnumTests>.Invoke(data);
        Assert.Equal(SortDirectionEnum.Ascending, actual.Value);
    }
    [Fact]
    public void Bool_One()
    {
        Dictionary<string, object?> data = GetTestData(SortDirectionEnum.Descending);
        EnumTests actual = Invoker<EnumTests>.Invoke(data);
        Assert.Equal(SortDirectionEnum.Descending, actual.Value);
    }
}
