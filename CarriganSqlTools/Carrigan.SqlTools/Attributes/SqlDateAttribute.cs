using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a SQL <c>DATE</c> column and overrides the
/// default SQL type mapping for that property in the data model.
/// </summary>
/// <remarks>
/// <para>
/// In Carrigan.SqlTools, this attribute supplies the <see cref="SqlTypeDefinition"/> used by the SQL generator
/// when emitting SQL for the decorated property.
/// </para>
/// <para>
/// SQL Server <c>DATE</c> stores only the date component (year, month, day) with no time-of-day information.
/// </para>
/// <para>
/// This attribute affects only SQL generation within <c>Carrigan.SqlTools</c> and does not influence Entity Framework
/// or database schema.
/// </para>
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
