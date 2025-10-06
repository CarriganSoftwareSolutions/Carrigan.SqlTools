using Carrigan.SqlTools.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Tests.TestEntities.Exceptionals.Columns;
internal class ColumnNameFromInvalidIdentifier
{
    [Identifier("123")]
    public int ExceptionColumn { get; set; }
}
