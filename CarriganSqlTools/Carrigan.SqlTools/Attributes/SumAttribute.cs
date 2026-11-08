using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Maps a projection property to an SQL <c>SUM(column)</c> aggregate expression.
/// </summary>
/// <typeparam name="T">The model type containing the source column.</typeparam>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class SumAttribute<T> : SelectTagAttribute<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SumAttribute{T}"/> class.
    /// </summary>
    /// <param name="propertyName">The C# property name representing the source column.</param>
    /// <param name="aliasName">
    /// The optional SQL result-column alias. When omitted, the decorated projection property's
    /// mapped result-column name is used.
    /// </param>
    [ExternalOnly]
    public SumAttribute(string propertyName, string? aliasName = null)
        : this(new PropertyName(propertyName), false, ColumnAlias.New(aliasName))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SumAttribute{T}"/> class.
    /// </summary>
    /// <param name="propertyName">The C# property name representing the source column.</param>
    /// <param name="distinct">Whether duplicate input values are removed before summing.</param>
    /// <param name="aliasName">The optional SQL result-column alias.</param>
    [ExternalOnly]
    public SumAttribute(string propertyName, bool distinct, string? aliasName = null)
        : this(new PropertyName(propertyName), distinct, ColumnAlias.New(aliasName))
    {
    }

    /// <summary>
    /// Initializes a new instance from strongly typed identifier values.
    /// </summary>
    internal SumAttribute(PropertyName propertyName, ColumnAlias? aliasName = null)
        : this(propertyName, false, aliasName)
    {
    }

    /// <summary>
    /// Initializes a new instance from strongly typed identifier values.
    /// </summary>
    internal SumAttribute(PropertyName propertyName, bool distinct, ColumnAlias? aliasName = null)
        : base(propertyName, aliasName)
    {
        ColumnAlias = aliasName;
        SelectTag = new SelectTag(new Sum(SelectTag!.SqlExpression, distinct), ColumnAlias);
        UseDecoratedPropertyNameAsDefaultAlias = true;
    }
}
