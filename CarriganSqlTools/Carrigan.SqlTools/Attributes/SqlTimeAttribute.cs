using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a SQL Server <c>TIME</c> column and overrides the default SQL type mapping
/// for that column in the data model.
/// </summary>
/// <remarks>
/// This attribute defines SQL metadata for a property, and that property represents an SQL column in the data model.
/// The attribute supplies the <see cref="SqlTypeDefinition"/> consumed by the SQL generator when emitting SQL.
/// <para>
/// SQL Server <c>TIME</c> supports an optional fractional-second precision of <c>0</c>–<c>7</c>. When precision is not
/// specified, SQL Server applies its default fractional-second precision.
/// </para>
/// <para>
/// This attribute affects only SQL generation within <c>Carrigan.SqlTools</c> and does not influence Entity Framework
/// or database schema.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlTimeAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlTimeAttribute"/> class and configures
    /// the associated property to represent a SQL Server <c>TIME</c> column using SQL Server's default
    /// fractional-second precision.
    /// </summary>
    public SqlTimeAttribute() : base(SqlTypeDefinition.AsTime())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlTimeAttribute"/> class and configures
    /// the associated property to represent a SQL Server <c>TIME</c> column with an explicit
    /// fractional-second precision (0–7).
    /// </summary>
    /// <param name="FractionalSecondPrecision">
    /// The fractional-second precision to apply to the <c>TIME</c> column (0–7), where the value represents the number
    /// of decimal places used to store fractional seconds.
    /// </param>
    /// <exception cref="Carrigan.SqlTools.Exceptions.SqlTypeArgumentOutOfRangeException">
    /// Thrown when <paramref name="FractionalSecondPrecision"/> is greater than 7.
    /// </exception>
    public SqlTimeAttribute(byte FractionalSecondPrecision) : base(SqlTypeDefinition.AsTime(FractionalSecondPrecision))
    {
    }
}
