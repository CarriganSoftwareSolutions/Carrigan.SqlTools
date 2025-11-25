using Carrigan.SqlTools.Types;
//TODO: proof read documentation. Unit tests.
namespace Carrigan.SqlTools.Attributes;
/// <summary>
/// Specifies that a property represents a <c>DATE</c> column
/// and overrides the default SQL type mapping for that column in the data model.
/// </summary>
/// <remarks>
/// This attribute defines SQL metadata for a property that represents a <c>DATE</c> column on a table model
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlDateAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlDateTimeAttribute"/> class and configures
    /// the associated property to represent a <c>DATE</c> column.
    /// </summary>
    public SqlDateAttribute() : base (SqlTypeDefinition.AsDate())
    {
    }
}
