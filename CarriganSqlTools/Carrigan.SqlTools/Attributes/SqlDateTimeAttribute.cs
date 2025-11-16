using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;
/// <summary>
/// Specifies that a property represents a <c>DATETIME</c> or <c>SMALLDATETIME</c> column
/// and overrides the default SQL type mapping for that column in the data model.
/// </summary>
/// <remarks>
/// This attribute defines SQL metadata for a property that represents a <c>DATETIME</c>
/// or <c>SMALLDATETIME</c> column on a table model.  
/// The selected SQL type depends on the <see cref="SizeableEnum"/> value supplied.
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlDateTimeAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlDateTimeAttribute"/> class and configures
    /// the associated property to represent a <c>DATETIME</c> or <c>SMALLDATETIME</c> column,
    /// based on the specified <see cref="SizeableEnum"/>.
    /// </summary>
    /// <param name="dateTimeSize">
    /// Determines whether the column is generated as <c>DATETIME</c> 
    /// (<see cref="SizeableEnum.Regular"/>) or <c>SMALLDATETIME</c> 
    /// (<see cref="SizeableEnum.Smaller"/>).
    /// </param>
    /// <exception cref="NotSupportedException">
    /// Thrown when an unsupported <see cref="SizeableEnum"/> value is provided.
    /// This typically indicates that the enumeration was extended without updating the attribute.
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
