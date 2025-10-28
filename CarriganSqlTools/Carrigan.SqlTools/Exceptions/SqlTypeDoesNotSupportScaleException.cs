using System.Data;

namespace Carrigan.SqlTools.Exceptions;
public class SqlTypeDoesNotSupportScaleException : Exception
{
    internal SqlTypeDoesNotSupportScaleException(SqlDbType sqlDbType) : base
        ($"{sqlDbType}, does not support the size argument. " +
        "Size is supported by the SQL types: DateTime2, DateTimeOffset, Decimal and Time")
    { } 
}
