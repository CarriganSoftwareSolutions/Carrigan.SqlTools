using System.Data;

namespace Carrigan.SqlTools.Exceptions;

/// <summary>
/// Thrown when a SQL type is configured with <c>MAX</c> sizing, but the specified
/// <see cref="SqlDbType"/> does not support <c>MAX</c>.
/// </summary>
/// <remarks>
/// In SQL Server, <c>MAX</c> sizing is supported only by a limited set of types
/// (for example: <c>VARCHAR(MAX)</c>, <c>NVARCHAR(MAX)</c>, <c>VARBINARY(MAX)</c>).
/// </remarks>
public class SqlTypeDoesNotSupportMaxSizeException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlTypeDoesNotSupportMaxSizeException"/> class.
    /// </summary>
    /// <param name="sqlDbType">The SQL type that was incorrectly configured to use <c>MAX</c> sizing.</param>
    internal SqlTypeDoesNotSupportMaxSizeException(SqlDbType sqlDbType) : base
        ($"{sqlDbType} does not support the MAX size argument. MAX size is supported only by VarChar, NVarChar, and VarBinary.")
    { } 
}
