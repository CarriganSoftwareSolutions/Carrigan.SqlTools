using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Tests.TestEntities.Exceptionals.SqlTypes;

public sealed class SqlTypeMismatchAttributeEntity
{
    public int Id { get; set; }

    [SqlChar(EncodingEnum.Unicode, StorageTypeEnum.Fixed, 10)]
    public int InvalidChar { get; set; }
}