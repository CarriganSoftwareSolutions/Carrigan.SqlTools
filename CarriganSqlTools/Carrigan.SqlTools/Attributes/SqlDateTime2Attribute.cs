using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a <c>DATETIME2</c> column
/// and overrides the default SQL type mapping for that column in the data model.
/// </summary>
/// <remarks>
/// <para>
/// In Carrigan.SqlTools, this attribute supplies the <see cref="SqlTypeDefinition"/> used by the SQL generator
/// when emitting SQL for the decorated property.
/// </para>
/// <para>
/// <c>DATETIME2</c> supports an optional fractional-second precision of <c>0</c>–<c>7</c>. When precision is not
/// specified, SQL Server applies its default fractional-second precision.
/// </para>
/// <para>
/// This attribute affects only SQL generation within <c>Carrigan.SqlTools</c> and does not influence Entity Framework
/// or database schema.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlDateTime2Attribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlDateTime2Attribute"/> class and configures
    /// the associated property to represent a <c>DATETIME2</c> column using SQL Server's default
    /// fractional-second precision.
    /// </summary>
    public SqlDateTime2Attribute() : base(SqlTypeDefinition.AsDateTime2()) 
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlDateTime2Attribute"/> class and configures
    /// the associated property to represent a <c>DATETIME2</c> column with an explicit
    /// fractional-second precision (0–7).
    /// </summary>
    /// <param name="fractionalSecondPrecision">
    /// The fractional-second precision to apply to the <c>DATETIME2</c> column (0–7), where
    /// the value represents the number of decimal places used to store fractional seconds.
    /// </param>
    /// <exception cref=".Exceptions.SqlTypeArgumentOutOfRangeException">
    /// Thrown when <paramref name="fractionalSecondPrecision"/> is greater than 7.
    /// </exception>
    public SqlDateTime2Attribute(byte fractionalSecondPrecision) : base(SqlTypeDefinition.AsDateTime2(fractionalSecondPrecision)) 
    {
    }
}
