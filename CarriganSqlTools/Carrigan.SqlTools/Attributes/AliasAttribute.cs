using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Attributes;
[AttributeUsage(AttributeTargets.Property)]
//TODO: Proof read documentation for entire class

/// <summary>
/// Allows for setting a default <c>AS</c> alias on a property.
/// </summary>
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
    /// <summary>
    /// Public getter to get the <c>Alias</c> for the <c>AS</c> clause for a given property.
    /// </summary>
    internal AliasName Name { get; set; }

    /// <summary>
    /// Public constructor
    /// </summary>
    /// <param name="aliasName">name of alias</param>
    public AliasAttribute(string aliasName)
    {
        ArgumentNullException.ThrowIfNull(aliasName, nameof(aliasName));
        if (aliasName == string.Empty)
            throw new ArgumentException("aliasName is an empty string", nameof(aliasName));
        Name = new(aliasName);
    }
}
