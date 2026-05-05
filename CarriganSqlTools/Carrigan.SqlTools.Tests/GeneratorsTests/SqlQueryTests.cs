using Carrigan.SqlTools.Dialects.SqlServer;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;
using System.Data;
using Carrigan.SqlTools.Tests.Helpers;
using Carrigan.SqlTools.Fragments;

namespace Carrigan.SqlTools.Tests.GeneratorsTests;

public sealed class SqlQueryTests
{
    [Fact]
    public void InternalConstructor()
    {
        SqlServerQuery query = new(new SqlServerDialect(), []);

        Assert.Equal(string.Empty, query.QueryText);
        Assert.NotNull(query.Parameters);
        SqlQueryTestHelper.AssertParameterCount(query, 0);
        Assert.Equal(CommandType.Text, query.CommandType);
    }

    [Fact]
    public void Constructor_NullDialect_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new SqlServerQuery(null!, []));

    [Fact]
    public void Constructor_NullFragments_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new SqlServerQuery(new SqlServerDialect(), null!));

    [Fact]
    public void GetParameterCount()
    {
        List<SqlFragment> sql =
        [
            new SqlFragmentText("SELECT "),
            new SqlFragmentParameter(new ParameterTag("@p1"), 1),
            new SqlFragmentText(" "),
            new SqlFragmentParameter(new ParameterTag("@p1"), 1)
        ];

        SqlQuery query = new SqlServerQuery(new SqlServerDialect(), sql);

        Assert.Equal(2, query.GetParameterCount());
    }

    [Fact]
    public void GetParameterValue()
    {
        List<SqlFragment> sql =
        [
            new SqlFragmentText("SELECT "),
            new SqlFragmentParameter(new ParameterTag("@p1"), 1),
            new SqlFragmentText(" "),
            new SqlFragmentParameter(new ParameterTag("@p2"), 2)
        ];

        SqlQuery query = new SqlServerQuery(new SqlServerDialect(), sql);

        Assert.Equal(2, query.GetParameterCount());

        Assert.Equal(1, (int?)query.GetParameterValue("@p1_1"));
        Assert.Equal(2, (int?)query.GetParameterValue("@p2_2"));
    }

    [Fact]
    public void GetParameterValue_NotFound_Exception()
    {
        List<SqlFragment> sql =
        [
            new SqlFragmentText("SELECT "),
            new SqlFragmentParameter(new ParameterTag("@p1"), 1),
            new SqlFragmentText(" "),
            new SqlFragmentParameter(new ParameterTag("@p2"), 2)
        ];
        SqlQuery query = new SqlServerQuery(new SqlServerDialect(), sql);

        Assert.Throws<KeyNotFoundException>(() => (int?)query.GetParameterValue("@missing"));
    }
}
