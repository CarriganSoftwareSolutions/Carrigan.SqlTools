using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Tests.TestEntities.Exceptionals.Table;

[Identifier(null!)]
internal class TableNameFromNullIdentifier
{
    public int SomeColumn { get; set; }
}
