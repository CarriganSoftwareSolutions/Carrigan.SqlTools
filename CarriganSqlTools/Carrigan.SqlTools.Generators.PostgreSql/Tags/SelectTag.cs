using Carrigan.SqlTools.IdentifierTypes;

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
    public SelectTag(PropertyName propertyName, AliasName? aliasName = null)
        : this(SelectTagGenerator.Get<modelT>(propertyName, aliasName))
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
    /// Initializes a strongly typed select tag from the canonical reflected projection resolved by
    /// <see cref="SelectTagGenerator"/>.
    /// </summary>
    /// <param name="selectTag">The resolved projection and alias metadata.</param>
    private SelectTag(SelectTag selectTag)
        : base(selectTag.SqlExpression, selectTag.AliasTag)
    {
    }

}
