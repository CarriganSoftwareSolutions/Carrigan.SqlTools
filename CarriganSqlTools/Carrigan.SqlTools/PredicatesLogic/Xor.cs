using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents SQL’s logical <c>XOR</c> operator for combining two predicate expressions
/// in <c>WHERE</c> or <c>JOIN</c> clauses.
/// </summary>
public class Xor : DialectOperator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Xor"/> class,
    /// representing SQL’s logical <c>XOR</c> (exclusive OR) operator.
    /// </summary>
    /// <param name="left">The left-hand predicate operand.</param>
    /// <param name="right">The right-hand predicate operand.</param>
    public Xor(SqlExpression left, SqlExpression right) : base(left, right)
    {
    }

    /// <summary>
    /// Produces the SQL fragment represented by this Dialect operator and its operands.
    /// </summary>
    /// <returns>
    /// A SQL fragment in the form <c>(&lt;left-sql&gt; OP &lt;right-sql&gt;)</c>, e.g.,
    /// <c>([T].[Col] = @Parameter_Col)</c>.
    /// </returns>
    internal override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        yield return new SqlFragmentText("(");

        foreach (ISqlFragment fragment in _left.ToSqlFragments(dialect))
            yield return fragment;

        yield return ISqlFragment.Space;

        yield return dialect.GetXOrSymbol();

        yield return ISqlFragment.Space;

        foreach (ISqlFragment fragment in _right.ToSqlFragments(dialect))
            yield return fragment;

        yield return new SqlFragmentText(")");
    }
}