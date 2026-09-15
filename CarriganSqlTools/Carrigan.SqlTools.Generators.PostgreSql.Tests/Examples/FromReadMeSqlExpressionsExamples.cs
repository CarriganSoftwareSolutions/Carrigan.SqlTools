using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.Examples;

public class FromReadMeSqlExpressionsExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();

    [Fact]
    public void Coalesce_Example()
    {
        Coalesce coalesce = new(new Column<Customer>(nameof(Customer.Phone)), new Column<Customer>(nameof(Customer.Email)));
        SelectTags selects = new(new SelectTag(coalesce, "Coalescence"));

        SelectBuilder<Customer> selectBuilder = new()
        {
            Selects = selects
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expected = "SELECT COALESCE(\"Customer\".\"Phone\", \"Customer\".\"Email\") AS \"Coalescence\" FROM \"Customer\"";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void NullIf_Example()
    {
        NullIf nullIf = new(new Column<Customer>(nameof(Customer.Phone)), new Parameter(string.Empty));
        SelectTags selects = new(new SelectTag(nullIf, "NullIf"));

        SelectBuilder<Customer> selectBuilder = new()
        {
            Selects = selects
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expected = "SELECT NULLIF(\"Customer\".\"Phone\", $1) AS \"NullIf\" FROM \"Customer\"";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
    }
}
