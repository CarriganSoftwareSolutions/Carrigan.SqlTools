using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Base class for SQL arithmetic expressions that combine one or more numeric expressions with an arithmetic operator.
/// </summary>
public abstract class ArithmeticExpression : NumericExpression
{
    /// <summary>
    /// The SQL arithmetic operator text placed between rendered child expressions.
    /// </summary>
    private readonly string _operator;

    /// <summary>
    /// Base constructor for all arithmetic expression classes.
    /// </summary>
    /// <param name="operation">The operator for the arithmetic operation.</param>
    /// <param name="numericExpressions">The child numeric expressions.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="numericExpressions"/> or <paramref name="operation"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="operation"/> is empty or whitespace, or when <paramref name="numericExpressions"/> contains no elements.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// Thrown when <paramref name="numericExpressions"/> contains disallowed <c>null</c> values.
    /// </exception>
    protected ArithmeticExpression(string operation, IEnumerable<NumericExpression> numericExpressions)
        : this(ValidateNumericExpressions(numericExpressions), ValidateOperation(operation))
    {
    }

    private ArithmeticExpression(IEnumerable<NumericExpression> numericExpressions, string operation)
        : base(numericExpressions, ToDialectNeutralString(operation, numericExpressions)) =>
        _operator = operation;

    private static string ValidateOperation(string operation)
    {
        ArgumentNullException.ThrowIfNull(operation, nameof(operation));

        if (operation.IsNullOrWhiteSpace())
            throw new ArgumentException("Numeric operator text cannot be empty or whitespace.", nameof(operation));

        return operation;
    }

    private static IEnumerable<NumericExpression> ValidateNumericExpressions(IEnumerable<NumericExpression> numericExpressions)
    {
        ArgumentNullException.ThrowIfNull(numericExpressions, nameof(numericExpressions));

        NumericExpression[] expressions = [.. numericExpressions];
        if (numericExpressions.None())
            throw new ArgumentException($"{nameof(numericExpressions)} must contain at least one value.", nameof(numericExpressions));
        if (numericExpressions.Any(static expression => expression is null))
            throw new NullReferenceException($"{nameof(numericExpressions)} cannot contain null values.");

        return numericExpressions;
    }

    private static string ToDialectNeutralString(string operation, IEnumerable<NumericExpression> numericExpressions) =>
        numericExpressions.Count() == 1 ? numericExpressions.ElementAt(0).ToString() : $"({string.Join($" {operation} ", numericExpressions)})";

    /// <summary>
    /// Converts the arithmetic expression to SQL fragments for the supplied dialect.
    /// </summary>
    /// <param name="dialect">The SQL dialect used to render each child expression.</param>
    /// <returns>The SQL fragments representing the arithmetic expression.</returns>
    public override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        int index = 0;

        if (ChildNodes.Count() == 1)
        {
            foreach (ISqlFragment fragment in ChildNodes.Single().ToSqlFragments(dialect))
                yield return fragment;

            yield break;
        }

        yield return new SqlFragmentText("(");
        foreach (SqlExpression expression in ChildNodes)
        {
            if (index > 0)
                yield return new SqlFragmentText($" {_operator} ");
            foreach (ISqlFragment fragment in expression.ToSqlFragments(dialect))
                yield return fragment;
            index++;
        }
        yield return new SqlFragmentText(")");
    }
}
