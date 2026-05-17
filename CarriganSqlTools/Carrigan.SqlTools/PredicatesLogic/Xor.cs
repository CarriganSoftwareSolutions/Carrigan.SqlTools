using Carrigan.SqlTools.Dialects;
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
    public Xor(Predicates left, Predicates right) : base(left, right)
    {
    }

    /// <summary>
    /// Produces the SQL fragment represented by this Dialect operator and its operands.
    /// </summary>
    /// <param name="prefix">
    /// A disambiguation prefix accumulated during predicate-tree traversal.
    /// Used to ensure parameter names are unique when duplicates exist.
    /// </param>
    /// <param name="duplicates">
    /// The set of user-supplied <see cref="ParameterTag"/> values detected as duplicates.
    /// Leaf nodes use this to decide if the <paramref name="prefix"/> should be applied.
    /// </param>
    /// <returns>
    /// A SQL fragment in the form <c>(&lt;left-sql&gt; OP &lt;right-sql&gt;)</c>, e.g.,
    /// <c>([T].[Col] = @Parameter_Col)</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="prefix"/> or <paramref name="branchName"/> or <paramref name="duplicates"/> is <c>null</c>.
    /// </exception>
    internal override IEnumerable<SqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        yield return new SqlFragmentText("(");

        foreach (SqlFragment fragment in _left.ToSqlFragments(dialect))
            yield return fragment;

        yield return SqlFragment.Space;

        yield return dialect.GetXOrSymbol();

        yield return SqlFragment.Space;

        foreach (SqlFragment fragment in _right.ToSqlFragments(dialect))
            yield return fragment;

        yield return new SqlFragmentText(")");
    }
}