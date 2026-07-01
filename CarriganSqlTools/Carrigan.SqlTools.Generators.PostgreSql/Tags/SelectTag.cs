using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Expressions;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Represents a dialect package's concrete SELECT projection tag for a single column.
/// </summary>
public sealed class SelectTag : SelectTagBase
{

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTag"/> class using the provided property and optional alias names.
    /// </summary>
    /// <param name="propertyName">The property/column name to project.</param>
    /// <param name="aliasName">An optional alias to use for this projection.</param>
    public SelectTag(PropertyName propertyName, AliasName? aliasName = null) : base(propertyName, aliasName)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTag"/> class using the provided property and optional alias names.
    /// </summary>
    /// <param name="propertyName">The property/column name to project.</param>
    /// <param name="aliasName">An optional alias to use for this projection.</param>
    [ExternalOnly]
    public SelectTag(string propertyName, string? aliasName = null) : this(new PropertyName(propertyName), AliasName.New(aliasName))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTag"/> class using the provided SQL expression and optional alias name.
    /// </summary>
    /// <param name="sqlExpression">The SQL expression to project.</param>
    /// <param name="aliasName">An optional alias to use for this projection.</param>
    public SelectTag(SqlExpression sqlExpression, AliasName? aliasName = null) : base(sqlExpression, AliasTag.New(aliasName))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTag"/> class using the provided SQL expression and optional alias name.
    /// </summary>
    /// <param name="sqlExpression">The SQL expression to project.</param>
    /// <param name="aliasName">An optional alias to use for this projection.</param>
    [ExternalOnly]
    public SelectTag(SqlExpression sqlExpression, string? aliasName) : this(sqlExpression, AliasName.New(aliasName))
    {
    }

    /// <summary>
    /// Creates a <see cref="SelectTags"/> collection containing this tag and the specified tag.
    /// </summary>
    /// <param name="selectTag">The select tag to append.</param>
    /// <returns>A new <see cref="SelectTags"/> collection containing both tags.</returns>
    public SelectTags Append(SelectTagBase selectTag) =>
        new SelectTags(this).Append(selectTag);

    /// <summary>
    /// Creates a <see cref="SelectTags"/> collection containing this tag and a tag for the specified property.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="propertyName">The C# property name representing the SQL column or parameter.</param>
    /// <param name="aliasName">The SQL alias name to apply.</param>
    public SelectTags Append<T>(PropertyName propertyName, AliasName? aliasName = null) where T : class =>
        new SelectTags(this).Append<T>(propertyName, aliasName);

    /// <summary>
    /// Creates a <see cref="SelectTags"/> collection containing this tag and a tag for the specified property.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="propertyName">The C# property name representing the SQL column or parameter.</param>
    /// <param name="aliasName">The SQL alias name to apply.</param>
    [ExternalOnly]
    public SelectTags Append<T>(string propertyName, string? aliasName = null) where T : class =>
        Append<T>(new PropertyName(propertyName), AliasName.New(aliasName));

    /// <summary>
    /// Creates a <see cref="SelectTags"/> collection containing this tag and the supplied tags.
    /// </summary>
    /// <param name="selectTags">The select tags to append.</param>
    public SelectTags Concat(SelectTagsBase selectTags) =>
        new SelectTags(this).Concat(selectTags);

    /// <summary>
    /// Creates a <see cref="SelectTags"/> collection containing this tag and the supplied tags.
    /// </summary>
    /// <param name="selectTags">The select tags to append.</param>
    public SelectTags Concat(params IEnumerable<SelectTagBase> selectTags) =>
        new SelectTags(this).Concat(selectTags);

    /// <summary>
    /// Creates a <see cref="SelectTags"/> collection containing this tag and tags for the specified properties.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="properties">The C# property names representing SQL columns or parameters.</param>
    public SelectTags Concat<T>(params IEnumerable<PropertyName> properties) where T : class =>
        new SelectTags(this).Concat<T>(properties);

    /// <summary>
    /// Creates a <see cref="SelectTags"/> collection containing this tag and tags for the specified properties.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="properties">The C# property names representing SQL columns or parameters.</param>
    [ExternalOnly]
    public SelectTags Concat<T>(params IEnumerable<string> properties) where T : class =>
        Concat<T>(properties.Select(name => new PropertyName(name)));

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTag"/> class using an already resolved expression and alias tag.
    /// </summary>
    /// <param name="sqlExpression">The SQL expression to project.</param>
    /// <param name="aliasTag">An optional alias to use for this projection.</param>
    internal SelectTag(SqlExpression sqlExpression, AliasTag? aliasTag) : base(sqlExpression, aliasTag)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTag"/> class using reflection-resolved column and alias tags.
    /// </summary>
    /// <param name="columnTag">The fully qualified column identifier to project.</param>
    /// <param name="aliasTag">An optional alias to use for this projection.</param>
    internal SelectTag(ColumnTag columnTag, AliasTag? aliasTag = null) : base(columnTag, aliasTag)
    {
    }

    /// <summary>
    /// Creates a new <see cref="SelectTag"/> instance with the same column as the current instance but without any alias.
    /// </summary>
    public override SelectTag WithNoAlias() =>
        new(SqlExpression);
}
