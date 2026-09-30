using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests;

public sealed class PatIndexTests : FunctionTestsWithTwoExpressionsBase
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new PatIndex((SqlExpression)null!, Second)),
        (() => new PatIndex(First, null!)),
        (() => new PatIndex((string)null!, Second)),
        (() => new PatIndex("%[0-9]%", null!)),
    ];


    protected override string ExpectedFunctionName =>
        "PATINDEX";

    protected override FunctionalExpression New(SqlExpression? first, SqlExpression? second) =>
        new PatIndex(first!, second!);

    [Fact]
    public void StringConstructor_RendersPatternAsParameter()
    {
        SqlExpression expression = new PatIndex("%[0-9]%", new Parameter("ABC123", "Value"));

        Assert.Equal("PATINDEX(Parameter, Value)", expression.ToString());
    }

    [Fact]
    public void StringConstructor_NullPattern_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new PatIndex((string)null!, new Parameter("ABC123")));

    [Fact]
    public void StringConstructor_NullExpression_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new PatIndex("%[0-9]%", null!));

    [Fact]
    public void StringConstructor_WrapsPatternInParameter()
    {
        PatIndex expression = new("%[0-9]%", new Parameter("ABC123", "Value"));

        Parameter pattern = Assert.IsType<Parameter>(expression.ChildNodes.First());
        Assert.Equal("%[0-9]%", pattern.Value);
    }

    [Fact]
    public void Pattern_IsNotEmbeddedInRenderedSql()
    {
        const string pattern = "'); DROP TABLE Customer;--";

        string actual = new PatIndex(pattern, new Parameter("ABC123", "Value")).ToString();

        Assert.Equal("PATINDEX(Parameter, Value)", actual);
        Assert.DoesNotContain(pattern, actual);
    }
}
