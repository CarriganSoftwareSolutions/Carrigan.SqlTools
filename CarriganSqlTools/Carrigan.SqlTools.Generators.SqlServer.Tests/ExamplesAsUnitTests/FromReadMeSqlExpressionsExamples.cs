using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExamplesAsUnitTests;

public class FromReadMeSqlExpressionsExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();
    private static readonly SqlGenerator<Order> orderGenerator = new();

    [Fact]
    public void Abs_Example()
    {
        Abs abs = new(new Column<Order>(nameof(Order.Total)));
        SelectTags selects = new(new SelectTag(abs, "AbsValue"));

        SelectBuilder<Order> selectBuilder = new()
        {
            Selects = selects
        };

        SqlQuery query = orderGenerator.Select(selectBuilder);

        string expected = "SELECT ABS([Order].[Total]) AS [AbsValue] FROM [Order]";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
    }

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

        string expected = "SELECT COALESCE([Customer].[Phone], [Customer].[Email]) AS [Coalescence] FROM [Customer]";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Sign_Example()
    {
        Sign expression = new(new Column<Order>(nameof(Order.Total)));
        SelectTags selects = new(new SelectTag(expression, "Value"));

        SelectBuilder<Order> selectBuilder = new()
        {
            Selects = selects
        };

        SqlQuery query = orderGenerator.Select(selectBuilder);

        string expected = "SELECT SIGN([Order].[Total]) AS [Value] FROM [Order]";
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

        string expected = "SELECT NULLIF([Customer].[Phone], @Parameter_1) AS [NullIf] FROM [Customer]";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
    }
}
