using System.Data;

namespace Carrigan.SqlTools.Exceptions;

/// <summary>
/// Thrown when a numeric SQL type argument (such as length, precision, or scale)
/// is outside the valid inclusive range for the associated <see cref="SqlDbType"/>.
/// </summary>
/// <remarks>
/// This exception is primarily thrown by <c>SqlTypeDefinition</c> factory methods when validating
/// SQL type metadata (for example, <c>FLOAT(precision)</c> where precision must be within a fixed range).
/// </remarks>
public class SqlTypeArgumentOutOfRangeException : ArgumentOutOfRangeException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlTypeArgumentOutOfRangeException"/> class.
    /// </summary>
    /// <param name="sqlDbType">The SQL type associated with the out-of-range value.</param>
    /// <param name="parameterName">The parameter name associated with the out-of-range value.</param>
    /// <param name="value">The provided value.</param>
    /// <param name="minValue">The inclusive minimum allowed value.</param>
    /// <param name="maxValue">The inclusive maximum allowed value.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="parameterName"/> is <c>null</c>.</exception>
    internal SqlTypeArgumentOutOfRangeException(SqlDbType sqlDbType, string parameterName, int value, int minValue, int maxValue) : base
        (parameterName, value, $"{parameterName} is out of range for SQL type {sqlDbType}. The expected range is [{minValue}, {maxValue}] inclusive.")
    { } 
}
