using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a <c>VARBINARY(MAX)</c> column
/// and overrides the default SQL type mapping for that column in the data model.
/// </summary>
/// <remarks>
/// This attribute defines SQL metadata for a property that represents a SQL Server
/// <c>VARBINARY(MAX)</c> column on a table model.
///
/// <para>
/// <c>VARBINARY(MAX)</c> is the recommended modern replacement for the legacy <c>IMAGE</c>
/// type when modeling large binary data, such as files or blobs.
/// </para>
///
/// <para><strong>Suggested C# Data Type:</strong><br/>
/// Properties mapped to <c>VARBINARY(MAX)</c> columns should use the .NET <see cref="byte"/>[]
/// type. ADO.NET exposes <c>VARBINARY</c> and <c>VARBINARY(MAX)</c> values as <see cref="byte"/>[]
/// instances, making <see cref="byte"/>[] the natural representation in the data model.
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
