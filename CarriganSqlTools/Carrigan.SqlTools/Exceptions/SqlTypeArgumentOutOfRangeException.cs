using System.Data;

namespace Carrigan.SqlTools.Exceptions;
public class SqlTypeArgumentOutOfRangeException : ArgumentOutOfRangeException
{
    internal SqlTypeArgumentOutOfRangeException(SqlDbType sqlDbType, string parameterName, int value, int minValue, int maxValue) : base
        (parameterName, value, $"{parameterName} is out of range for an SQL type of {sqlDbType}. The expected range is [{minValue}, {maxValue}] inclusive.")
    { } 
}
