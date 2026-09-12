using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Provides the structural column identity used when comparing SQL expression nodes that represent columns.
/// </summary>
internal interface IColumnExpressionIdentity
{
    /// <summary>
    /// Gets the structural column tag represented by the expression.
    /// </summary>
    ColumnTag EqualityColumnTag { get; }
}
