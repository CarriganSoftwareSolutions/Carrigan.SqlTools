using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.PostgreSql;

/// <summary>
/// Builds subquery options for the specified model type.
/// </summary>
/// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
public sealed record SubqueryBuilder<T> : QueryBuilders.SubqueryBuilderBase<T> where T : class
{
    /// <summary>
    /// Initializes a subquery builder with the model table as its FROM source.
    /// </summary>
    public SubqueryBuilder() =>
        From = new From<T>();

    /// <summary>
    /// Initializes a subquery builder with an optional alias for the model table.
    /// </summary>
    /// <param name="tableAlias">The optional table alias.</param>
    public SubqueryBuilder(TableAlias? tableAlias) =>
        From = new From<T>(tableAlias);
}
