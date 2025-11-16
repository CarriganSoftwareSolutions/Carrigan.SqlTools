using Carrigan.SqlTools.Types;
using System.Data;

namespace Carrigan.SqlTools.Attributes;
/// <summary>
/// Specifies that a property represents a <c>DateTimeOffset</c> column
/// and overrides the default SQL type mapping for that column in the data model.
/// </summary>
/// <remarks>
/// This attribute defines SQL metadata for a property that represents a <c>DateTimeOffset</c>
/// column on a table model, including the optional fractional-second precision.
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class SqlDateTimeOffsetAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlDateTimeOffsetAttribute"/> class and configures
    /// the associated property to represent a <c>DateTimeOffset</c> column using SQL Server's default
    /// fractional-second precision.
    /// </summary>
    public SqlDateTimeOffsetAttribute() : base(SqlTypeDefinition.AsDateTimeOffset())
    {
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlDateTimeOffsetAttribute"/> class and configures
    /// the associated property to represent a <c>DateTimeOffset</c> column with an explicit
    /// fractional-second precision (0–7).
    /// </summary>
    /// <param name="fractionalSecondPrecision">
    /// The fractional-second precision to apply to the <c>DateTimeOffset</c> column (0–7), where
    /// the value represents the number of decimal places used to store fractional seconds.
    /// </param>
    public SqlDateTimeOffsetAttribute(byte fractionalSecondPrecision) : base(SqlTypeDefinition.AsDateTimeOffset(fractionalSecondPrecision))
    {
    }
}
