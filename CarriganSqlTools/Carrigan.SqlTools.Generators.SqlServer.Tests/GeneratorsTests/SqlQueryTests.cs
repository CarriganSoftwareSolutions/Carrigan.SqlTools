using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Base.Tests.Helpers;
using System.Data;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.GeneratorsTests;

public sealed class SqlQueryTests
{
    private static readonly SqlServerDialect Dialect = new();

    [Fact]
    public void InternalConstructor()
    {
        SqlQuery query = new(Dialect, []);

        Assert.Equal(string.Empty, query.QueryText);
        Assert.NotNull(query.Parameters);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
        Assert.Equal(CommandType.Text, query.CommandType);
    }

    [Fact]
    public void Constructor_NullDialect_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new SqlQuery(null!, []));

    [Fact]
    public void Constructor_NullFragments_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new SqlQuery(Dialect, null!));

    [Fact]
    public void GetParameterCount()
    {
        List<ISqlFragment> sql =
        [
            new SqlFragmentText("SELECT "),
            new SqlFragmentParameter(new ParameterTag("@p1"), 1),
            new SqlFragmentText(" "),
            new SqlFragmentParameter(new ParameterTag("@p1"), 1)
        ];

        SqlQuery query = new(Dialect, sql);

        Assert.Equal(2, query.GetParameterCount());
    }

    [Fact]
    public void GetParameterValue()
    {
        List<ISqlFragment> sql =
        [
            new SqlFragmentText("SELECT "),
            new SqlFragmentParameter(new ParameterTag("@p1"), 1),
            new SqlFragmentText(" "),
            new SqlFragmentParameter(new ParameterTag("@p2"), 2)
        ];

        SqlQuery query = new(Dialect, sql);

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
            new SqlFragmentParameter(new ParameterTag("@p1"), 1),
            new SqlFragmentText(" "),
            new SqlFragmentParameter(new ParameterTag("@p2"), 2)
        ];
        SqlQuery query = new(Dialect, sql);

        Assert.Throws<KeyNotFoundException>(() => (int?)query.GetParameterValue("@missing"));
    }
}
