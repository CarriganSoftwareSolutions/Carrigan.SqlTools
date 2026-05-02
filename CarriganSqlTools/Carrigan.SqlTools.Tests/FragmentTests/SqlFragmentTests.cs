using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;

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

        internal override IEnumerable<Parameter> GetParameters() =>
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

        Dictionary<ParameterTag, object> parameters = fragments.GetParameters(new SqlServerDialect());

        Assert.Equal(2, parameters.Count);
        Assert.Equal("Jonathan", parameters[new Parameter("@Name_1", "Jonathan").Name]);
        Assert.Equal(42, parameters[new Parameter("@Age_2", 42).Name]);
    }
}