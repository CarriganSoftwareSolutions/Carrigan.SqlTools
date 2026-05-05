using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tests.Helpers;

namespace Carrigan.SqlTools.Tests.FragmentTests;

public class SqlFragmentTests
{
    [Fact]
    public void ToString_DelegatesToToSql()
    {
        SqlFragment fragment = new TestFragment("SELECT 1");

        string actual = fragment.ToSql();

        Assert.Equal("SELECT 1", actual);
    }

    private sealed class TestFragment : SqlFragment
    {
        private readonly string _sql;

        public TestFragment(string sql) =>
            _sql = sql;

        internal override string ToSql() =>
            _sql;

        internal override IEnumerable<SqlFragmentParameter> GetSqlFragmentParameters() =>
            [];

        internal override IEnumerable<SqlFragment> Flaten() => [this];
    }

    [Fact]
    public void GetParameters_WithMixedFragments_ReturnsOnlyParameters()
    {
        Parameter parameter1 = new("Name", "Jonathan");
        Parameter parameter2 = new("Age", 42);

        IEnumerable<SqlFragment> fragments =
        [
            new SqlFragmentText("WHERE Name = "),
            new SqlFragmentParameter(parameter1),
            new SqlFragmentText(" AND Age = "),
            new SqlFragmentParameter(parameter2)
        ];

        IEnumerable<SqlFragmentParameter> parameters = fragments.GetSqlFragmentParameters(new SqlServerDialect());

        Assert.Equal(2, parameters.Count());
        SqlQueryTestHelper.AssertParameterValue(parameters, "@Name_1", "Jonathan");
        SqlQueryTestHelper.AssertParameterValue(parameters, "@Age_2", 42);
    }
}