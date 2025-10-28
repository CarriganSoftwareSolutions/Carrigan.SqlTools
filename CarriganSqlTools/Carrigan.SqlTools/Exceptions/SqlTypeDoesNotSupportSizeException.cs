using System.Data;

namespace Carrigan.SqlTools.Exceptions;
public class SqlTypeDoesNotSupportSizeException : Exception
{
    internal SqlTypeDoesNotSupportSizeException(SqlDbType sqlDbType) : base
        ($"{sqlDbType}, does not support the size argument. " +
        "Size is supported by the SQL types: Binary, Char, NChar, NVarChar, VarBinary and VarChar")
    { } 
}
