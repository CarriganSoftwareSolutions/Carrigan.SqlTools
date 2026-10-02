using Carrigan.SqlTools.Base.Tests.Expressions;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Base.Tests.PredicateLogicTests;

public abstract class PredicateLogicBaseTests : SqlExpressionsBaseTests
{
    protected static IEnumerable<Func<SqlExpression>> BinaryAttemptMixedAggregateConstructions
    (
        Func<SqlExpression, SqlExpression, SqlExpression> factory
    ) =>
    [
        (() => factory(AggregateWithParameterNoColumn, ColumnA)),
        (() => factory(ColumnA, AggregateWithParameterNoColumn)),
    ];

    protected static IEnumerable<SqlExpression> BinaryExpressionsThatAreNotEqual
    (
        Func<SqlExpression, SqlExpression, SqlExpression> factory
    ) =>
    [
        factory(First, Second),
        factory(First, Third),
        factory(Second, Third),
        factory(ColumnA, ParameterValue),
        factory(ColumnB, ParameterValue),
    ];

    protected static IEnumerable<SqlExpression> BinaryExpressionsThatAreEqual
    (
        Func<SqlExpression, SqlExpression, SqlExpression> factory
    ) =>
    [
        factory(ColumnA, ParameterValue),
        factory(ColumnA, ParameterDifferentValue),
    ];

    protected static IEnumerable<SqlExpression> BinaryExpressionsThatHaveAggregates
    (
        Func<SqlExpression, SqlExpression, SqlExpression> factory
    ) =>
    [
        factory(AggregateWithParameterNoColumn, Second),
        factory(First, AggregateWithParameterNoColumn),
    ];

    protected static IEnumerable<SqlExpression> BinaryExpressionsThatHaveNoAggregates
    (
        Func<SqlExpression, SqlExpression, SqlExpression> factory
    ) =>
    [
        factory(First, Second),
        factory(ColumnA, ColumnB),
    ];

    protected static IEnumerable<SqlExpression> BinaryExpressionsThatHaveColumns
    (
        Func<SqlExpression, SqlExpression, SqlExpression> factory
    ) =>
    [
        factory(ColumnA, Second),
        factory(First, ColumnA),
    ];

    protected static IEnumerable<SqlExpression> BinaryExpressionsThatHaveNoColumns
    (
        Func<SqlExpression, SqlExpression, SqlExpression> factory
    ) =>
    [
        factory(First, Second),
    ];

    protected static IEnumerable<SqlExpression> BinaryExpressionsThatHaveParameters
    (
        Func<SqlExpression, SqlExpression, SqlExpression> factory
    ) =>
    [
        factory(First, Second),
        factory(ColumnA, Second),
    ];

    protected static IEnumerable<SqlExpression> BinaryExpressionsThatHaveNoParameters
    (
        Func<SqlExpression, SqlExpression, SqlExpression> factory
    ) =>
    [
        factory(ColumnA, ColumnB),
    ];

    protected static IEnumerable<Func<SqlExpression>> TernaryAttemptMixedAggregateConstructions
    (
        Func<SqlExpression, SqlExpression, SqlExpression, SqlExpression> factory
    ) =>
    [
        (() => factory(AggregateWithParameterNoColumn, ColumnA, Third)),
        (() => factory(AggregateWithParameterNoColumn, Second, ColumnA)),
        (() => factory(ColumnA, AggregateWithParameterNoColumn, Third)),
        (() => factory(First, AggregateWithParameterNoColumn, ColumnA)),
        (() => factory(ColumnA, Second, AggregateWithParameterNoColumn)),
        (() => factory(First, ColumnA, AggregateWithParameterNoColumn)),
    ];

    protected static IEnumerable<SqlExpression> TernaryExpressionsThatAreNotEqual
    (
        Func<SqlExpression, SqlExpression, SqlExpression, SqlExpression> factory
    ) =>
    [
        factory(First, Second, Third),
        factory(DifferentParameter, Second, Third),
        factory(First, DifferentParameter, Third),
        factory(First, Second, DifferentParameter),
        factory(ColumnA, ParameterValue, ParameterValue),
        factory(ColumnB, ParameterValue, ParameterValue),
    ];

    protected static IEnumerable<SqlExpression> TernaryExpressionsThatAreEqual
    (
        Func<SqlExpression, SqlExpression, SqlExpression, SqlExpression> factory
    ) =>
    [
        factory(ColumnA, ParameterValue, ParameterValue),
        factory(ColumnA, ParameterDifferentValue, ParameterDifferentValue),
    ];

    protected static IEnumerable<SqlExpression> TernaryExpressionsThatHaveAggregates
    (
        Func<SqlExpression, SqlExpression, SqlExpression, SqlExpression> factory
    ) =>
    [
        factory(AggregateWithParameterNoColumn, Second, Third),
        factory(First, AggregateWithParameterNoColumn, Third),
        factory(First, Second, AggregateWithParameterNoColumn),
    ];

    protected static IEnumerable<SqlExpression> TernaryExpressionsThatHaveNoAggregates
    (
        Func<SqlExpression, SqlExpression, SqlExpression, SqlExpression> factory
    ) =>
    [
        factory(First, Second, Third),
        factory(ColumnA, ColumnB, Column1),
    ];

    protected static IEnumerable<SqlExpression> TernaryExpressionsThatHaveColumns
    (
        Func<SqlExpression, SqlExpression, SqlExpression, SqlExpression> factory
    ) =>
    [
        factory(ColumnA, Second, Third),
        factory(First, ColumnA, Third),
        factory(First, Second, ColumnA),
    ];

    protected static IEnumerable<SqlExpression> TernaryExpressionsThatHaveNoColumns
    (
        Func<SqlExpression, SqlExpression, SqlExpression, SqlExpression> factory
    ) =>
    [
        factory(First, Second, Third),
        factory(AggregateWithParameterNoColumn, Second, Third),
    ];

    protected static IEnumerable<SqlExpression> TernaryExpressionsThatHaveParameters
    (
        Func<SqlExpression, SqlExpression, SqlExpression, SqlExpression> factory
    ) =>
    [
        factory(First, Second, Third),
        factory(ColumnA, Second, Third),
    ];

    protected static IEnumerable<SqlExpression> TernaryExpressionsThatHaveNoParameters
    (
        Func<SqlExpression, SqlExpression, SqlExpression, SqlExpression> factory
    ) =>
    [
        factory(ColumnA, ColumnB, Column1),
    ];

    protected static IEnumerable<Func<SqlExpression>> UnaryAttemptMixedAggregateConstructions() =>
        [];

    protected static IEnumerable<SqlExpression> UnaryExpressionsThatAreNotEqual
    (
        Func<SqlExpression, SqlExpression> factory
    ) =>
    [
        factory(ParameterValue),
        factory(DifferentParameter),
        factory(ColumnA),
        factory(ColumnB),
    ];

    protected static IEnumerable<SqlExpression> UnaryExpressionsThatAreEqual
    (
        Func<SqlExpression, SqlExpression> factory
    ) =>
    [
        factory(new Add(ColumnA, ParameterValue)),
        factory(new Add(ColumnA, ParameterDifferentValue)),
    ];

    protected static IEnumerable<SqlExpression> UnaryExpressionsThatHaveAggregates
    (
        Func<SqlExpression, SqlExpression> factory
    ) =>
    [
        factory(AggregateWithParameterNoColumn),
    ];

    protected static IEnumerable<SqlExpression> UnaryExpressionsThatHaveNoAggregates
    (
        Func<SqlExpression, SqlExpression> factory
    ) =>
    [
        factory(ParameterValue),
        factory(ColumnA),
    ];

    protected static IEnumerable<SqlExpression> UnaryExpressionsThatHaveColumns
    (
        Func<SqlExpression, SqlExpression> factory
    ) =>
    [
        factory(ColumnA),
    ];

    protected static IEnumerable<SqlExpression> UnaryExpressionsThatHaveNoColumns
    (
        Func<SqlExpression, SqlExpression> factory
    ) =>
    [
        factory(ParameterValue),
        factory(AggregateWithParameterNoColumn),
    ];

    protected static IEnumerable<SqlExpression> UnaryExpressionsThatHaveParameters
    (
        Func<SqlExpression, SqlExpression> factory
    ) =>
    [
        factory(ParameterValue),
        factory(AggregateWithParameterNoColumn),
    ];

    protected static IEnumerable<SqlExpression> UnaryExpressionsThatHaveNoParameters
    (
        Func<SqlExpression, SqlExpression> factory
    ) =>
    [
        factory(ColumnA),
    ];

    protected static IEnumerable<Func<SqlExpression>> MultipleAttemptMixedAggregateConstructions
    (
        Func<IEnumerable<SqlExpression>, SqlExpression> factory
    ) =>
    [
        (() => factory([AggregateWithParameterNoColumn, ColumnA])),
        (() => factory([ColumnA, AggregateWithParameterNoColumn])),
        (() => factory([AggregateWithParameterNoColumn, Second, ColumnA])),
        (() => factory([ColumnA, Second, AggregateWithParameterNoColumn])),
    ];

    protected static IEnumerable<SqlExpression> MultipleExpressionsThatAreNotEqual
    (
        Func<IEnumerable<SqlExpression>, SqlExpression> factory
    ) =>
    [
        factory([First, Second]),
        factory([First, Third]),
        factory([Second, Third]),
        factory([ColumnA, ParameterValue]),
        factory([ColumnB, ParameterValue]),
    ];

    protected static IEnumerable<SqlExpression> MultipleExpressionsThatAreEqual
    (
        Func<IEnumerable<SqlExpression>, SqlExpression> factory
    ) =>
    [
        factory([ColumnA, ParameterValue]),
        factory([ColumnA, ParameterDifferentValue]),
    ];

    protected static IEnumerable<SqlExpression> MultipleExpressionsThatHaveAggregates
    (
        Func<IEnumerable<SqlExpression>, SqlExpression> factory
    ) =>
    [
        factory([AggregateWithParameterNoColumn, Second]),
        factory([First, AggregateWithParameterNoColumn]),
    ];

    protected static IEnumerable<SqlExpression> MultipleExpressionsThatHaveNoAggregates
    (
        Func<IEnumerable<SqlExpression>, SqlExpression> factory
    ) =>
    [
        factory([First, Second]),
        factory([ColumnA, ColumnB]),
    ];

    protected static IEnumerable<SqlExpression> MultipleExpressionsThatHaveColumns
    (
        Func<IEnumerable<SqlExpression>, SqlExpression> factory
    ) =>
    [
        factory([ColumnA, Second]),
        factory([First, ColumnA]),
    ];

    protected static IEnumerable<SqlExpression> MultipleExpressionsThatHaveNoColumns
    (
        Func<IEnumerable<SqlExpression>, SqlExpression> factory
    ) =>
    [
        factory([First, Second]),
    ];

    protected static IEnumerable<SqlExpression> MultipleExpressionsThatHaveParameters
    (
        Func<IEnumerable<SqlExpression>, SqlExpression> factory
    ) =>
    [
        factory([First, Second]),
        factory([ColumnA, Second]),
    ];

    protected static IEnumerable<SqlExpression> MultipleExpressionsThatHaveNoParameters
    (
        Func<IEnumerable<SqlExpression>, SqlExpression> factory
    ) =>
    [
        factory([ColumnA, ColumnB]),
    ];
}
