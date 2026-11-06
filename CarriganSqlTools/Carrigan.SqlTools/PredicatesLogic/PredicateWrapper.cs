using Carrigan.Core.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents a transparent adapter that allows a <see cref="SqlExpression"/> to be used where a <see cref="Predicates"/> expression is required.
/// </summary>
internal sealed class PredicateWrapper : Predicates
{
    /// <summary>
    /// The underlying expression being treated as a predicate without predicate-type validation.
    /// </summary>
    private readonly SqlExpression _sqlExpression;

    /// <summary>
    /// Initializes a new instance of the <see cref="PredicateWrapper"/> class.
    /// </summary>
    /// <param name="sqlExpression">The expression to treat as a predicate without predicate-type validation.</param>
    internal PredicateWrapper(SqlExpression sqlExpression)
        : base(sqlExpression is not null ? [sqlExpression] : throw new ArgumentNullException(nameof(sqlExpression))) =>
        _sqlExpression = sqlExpression;

    /// <summary>
    /// Uses the wrapped expression as the semantic identity of this transparent adapter.
    /// </summary>
    protected override SqlExpression EqualityExpression =>
        _sqlExpression;

    /// <summary>
    /// Gets the leaf tables represented by the underlying expression.
    /// </summary>
    public override IEnumerable<TableTag> LeafTables =>
        _sqlExpression.LeafTables;

    /// <summary>
    /// Converts the underlying expression to SQL fragments for the specified dialect.
    /// </summary>
    public override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect) =>
        _sqlExpression.ToSqlFragments(dialect);
}
