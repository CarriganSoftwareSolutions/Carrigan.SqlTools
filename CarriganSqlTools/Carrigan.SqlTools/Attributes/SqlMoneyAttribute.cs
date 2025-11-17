using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;
/// <summary>
/// Specifies that a property represents a <c>MONEY</c> or <c>SMALLMONEY</c> column
/// and overrides the default SQL type mapping for that column in the data model.
/// </summary>
/// <remarks>
/// This attribute defines SQL metadata for a property that represents a SQL Server  
/// <c>MONEY</c> or <c>SMALLMONEY</c> column on a table model.  
///
/// <para>
/// The SQL type is determined by the supplied <see cref="SizeableEnum"/>:
/// <list type="bullet">
/// <item><description><see cref="SizeableEnum.Regular"/> → <c>MONEY</c></description></item>
/// <item><description><see cref="SizeableEnum.Smaller"/> → <c>SMALLMONEY</c></description></item>
/// </list>
/// </para>
///
/// <para>
/// When applied to a property, this attribute overrides the default type mapping  
/// used by <see cref="Carrigan.SqlTools"/> during SQL generation and column metadata  
/// reflection.
/// </para>
///
/// <para><strong>Suggested C# Data Type:</strong><br/>
/// Properties mapped to <c>MONEY</c> or <c>SMALLMONEY</c> should use the .NET  
/// <see cref="decimal"/> type.  
/// SQL Server returns both <c>MONEY</c> and <c>SMALLMONEY</c> as <see cref="decimal"/>
/// values through ADO.NET, making <see cref="decimal"/> the correct and lossless .NET representation.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
public sealed class SqlMoneyAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlMoneyAttribute"/> class and configures
    /// the associated property to represent either a <c>MONEY</c> or <c>SMALLMONEY</c> column,
    /// depending on the supplied <see cref="SizeableEnum"/>.
    /// </summary>
    /// <param name="moneySize">
    /// Determines whether the column is generated as <c>MONEY</c>  
    /// (<see cref="SizeableEnum.Regular"/>) or <c>SMALLMONEY</c>  
    /// (<see cref="SizeableEnum.Smaller"/>).
    /// </param>
    /// <exception cref="NotSupportedException">
    /// Thrown when an unsupported <see cref="SizeableEnum"/> value is supplied.
    /// This typically indicates that the enumeration was extended without updating the
    /// <see cref="SqlMoneyAttribute"/> logic.
    /// </exception>
    public SqlMoneyAttribute(SizeableEnum moneySize) :
        base
        (
            moneySize switch
            {
                SizeableEnum.Regular => SqlTypeDefinition.AsMoney(),
                SizeableEnum.Smaller => SqlTypeDefinition.AsSmallMoney(),
                _ => throw new NotSupportedException($"Unsupported {nameof(SizeableEnum)} value '{moneySize}' for SqlMoneyAttribute."),
            }
        )
    {
    }
}
