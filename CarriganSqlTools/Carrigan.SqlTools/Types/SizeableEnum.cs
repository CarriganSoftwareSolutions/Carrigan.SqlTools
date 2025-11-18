namespace Carrigan.SqlTools.Types;
/// <summary>
/// Specifies whether a SQL type should use its smaller or regular storage variant.
/// </summary>
/// <remarks>
/// This enumeration is intended for use with SQL Server data types that support
/// both a standard and a reduced-precision (or reduced-range) variant.  
/// For example:
/// <list type="bullet">
///   <item><description><c>Money</c> vs. <c>SmallMoney</c></description></item>
///   <item><description><c>DateTime</c> vs. <c>SmallDateTime</c></description></item>
/// </list>
/// </remarks>
public enum SizeableEnum
{
    /// <summary>
    /// Indicates that the smaller, reduced-range, or reduced-precision SQL type
    /// should be used (e.g., <c>SmallMoney</c>, <c>SmallDateTime</c>).
    /// </summary>
    Smaller = 0,

    /// <summary>
    /// Indicates that the regular, full-range SQL type should be used 
    /// (e.g., <c>Money</c>, <c>DateTime</c>).
    Regular = 1
}
