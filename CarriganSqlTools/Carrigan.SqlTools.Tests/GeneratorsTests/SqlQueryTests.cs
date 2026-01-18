using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;
using System.Data;

namespace Carrigan.SqlTools.Tests.GeneratorsTests;

public sealed class SqlQueryTests
{
    [Fact]
    public void InternalConstructor()
    {
        SqlQuery query = new();

        Assert.Equal(string.Empty, query.QueryText);
        Assert.NotNull(query.Parameters);
        Assert.Empty(query.Parameters);
        Assert.Equal(CommandType.Text, query.CommandType);
    }

    [Fact]
    public void Constructor_NullQuery_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new SqlQuery(null!, []));

    [Fact]
    public void Constructor_NullParameters_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new SqlQuery("SELECT 1;", null!));

    [Fact]
    public void GetParameterCount()
    {
        Dictionary<ParameterTag, object> parameters = new()
        {
            [new ParameterTag(null, "@p1", null, SqlTypeDefinition.AsInt())] = 1,
            [new ParameterTag(null, "@p2", null, SqlTypeDefinition.AsInt())] = 2
        };

        SqlQuery query = new("SELECT @p1, @p2;", parameters);

        Assert.Equal(2, query.GetParameterCount());
    }

    [Fact]
    public void GetParameterValue()
    {
        Dictionary<ParameterTag, object> parameters = new()
        {
            [new ParameterTag(null, "@p1", null, SqlTypeDefinition.AsInt())] = 123
        };

        SqlQuery query = new("SELECT @p1;", parameters);

        Assert.Equal(123, query.GetParameterValue<int>("@p1"));
    }

    [Fact]
    public void GetParameterValue_NullParameterTestName_Exception()
    {
        SqlQuery query = new("SELECT 1;", []);

        Assert.Throws<ArgumentNullException>(() => query.GetParameterValue<int>(null!));
    }

    [Fact]
    public void GetParameterValue_NotFound_Exception()
    {
        SqlQuery query = new("SELECT 1;", []);

        Assert.Throws<KeyNotFoundException>(() => query.GetParameterValue<int>("@missing"));
    }
}
