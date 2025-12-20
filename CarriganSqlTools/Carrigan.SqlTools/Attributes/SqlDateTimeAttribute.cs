using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a SQL <c>DATETIME</c> or <c>SMALLDATETIME</c> column and overrides
/// the default SQL type mapping for that column in the data model.
/// </summary>
/// <remarks>
/// This attribute defines SQL metadata for a property, and that property represents an SQL column in the data model.
/// The SQL type is selected by the <see cref="SizeableEnum"/> value supplied:
/// <list type="bullet">
/// <item><description><see cref="SizeableEnum.Regular"/> produces <c>DATETIME</c>.</description></item>
/// <item><description><see cref="SizeableEnum.Smaller"/> produces <c>SMALLDATETIME</c>.</description></item>
/// </list>
/// This attribute affects only SQL generation within <c>Carrigan.SqlTools</c> and does not influence Entity Framework
/// or database schema.
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlDateTimeAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlDateTimeAttribute"/> class and configures the associated
    /// property to represent a <c>DATETIME</c> or <c>SMALLDATETIME</c> column, based on <paramref name="sizeableEnum"/>.
    /// </summary>
    /// <param name="sizeableEnum">Selects whether the SQL type is <c>DATETIME</c> or <c>SMALLDATETIME</c>.</param>
    /// <exception cref="NotSupportedException">
    /// Thrown when an unsupported <see cref="SizeableEnum"/> value is supplied. This typically indicates that the
    /// enumeration was extended without updating the attribute.
    /// </exception>
    public SqlDateTimeAttribute(SizeableEnum sizeableEnum)
        : base(GetSqlTypeDefinition(sizeableEnum))
    {
    }

    private static SqlTypeDefinition GetSqlTypeDefinition(SizeableEnum sizeableEnum) =>
        sizeableEnum switch
        {
            SizeableEnum.Regular => SqlTypeDefinition.AsDateTime(),
            SizeableEnum.Smaller => SqlTypeDefinition.AsSmallDateTime(),
            _ => throw new NotSupportedException($"Unsupported date/time size '{sizeableEnum}'."),
        };
}
