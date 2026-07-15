using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Maps a projection property to an SQL <c>MIN(column)</c> aggregate expression.
/// </summary>
/// <typeparam name="T">The model type containing the source column.</typeparam>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class MinAttribute<T> : SelectTagAttribute<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MinAttribute{T}"/> class.
    /// </summary>
    /// <param name="propertyName">The C# property name representing the source column.</param>
    /// <param name="aliasName">
    /// The optional SQL result-column alias. When omitted, the decorated projection property's
    /// mapped result-column name is used.
    /// </param>
    [ExternalOnly]
    public MinAttribute(string propertyName, string? aliasName = null)
        : this(new PropertyName(propertyName), AliasName.New(aliasName))
    {
    }

    /// <summary>
    /// Initializes a new instance from strongly typed identifier values.
    /// </summary>
    internal MinAttribute(PropertyName propertyName, AliasName? aliasName = null)
        : base(propertyName, aliasName)
    {
        AliasTag = AliasTag.New(aliasName);
        SelectTag = new ReflectedSelectTag(new Min(new ColumnTagExpression(ColumnTag!)), AliasTag);
        UseDecoratedPropertyNameAsDefaultAlias = true;
    }
}
