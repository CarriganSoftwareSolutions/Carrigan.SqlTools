namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.DialectTests.PostgreSqlTests;

public partial class PostgreSqlDialectTests
{
    [Theory]
    [InlineData("Name", 1, "$1")]
    [InlineData("@Name", 2, "$2")]
    [InlineData("Anything", 25, "$25")]
    public void RenderFinalParameterName_ReturnsPostgreSqlPositionalParameterName(string baseParameterName, int parameterIndex, string expected)
    {
        string actual = Dialect.RenderFinalParameterName(baseParameterName, parameterIndex);
        Assert.Equal(expected, actual);
    }
}
