using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Tests.TestEntities.Exceptionals.Columns;
internal class ColumnNameFromNullIdentifier
{
    [Identifier(null!)]
    public int ExceptionColumn { get; set; }
}
