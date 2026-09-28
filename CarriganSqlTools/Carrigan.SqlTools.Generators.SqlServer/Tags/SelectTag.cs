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
