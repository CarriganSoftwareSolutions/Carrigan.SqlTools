using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests;

public class MakeDateTests
{
    [Fact]
    public void Constructor_Test()
    {
        SqlExpression expression = new MakeDate(new Parameter(2026, "Year"), new Parameter(9, "Month"), new Parameter(23, "Day"));
        Assert.Equal("MAKE_DATE(Year, Month, Day)", expression.ToString());
    }
}
