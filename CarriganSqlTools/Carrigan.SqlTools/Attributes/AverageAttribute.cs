using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Maps a projection property to an SQL <c>AVG(column)</c> aggregate expression.
/// </summary>
/// <typeparam name="T">The model type containing the source column.</typeparam>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class AverageAttribute<T> : SelectTagAttribute<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AverageAttribute{T}"/> class.
    /// </summary>
    /// <param name="propertyName">The C# property name representing the source column.</param>
    /// <param name="aliasName">
    /// The optional SQL result-column alias. When omitted, the decorated projection property's
    /// mapped result-column name is used.
    /// </param>
    [ExternalOnly]
    public AverageAttribute(string propertyName, string? aliasName = null)
        : this(new PropertyName(propertyName), false, ColumnAlias.New(aliasName))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AverageAttribute{T}"/> class.
    /// </summary>
    /// <param name="propertyName">The C# property name representing the source column.</param>
    /// <param name="distinct">Whether duplicate input values are removed before averaging.</param>
    /// <param name="aliasName">The optional SQL result-column alias.</param>
    [ExternalOnly]
    public AverageAttribute(string propertyName, bool distinct, string? aliasName = null)
        : this(new PropertyName(propertyName), distinct, ColumnAlias.New(aliasName))
    {
    }

    /// <summary>
    /// Initializes a new instance from strongly typed identifier values.
    /// </summary>
    internal AverageAttribute(PropertyName propertyName, ColumnAlias? aliasName = null)
        : this(propertyName, false, aliasName)
    {
    }

    /// <summary>
    /// Initializes a new instance from strongly typed identifier values.
    /// </summary>
    internal AverageAttribute(PropertyName propertyName, bool distinct, ColumnAlias? aliasName = null)
        : base(propertyName, aliasName)
    {
        AliasTag = ColumnAliasTag.New(aliasName);
        SelectTag = new SelectTag(new Average(SelectTag!.SqlExpression, distinct), AliasTag);
        UseDecoratedPropertyNameAsDefaultAlias = true;
    }
}
