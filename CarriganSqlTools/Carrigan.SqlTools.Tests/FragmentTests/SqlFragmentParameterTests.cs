using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Tests.FragmentTests;

public class SqlFragmentParameterTests
{
    [Fact]
    public void Constructor_NullParameter_Exception() => 
        Assert.Throws<ArgumentNullException>(() => new SqlFragmentParameter(null!));

    [Fact]
    public void Constructor_StoresSameInstance()
    {
        Parameter parameter = new("Name", 123);

        SqlFragmentParameter fragment = new(parameter);

        Assert.Same(parameter, fragment.Parameter);
    }

    [Fact]
    public void ToSql_DelegatesToParameter()
    {
        Parameter parameter = new("Name", 123);
        SqlFragmentParameter fragment = new(parameter);

        string expectedValue = parameter.ToSql();
        string actualValue = fragment.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }
}
