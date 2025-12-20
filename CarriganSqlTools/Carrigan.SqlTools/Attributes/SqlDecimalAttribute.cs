using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a <c>DECIMAL</c> SQL column and overrides the default SQL type mapping
/// for that column in the data model.
/// </summary>
/// <remarks>
/// This attribute defines SQL metadata for a property, and that property represents an SQL column in the data model.
/// The attribute supplies the <see cref="SqlTypeDefinition"/> consumed by the SQL generator when emitting SQL.
/// <para>
/// When precision and scale are not specified, the SQL generator emits <c>DECIMAL</c> with no explicit
/// <c>(precision, scale)</c>. SQL Server applies its default precision and scale for <c>DECIMAL</c>.
/// </para>
/// <para>
/// When explicitly specified:
/// </para>
/// <list type="bullet">
/// <item><description><paramref name="Precision"/> must be in the range <c>1</c>–<c>38</c>.</description></item>
/// <item><description><paramref name="Scale"/> must be in the range <c>0</c>–<c>precision</c>.</description></item>
/// </list>
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlDecimalAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlDecimalAttribute"/> class and configures the associated property
    /// to represent a <c>DECIMAL</c> column using SQL Server's default precision and scale.
    /// </summary>
    public SqlDecimalAttribute() : base(SqlTypeDefinition.AsDecimal())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlDecimalAttribute"/> class and configures the associated property
    /// to represent a <c>DECIMAL</c> column with an explicit precision.
    /// </summary>
    /// <param name="Precision">The total number of digits for the <c>DECIMAL</c> column (1–38).</param>
    /// <exception cref="Carrigan.SqlTools.Exceptions.SqlTypeArgumentOutOfRangeException">
    /// Thrown when <paramref name="Precision"/> is less than 1 or greater than 38.
    /// </exception>
    public SqlDecimalAttribute(byte Precision) : base(SqlTypeDefinition.AsDecimal(Precision))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlDecimalAttribute"/> class and configures the associated property
    /// to represent a <c>DECIMAL</c> column with an explicit precision and scale.
    /// </summary>
    /// <param name="Precision">The total number of digits for the <c>DECIMAL</c> column (1–38).</param>
    /// <param name="Scale">
    /// The number of digits to the right of the decimal point. Must be less than or equal to <paramref name="Precision"/>.
    /// </param>
    /// <exception cref="Carrigan.SqlTools.Exceptions.SqlTypeArgumentOutOfRangeException">
    /// Thrown when <paramref name="Precision"/> or <paramref name="Scale"/> is out of range,
    /// or when <paramref name="Scale"/> is greater than <paramref name="Precision"/>.
    /// </exception>
    public SqlDecimalAttribute(byte Precision, byte Scale) : base(SqlTypeDefinition.AsDecimal(Precision, Scale))
    {
    }
}
