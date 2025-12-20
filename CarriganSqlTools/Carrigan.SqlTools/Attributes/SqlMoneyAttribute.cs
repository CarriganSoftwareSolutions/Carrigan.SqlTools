using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;

/// <summary>
/// Specifies that a property represents a <c>MONEY</c> or <c>SMALLMONEY</c> column
/// and overrides the default SQL type mapping for that column in the data model.
/// </summary>
/// <remarks>
/// This attribute defines SQL metadata for a property, and that property represents an SQL column in the data model.
/// The attribute supplies the <see cref="SqlTypeDefinition"/> consumed by the SQL generator when emitting SQL.
/// <para>
/// The SQL type is determined by the supplied <see cref="SizeableEnum"/>:
/// </para>
/// <list type="bullet">
/// <item><description><see cref="SizeableEnum.Regular"/> produces <c>MONEY</c>.</description></item>
/// <item><description><see cref="SizeableEnum.Smaller"/> produces <c>SMALLMONEY</c>.</description></item>
/// </list>
/// <para>
/// Suggested CLR type: <see cref="decimal"/>.
/// </para>
/// <para>
/// This attribute affects only SQL generation within <c>Carrigan.SqlTools</c> and does not influence Entity Framework
/// or database schema.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlMoneyAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlMoneyAttribute"/> class and configures
    /// the associated property to represent either a <c>MONEY</c> or <c>SMALLMONEY</c> column,
    /// depending on the supplied <see cref="SizeableEnum"/>.
    /// </summary>
    /// <param name="moneySize">
    /// Determines whether the column is generated as <c>MONEY</c> (<see cref="SizeableEnum.Regular"/>) or
    /// <c>SMALLMONEY</c> (<see cref="SizeableEnum.Smaller"/>).
    /// </param>
    /// <exception cref="NotSupportedException">
    /// Thrown when an unsupported <see cref="SizeableEnum"/> value is supplied. This typically indicates that the
    /// enumeration was extended without updating the <see cref="SqlMoneyAttribute"/> logic.
    /// </exception>
    public SqlMoneyAttribute(SizeableEnum moneySize) : base(GetSqlTypeDefinition(moneySize))
    {
    }

    private static SqlTypeDefinition GetSqlTypeDefinition(SizeableEnum MoneySize) =>
        MoneySize switch
        {
            SizeableEnum.Regular => SqlTypeDefinition.AsMoney(),
            SizeableEnum.Smaller => SqlTypeDefinition.AsSmallMoney(),
            _ => throw new NotSupportedException(
                $"Unsupported {nameof(SizeableEnum)} value '{MoneySize}' for {nameof(SqlMoneyAttribute)}."),
        };
}
