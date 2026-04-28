using Carrigan.SqlTools.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Dialects;

/// <summary>
/// This class provides the appropriate ISqlDialects implementation based on the specified DialectEnum. 
/// Currently, it only supports the SqlServer dialect, but it can be easily extended to support additional dialects in the future.
/// </summary>
internal static class DialectProvider
{
    /// <summary>
    /// Represents the SQL dialect implementation for Microsoft SQL Server.
    /// </summary>
    private static readonly ISqlDialects SqlServerDialect = new SqlServerDialect();
    /// <summary>
    /// Retrieves the SQL dialect implementation corresponding to the specified dialect enumeration value.
    /// </summary>
    /// <param name="dialect">The dialect enumeration value that specifies which SQL dialect implementation to retrieve.</param>
    /// <returns>An implementation of the ISqlDialects interface for the specified dialect.</returns>
    /// <exception cref="NotImplementedException">Thrown if the specified dialect is not supported.</exception>
    internal static ISqlDialects GetDialect(DialectEnum dialect) =>
        dialect switch
        {
            DialectEnum.SqlServer => SqlServerDialect, 
            _ => throw new NotImplementedException($"The {dialect} dialect is not implemented yet.")
        };
}
