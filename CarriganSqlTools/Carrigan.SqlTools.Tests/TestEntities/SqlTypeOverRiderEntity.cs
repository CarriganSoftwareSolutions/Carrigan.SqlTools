using Carrigan.SqlTools.Attributes;
using System.Data;

namespace Carrigan.SqlTools.Tests.TestEntities;

public class SqlTypeOverRiderEntity
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public SqlTypeOverRiderEntity() { }

    [SqlType(SqlDbType.NChar, size: 4000)]
    public string NText { get; set; }
    [SqlType(SqlDbType.Text)]
    public string Text { get; set; }
    [SqlType(SqlDbType.VarChar, size: 8000)]
    public string VarChar { get; set; }
    [SqlType(SqlDbType.NVarChar, size: 4000)]
    public string NVarChar { get; set; }
    [SqlType(SqlDbType.Binary, size: 4000)]
    public byte[] Binary { get; set; }
    [SqlType(SqlDbType.VarBinary, size: 4000)]
    public byte[] VarBinary { get; set; }
    [SqlType(SqlDbType.VarBinary, true)]
    public byte[] VarBinaryMax { get; set; }
    [SqlType(SqlDbType.VarBinary, false)]
    public byte[] VarBinaryDefault { get; set; }

    [SqlType(SqlDbType.Decimal, precision: 12, scale: 13)]
    public decimal Decimal { get; set; }

    [SqlType(SqlDbType.DateTime2, scale: 7)]
    public decimal DateTime2 { get; set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
}
