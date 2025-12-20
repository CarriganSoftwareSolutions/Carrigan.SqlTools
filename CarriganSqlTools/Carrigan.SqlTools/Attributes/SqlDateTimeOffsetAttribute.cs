using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a SQL <c>DATETIMEOFFSET</c> column and overrides the
/// default SQL type mapping for that property in the data model.
/// </summary>
/// <remarks>
/// <para>
/// In Carrigan.SqlTools, this attribute supplies the <see cref="SqlTypeDefinition"/> used by the SQL generator
/// when emitting SQL for the decorated property.
/// </para>
/// <para>
/// <c>DATETIMEOFFSET</c> supports an optional fractional-second precision of <c>0</c>–<c>7</c>. When precision is not
/// specified, SQL Server applies its default fractional-second precision.
/// </para>
/// <para>
/// This attribute affects only SQL generation within <c>Carrigan.SqlTools</c> and does not influence Entity Framework
/// or database schema.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlDateTimeOffsetAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlDateTimeOffsetAttribute"/> class and configures
    /// the associated property to represent a SQL <c>DATETIMEOFFSET</c> column using SQL Server's default
    /// fractional-second precision.
    /// </summary>
    public SqlDateTimeOffsetAttribute() : base(SqlTypeDefinition.AsDateTimeOffset())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlDateTimeOffsetAttribute"/> class and configures
    /// the associated property to represent a SQL <c>DATETIMEOFFSET</c> column with an explicit
    /// fractional-second precision (0–7).
    /// </summary>
    /// <param name="fractionalSecondPrecision">
    /// The fractional-second precision to apply to the <c>DATETIMEOFFSET</c> column (0–7), where
    /// the value represents the number of decimal places used to store fractional seconds.
    /// </param>
    /// <exception cref="Carrigan.SqlTools.Exceptions.SqlTypeArgumentOutOfRangeException">
    /// Thrown when <paramref name="fractionalSecondPrecision"/> is greater than 7.
    /// </exception>
    public SqlDateTimeOffsetAttribute(byte fractionalSecondPrecision) : base(SqlTypeDefinition.AsDateTimeOffset(fractionalSecondPrecision))
    {
    }
}
