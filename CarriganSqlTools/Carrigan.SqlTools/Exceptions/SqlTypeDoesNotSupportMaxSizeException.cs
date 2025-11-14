using System.Data;

namespace Carrigan.SqlTools.Exceptions;
public class SqlTypeDoesNotSupportMaxSizeException : Exception
{
    internal SqlTypeDoesNotSupportMaxSizeException(SqlDbType sqlDbType) : base
        ($"{sqlDbType}, does not support the MAX size argument. " +
        "MAX size is only supported by the SQL types: VarChar, NVarChar, and VarBinary ")
    { } 
}
