using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests;

public class DateFromPartsTests
{
    [Fact]
    public void ExpressionConstructor_Test()
    {
        SqlExpression expression = new DateFromParts(new Parameter(2026, "Year"), new Parameter(9, "Month"), new Parameter(23, "Day"));
        Assert.Equal("DATEFROMPARTS(Year, Month, Day)", expression.ToString());
    }

    [Fact]
    public void IntegerConstructor_WrapsValuesInParameters_Test()
    {
        DateFromParts expression = new(2026, 9, 23);
        SqlExpression[] children = [.. expression.ChildNodes];

        Assert.Equal(3, children.Length);
        Assert.Equal(2026, (int)Assert.IsType<Parameter>(children[0]).Value!);
        Assert.Equal(9, (int)Assert.IsType<Parameter>(children[1]).Value!);
        Assert.Equal(23, (int)Assert.IsType<Parameter>(children[2]).Value!);
    }
}
