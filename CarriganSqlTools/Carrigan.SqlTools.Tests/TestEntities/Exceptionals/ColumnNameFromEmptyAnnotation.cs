using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Tests.TestEntities.Exceptionals;
internal class ColumnNameFromEmptyAnnotation
{
    [Column("")]
    public int ExceptionColumn { get; set; }
}
