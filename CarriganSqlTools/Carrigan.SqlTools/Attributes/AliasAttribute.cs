using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Attributes;
[AttributeUsage(AttributeTargets.Property)]

/// <summary>
/// Specifies a default SQL alias (the <c>AS</c> identifier) for a property when it is projected
/// in a SELECT list. When present, SQL generation uses this alias unless an explicit override
/// is provided at call time.
/// </summary>
/// <remarks>
/// The value is stored as a strongly typed <see cref="AliasName"/> and is typically consumed by
/// reflection-driven components (e.g., the column/selection caches) to derive a result set
/// column name. Library consumers can still override the alias per query.
/// </remarks>
/// <example>
/// <code language="csharp"><![CDATA[
/// internal class AliasEntity
/// {
///      public int Id { get; set; }
///      [Alias("AnAlias")]
///      public string? TestColumn { get; set; }
///      public string? NoAlias { get; set; }
/// }
/// SelectTags tags = SelectTags.GetMany<AliasEntity>
/// (
///     nameof(AliasEntity.Id),
///     nameof(AliasEntity.TestColumn),
///     nameof(AliasEntity.NoAlias)
/// );
/// 
/// SqlGenerator<AliasEntity> generator = new();
/// SqlQuery query = generator.Select(tags, null, null, null, null)
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [AliasEntity].[Id], [AliasEntity].[TestColumn] AS AnAlias, [AliasEntity].[NoAlias] 
/// FROM [AliasEntity]
/// ]]></code>
/// </example>
public class AliasAttribute : Attribute
{
    [SqlDateTime2()]
    public byte[] Test { get; set; }
    /// <summary>
    /// Gets the alias name to apply in the <c>AS</c> clause for the decorated property.
    /// </summary>
    internal AliasName Name { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AliasAttribute"/> class.
    /// </summary>
    /// <param name="aliasName">The alias to use for the property’s <c>AS</c> clause.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="aliasName"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="aliasName"/> is an empty string.</exception>
    public AliasAttribute(string aliasName)
    {
        ArgumentNullException.ThrowIfNull(aliasName, nameof(aliasName));
        if (aliasName == string.Empty)
            throw new ArgumentException("aliasName is an empty string", nameof(aliasName));
        Name = new(aliasName);
    }
}
