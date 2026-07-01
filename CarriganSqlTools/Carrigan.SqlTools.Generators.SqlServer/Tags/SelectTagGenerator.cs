using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.GroupByClause;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Generates dialect select tags from reflection metadata.
/// </summary>
public static class SelectTagGenerator
{
    /// <summary>
    /// Creates a select tag for the specified SQL expression.
    /// </summary>
    /// <param name="sqlExpression">The SQL expression to select.</param>
    /// <param name="aliasName">The SQL alias name to apply.</param>
    public static SelectTag Get(SqlExpression sqlExpression, AliasName? aliasName = null) =>
        new(sqlExpression, aliasName);

    /// <summary>
    /// Creates a select tag for the specified group-by item.
    /// </summary>
    /// <param name="groupBy">The group-by item to select.</param>
    public static SelectTag Get(GroupByBase groupBy)
    {
        ArgumentNullException.ThrowIfNull(groupBy, nameof(groupBy));

        return new(groupBy.ColumnInfo.ColumnTag);
    }

    /// <summary>
    /// Creates select tags for the specified group-by items.
    /// </summary>
    /// <param name="groupBys">The group-by items to select.</param>
    public static SelectTags GetMany(GroupBysBase groupBys)
    {
        ArgumentNullException.ThrowIfNull(groupBys, nameof(groupBys));

        return new(groupBys.AsEnumerable().Select(Get));
    }

    /// <summary>
    /// Creates a select tag for the specified property on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="propertyName">The C# property name representing the SQL column or parameter.</param>
    /// <param name="aliasName">The SQL alias name to apply.</param>
    public static SelectTag Get<T>(PropertyName propertyName, AliasName? aliasName = null) where T : class =>
        SqlToolsReflectorCache<T>
            .CreateSelectTag
            (
                propertyName,
                DialectStatics.SupportedTypes,
                static (columnTag, aliasTag) => new SelectTag(new ColumnTagExpression(columnTag), aliasTag),
                aliasName
            );

    /// <summary>
    /// Creates a select tag for the specified property on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="propertyName">The C# property name representing the SQL column or parameter.</param>
    /// <param name="aliasName">The SQL alias name to apply.</param>
    [ExternalOnly]
    public static SelectTag Get<T>(string propertyName, string? aliasName = null) where T : class =>
        Get<T>(new PropertyName(propertyName), AliasName.New(aliasName));

    /// <summary>
    /// Creates select tags for the specified properties on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="properties">The C# property names representing SQL columns or parameters.</param>
    public static SelectTags GetMany<T>(params IEnumerable<PropertyName> properties) where T : class
    {
        ArgumentNullException.ThrowIfNull(properties, nameof(properties));

        return new(properties.Select(property => Get<T>(property)));
    }

    /// <summary>
    /// Creates select tags for the specified properties on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="properties">The C# property names representing SQL columns or parameters.</param>
    [ExternalOnly]
    public static SelectTags GetMany<T>(params IEnumerable<string> properties) where T : class =>
        GetMany<T>(properties.Select(name => new PropertyName(name)));

    /// <summary>
    /// Creates select tags for every supported mapped property on <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    public static SelectTags GetAll<T>() where T : class =>
        GetMany<T>(SqlToolsReflectorCache<T>.GetColumnInfo(DialectStatics.SupportedTypes).Select(static column => column.PropertyName));
}
