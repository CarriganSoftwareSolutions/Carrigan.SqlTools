using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a <c>VARBINARY(MAX)</c> column
/// and overrides the default SQL type mapping for that column in the data model.
/// </summary>
/// <remarks>
/// This attribute defines SQL metadata for a property, and that property represents an SQL column in the data model.
/// The attribute supplies the <see cref="SqlTypeDefinition"/> consumed by the SQL generator when emitting SQL.
/// <para>
/// <c>VARBINARY(MAX)</c> is the recommended modern replacement for the legacy <c>IMAGE</c>
/// type when modeling large binary data, such as files or blobs.
/// </para>
/// <para>
/// Suggested CLR type: <see cref="byte"/>[].
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlVarBinaryMaxAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlVarBinaryMaxAttribute"/> class and configures
    /// the associated property to represent a <c>VARBINARY(MAX)</c> column.
    /// </summary>
    public SqlVarBinaryMaxAttribute() : base(SqlTypeDefinition.AsVarBinaryMax())
    {
    }
}
