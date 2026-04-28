using Carrigan.Core.DataTypes;
using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using System.Reflection;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Represents a table identifier (“tag”) in the form <c>[Schema].[Table]</c>.
/// The <c>[Schema]</c> segment is included only when explicitly provided.
/// </summary>
/// <remarks>
/// This type uses <see cref="StringWrapper"/> to provide consistent equality, ordering,
/// and hashing semantics (case-sensitive via <see cref="StringComparison.Ordinal"/>).
/// </remarks>
/// <example>
/// <para>
/// Using Table Attribute
/// </para>
/// <code language="csharp"><![CDATA[
/// using Carrigan.SqlTools.SqlGenerators;
/// 
/// [Table("Phone", Schema = "schema")]
/// public class PhoneModel
/// {
///     [Key]
///     public int Id { get; set; }
///     public int CustomerId { get; set; }
///     [Column("Phone")]
///     public string? PhoneNumber { get; set; }
/// }
/// 
/// SqlGenerator<PhoneModel> phoneGenerator = new();
/// 
/// PhoneModel phone = new()
/// {
///     Id = 2718,
///     CustomerId = 3141,
///     PhoneNumber = "07700 900461"
/// };
/// SqlQuery query = phoneGenerator.UpdateById(phone);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// UPDATE [schema].[Phone] 
/// SET [CustomerId] = @CustomerId, [Phone] = @Phone 
/// WHERE [Id] = @Id;
/// ]]></code>
/// </example>
/// <example>
/// <para>
/// Using Identifier Attribute
/// </para>
/// <code language="csharp"><![CDATA[
/// using Carrigan.SqlTools.SqlGenerators;
/// 
/// [Identifier("Email", "schema")]
/// public class EmailModel
/// {
///     [PrimaryKey]
///     public int Id { get; set; }
///     public int CustomerId { get; set; }
///     [Identifier("Email")]
///     public string? EmailAddress { get; set; }
/// }
/// 
/// SqlGenerator<EmailModel> emailGenerator = new();
/// 
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
/// UPDATE [schema].[Phone] 
/// SET [CustomerId] = @CustomerId, [Phone] = @Phone 
/// WHERE [Id] = @Id;
/// ]]></code>
/// </example>
public class TableTag : StringWrapper
{
    /// <summary>
    /// Gets or sets the SQL dialect configuration used for generating database queries.
    /// </summary>
    /// <remarks>Use this property to specify the SQL dialect that determines how queries are constructed and
    /// formatted. Changing the dialect may affect compatibility with different database systems.</remarks>
    internal readonly ISqlDialects Dialect;

    /// <summary>
    /// Initializes a new instance of the <see cref="TableTag"/> class.
    /// </summary>
    /// <param name="dialect">The SQL dialect configuration used for generating database queries.</param>
    /// <param name="schemaName">
    /// The optional schema name. If <c>null</c> or empty, only the table name is used.
    /// </param>
    /// <exception cref="Exceptions.InvalidSqlIdentifierException">
    /// Thrown when <paramref name="tableName"/> or a non-empty <paramref name="schemaName"/> fails SQL identifier validation.
    /// </exception>
    /// <param name="tableName">The table name. Must not be <c>null</c> or empty.</param>
    internal TableTag(ISqlDialects dialect, SchemaName? schemaName, TableName tableName)
        : base(dialect.RenderTable(schemaName, tableName), StringComparison.Ordinal) => 
        Dialect = dialect;

    /// <summary>
    /// Initializes a new instance of the <see cref="TableTag"/> class.
    /// </summary>
    /// <remarks>
    /// Marked <see cref="ExternalOnlyAttribute"/> so unit tests can access it while keeping the API internal.
    /// </remarks>
    /// <param name="dialect">The SQL dialect configuration used for generating database queries.</param>
    /// <param name="schemaName">
    /// The optional schema name. If <c>null</c> or empty, only the table name is used.
    /// </param>
    /// <exception cref="Exceptions.InvalidSqlIdentifierException">
    /// Thrown when <paramref name="tableName"/> or a non-empty <paramref name="schemaName"/> fails SQL identifier validation.
    /// </exception>
    /// <param name="tableName">The table name. Must not be <c>null</c> or empty.</param>
    [ExternalOnly]//An external only marked as internal can still be used by the unit tests class.
    internal TableTag(ISqlDialects dialect, string? schemaName, string tableName)
        : this(dialect, SchemaName.New(schemaName), new(tableName))
    {
    }

    /// <summary>
    /// Retrieves the <see cref="TableTag"/> associated with the specified entity type by using
    /// the internal SQL tools reflection cache.
    /// </summary>
    /// <param name="value">The entity CLR <see cref="System.Type"/> for which to retrieve the table tag.</param>
    /// <returns>The <see cref="TableTag"/> for the specified type.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the reflection cache does not expose a non-public static <c>Table</c> property,
    /// or when that property returns <c>null</c>.
    /// </exception>
    internal static TableTag Get(Type value)
    {
        ArgumentNullException.ThrowIfNull(value);

        Type cacheType = typeof(SqlToolsReflectorCache<>).MakeGenericType(value);

        PropertyInfo tableTagProperty = cacheType.GetProperty("Table", BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new InvalidOperationException($"The property 'Table' was not found on type '{cacheType.FullName}'.");

        return (TableTag?)tableTagProperty.GetValue(null)
            ?? throw new InvalidOperationException($"The property 'Table' on type '{cacheType.FullName}' returned null.");
    }
}
