using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.SqlGenerators;
using System.Reflection.Emit;

namespace Carrigan.SqlTools.Attributes;


/// <summary>
/// Specifies a default SQL alias (the <c>AS</c> identifier) for a property when it is projected
/// in a SELECT list. When present, SQL generation uses this alias unless an explicit override
/// is provided at call time.
/// </summary>
/// <remarks>
/// <para>
/// The value is stored as a strongly typed <see cref="AliasName"/> and is typically consumed by
/// reflection-driven components (e.g., the column/selection caches) to derive a result set
/// column name. Library consumers can still override the alias per query.
/// </para>
/// <para>
/// This attribute enforces only null and empty-string validation. SQL identifier rules
/// (e.g., whitespace rules, invalid characters, reserved words, and length constraints)
/// are validated by the SQL generator.
/// </para>
/// </remarks>
/// <example>
/// <code language="csharp"><![CDATA[
/// using Carrigan.SqlTools.Base.Tests.Helpers;
/// using Carrigan.SqlTools.Base.Tests.TestEntities;
/// using Carrigan.SqlTools.Base.Tests.TestEntities.Attributes;
/// using Carrigan.SqlTools.SqlGenerators;
/// using Carrigan.SqlTools.PostgreSql;
/// using Carrigan.SqlTools.SqlServer;
/// using Carrigan.SqlTools.Tags;
///
/// internal class AliasEntity
/// {
///     public int Id { get; set; }
///
///     [Alias("AnAlias")]
///     public string? TestColumn { get; set; }
///
///     public string? NoAlias { get; set; }
/// }
///
/// SelectTags tags = SelectTagGenerator.GetMany<AliasEntity>
/// (
///     nameof(AliasEntity.Id),
///     nameof(AliasEntity.TestColumn),
///     nameof(AliasEntity.NoAlias)
/// );
/// 
/// SqlGenerator<AliasEntity> generator = new();
/// SelectBuilder<AliasEntity> selectBuilder = new()
/// {
///     Selects = tags
/// };
/// 
/// SqlQuery query = generator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// --PostgreSql
/// SELECT "AliasEntity"."Id", "AliasEntity"."TestColumn" AS "AnAlias", "AliasEntity"."NoAlias" 
/// FROM "AliasEntity"
/// 
/// --SqlServer
/// SELECT [AliasEntity].[Id], [AliasEntity].[TestColumn] AS [AnAlias], [AliasEntity].[NoAlias]
/// FROM [AliasEntity]
/// ]]></code>
/// </example>///
///
/// <example>
/// <code language="csharp"><![CDATA[
/// using Carrigan.SqlTools.Base.Tests.Helpers;
/// using Carrigan.SqlTools.Base.Tests.TestEntities;
/// using Carrigan.SqlTools.Base.Tests.TestEntities.Attributes;
/// using Carrigan.SqlTools.SqlGenerators;
/// using Carrigan.SqlTools.PostgreSql;
/// using Carrigan.SqlTools.SqlServer;
/// using Carrigan.SqlTools.Tags;
///
/// [Identifier("Email", "schema")]
/// internal class EmailModel
/// {
///     [PrimaryKey]
///     public int Id { get; set; }
///     public int CustomerId { get; set; }
///     [Identifier("Email")]
///     public string? EmailAddress { get; set; }
/// }
///
/// SqlGenerator<EmailModel> emailGenerator = new();
/// EmailModel email = new()
/// {
///     Id = 10,
///     CustomerId = 313,
///     EmailAddress = "Exterminate@GenericTinCanLand.gov"
/// };
/// SqlQuery query = emailGenerator.UpdateById(email);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// --PostgreSql
/// UPDATE "schema"."Email" 
/// SET "CustomerId" = $1, "Email" = $2 
/// WHERE "Id" = $3;
/// 
/// --SqlServer
/// UPDATE [schema].[Email]
/// SET [CustomerId] = @CustomerId_1, [Email] = @Email_2 
/// WHERE [Id] = @Id_3;
/// ]]></code>
/// </example>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class AliasAttribute : Attribute
{
    /// <summary>
    /// Gets the alias name to apply in the <c>AS</c> clause for the decorated property.
    /// </summary>
    internal AliasName Name { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AliasAttribute"/> class.
    /// </summary>
    /// <param name="aliasName">The alias to use for the property’s <c>AS</c> clause.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="aliasName"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="aliasName"/> is empty.</exception>
    public AliasAttribute(string aliasName)
    {
        ArgumentNullException.ThrowIfNull(aliasName, nameof(aliasName));
        if (aliasName == string.Empty)
            throw new ArgumentException("aliasName is empty", nameof(aliasName));
        Name = new(aliasName);
    }
}
