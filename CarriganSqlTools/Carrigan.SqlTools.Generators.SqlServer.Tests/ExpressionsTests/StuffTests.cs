using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests;

public sealed class StuffTests
{
    private static Parameter Value => new("abcdef", "Value");
    private static Parameter Start => new(2, "Start");
    private static Parameter Length => new(3, "Length");
    private static Parameter Replacement => new("X", "Replacement");

    [Fact]
    public void ExpressionConstructor_RendersValues() =>
        Assert.Equal("STUFF(Value, Start, Length, Replacement)", new Stuff(Value, Start, Length, Replacement).ToString());

    [Fact]
    public void ConstantConstructor_RendersConstantsAsParameters() =>
        Assert.Equal("STUFF(Value, Parameter, Parameter, Parameter)", new Stuff(Value, 2, 3, "X").ToString());

    [Fact]
    public void ExpressionConstructor_NullValue_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new Stuff(null!, Start, Length, Replacement));

    [Fact]
    public void ExpressionConstructor_NullStart_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new Stuff(Value, null!, Length, Replacement));

    [Fact]
    public void ExpressionConstructor_NullLength_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new Stuff(Value, Start, null!, Replacement));

    [Fact]
    public void ExpressionConstructor_NullReplacement_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new Stuff(Value, Start, Length, null!));

    [Fact]
    public void ConstantConstructor_NullValue_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new Stuff(null!, 2, 3, "X"));

    [Fact]
    public void ConstantConstructor_NullReplacement_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new Stuff(Value, 2, 3, null!));

    [Fact]
    public void ExpressionConstructor_ChildNodesContainValues()
    {
        Stuff expression = new(Value, Start, Length, Replacement);

        Assert.Equal(new SqlExpression[] { Value, Start, Length, Replacement }, expression.ChildNodes);
    }

    [Fact]
    public void ConstantConstructor_WrapsConstantsInParameters()
    {
        Stuff expression = new(Value, 2, 3, "X");
        SqlExpression[] children = [.. expression.ChildNodes];

        Assert.Equal(Value, children[0]);
        Assert.Equal(2, Assert.IsType<Parameter>(children[1]).Value);
        Assert.Equal(3, Assert.IsType<Parameter>(children[2]).Value);
        Assert.Equal("X", Assert.IsType<Parameter>(children[3]).Value);
    }

    [Fact]
    public void Constants_AreNotEmbeddedInRenderedSql()
    {
        const string replacement = "'); DROP TABLE Customer;--";

        string actual = new Stuff(Value, 1776, 2026, replacement).ToString();

        Assert.Equal("STUFF(Value, Parameter, Parameter, Parameter)", actual);
        Assert.DoesNotContain("1776", actual);
        Assert.DoesNotContain("2026", actual);
        Assert.DoesNotContain(replacement, actual);
    }

    [Fact]
    public void Equality_UsesExpressionStructure()
    {
        Stuff first = new(new Parameter("abcdef", "Value"), 2, 3, "X");
        Stuff equivalent = new(new Parameter("uvwxyz", "Value"), 20, 30, "Y");
        Stuff different = new(new Parameter("abcdef", "Different"), 2, 3, "X");

        Assert.Equal(first, equivalent);
        Assert.Equal(first.GetHashCode(), equivalent.GetHashCode());
        Assert.NotEqual(first, different);
    }

    [Fact]
    public void IsAggregate_AggregateAndRowIndependentValues_ReturnsTrue() =>
        Assert.True(new Stuff(new Count(Value), 2, 3, "X").IsAggregate());

    [Fact]
    public void IsAggregate_AggregateAndColumnValue_Exception() =>
        Assert.Throws<AggregateInconsistencyException>
        (
            () => new Stuff(new Count(Value), Start, Length, new TestColumnExpression()).IsAggregate()
        );
}
