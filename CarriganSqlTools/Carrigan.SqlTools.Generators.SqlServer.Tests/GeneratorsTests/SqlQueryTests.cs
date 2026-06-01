using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using System.Data;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.GeneratorsTests;

public sealed class SqlQueryTests
{
    private static readonly SqlServerDialect Dialect = new();

    [Fact]
    public void Constructor_NullDialect_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new SqlQuery(null!, CommandType.Text, []));

    [Fact]
    public void Constructor_NullFragments_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new SqlQuery(Dialect, CommandType.Text, null!));

    [Fact]
    public void GetParameterCount()
    {
        List<ISqlFragment> sql =
        [
            new SqlFragmentText("SELECT "),
            new SqlFragmentParameter(new ParameterTag("@p1"), null, 1),
            new SqlFragmentText(" "),
            new SqlFragmentParameter(new ParameterTag("@p1"), null, 1)
        ];

        SqlQuery query = new(Dialect, CommandType.Text, sql);

        Assert.Equal(2, query.GetParameterCount());
    }

    [Fact]
    public void GetParameterValue()
    {
        List<ISqlFragment> sql =
        [
            new SqlFragmentText("SELECT "),
            new SqlFragmentParameter(new ParameterTag("@p1"), null, 1),
            new SqlFragmentText(" "),
            new SqlFragmentParameter(new ParameterTag("@p2"), null, 2)
        ];

        SqlQuery query = new(Dialect, CommandType.Text, sql);

        Assert.Equal(2, query.GetParameterCount());

        Assert.Equal(1, (int?)query.GetParameterValue("@p1_1"));
        Assert.Equal(2, (int?)query.GetParameterValue("@p2_2"));
    }

    [Fact]
    public void GetParameterValue_NotFound_Exception()
    {
        List<ISqlFragment> sql =
        [
            new SqlFragmentText("SELECT "),
            new SqlFragmentParameter(new ParameterTag("@p1"), null, 1),
            new SqlFragmentText(" "),
            new SqlFragmentParameter(new ParameterTag("@p2"), null, 2)
        ];
        SqlQuery query = new(Dialect, CommandType.Text, sql);

        Assert.Throws<KeyNotFoundException>(() => (int?)query.GetParameterValue("@missing"));
    }
}
