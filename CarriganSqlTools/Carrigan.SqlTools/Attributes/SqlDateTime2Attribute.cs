using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a DateTime2 column
/// and overrides the default SQL type mapping for that column in the data model.
/// </summary>
/// <remarks>
/// This attribute defines SQL metadata for a property that represents a DateTime2 column
/// on a table model as well as the fractional second precision.
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlDateTime2Attribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlDateTime2Attribute"/> class that configures the
    /// associated property to represent a DateTime2 column using the specified encoding and
    /// storage type using SQL default fractional second precision.
    /// </summary>
    public SqlDateTime2Attribute() : base(SqlTypeDefinition.AsDateTime2()) 
    {
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlDateTime2Attribute"/> class that configures the
    /// associated property to represent a DateTime2 column using the specified encoding and
    /// storage type using SQL default fractional second precision.
    /// </summary>
    /// <param name="fractionalSecondPrecision">
    /// The optional SQL fractional second precision for the DateTime2 column.
    /// </param>
    public SqlDateTime2Attribute(byte fractionalSecondPrecision) : base(SqlTypeDefinition.AsDateTime2(fractionalSecondPrecision)) 
    {
    }
}
