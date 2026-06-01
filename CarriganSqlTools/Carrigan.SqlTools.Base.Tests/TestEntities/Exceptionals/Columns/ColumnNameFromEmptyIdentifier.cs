using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Base.Tests.TestEntities.Exceptionals.Columns;
internal class ColumnNameFromEmptyIdentifier
{
    [Identifier("")]
    public int ExceptionColumn { get; set; }
}
