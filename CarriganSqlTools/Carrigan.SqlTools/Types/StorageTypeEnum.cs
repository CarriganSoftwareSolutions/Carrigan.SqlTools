namespace Carrigan.SqlTools.Types;
/// <summary>
/// Specifies whether a SQL Server character or binary data type uses fixed-length
/// or variable-length storage.
/// </summary>
/// <remarks>
/// This enumeration is intended for use when selecting between SQL types that
/// share the same general category (character or binary) but differ in how they
/// allocate storage.
/// <para>Examples include:</para>
/// <list type="bullet">
///   <item>
///     <description><c>CHAR</c> (fixed) vs. <c>VARCHAR</c> (variable)</description>
///   </item>
///   <item>
///     <description><c>NCHAR</c> (fixed) vs. <c>NVARCHAR</c> (variable)</description>
///   </item>
///   <item>
///     <description><c>BINARY</c> (fixed) vs. <c>VARBINARY</c> (variable)</description>
///   </item>
/// </list>
/// </remarks>
public enum StorageTypeEnum
{
    /// <summary>
    /// Indicates the use of a fixed-length SQL type (e.g., <c>CHAR</c>,
    /// <c>NCHAR</c>, <c>BINARY</c>).
    /// </summary>
    Fixed = 0,

    /// <summary>
    /// Indicates the use of a variable-length SQL type (e.g., <c>VARCHAR</c>,
    /// <c>NVARCHAR</c>, <c>VARBINARY</c>).
    /// </summary>
    Var = 1
}
