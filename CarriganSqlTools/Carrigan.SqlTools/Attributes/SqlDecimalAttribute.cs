using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a <c>DECIMAL</c> column
/// and overrides the default SQL type mapping for that column in the data model.
/// </summary>
/// <remarks>
/// This attribute defines SQL metadata for a property that represents a <c>DECIMAL</c>
/// column on a table model, including the precision and scale used by SQL Server when
/// generating the column definition.
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlDecimalAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlDecimalAttribute"/> class and configures
    /// the associated property to represent a <c>DECIMAL</c> column using SQL Server's default
    /// precision and scale.
    /// </summary>
    public SqlDecimalAttribute() : base(SqlTypeDefinition.AsDecimal())
    {
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlDecimalAttribute"/> class and configures
    /// the associated property to represent a <c>DECIMAL</c> column with an explicit precision.
    /// </summary>
    /// <param name="precision">
    /// The total number of digits for the <c>DECIMAL</c> column (1–38).
    /// </param>
    public SqlDecimalAttribute(byte precision) : base(SqlTypeDefinition.AsDecimal(precision))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlDecimalAttribute"/> class and configures
    /// the associated property to represent a <c>DECIMAL</c> column with an explicit precision
    /// and scale.
    /// </summary>
    /// <param name="precision">
    /// The total number of digits for the <c>DECIMAL</c> column (1–38).
    /// </param>
    /// <param name="scale">
    /// The number of digits to the right of the decimal point, which must be less than or equal
    /// to <paramref name="precision"/>.
    /// </param>
    public SqlDecimalAttribute(byte precision, byte scale) : base(SqlTypeDefinition.AsDecimal(precision, scale))
    {
    }
}
