using Carrigan.Core.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents a transparent adapter that allows a <see cref="SqlExpression"/> to be used where a <see cref="NumericExpression"/> is required.
/// </summary>
internal sealed class NumericExpressionWrapper : NumericExpression
{
    /// <summary>
    /// The underlying expression being treated as numeric without numeric-type validation.
    /// </summary>
    private readonly SqlExpression _sqlExpression;

    /// <summary>
    /// Initializes a new instance of the <see cref="NumericExpressionWrapper"/> class.
    /// </summary>
    /// <param name="sqlExpression">The expression to treat as numeric without numeric-type validation.</param>
    [TypeSafetyLoss]
    internal NumericExpressionWrapper(SqlExpression sqlExpression)
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
