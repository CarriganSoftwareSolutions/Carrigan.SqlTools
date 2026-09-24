using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Generators.PostgreSql.Tests.ExpressionsTests;

public class SplitPartTests : FunctionTestsWithThreeExpressionsBase
{
    protected override string ExpectedFunctionName => "SPLIT_PART";

    protected override FunctionalExpression New(SqlExpression? first, SqlExpression? second, SqlExpression? third) =>
        new SplitPart(first!, second!, third!);

    [Fact]
    public void TestConstantConstructor()
    {
        SplitPart expression = new(new Column<Order>(nameof(Order.CustomerId)), "test", 3);

        Column<Order> first = Assert.IsType<Column<Order>>(expression.ChildNodes.First());
        Parameter second = Assert.IsType<Parameter>(expression.ChildNodes.ElementAt(1));
        Parameter third = Assert.IsType<Parameter>(expression.ChildNodes.ElementAt(2));

        Assert.Equal("Order.CustomerId", first.ToString());
        Assert.Equal("test", second.Value);
        Assert.Equal(3, third.Value);
    }
}
