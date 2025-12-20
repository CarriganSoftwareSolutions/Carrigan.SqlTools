using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a SQL Server <c>FLOAT</c> column and overrides the default SQL type
/// mapping for that column in the data model.
/// </summary>
/// <remarks>
/// This attribute defines SQL metadata for a property, and that property represents an SQL column in the data model.
/// The attribute supplies the <see cref="SqlTypeDefinition"/> consumed by the SQL generator when emitting SQL.
/// <para>
/// SQL Server <c>FLOAT</c> supports an optional precision value (<c>1</c>–<c>53</c>) that controls the number of bits
/// used to store the mantissa and therefore the storage size and approximate numeric range:
/// </para>
/// <list type="bullet">
/// <item><description><c>1</c>–<c>24</c> uses 4 bytes of storage (single-precision behavior).</description></item>
/// <item><description><c>25</c>–<c>53</c> uses 8 bytes of storage (double-precision behavior).</description></item>
/// </list>
/// <para>
/// When precision is not specified, SQL Server treats <c>FLOAT</c> as <c>FLOAT(53)</c>.
/// </para>
/// <para>
/// Suggested C# data type: use <see cref="double"/>. If you explicitly choose a precision in the <c>1</c>–<c>24</c>
/// range, <see cref="float"/> may be appropriate depending on your loss/rounding expectations.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlFloatAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlFloatAttribute"/> class and configures the associated property
    /// to represent a <c>FLOAT</c> column using SQL Server's default precision.
    /// </summary>
    public SqlFloatAttribute() : base(SqlTypeDefinition.AsFloat())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlFloatAttribute"/> class and configures the associated property
    /// to represent a <c>FLOAT</c> column with an explicit precision.
    /// </summary>
    /// <param name="Precision">The SQL Server <c>FLOAT</c> precision (1–53).</param>
    /// <exception cref="Exceptions.SqlTypeArgumentOutOfRangeException">
    /// Thrown when <paramref name="Precision"/> is outside the valid SQL Server range of 1–53.
    /// </exception>
    public SqlFloatAttribute(byte Precision) : base(SqlTypeDefinition.AsFloat(Precision))
    {
    }
}
