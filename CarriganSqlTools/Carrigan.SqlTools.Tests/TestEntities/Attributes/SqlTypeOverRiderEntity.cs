using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Tests.TestEntities.Attributes;

public sealed class SqlTypeOverRiderEntity
{
    public int Id { get; set; }

    #region CharacterAndTextOverrides

    [SqlChar(EncodingEnum.Unicode, StorageTypeEnum.Fixed, 4000)]
    public string NChar { get; set; } = string.Empty;

    [SqlText(EncodingEnum.Ascii)]
    public string Text { get; set; } = string.Empty;

    [SqlChar(EncodingEnum.Ascii, StorageTypeEnum.Var, 8000)]
    public string VarChar { get; set; } = string.Empty;

    [SqlChar(EncodingEnum.Unicode, StorageTypeEnum.Var, 4000)]
    public string NVarChar { get; set; } = string.Empty;

    #endregion

    #region BinaryOverrides

    [SqlBinary(StorageTypeEnum.Fixed, 4000)]
    public byte[] Binary { get; set; } = [];

    [SqlBinary(StorageTypeEnum.Var, 4000)]
    public byte[] VarBinary { get; set; } = [];

    [SqlVarBinaryMax]
    public byte[] VarBinaryMax { get; set; } = [];

    #endregion

    #region NumericOverrides

    [SqlDecimal(18, 4)]
    public decimal Decimal { get; set; }

    [SqlFloat(53)]
    public double Float { get; set; }

    #endregion

    #region DateAndTimeOverrides

    [SqlDateTime2(7)]
    public DateTime DateTime2 { get; set; }

    [SqlDate]
    public DateOnly Date { get; set; }

    [SqlTime(7)]
    public TimeOnly Time { get; set; }

    [SqlDateTimeOffset(7)]
    public DateTimeOffset DateTimeOffset { get; set; }

    #endregion
}
