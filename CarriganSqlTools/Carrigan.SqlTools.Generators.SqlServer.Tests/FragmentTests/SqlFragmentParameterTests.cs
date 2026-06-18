using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.FragmentTests;

public class SqlFragmentParameterTests
{
    private static readonly SqlServerDialect Dialect = new();

    [Fact]
    public void ToSql_DelegatesToParameter()
    {
        Parameter parameter = new(123, "@Name");
        SqlFragmentParameter fragment = new(parameter);

        string expectedValue = parameter.ToSql();
        string actualValue = fragment.ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void SqlFragmentParameter_ToSql_DoesNotDuplicateAtSign()
    {
        SqlFragmentParameter fragment = new(new Parameter("Jonathan", "@Name"));

        string sql = fragment.ToSql(Dialect);

        Assert.Equal("@Name", sql);
    }
}