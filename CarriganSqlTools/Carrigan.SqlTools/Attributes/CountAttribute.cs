using Carrigan.SqlTools.AggregateLogic;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Maps a projection property to an SQL <c>COUNT(*)</c> aggregate expression.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class CountAttribute : SelectTagAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CountAttribute"/> class.
    /// </summary>
    /// <param name="aliasName">
    /// The optional SQL result-column alias. When omitted, the decorated projection property's
    /// mapped result-column name is used.
    /// </param>
    [ExternalOnly]
    public CountAttribute(string? aliasName = null)
        : this(AliasName.New(aliasName))
    {
    }

    /// <summary>
    /// Initializes a new instance from a strongly typed alias value.
    /// </summary>
    internal CountAttribute(AliasName? aliasName)
    {
        AliasTag = AliasTag.New(aliasName);
        SelectTag = new ReflectedSelectTag(new Count(), AliasTag);
        UseDecoratedPropertyNameAsDefaultAlias = true;
    }
}

/// <summary>
/// Maps a projection property to an SQL <c>COUNT(column)</c> aggregate expression.
/// </summary>
/// <typeparam name="T">The model type containing the source column.</typeparam>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class CountAttribute<T> : SelectTagAttribute<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CountAttribute{T}"/> class.
    /// </summary>
    /// <param name="propertyName">The C# property name representing the source column.</param>
    /// <param name="aliasName">
    /// The optional SQL result-column alias. When omitted, the decorated projection property's
    /// mapped result-column name is used.
    /// </param>
    [ExternalOnly]
    public CountAttribute(string propertyName, string? aliasName = null)
        : this(new PropertyName(propertyName), AliasName.New(aliasName))
    {
    }

    /// <summary>
    /// Initializes a new instance from strongly typed identifier values.
    /// </summary>
    internal CountAttribute(PropertyName propertyName, AliasName? aliasName = null)
        : base(propertyName, aliasName)
    {
        AliasTag = AliasTag.New(aliasName);
        SelectTag = new ReflectedSelectTag(new Count(new ColumnTagExpression(ColumnTag!)), AliasTag);
        UseDecoratedPropertyNameAsDefaultAlias = true;
    }
}
