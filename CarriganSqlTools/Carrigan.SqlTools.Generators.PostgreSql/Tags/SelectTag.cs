using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Represents a dialect package's concrete SELECT projection tag for a single column, strongly typed to a model class.
/// </summary>
/// <typeparam name="modelT">
/// The model type whose C# properties represent SQL columns or parameters.
/// </typeparam>
public sealed class SelectTag<modelT> : SelectTag where modelT : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTag{modelT}"/> class using the provided property and optional alias names.
    /// </summary>
    /// <param name="propertyName">
    /// The property/column name to project, strongly typed to the model class <typeparamref name="modelT"/>.
    /// </param>
    /// <param name="aliasName">
    /// An optional alias to use for this projection.
    /// </param>
    public SelectTag(PropertyName propertyName, AliasName? aliasName = null) : base(GetColumnInfo(propertyName), GetAliasTag(propertyName, aliasName))
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTag{modelT}"/> class using the provided property name and optional alias name.
    /// </summary>
    /// <param name="propertyName">
    /// The property/column name to project, strongly typed to the model class <typeparamref name="modelT"/>.
    /// </param>
    /// <param name="aliasName">
    /// An alias to use for this projection.
    /// </param>
    public SelectTag(string propertyName, string aliasName) : this(new PropertyName(propertyName), AliasName.New(aliasName))
    {
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTag{modelT}"/> class using the provided property name and alias name.
    /// </summary>
    /// <param name="propertyName">
    /// The property/column name to project, strongly typed to the model class <typeparamref name="modelT"/>.
    /// </param>
    /// <param name="aliasName">
    /// An optional alias to use for this projection.
    /// </param>
    public SelectTag(string propertyName, AliasName? aliasName = null) : this(new PropertyName(propertyName), aliasName)
    {
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTag{modelT}"/> class using the provided property name and optional alias name.
    /// </summary>
    /// <param name="propertyName">
    /// The property/column name to project, strongly typed to the model class <typeparamref name="modelT"/>.
    /// </param>
    /// <param name="aliasName">
    /// An alias to use for this projection.
    /// </param>
    public SelectTag(PropertyName propertyName, string aliasName) : this(propertyName, AliasName.New(aliasName))
    {
    }

    /// <summary>
    /// Retrieves the <see cref="ColumnTag"/> for the specified property name from the reflector cache.
    /// </summary>
    /// <param name="propertyName">
    /// The property/column name to retrieve the <see cref="ColumnTag"/> for, strongly typed to the model class <typeparamref name="modelT"/>.
    /// </param>
    /// <returns>
    ///  The <see cref="ColumnTag"/> corresponding to the specified property name.
    /// </returns>
    private static ColumnTag GetColumnInfo(PropertyName propertyName) =>
        SqlToolsReflectorCache<modelT>.GetColumnsFromProperty(DialectStatics.SupportedTypes, propertyName).ColumnTag;

    /// <summary>
    /// Retrieves the <see cref="AliasTag"/> for the specified property name and optional alias name from the reflector cache.
    /// </summary>
    /// <param name="propertyName">
    /// The property/column name to retrieve the <see cref="AliasTag"/> for, strongly typed to the model class <typeparamref name="modelT"/>.
    /// </param>
    /// <param name="aliasName">
    /// An optional alias name for the projection.
    /// </param>
    /// <returns>
    /// The <see cref="AliasTag"/> corresponding to the specified property name and alias name.
    /// </returns>
    private static AliasTag? GetAliasTag(PropertyName propertyName, AliasName? aliasName) =>
        aliasName is null
            ? SelectTag<modelT>.GetAliasTagFromColumnInfo(SqlToolsReflectorCache<modelT>.GetColumnsFromProperty(DialectStatics.SupportedTypes, propertyName))
            : AliasTag.New(aliasName);

    /// <summary>
    /// Retrieves the <see cref="AliasTag"/> from the provided <see cref="ColumnInfo"/> instance.
    /// </summary>
    /// <param name="columnInfo">
    ///     The <see cref="ColumnInfo"/> instance from which to retrieve the <see cref="AliasTag"/>.
    /// </param>
    /// <returns>
    /// The <see cref="AliasTag"/> corresponding to the specified property name and alias name.
    /// </returns>
    private static AliasTag? GetAliasTagFromColumnInfo(ColumnInfo columnInfo) =>
        columnInfo.SelectTag?.AliasTag ?? AliasTag.New(columnInfo.AliasName);
}

/// <summary>
/// Represents a dialect package's concrete SELECT projection tag for a single column.
/// </summary>
public class SelectTag : SelectTagBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTag"/> class using reflection-resolved column and alias tags.
    /// </summary>
    /// <param name="columnTag">The fully qualified column identifier to project.</param>
    /// <param name="aliasTag">An optional alias to use for this projection.</param>
    internal SelectTag(ColumnTag columnTag, AliasTag? aliasTag = null) : base(columnTag, aliasTag)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTag"/> class using the provided SQL expression and optional alias name.
    /// </summary>
    /// <param name="sqlExpression">The SQL expression to project.</param>
    /// <param name="aliasName">An optional alias to use for this projection.</param>
    public SelectTag(SqlExpression sqlExpression, AliasName aliasName) : base(sqlExpression, AliasTag.New(aliasName))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTag"/> class using the provided SQL expression and optional alias name.
    /// </summary>
    /// <param name="sqlExpression">The SQL expression to project.</param>
    /// <param name="aliasName">An optional alias to use for this projection.</param>
    [ExternalOnly]
    public SelectTag(SqlExpression sqlExpression, string aliasName) : this(sqlExpression, new AliasName(aliasName))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTag"/> class using the provided SQL expression and optional alias name.
    /// </summary>
    /// <param name="sqlExpression">The SQL expression to project.</param>
    private SelectTag(SqlExpression sqlExpression) : base(sqlExpression, null)
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
    /// Creates a new <see cref="SelectTag"/> instance with the same column as the current instance but without any alias.
    /// </summary>
    public override SelectTag WithNoAlias() =>
        new(SqlExpression);

    /// <summary>
    /// Implicitly creates a new <see cref="SelectTagsBase"/> instance from a single <see cref="SelectTag"/>.
    /// </summary>
    /// <param name="selectTag">
    /// The <see cref="SelectTag"/> instance to convert into a <see cref="SelectTagsBase"/> collection.
    /// </param>
    public static implicit operator SelectTagsBase(SelectTag selectTag) =>
        new SelectTags(selectTag);
}
