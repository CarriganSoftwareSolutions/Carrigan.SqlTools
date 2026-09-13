using Carrigan.Core.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents a wrapper for a <see cref="SqlExpression"/> that is treated as a <see cref="NumericExpression"/>.
/// </summary>
internal sealed class PredicateWrapper : Predicates
{
    /// <summary>
    /// The underlying <see cref="SqlExpression"/> that is wrapped and treated as a <see cref="NumericExpression"/>.
    /// </summary>
    private readonly SqlExpression _sqlExpression;

    /// <summary>
    /// Initializes a new instance of the <see cref="PredicateWrapper"/> class that wraps the specified <see cref="SqlExpression"/>.
    /// </summary>
    /// <param name="sqlExpression">
    /// The <see cref="SqlExpression"/> to wrap and treat as a <see cref="Predicates"/> expression.
    /// </param>
    [TypeSafetyLoss]
    internal PredicateWrapper(SqlExpression sqlExpression)
        : base([])
    {
        ArgumentNullException.ThrowIfNull(sqlExpression);

        _sqlExpression = sqlExpression;
    }

    /// <summary>
    /// Gets the leaf tables represented by the underlying <see cref="SqlExpression"/>.
    /// </summary>
    public override IEnumerable<TableTag> LeafTables =>
        _sqlExpression.LeafTables;

    /// <summary>
    /// Determines whether the specified object is equal to the current <see cref="PredicateWrapper"/> instance.
    /// </summary>
    /// <param name="obj">
    /// The object to compare with the current instance. This can be another <see cref="PredicateWrapper"/> or a <see cref="SqlExpression"/>.
    /// </param>
    /// <returns></returns>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;

        if (obj is PredicateWrapper numericExpressionWrapper)
            return _sqlExpression.Equals(numericExpressionWrapper._sqlExpression);

        return _sqlExpression.Equals(obj);
    }

    /// <summary>
    /// Returns a hash code for the current <see cref="PredicateWrapper"/> instance based on the underlying <see cref="SqlExpression"/>.
    /// </summary>
    /// <returns>
    /// A hash code for the current instance.
    /// </returns>
    public override int GetHashCode() =>
        _sqlExpression.GetHashCode();

    /// <summary>
    /// Converts the underlying <see cref="SqlExpression"/> to SQL fragments for the specified SQL dialect.
    /// </summary>
    /// <param name="dialect">
    /// The SQL dialect to use for generating the SQL fragments.
    /// </param>
    /// <returns>
    /// An enumerable collection of <see cref="ISqlFragment"/> representing the SQL fragments for the underlying <see cref="SqlExpression"/>.
    /// </returns>
    public override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect) =>
        _sqlExpression.ToSqlFragments(dialect);

    /// <summary>
    /// Adds the underlying <see cref="SqlExpression"/> to the hash code computation for the current <see cref="PredicateWrapper"/> instance.
    /// </summary>
    /// <param name="hashCode">
    /// The <see cref="HashCode"/> instance to which the underlying <see cref="SqlExpression"/> will be added for hash code computation.
    /// </param>
    protected override void AddToHashCode(ref HashCode hashCode) =>
        hashCode.Add(_sqlExpression);
}