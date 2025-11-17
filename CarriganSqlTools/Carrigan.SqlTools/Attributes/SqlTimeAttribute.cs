using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;
/// <summary>
/// Specifies that a property represents a <c>TIME</c> column
/// and overrides the default SQL type mapping for that column in the data model.
/// </summary>
/// <remarks>
/// This attribute defines SQL metadata for a property that represents a SQL Server
/// <c>TIME</c> column on a table model, including the optional fractional-second precision
/// used when generating the column definition.
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlTimeAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlTimeAttribute"/> class and configures
    /// the associated property to represent a <c>TIME</c> column using SQL Server's default
    /// fractional-second precision.
    /// </summary>
    public SqlTimeAttribute() : base(SqlTypeDefinition.AsTime())
    {
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlTimeAttribute"/> class and configures
    /// the associated property to represent a <c>TIME</c> column with an explicit
    /// fractional-second precision (0–7).
    /// </summary>
    /// <param name="fractionalSecondPrecision">
    /// The fractional-second precision to apply to the <c>TIME</c> column (0–7), where the
    /// value represents the number of decimal places used to store fractional seconds.
    /// </param>
    /// <exception cref="Carrigan.SqlTools.Exceptions.SqlTypeArgumentOutOfRangeException">
    /// Thrown when <paramref name="fractionalSecondPrecision"/> is outside the valid range (0–7).
    /// </exception>
    public SqlTimeAttribute(byte fractionalSecondPrecision) : base(SqlTypeDefinition.AsTime(fractionalSecondPrecision))
    {
    }
}
