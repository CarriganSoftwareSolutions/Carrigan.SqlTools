namespace Carrigan.SqlTools.Types;

/// <summary>
/// Specifies the character encoding to use when mapping .NET members to SQL
/// character-based data types.
/// </summary>
/// <remarks>
/// This enumeration is intended to distinguish between SQL Server types that use
/// single-byte ASCII character storage and those that use Unicode storage.
/// <para>
/// Typical mappings include:
/// </para>
/// <list type="bullet">
///   <item>
///     <description><see cref="EncodingEnum.Ascii"/> → <c>CHAR</c> / <c>VARCHAR</c>/ <c>TEXT</c></description>
///   </item>
///   <item>
///     <description><see cref="EncodingEnum.Unicode"/> → <c>NCHAR</c> / <c>NVARCHAR</c> / <c>NTEXT</c></description>
///   </item>
/// </list>
/// </remarks>
public enum EncodingEnum
{
    /// <summary>
    /// Indicates that an ASCII (single-byte) SQL character type should be used,
    /// such as <c>CHAR</c>, <c>VARCHAR</c> or  <c>TEXT</c>.
    /// </summary>
    Ascii = 0,

    /// <summary>
    /// Indicates that a Unicode SQL character type should be used,
    /// such as <c>NCHAR</c>, <c>NVARCHAR</c> or <c>NTEXT</c>.
    /// </summary>
    Unicode = 1
}
