using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Tests.FragmentTests;

public class SqlFragmentParameterTests
{
    private static readonly SqlServerDialect Dialect = new();

    [Fact]
    public void Constructor_NullParameter_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new SqlFragmentParameter(null!));

    [Fact]
    public void ToSql_DelegatesToParameter()
    {
        Parameter parameter = new("@Name", 123);
        SqlFragmentParameter fragment = new(parameter);

        string expectedValue = parameter.ToSql();
        string actualValue = fragment.ToSql(Dialect);

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void GetParameters_WithNullParameterValue_ReturnsDBNullValue()
    {
        Parameter parameter = new("Name", null);

        IEnumerable<ISqlFragment> fragments =
        [
            new SqlFragmentParameter(parameter)
        ];

        IEnumerable<SqlFragmentParameter> parameters = fragments.GetSqlFragmentParameters(Dialect);

        SqlFragmentParameter actual = Assert.Single(parameters);

        Assert.Equal("@Name_1", actual.ParameterTag.ToString());
        Assert.Equal(null!, actual.Value); //value substitutions are now done late, in the SqlServerQuery. This should now be null instead of DBNull
    }

    [Fact]
    public void SqlFragmentParameter_ToSql_DoesNotDuplicateAtSign()
    {
        SqlFragmentParameter fragment = new(new Parameter("@Name", "Jonathan"));

        string sql = fragment.ToSql(Dialect);

        Assert.Equal("@Name", sql);
    }
}