using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.PostgreSql;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;

//IGNORE SPELLING: TRUNC

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.Examples;

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

        string expected = "SELECT CURRENT_DATE AS \"Date\", \"Customer\".\"Name\" AS \"Name\" FROM \"Customer\"";
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

        string expected = "SELECT CURRENT_TIMESTAMP AS \"TimeStamp\", \"Customer\".\"Name\" AS \"Name\" FROM \"Customer\"";
        string actual = query.QueryText;

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DateTrunc_Example()
    {
        DateTrunc expression = new(DateTruncDateTimePartEnum.Month, new Parameter(new DateTime(2026, 9, 23)));
        SelectBuilder<Customer> selectBuilder = new() { Selects = expression.AsSelectTag("Value") };
        SqlQuery query = customerGenerator.Select(selectBuilder);
        Assert.Equal("SELECT DATE_TRUNC('month', $1) AS \"Value\" FROM \"Customer\"", query.QueryText);
    }

    [Fact]
    public void Extract_Example()
    {
        Extract expression = new(ExtractDateTimePartEnum.Year, new Parameter(new DateTime(2026, 9, 23)));
        SelectBuilder<Customer> selectBuilder = new() { Selects = expression.AsSelectTag("Value") };
        SqlQuery query = customerGenerator.Select(selectBuilder);
        Assert.Equal("SELECT EXTRACT(year FROM $1) AS \"Value\" FROM \"Customer\"", query.QueryText);
    }

    [Fact]
    public void MakeDate_Example()
    {
        MakeDate expression = new(new Parameter(2026), new Parameter(9), new Parameter(23));
        SelectBuilder<Customer> selectBuilder = new() { Selects = expression.AsSelectTag("Value") };
        SqlQuery query = customerGenerator.Select(selectBuilder);
        Assert.Equal("SELECT MAKE_DATE($1, $2, $3) AS \"Value\" FROM \"Customer\"", query.QueryText);
    }

    [Fact]
    public void MakeInterval_Example()
    {
        MakeInterval expression = new(MakeIntervalDateTimePartEnum.Day, new Parameter(2));
        SelectBuilder<Customer> selectBuilder = new() { Selects = expression.AsSelectTag("Value") };
        SqlQuery query = customerGenerator.Select(selectBuilder);
        Assert.Equal("SELECT MAKE_INTERVAL(days => $1) AS \"Value\" FROM \"Customer\"", query.QueryText);
    }
}
