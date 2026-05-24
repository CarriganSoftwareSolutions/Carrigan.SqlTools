using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Base.Tests.Helpers;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.FragmentTests;

public class SqlFragmentTests
{
    private static readonly SqlServerDialect Dialect = new();

    [Fact]
    public void ToString_DelegatesToToSql()
    {
        ISqlFragment fragment = new TestFragment("SELECT 1");

        string actual = fragment.ToSql(Dialect);

        Assert.Equal("SELECT 1", actual);
    }

    private sealed class TestFragment : ISqlFragment
    {
        private readonly string _sql;

        public TestFragment(string sql) =>
            _sql = sql;

        public string ToSql(ISqlDialects dialect) =>
            _sql;

        public IEnumerable<SqlFragmentParameter> GetSqlFragmentParameters() =>
            [];

        public IEnumerable<ISqlFragment> Flatten() => [this];
    }

    [Fact]
    public void GetParameters_WithMixedFragments_ReturnsOnlyParameters()
    {
        Parameter parameter1 = new("Name", "Jonathan");
        Parameter parameter2 = new("Age", 42);

        IEnumerable<ISqlFragment> fragments =
        [
            new SqlFragmentText("WHERE Name = "),
            new SqlFragmentParameter(parameter1),
            new SqlFragmentText(" AND Age = "),
            new SqlFragmentParameter(parameter2)
        ];

        IEnumerable<SqlFragmentParameter> parameters = fragments.GetSqlFragmentParameters(Dialect);

        Assert.Equal(2, parameters.Count());
        SqlQueryTestHelper.AssertParameterValue(parameters, "@Name_1", "Jonathan");
        SqlQueryTestHelper.AssertParameterValue(parameters, "@Age_2", 42);
    }
}