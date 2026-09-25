using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExamplesAsUnitTests;

public class FromReadMeSqlExpressionsConversionExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();

    [Fact]
    public void Cast_Example()
    {
        Cast expression = new(new Parameter("123"), SqlServerTypesProvider.AsInt(true));

        SelectBuilder<Customer> selectBuilder = new()
        {
            Selects = expression.AsSelectTag("Value")
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expected = "SELECT CAST(@Parameter_1 AS INT) AS [Value] FROM [Customer]";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
        Assert.Equal("123", Assert.Single(query.Parameters).Value);
    }

    [Fact]
    public void TryCast_Example()
    {
        TryCast expression = new(new Parameter("123"), SqlServerTypesProvider.AsInt(true));

        SelectBuilder<Customer> selectBuilder = new()
        {
            Selects = expression.AsSelectTag("Value")
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expected = "SELECT TRY_CAST(@Parameter_1 AS INT) AS [Value] FROM [Customer]";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
        Assert.Equal("123", Assert.Single(query.Parameters).Value);
    }
}
