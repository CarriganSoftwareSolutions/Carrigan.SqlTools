using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Base.Tests.TestEntities.Exceptionals.Columns;
internal class ColumnNameFromInvalidIdentifier
{
    [Identifier("123")]
    public int ExceptionColumn { get; set; }
}
