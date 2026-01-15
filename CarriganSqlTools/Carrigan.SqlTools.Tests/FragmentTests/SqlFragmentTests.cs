using Carrigan.SqlTools.Fragments;

namespace Carrigan.SqlTools.Tests.FragmentTests;

public class SqlFragmentTests
{
    [Fact]
    public void ToString_DelegatesToToSql()
    {
        SqlFragment fragment = new TestFragment("SELECT 1");

        string actual = fragment.ToString();

        Assert.Equal("SELECT 1", actual);
    }

    private sealed class TestFragment : SqlFragment
    {
        private readonly string _sql;

        public TestFragment(string sql) => 
            _sql = sql;

        internal override string ToSql() =>
            _sql;
    }
}
