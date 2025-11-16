using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a <c>DATETIME2</c> column
/// and overrides the default SQL type mapping for that column in the data model.
/// </summary>
/// <remarks>
/// This attribute defines SQL metadata for a property that represents a <c>DATETIME2</c>
/// column on a table model, including the optional fractional-second precision.
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
    public SqlDateTime2Attribute(byte fractionalSecondPrecision) : base(SqlTypeDefinition.AsDateTime2(fractionalSecondPrecision)) 
    {
    }
}
