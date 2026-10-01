using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Base.Tests.TestEntities;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Generators.SqlServer.Tests.ExpressionsTests;

//IGNORE SPELLING: abcdef uvwxyz

public sealed class StuffTests : SqlExpressionsBaseTests
{
    public override IEnumerable<Func<SqlExpression>> AttemptNullConstructions() =>
    [
        (() => new Stuff(null!, Start, Length, Replacement)),
        (() => new Stuff(AbcValue, null!, Length, Replacement)),
        (() => new Stuff(AbcValue, Start, null!, Replacement)),
        (() => new Stuff(AbcValue, Start, Length, null!)),
        (() => new Stuff(null!, 2, 3, "X")),
        (() => new Stuff(AbcValue, 2, 3, null!)),
    ];



    public override IEnumerable<Func<SqlExpression>> AttemptMixedAggregateConstructions() =>
    [
        (() => new Stuff(AggregateWithParameterNoColumn, ColumnA, Third, Replacement)),
        (() => new Stuff(AggregateWithParameterNoColumn, Second, ColumnA, Replacement)),
        (() => new Stuff(AggregateWithParameterNoColumn, Second, Third, ColumnA)),
        (() => new Stuff(ColumnA, AggregateWithParameterNoColumn, Third, Replacement)),
        (() => new Stuff(First, AggregateWithParameterNoColumn, ColumnA, Replacement)),
        (() => new Stuff(First, AggregateWithParameterNoColumn, Third, ColumnA)),
        (() => new Stuff(ColumnA, Second, AggregateWithParameterNoColumn, Replacement)),
        (() => new Stuff(First, ColumnA, AggregateWithParameterNoColumn, Replacement)),
        (() => new Stuff(First, Second, AggregateWithParameterNoColumn, ColumnA)),
        (() => new Stuff(ColumnA, Second, Third, AggregateWithParameterNoColumn)),
        (() => new Stuff(First, ColumnA, Third, AggregateWithParameterNoColumn)),
        (() => new Stuff(First, Second, ColumnA, AggregateWithParameterNoColumn)),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreNotEqual =>
    [
        new Stuff(AbcValue, Start, Length, Replacement),
        new Stuff(DifferentParameter, Start, Length, Replacement),
        new Stuff(AbcValue, Start, Length, DifferentParameter),
        new Stuff(ColumnA, ParameterValue, ParameterValue, ParameterValue),
        new Stuff(ColumnB, ParameterValue, ParameterValue, ParameterValue),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatAreEqual =>
    [
        new Stuff(ColumnA, ParameterValue, ParameterValue, ParameterValue),
        new Stuff(ColumnA, ParameterDifferentValue, ParameterDifferentValue, ParameterDifferentValue),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveAggregates =>
    [
        new Stuff(AggregateWithParameterNoColumn, Start, Length, Replacement),
        new Stuff(AbcValue, AggregateWithParameterNoColumn, Length, Replacement),
        new Stuff(AbcValue, Start, AggregateWithParameterNoColumn, Replacement),
        new Stuff(AbcValue, Start, Length, AggregateWithParameterNoColumn),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoAggregates =>
    [
        new Stuff(AbcValue, Start, Length, Replacement),
        new Stuff(ColumnA, ColumnB, Column1, Column2),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveColumns =>
    [
        new Stuff(ColumnA, Start, Length, Replacement),
        new Stuff(AbcValue, ColumnA, Length, Replacement),
        new Stuff(AbcValue, Start, ColumnA, Replacement),
        new Stuff(AbcValue, Start, Length, ColumnA),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoColumns =>
    [
        new Stuff(AbcValue, Start, Length, Replacement),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveParameters =>
    [
        new Stuff(AbcValue, Start, Length, Replacement),
        new Stuff(ColumnA, Start, Length, Replacement),
    ];

    public override IEnumerable<SqlExpression> ExpressionsThatHaveNoParameters =>
    [
        new Stuff(ColumnA, ColumnB, Column1, Column2),
    ];

    [Fact]
    public void ExpressionConstructor_RendersValues() =>
        Assert.Equal("STUFF(Value, Start, Length, Replacement)", new Stuff(AbcValue, Start, Length, Replacement).ToString());

    [Fact]
    public void ConstantConstructor_RendersConstantsAsParameters() =>
        Assert.Equal("STUFF(Value, Parameter, Parameter, Parameter)", new Stuff(AbcValue, 2, 3, "X").ToString());

    [Fact]
    public void ExpressionConstructor_NullValue_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new Stuff(null!, Start, Length, Replacement));

    [Fact]
    public void ExpressionConstructor_NullStart_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new Stuff(AbcValue, null!, Length, Replacement));

    [Fact]
    public void ExpressionConstructor_NullLength_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new Stuff(AbcValue, Start, null!, Replacement));

    [Fact]
    public void ExpressionConstructor_NullReplacement_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new Stuff(AbcValue, Start, Length, null!));

    [Fact]
    public void ConstantConstructor_NullValue_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new Stuff(null!, 2, 3, "X"));

    [Fact]
    public void ConstantConstructor_NullReplacement_Exception() =>
        Assert.Throws<ArgumentNullException>(() => new Stuff(AbcValue, 2, 3, null!));

    [Fact]
    public void ExpressionConstructor_ChildNodesContainValues()
    {
        Stuff expression = new(AbcValue, Start, Length, Replacement);

        Assert.Equal([ AbcValue, Start, Length, Replacement ], expression.ChildNodes);
    }

    [Fact]
    public void ConstantConstructor_WrapsConstantsInParameters()
    {
        Stuff expression = new(AbcValue, 2, 3, "X");
        SqlExpression[] children = [.. expression.ChildNodes];

        Assert.Equal(AbcValue, children[0]);
        Assert.Equal(2, Assert.IsType<Parameter>(children[1]).Value);
        Assert.Equal(3, Assert.IsType<Parameter>(children[2]).Value);
        Assert.Equal("X", Assert.IsType<Parameter>(children[3]).Value);
    }

    [Fact]
    public void Constants_AreNotEmbeddedInRenderedSql()
    {
        const string replacement = "'); DROP TABLE Customer;--";

        string actual = new Stuff(AbcValue, 1776, 2026, replacement).ToString();

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
}
