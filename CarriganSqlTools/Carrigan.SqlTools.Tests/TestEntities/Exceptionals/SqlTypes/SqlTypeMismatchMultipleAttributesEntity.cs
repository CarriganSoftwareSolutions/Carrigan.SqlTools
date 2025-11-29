using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Tests.TestEntities.Exceptionals.SqlTypes;

public sealed class SqlTypeMismatchMultipleAttributesEntity
{
    public int Id { get; set; }

    [SqlChar(EncodingEnum.Unicode, StorageTypeEnum.Fixed, 10)]
    public int InvalidChar { get; set; }

    [SqlBinary(StorageTypeEnum.Fixed, 10)]
    public string InvalidBinary { get; set; } = string.Empty;
}