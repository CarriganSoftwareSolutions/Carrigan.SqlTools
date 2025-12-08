using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Attributes;
/// <summary>
/// Specifies that a property represents a <c>VARCHAR(MAX)</c> or <c>NVARCHAR(MAX)</c> column
/// and overrides the default SQL type mapping for that column in the data model.
/// </summary>
/// <remarks>
/// This attribute defines SQL metadata for a property that represents a SQL Server  
/// <c>VARCHAR(MAX)</c> or <c>NVARCHAR(MAX)</c> column on a table model.
///
/// <para>
/// The SQL type is determined by the supplied <see cref="EncodingEnum"/>:
/// <list type="bullet">
/// <item><description><see cref="EncodingEnum.Ascii"/> → <c>VARCHAR(MAX)</c></description></item>
/// <item><description><see cref="EncodingEnum.Unicode"/> → <c>NVARCHAR(MAX)</c></description></item>
/// </list>
/// </para>
///
/// <para><strong>Suggested C# Data Type:</strong><br/>
/// Properties mapped to <c>VARCHAR(MAX)</c> or <c>NVARCHAR(MAX)</c> columns should typically use  
/// the .NET <see cref="string"/> type. ADO.NET materializes both <c>VARCHAR</c> and  
/// <c>NVARCHAR</c> values as <see cref="string"/>, making it the natural representation in the data model.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SqlVarCharMaxAttribute : SqlTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlVarCharMaxAttribute"/> class and configures
    /// the associated property to represent a <c>VARCHAR(MAX)</c> or <c>NVARCHAR(MAX)</c> column,
    /// depending on the specified <see cref="EncodingEnum"/>.
    /// </summary>
    /// <param name="encoding">
    /// Determines whether the column is generated as <c>VARCHAR(MAX)</c> 
    /// (<see cref="EncodingEnum.Ascii"/>) or <c>NVARCHAR(MAX)</c> 
    /// (<see cref="EncodingEnum.Unicode"/>).
    /// </param>
    /// <exception cref="NotSupportedException">
    /// Thrown when an unsupported <see cref="EncodingEnum"/> value is supplied.
    /// This typically indicates that the enumeration was extended without updating the
    /// <see cref="SqlVarCharMaxAttribute"/> logic.
    /// </exception>
    //TODO: I wrote this assuming  default Char length should be the default, for parameters, since parameters do some stuff for you when you leave the length null, however that creates an overflow when returning on insert for a string field. THis needs to be re-evaluated.
    //TODO: the above also impacts the default setting, which is likely more critical as it is the most likely to be left null by a developer.
    public SqlVarCharMaxAttribute(EncodingEnum encoding) :
        base
        (
            encoding switch
            {
                EncodingEnum.Ascii => SqlTypeDefinition.AsVarCharMax(),
                EncodingEnum.Unicode => SqlTypeDefinition.AsNVarCharMax(),
                _ => throw new NotSupportedException($"Unsupported EncodingEnum value '{encoding}' for SqlVarCharMaxAttribute. " +
                "This usually indicates that the enumeration was extended without updating this attribute."),
            }
        )
    {
    }
}
