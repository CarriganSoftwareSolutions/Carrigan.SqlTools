using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.PostgreSql;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.Examples;

public class FromReadMeSqlExpressionsConversionExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();

    [Fact]
    public void Cast_Example()
    {
        Cast expression = new(new Parameter("123"), PostgreSqlTypesProvider.AsNumeric(false, true));

        SelectBuilder<Customer> selectBuilder = new()
        {
            Selects = expression.AsSelectTag("Value")
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expected = "SELECT CAST($1 AS NUMERIC) AS \"Value\" FROM \"Customer\"";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
        Assert.Equal("123", Assert.Single(query.Parameters).Value);
    }
}
