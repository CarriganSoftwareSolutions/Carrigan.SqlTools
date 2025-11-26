using Carrigan.SqlTools.Types;
//TODO: proof read documentation. Unit tests.
namespace Carrigan.SqlTools.Attributes;
/// <summary>
/// Specifies that a property represents a SQL <c>DATE</c> column and overrides the
/// default SQL type mapping for that property in the data model.
/// </summary>
/// <remarks>
/// This attribute applies SQL metadata indicating that the decorated property
/// should be stored in a SQL Server <c>DATE</c> column, which contains only the
/// date component (year, month, day) with no time-of-day information.
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlDateAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlDateAttribute"/> class and configures
    /// the associated property to be mapped to a SQL <c>DATE</c> column.
    /// </summary>
    public SqlDateAttribute() : base (SqlTypeDefinition.AsDate())
    {
    }
}
