using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Base.Tests.TestEntities.Exceptionals.Columns;
internal class ColumnNameFromEmptyAnnotation
{
    [Column("")]
    public int ExceptionColumn { get; set; }
}
