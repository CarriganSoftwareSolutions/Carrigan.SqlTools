using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Base class for SQL expressions that combine one or more expressions with an arithmetic operator.
/// Operand type compatibility is intentionally delegated to the SQL database server.
/// </summary>
public abstract class ArithmeticExpression : SqlExpression
{
    /// <summary>
    /// The SQL arithmetic operator text placed between rendered child expressions.
    /// </summary>
    private readonly string _operator;

    /// <summary>
    /// Base constructor for arithmetic expressions.
    /// </summary>
    /// <param name="operation">The arithmetic operator.</param>
    /// <param name="sqlExpressions">The expressions to combine. No SQL data-type validation is performed.</param>
    protected ArithmeticExpression(string operation, IEnumerable<SqlExpression> sqlExpressions)
        : base(ValidateSqlExpressions(sqlExpressions)) =>
        _operator = ValidateOperation(operation);

    private static string ValidateOperation(string operation)
    {
        ArgumentNullException.ThrowIfNull(operation, nameof(operation));

        if (operation.IsNullOrWhiteSpace())
            throw new ArgumentException("Arithmetic operator text cannot be empty or whitespace.", nameof(operation));

        return operation;
    }

    private static IEnumerable<SqlExpression> ValidateSqlExpressions(IEnumerable<SqlExpression> sqlExpressions)
    {
        ArgumentNullException.ThrowIfNull(sqlExpressions, nameof(sqlExpressions));

        if (sqlExpressions.None())
            throw new ArgumentException($"{nameof(sqlExpressions)} must contain at least one value.", nameof(sqlExpressions));
        if (sqlExpressions.Any(static expression => expression is null))
            throw new NullReferenceException($"{nameof(sqlExpressions)} cannot contain null values.");

        return sqlExpressions;
    }

    protected override object EqualityContract =>
        typeof(ArithmeticExpression);

    protected override bool EqualsCore(SqlExpression other) =>
        other is ArithmeticExpression arithmeticExpression && string.Equals(_operator, arithmeticExpression._operator, StringComparison.Ordinal) && base.EqualsCore(other);

    protected override void AddToHashCode(ref HashCode hashCode)
    {
        hashCode.Add(_operator, StringComparer.Ordinal);
        base.AddToHashCode(ref hashCode);
    }

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
