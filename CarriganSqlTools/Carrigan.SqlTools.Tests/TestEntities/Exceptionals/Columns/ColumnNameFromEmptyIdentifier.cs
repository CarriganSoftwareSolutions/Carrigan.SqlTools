using Carrigan.SqlTools.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Tests.TestEntities.Exceptionals.Columns;
internal class ColumnNameFromEmptyIdentifier
{
    [Identifier("")]
    public int ExceptionColumn { get; set; }
}
