using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Tests.FragmentTests;

public class SqlFragmentParameterTests
{
    [Fact]
    public void Constructor_NullParameter_Exception() => 
        Assert.Throws<ArgumentNullException>(() => new SqlFragmentParameter(null!));

    [Fact]
    public void Constructor_StoresSameInstance()
    {
        Parameter parameter = new("@Name", 123);

        SqlFragmentParameter fragment = new(parameter);

        Assert.Same(parameter, fragment.Parameter);
    }

    [Fact]
    public void ToSql_DelegatesToParameter()
    {
        Parameter parameter = new("@Name", 123);
        SqlFragmentParameter fragment = new(parameter);

        string expectedValue = parameter.ToSql();
        string actualValue = fragment.ToSql();

        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void GetParameters_WithNullParameterValue_ReturnsDBNullValue()
    {
        Parameter parameter = new("Name", null);

        IEnumerable<SqlFragment> fragments =
        [
            new SqlFragmentParameter(parameter)
        ];

        Dictionary<ParameterTag, object> parameters = fragments.GetParameters(new SqlServerDialect());

        Assert.Single(parameters);
        Assert.Same(DBNull.Value, parameters[new Parameter ("@Name_1", null).Name]);
    }

    [Fact]
    public void SqlFragmentParameter_ToSql_DoesNotDuplicateAtSign()
    {
        SqlFragmentParameter fragment = new(new Parameter("@Name", "Jonathan"));

        string sql = fragment.ToSql();

        Assert.Equal("@Name", sql);
    }

    [Fact]
    public void SqlFragmentParameter_GetParameters_ReturnsWrappedParameter()
    {
        Parameter parameter = new("Name", "Jonathan");
        SqlFragmentParameter fragment = new(parameter);

        Parameter actual = Assert.Single(fragment.GetParameters());

        Assert.Same(parameter, actual);
    }
}
