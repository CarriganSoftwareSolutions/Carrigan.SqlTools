using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a SQL Server <c>MONEY</c> or <c>SMALLMONEY</c> column and overrides the default SQL type mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlMoneyAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlMoneyAttribute"/> class.
    /// </summary>
    /// <param name="moneySize">The SQL money type size to apply.</param>
    /// <exception cref="NotSupportedException">
    /// Thrown when the requested SQL type option is not supported.
    /// </exception>
    public SqlMoneyAttribute(SizeableEnum moneySize) : base(GetFieldProperties(moneySize))
    { }

    private static FieldProperties GetFieldProperties(SizeableEnum moneySize) =>
        moneySize switch
        {
            SizeableEnum.Regular => SqlServerTypesProvider.AsMoney(),
            SizeableEnum.Smaller => SqlServerTypesProvider.AsSmallMoney(),
            _ => throw new NotSupportedException($"Unsupported {nameof(SizeableEnum)} value '{moneySize}' for {nameof(SqlMoneyAttribute)}."),
        };
}
