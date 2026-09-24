using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExamplesAsUnitTests;
//IGNORE SPELLING: Substring
public class FromReadMeSqlExpressionsNullExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();

    [Fact]
    public void CharIndex_Example()
    {
        CharIndex expression = new("a", new Column<Customer>(nameof(Customer.Name)));

        SelectBuilder<Customer> selectBuilder = new()
        {
            Selects = expression.AsSelectTag("Value")
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expected = "SELECT CHARINDEX(@Parameter_1, [Customer].[Name]) AS [Value] FROM [Customer]";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
        Assert.Equal("a", Assert.Single(query.Parameters).Value);
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
    public void IndexOf_Example()
    {
        IndexOf expression = new(new Column<Customer>(nameof(Customer.Name)), "a");

        SelectBuilder<Customer> selectBuilder = new()
        {
            Selects = expression.AsSelectTag("Value")
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expected = "SELECT CHARINDEX(@Parameter_1, [Customer].[Name]) AS [Value] FROM [Customer]";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
        Assert.Equal("a", Assert.Single(query.Parameters).Value);
    }

    [Fact]
    public void Left_Example()
    {
        Left expression = new(new Column<Customer>(nameof(Customer.Name)), 3);

        SelectBuilder<Customer> selectBuilder = new()
        {
            Selects = expression.AsSelectTag("Value")
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expected = "SELECT LEFT([Customer].[Name], @Parameter_1) AS [Value] FROM [Customer]";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
        Assert.Equal(3, Assert.Single(query.Parameters).Value);
    }

    [Fact]
    public void Length_Example()
    {
        Length expression = new(new Column<Customer>(nameof(Customer.Name)));

        SelectBuilder<Customer> selectBuilder = new()
        {
            Selects = expression.AsSelectTag("Value")
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expected = "SELECT LEN([Customer].[Name]) AS [Value] FROM [Customer]";
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
    public void Repeat_Example()
    {
        Repeat expression = new(new Column<Customer>(nameof(Customer.Name)), 2);

        SelectBuilder<Customer> selectBuilder = new()
        {
            Selects = expression.AsSelectTag("Value")
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expected = "SELECT REPLICATE([Customer].[Name], @Parameter_1) AS [Value] FROM [Customer]";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
        Assert.Equal(2, Assert.Single(query.Parameters).Value);
    }

    [Fact]
    public void Replace_Example()
    {
        Replace expression = new(new Column<Customer>(nameof(Customer.Name)), new Parameter("'"), new Parameter(" "));
        SelectTags selects = new(new SelectTag(expression, "Value"));

        SelectBuilder<Customer> selectBuilder = new()
        {
            Selects = selects
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expected = "SELECT REPLACE([Customer].[Name], @Parameter_1, @Parameter_2) AS [Value] FROM [Customer]";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
        Assert.Equal("'", query.Parameters.First().Value);
        Assert.Equal(" ", query.Parameters.ElementAt(1).Value);
    }

    [Fact]
    public void ReplaceWithStrings_Example()
    {
        Replace expression = new(new Column<Customer>(nameof(Customer.Name)), "'", " ");
        SelectTags selects = new(new SelectTag(expression, "Value"));

        SelectBuilder<Customer> selectBuilder = new()
        {
            Selects = selects
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expected = "SELECT REPLACE([Customer].[Name], @Parameter_1, @Parameter_2) AS [Value] FROM [Customer]";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
        Assert.Equal("'", query.Parameters.First().Value);
        Assert.Equal(" ", query.Parameters.ElementAt(1).Value);
    }

    [Fact]
    public void Right_Example()
    {
        Right expression = new(new Column<Customer>(nameof(Customer.Name)), 3);

        SelectBuilder<Customer> selectBuilder = new()
        {
            Selects = expression.AsSelectTag("Value")
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expected = "SELECT RIGHT([Customer].[Name], @Parameter_1) AS [Value] FROM [Customer]";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
        Assert.Equal(3, Assert.Single(query.Parameters).Value);
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

        SelectBuilder<Customer> selectBuilder = new()
        {
            Selects = expression.AsSelectTag("Value")
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expected = "SELECT RTRIM([Customer].[Name], @Parameter_1) AS [Value] FROM [Customer]";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
        Assert.Equal(" x", Assert.Single(query.Parameters).Value);
    }

    [Fact]
    public void Substring_Example()
    {
        Substring expression = new(new Column<Customer>(nameof(Customer.Name)), 2, 3);

        SelectBuilder<Customer> selectBuilder = new()
        {
            Selects = expression.AsSelectTag("Value")
        };

        SqlQuery query = customerGenerator.Select(selectBuilder);

        string expected = "SELECT SUBSTRING([Customer].[Name], @Parameter_1, @Parameter_2) AS [Value] FROM [Customer]";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
        Assert.Equal(2, query.Parameters.First().Value);
        Assert.Equal(3, query.Parameters.ElementAt(1).Value);
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
