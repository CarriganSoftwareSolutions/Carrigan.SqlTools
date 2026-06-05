using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Represents a dialect package's concrete collection of SELECT projection tags.
/// </summary>
public sealed class SelectTags : SelectTagsBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SelectTags"/> class.
    /// </summary>
    /// <param name="selectTags">The select tags to include in this collection.</param>
    public SelectTags(params IEnumerable<SelectTagBase> selectTags) : base(selectTags)
    {
    }

    /// <summary>
    /// Executes the New operation.
    /// </summary>
    /// <param name="selectTags">The select tags to append.</param>
    /// <returns>The result of the New operation.</returns>
    protected override SelectTags New(params IEnumerable<SelectTagBase> selectTags) =>
        new(selectTags);

    /// <summary>
    /// Executes the <c>NewSelectTag&lt;T&gt;</c> operation.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="propertyName">The C# property name representing the SQL column or parameter.</param>
    /// <param name="aliasName">The SQL alias name to apply.</param>
    /// <returns>The result of the <c>NewSelectTag&lt;T&gt;</c> operation.</returns>
    protected override SelectTag NewSelectTag<T>(PropertyName propertyName, AliasName? aliasName = null) =>
        SelectTagGenerator.Get<T>(propertyName, aliasName);

    /// <summary>
    /// Executes the <c>NewSelectTags&lt;T&gt;</c> operation.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="properties">The C# property names representing SQL columns or parameters.</param>
    /// <returns>The result of the <c>NewSelectTags&lt;T&gt;</c> operation.</returns>
    protected override SelectTags NewSelectTags<T>(params IEnumerable<PropertyName> properties) =>
        SelectTagGenerator.GetMany<T>(properties);

    /// <summary>
    /// Creates a new collection with the supplied item appended.
    /// </summary>
    /// <param name="selectTag">The select tag to append.</param>
    /// <returns>The result of the Append operation.</returns>
    public override SelectTags Append(SelectTagBase selectTag) =>
        new(All().Append(selectTag));

    /// <summary>
    /// Executes the <c>Append&lt;T&gt;</c> operation.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="propertyName">The C# property name representing the SQL column or parameter.</param>
    /// <param name="aliasName">The SQL alias name to apply.</param>
    /// <returns>The result of the <c>Append&lt;T&gt;</c> operation.</returns>
    public override SelectTags Append<T>(PropertyName propertyName, AliasName? aliasName = null) =>
        Append(SelectTagGenerator.Get<T>(propertyName, aliasName));

    /// <summary>
    /// Executes the <c>Append&lt;T&gt;</c> operation.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="property">The C# property name representing the SQL column or parameter.</param>
    /// <param name="aliasName">The SQL alias name to apply.</param>
    /// <returns>The result of the <c>Append&lt;T&gt;</c> operation.</returns>
    [ExternalOnly]
    public override SelectTags Append<T>(string property, string? aliasName = null) =>
        Append<T>(new PropertyName(property), AliasName.New(aliasName));

    /// <summary>
    /// Creates a new collection with the supplied items appended.
    /// </summary>
    /// <param name="selectTags">The select tags to append.</param>
    /// <returns>The result of the Concat operation.</returns>
    public override SelectTags Concat(SelectTagsBase selectTags) =>
        new(All().Concat(selectTags.All()));

    /// <summary>
    /// Creates a new collection with the supplied items appended.
    /// </summary>
    /// <param name="selectTags">The select tags to append.</param>
    /// <returns>The result of the Concat operation.</returns>
    public override SelectTags Concat(params IEnumerable<SelectTagBase> selectTags) =>
        new(All().Concat(selectTags));

    /// <summary>
    /// Executes the <c>Concat&lt;T&gt;</c> operation.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="properties">The C# property names representing SQL columns or parameters.</param>
    /// <returns>The result of the <c>Concat&lt;T&gt;</c> operation.</returns>
    public override SelectTags Concat<T>(params IEnumerable<PropertyName> properties) =>
        Concat((SelectTagsBase)SelectTagGenerator.GetMany<T>(properties));

    /// <summary>
    /// Executes the <c>Concat&lt;T&gt;</c> operation.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <param name="properties">The C# property names representing SQL columns or parameters.</param>
    /// <returns>The result of the <c>Concat&lt;T&gt;</c> operation.</returns>
    [ExternalOnly]
    public override SelectTags Concat<T>(params IEnumerable<string> properties) =>
        Concat<T>(properties.Select(name => new PropertyName(name)));

    /// <summary>
    /// Executes the operator operation.
    /// </summary>
    /// <param name="selectTag">The select tag to append.</param>
    /// <returns>The result of the operator operation.</returns>
    public static implicit operator SelectTags(SelectTag selectTag) =>
        new(selectTag);
}
