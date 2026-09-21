using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using Carrigan.SqlTools.Tags;

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
