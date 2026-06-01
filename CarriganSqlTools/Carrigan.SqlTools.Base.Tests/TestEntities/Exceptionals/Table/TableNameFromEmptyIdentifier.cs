using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Base.Tests.TestEntities.Exceptionals.Table;

[Identifier("")]
internal class TableNameFromEmptyIdentifier
{
    public int SomeColumn { get; set; }
}
