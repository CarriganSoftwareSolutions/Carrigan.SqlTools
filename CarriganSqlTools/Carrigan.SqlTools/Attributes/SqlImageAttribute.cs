using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents an <c>IMAGE</c> column
/// and overrides the default SQL type mapping for that column in the data model.
/// </summary>
/// <remarks>
/// This attribute defines SQL metadata for a property that represents an  
/// <c>IMAGE</c> column on a table model.  
///
/// <para>
/// The SQL Server <c>IMAGE</c> type is deprecated, but this attribute is provided to support
/// legacy database schemas that still rely on it.  
/// </para>
///
/// <para>
/// When applied to a property, this attribute forces <see cref="Carrigan.SqlTools"/> to generate
/// an <c>IMAGE</c> column definition regardless of the property's .NET type.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlImageAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlImageAttribute"/> class and configures
    /// the associated property to represent an <c>IMAGE</c> column.
    /// </summary>
    public SqlImageAttribute() : base (SqlTypeDefinition.AsImage())
    {
    }
}
