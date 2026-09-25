using Carrigan.SqlTools.Base.Tests.Helpers;
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

        SqlQueryTestHelper.AssertParameterValue(query, "@p1_1", 1);
        SqlQueryTestHelper.AssertParameterValue(query, "@p2_2", 2);

    }
    [Fact]
    public void Constructor_MaterializesFragments()
    {
        List<ISqlFragment> source = [new SqlFragmentText("SELECT 1;")];
        SqlQuery query = new(Dialect, CommandType.Text, source);

        source[0] = new SqlFragmentText("SELECT 2;");
        source.Add(new SqlFragmentText(" SELECT 3;"));

        Assert.Equal("SELECT 1;", query.QueryText);
    }

    [Fact]
    public void SqlFragments_Setter_MaterializesFragments()
    {
        SqlQuery query = new(Dialect, CommandType.Text, [new SqlFragmentText("SELECT 1;")]);
        List<ISqlFragment> source = [new SqlFragmentText("SELECT 2;")];

        query.SqlFragments = source;
        source[0] = new SqlFragmentText("SELECT 3;");

        Assert.Equal("SELECT 2;", query.QueryText);
    }

}
