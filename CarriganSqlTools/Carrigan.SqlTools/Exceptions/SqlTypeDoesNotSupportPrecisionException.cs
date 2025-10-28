using System.Data;

namespace Carrigan.SqlTools.Exceptions;
public class SqlTypeDoesNotSupportPrecisionException : Exception
{
    internal SqlTypeDoesNotSupportPrecisionException(SqlDbType sqlDbType) : base
        ($"{sqlDbType}, does not support the precision argument. " +
        "Size is supported by the SQL types: Float and Decimal.")
    { } 
}
