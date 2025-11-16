using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;
/// <summary>
/// Specifies that a property represents a <c>DATETIME</c> / <c>SMALLDATETIME</c> column
/// and overrides the default SQL type mapping for that column in the data model.
/// </summary>
/// <remarks>
/// This attribute defines SQL metadata for a property that represents a <c>DATETIME</c> / <c>SMALLDATETIME</c>
/// column on a table model, including the optional fractional-second precision.
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlDateTimeAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlDateTimeAttribute"/> class and configures
    /// the associated property to represent a <c>DATETIME</c> or <c>SMALLDATETIME</c> column.
    /// </summary>
    /// <param name="dateTimeSize">
    /// Indicates whether the column uses <c>DATETIME</c> or <c>SMALLDATETIME</c>.
    /// </param>
    /// <exception cref="NotSupportedException">
    /// Thrown when an unsupported <see cref="SizeableEnum"/> is provided.  
    /// This typically indicates that the enumeration was extended without updating this method.
    /// </exception>
    public SqlDateTimeAttribute(SizeableEnum dateTimeSize) : base
        (
            dateTimeSize switch
            {
                SizeableEnum.Regular => SqlTypeDefinition.AsDateTime(),
                SizeableEnum.Smaller => SqlTypeDefinition.AsSmallDateTime(),
                _ => throw new NotSupportedException($"Unsupported storage type '{dateTimeSize}'."),
            }
        )
    {
    }
}
