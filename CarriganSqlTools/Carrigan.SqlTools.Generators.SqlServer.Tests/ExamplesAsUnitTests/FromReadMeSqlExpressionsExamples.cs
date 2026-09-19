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
    public void Concat_Example()
    {
        Concat expression = new(new Column<Customer>(nameof(Customer.Phone)), new Column<Customer>(nameof(Customer.Email)));
        SelectTags selects = new(new SelectTag(expression, "Value"));

        SelectBuilder<Customer> selectBuilder = new()
        {
            Selects = selects
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expected = "SELECT CONCAT([Customer].[Phone], [Customer].[Email]) AS [Value] FROM [Customer]";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CurrentDate_Example()
    {
        CurrentDate currentDate = new();
        ColumnBase columnBase = new Column<Customer>(nameof(Customer.Name));
        SelectBuilder<Customer> selectBuilder = new()
        {
            Selects = new SelectTags(new SelectTag(currentDate, "Date"), new SelectTag(columnBase, "Name"))
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expected = "SELECT CURRENT_DATE AS [Date], [Customer].[Name] AS [Name] FROM [Customer]";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CurrentTimeStamp_Example()
    {
        CurrentTimeStamp currentTimeStamp = new();
        ColumnBase columnBase = new Column<Customer>(nameof(Customer.Name));
        SelectBuilder<Customer> selectBuilder = new()
        {
            Selects = new SelectTags(new SelectTag(currentTimeStamp, "TimeStamp"), new SelectTag(columnBase, "Name"))
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expected = "SELECT CURRENT_TIMESTAMP AS [TimeStamp], [Customer].[Name] AS [Name] FROM [Customer]";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetDate_Example()
    {
        GetDate getDate = new();
        ColumnBase columnBase = new Column<Customer>(nameof(Customer.Name));
        SelectBuilder<Customer> selectBuilder = new()
        {
            Selects = new SelectTags(new SelectTag(getDate, "Date"), new SelectTag(columnBase, "Name"))
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expected = "SELECT GETDATE() AS [Date], [Customer].[Name] AS [Name] FROM [Customer]";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Lower_Example()
    {
        Lower expression = new(new Column<Customer>(nameof(Customer.Name)));
        SelectTags selects = new(new SelectTag(expression, "Value"));

        SelectBuilder<Customer> selectBuilder = new()
        {
            Selects = selects
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expected = "SELECT LOWER([Customer].[Name]) AS [Value] FROM [Customer]";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void LRTrim_Example()
    {
        LRTrim expression = new(new Column<Customer>(nameof(Customer.Name)));
        SelectTags selects = new(new SelectTag(expression, "Value"));

        SelectBuilder<Customer> selectBuilder = new()
        {
            Selects = selects
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expected = "SELECT LTRIM(RTRIM([Customer].[Name])) AS [Value] FROM [Customer]";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void LTrim_Example()
    {
        LTrim expression = new(new Column<Customer>(nameof(Customer.Name)));
        SelectTags selects = new(new SelectTag(expression, "Value"));

        SelectBuilder<Customer> selectBuilder = new()
        {
            Selects = selects
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expected = "SELECT LTRIM([Customer].[Name]) AS [Value] FROM [Customer]";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void LTrim_WithCharacters_Example()
    {
        LTrim expression = new(new Column<Customer>(nameof(Customer.Name)), " x");
        SelectTags selects = new(new SelectTag(expression, "Value"));

        SelectBuilder<Customer> selectBuilder = new()
        {
            Selects = selects
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expected = "SELECT LTRIM([Customer].[Name], @Parameter_1) AS [Value] FROM [Customer]";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
        Assert.Equal(" x", Assert.Single(query.Parameters).Value);
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

    [Fact]
    public void Round_Example()
    {
        Round expression = new(new Column<Customer>(nameof(Customer.Name)));
        SelectTags selects = new(new SelectTag(expression, "Value"));

        SelectBuilder<Customer> selectBuilder = new()
        {
            Selects = selects
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expected = "SELECT ROUND([Customer].[Name], @Parameter_1) AS [Value] FROM [Customer]";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
        Assert.Equal(0, Assert.Single(query.Parameters).Value);
    }

    [Fact]
    public void RoundWithPrecision_Example()
    {
        Round expression = new(new Column<Customer>(nameof(Customer.Name)), 2);
        SelectTags selects = new(new SelectTag(expression, "Value"));

        SelectBuilder<Customer> selectBuilder = new()
        {
            Selects = selects
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expected = "SELECT ROUND([Customer].[Name], @Parameter_1) AS [Value] FROM [Customer]";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
        Assert.Equal(2, Assert.Single(query.Parameters).Value);
    }

    [Fact]
    public void RTrim_Example()
    {
        RTrim expression = new(new Column<Customer>(nameof(Customer.Name)));
        SelectTags selects = new(new SelectTag(expression, "Value"));

        SelectBuilder<Customer> selectBuilder = new()
        {
            Selects = selects
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expected = "SELECT RTRIM([Customer].[Name]) AS [Value] FROM [Customer]";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void RTrim_WithCharacters_Example()
    {
        RTrim expression = new(new Column<Customer>(nameof(Customer.Name)), " x");
        SelectTags selects = new(new SelectTag(expression, "Value"));

        SelectBuilder<Customer> selectBuilder = new()
        {
            Selects = selects
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expected = "SELECT RTRIM([Customer].[Name], @Parameter_1) AS [Value] FROM [Customer]";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
        Assert.Equal(" x", Assert.Single(query.Parameters).Value);
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
    public void Trim_Example()
    {
        Trim expression = new(new Column<Customer>(nameof(Customer.Name)));
        SelectTags selects = new(new SelectTag(expression, "Value"));

        SelectBuilder<Customer> selectBuilder = new()
        {
            Selects = selects
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expected = "SELECT TRIM([Customer].[Name]) AS [Value] FROM [Customer]";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Trim_WithCharacters_Example()
    {
        Trim expression = new(new Column<Customer>(nameof(Customer.Name)), " x");
        SelectTags selects = new(new SelectTag(expression, "Value"));

        SelectBuilder<Customer> selectBuilder = new()
        {
            Selects = selects
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expected = "SELECT TRIM([Customer].[Name], @Parameter_1) AS [Value] FROM [Customer]";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
        Assert.Equal(" x", Assert.Single(query.Parameters).Value);
    }

    [Fact]
    public void Upper_Example()
    {
        Upper expression = new (new Column<Customer>(nameof(Customer.Name)));
        SelectTags selects = new(new SelectTag(expression, "Value"));

        SelectBuilder<Customer> selectBuilder = new()
        {
            Selects = selects
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expected = "SELECT UPPER([Customer].[Name]) AS [Value] FROM [Customer]";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
    }
}
