using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using System.Numerics;
using System.Reflection;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Represents a table identifier (“tag”) in the form <c>[Schema].[Table]</c>.
/// The <c>[Schema]</c> segment is included only when explicitly provided.
/// </summary>
/// <remarks>
/// Equality and hashing are based on the schema and table-name components using their
/// case-sensitive identifier semantics. An empty schema is treated the same as no schema.
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
/// --PostgreSql
/// UPDATE "schema"."Phone"
/// SET "CustomerId" = $1, 
///     "Phone" = $2 
/// WHERE "Id" = $3;
/// 
/// --SqlServer
/// "UPDATE [schema].[Phone] 
/// SET [CustomerId] = @CustomerId_1, 
///     [Phone] = @Phone_2
/// WHERE [Id] = @Id_3;
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
/// --PostgreSql
/// UPDATE "schema"."Email" 
/// SET "CustomerId" = $1, 
///     "Email" = $2 
/// WHERE "Id" = $3;
/// 
/// --SqlServer
/// UPDATE [schema].[Email] 
/// SET [CustomerId] = @CustomerId_1, 
///     [Email] = @Email_2 
/// WHERE [Id] = @Id_3;
/// ]]></code>
/// </example>
public class TableTag : IEquatable<TableTag>, IEqualityOperators<TableTag, TableTag, bool>, ISqlFragment, IWhiteSpace, IEmpty
{
    /// <summary>
    /// Gets the reflected table tag for the specified model type.
    /// </summary>
    /// <typeparam name="T">The model type whose C# properties represent SQL columns or parameters.</typeparam>
    /// <returns>The table tag resolved from the reflection cache for <typeparamref name="T"/>.</returns>
    public static TableTag Get<T>() where T : class =>
        SqlToolsReflectorCache<T>.Table;

    /// <summary>
    /// The optional schema that qualifies the table name.
    /// </summary>
    internal readonly SchemaName? SchemaName;

    /// <summary>
    /// The table identifier without the schema qualifier.
    /// </summary>
    internal readonly TableName TableName;
    /// <summary>
    /// Initializes a new instance of the <see cref="TableTag"/> class.
    /// </summary>
    /// <param name="schemaName">
    /// The optional schema name. If <c>null</c> or empty, only the table name is used.
    /// </param>
    /// <param name="tableName">The table name. Must not be <c>null</c> or empty.</param>
    /// <exception cref="Exceptions.InvalidSqlIdentifierException">
    /// Thrown when <paramref name="tableName"/> or a non-empty <paramref name="schemaName"/> fails SQL identifier validation.
    /// </exception>
    ///
    internal TableTag(SchemaName? schemaName, TableName tableName)
    {
        ArgumentNullException.ThrowIfNull(tableName);

        TableName = tableName;
        SchemaName = schemaName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TableTag"/> class.
    /// </summary>
    /// <remarks>
    /// Marked <see cref="ExternalOnlyAttribute"/> so unit tests can access it while keeping the API internal.
    /// </remarks>
    /// <param name="schemaName">
    /// The optional schema name. If <c>null</c> or empty, only the table name is used.
    /// </param>
    /// <param name="tableName">The table name. Must not be <c>null</c> or empty.</param>
    /// <exception cref="Exceptions.InvalidSqlIdentifierException">
    /// Thrown when <paramref name="tableName"/> or a non-empty <paramref name="schemaName"/> fails SQL identifier validation.
    /// </exception>
    ///
    [ExternalOnly]//An external only marked as internal can still be used by the unit tests class.
    internal TableTag(string? schemaName, string tableName)
        : this(SchemaName.New(schemaName), new(tableName))
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

    /// <summary>
    /// Returns the schema-qualified table name represented by this tag.
    /// </summary>
    /// <returns>The table name, optionally qualified by its schema.</returns>
    public override string ToString() =>
        SchemaName.IsNotNullOrEmpty() ? $"{SchemaName}.{TableName}" : TableName.ToString();

    /// <summary>
    /// Determines whether this instance identifies the same table as another <see cref="TableTag"/>.
    /// </summary>
    /// <param name="other">The other table tag to compare.</param>
    /// <returns><c>true</c> when the effective schema and table name are equal; otherwise, <c>false</c>.</returns>
    public bool Equals(TableTag? other)
    {
        if (ReferenceEquals(this, other))
            return true;

        if (other is null)
            return false;

        if (SchemaName.IsNotNullOrEmpty() != other.SchemaName.IsNotNullOrEmpty())
            return false;

        if (SchemaName.IsNotNullOrEmpty() && !SchemaName.Equals(other.SchemaName))
            return false;

        return TableName.Equals(other.TableName);
    }

    /// <summary>
    /// Determines whether the specified object identifies the same table as this instance.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><c>true</c> when <paramref name="obj"/> is an equivalent <see cref="TableTag"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj) =>
        Equals(obj as TableTag);

    /// <summary>
    /// Returns a hash code for this table tag.
    /// </summary>
    /// <returns>A hash code consistent with <see cref="Equals(TableTag?)"/>.</returns>
    public override int GetHashCode()
    {
        SchemaName? schemaName = SchemaName.IsNotNullOrEmpty() ? SchemaName : null;
        return HashCode.Combine(schemaName, TableName);
    }

    /// <summary>
    /// Determines whether two table tags identify the same table.
    /// </summary>
    public static bool operator ==(TableTag? left, TableTag? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two table tags identify different tables.
    /// </summary>
    public static bool operator !=(TableTag? left, TableTag? right) =>
        !(left == right);

    /// <summary>
    /// Indicates whether the rendered table tag is empty or consists only of whitespace characters.
    /// </summary>
    public bool IsWhiteSpace() =>
        SchemaName.IsNullOrWhiteSpace() && TableName.IsWhiteSpace();

    /// <summary>
    /// Indicates whether the rendered table tag contains at least one non-whitespace character.
    /// </summary>
    public bool IsNotWhiteSpace() =>
        IsWhiteSpace() == false;

    /// <summary>
    /// Indicates whether the rendered table tag is empty.
    /// </summary>
    public bool IsEmpty() =>
        SchemaName.IsNullOrEmpty() && TableName.IsEmpty();

    /// <summary>
    /// Indicates whether the rendered table tag is not empty.
    /// </summary>
    public bool IsNotEmpty() =>
        IsEmpty() == false;

    /// <summary>
    /// Flattens this fragment into the sequence of fragments used to render SQL text.
    /// </summary>
    /// <returns>A single-item sequence containing this table tag.</returns>
    public IEnumerable<ISqlFragment> Flatten(ISqlDialects dialect)
    {
        yield return this;
    }
    /// <summary>
    /// Gets the SQL parameters contained by this fragment.
    /// </summary>
    /// <returns>An empty sequence because table-name fragments do not contain SQL parameters.</returns>
    public IEnumerable<SqlFragmentParameter> GetSqlFragmentParameters(ISqlDialects dialect) =>
        [];

    /// <summary>
    /// Renders the schema-qualified table name using the supplied SQL dialect.
    /// </summary>
    /// <param name="dialect">The SQL dialect used to quote and combine the schema and table identifiers.</param>
    /// <returns>The rendered table identifier.</returns>
    public string ToSql(ISqlDialects dialect) =>
        dialect.RenderTable(SchemaName, TableName);
}
