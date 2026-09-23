using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests;

public class DateFromPartsTests
{
    [Fact]
    public void Constructor_Test()
    {
        SqlExpression expression = new DateFromParts(new Parameter(2026, "Year"), new Parameter(9, "Month"), new Parameter(23, "Day"));
        Assert.Equal("DATEFROMPARTS(Year, Month, Day)", expression.ToString());
    }
}
