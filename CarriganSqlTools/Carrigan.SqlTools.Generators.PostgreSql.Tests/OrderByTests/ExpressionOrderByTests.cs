using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.OrderByClause;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.OrderByTests;

public class ExpressionOrderByTests
{
    private static readonly ISqlDialects Dialect = new PostgreSqlDialect();

    [Fact]
    public void Constructor_WithExpression_CreatesExpectedSql()
    {
        Lower expression = new(new Column<Address>(nameof(Address.City)));
        OrderBy orderBy = new(expression, SortDirectionEnum.Descending);

        Assert.Equal("LOWER(\"Address\".\"City\") DESC", orderBy.ToSql(Dialect));
    }

    [Fact]
    public void Constructor_WithNullExpression_ThrowsArgumentNullException()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => new OrderBy(null!));

        Assert.Equal("sqlExpression", exception.ParamName);
    }

    [Fact]
    public void ExpressionWithParameter_ExposesParameter()
    {
        Parameter parameter = new(10, "Offset");
        Add expression = new(new Column<Address>(nameof(Address.PostalCode)), parameter);
        OrderBy orderBy = new(expression);

        SqlFragmentParameter sqlFragmentParameter = Assert.Single(orderBy.GetSqlFragmentParameters(Dialect));
        
        Assert.Contains("Offset", sqlFragmentParameter.ParameterTag.ToString());
    }

    [Fact]
    public void ExpressionWithMultipleTables_ExposesAllLeafTables()
    {
        Add expression = new
        (
            new Column<Address>(nameof(Address.PostalCode)),
            new Column<ColumnTable>(nameof(ColumnTable.D000destruct0))
        );
        OrderBy orderBy = new(expression);

        Assert.Equal(2, orderBy.TableTags.Count());
    }
}
