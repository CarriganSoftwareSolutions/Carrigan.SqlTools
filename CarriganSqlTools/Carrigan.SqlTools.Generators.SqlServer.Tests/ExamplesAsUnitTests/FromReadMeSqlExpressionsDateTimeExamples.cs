using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using Carrigan.SqlTools.Tags;

//IGNORE SPELLING: TRUNC

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExamplesAsUnitTests;

public class FromReadMeSqlExpressionsDateTimeExamples
{
    private static readonly SqlGenerator<Customer> customerGenerator = new();

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
    public void DateAdd_Example()
    {
        DateAdd expression = new(DateAddDateTimePartEnum.Day, new Parameter(2), new Parameter(new DateTime(2026, 9, 23)));
        SelectBuilder<Customer> selectBuilder = new() { Selects = expression.AsSelectTag("Value") };
        SqlQuery query = customerGenerator.Select(selectBuilder);
        Assert.Equal("SELECT DATEADD(day, @Parameter_1, @Parameter_2) AS [Value] FROM [Customer]", query.QueryText);
    }

    [Fact]
    public void DateFromParts_Example()
    {
        DateFromParts expression = new(new Parameter(2026), new Parameter(9), new Parameter(23));
        SelectBuilder<Customer> selectBuilder = new() { Selects = expression.AsSelectTag("Value") };
        SqlQuery query = customerGenerator.Select(selectBuilder);
        Assert.Equal("SELECT DATEFROMPARTS(@Parameter_1, @Parameter_2, @Parameter_3) AS [Value] FROM [Customer]", query.QueryText);
    }

    [Fact]
    public void DatePart_Example()
    {
        DatePart expression = new(DatePartDateTimePartEnum.Year, new Parameter(new DateTime(2026, 9, 23)));
        SelectBuilder<Customer> selectBuilder = new() { Selects = expression.AsSelectTag("Value") };
        SqlQuery query = customerGenerator.Select(selectBuilder);
        Assert.Equal("SELECT DATEPART(year, @Parameter_1) AS [Value] FROM [Customer]", query.QueryText);
    }

    [Fact]
    public void DateTrunc_Example()
    {
        DateTrunc expression = new(DateTruncDateTimePartEnum.Month, new Parameter(new DateTime(2026, 9, 23)));
        SelectBuilder<Customer> selectBuilder = new() { Selects = expression.AsSelectTag("Value") };
        SqlQuery query = customerGenerator.Select(selectBuilder);
        Assert.Equal("SELECT DATETRUNC(month, @Parameter_1) AS [Value] FROM [Customer]", query.QueryText);
    }

    [Fact]
    public void EOMonth_Example()
    {
        EOMonth expression = new(new Parameter(new DateTime(2026, 9, 23)));
        SelectBuilder<Customer> selectBuilder = new() { Selects = expression.AsSelectTag("Value") };
        SqlQuery query = customerGenerator.Select(selectBuilder);
        Assert.Equal("SELECT EOMONTH(@Parameter_1) AS [Value] FROM [Customer]", query.QueryText);
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
}
