using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Base.Tests.TestEntities.Exceptionals.Columns;
internal class ColumnNameFromInvalidAnnotation
{
    [Column("123")]
    public int ExceptionColumn { get; set; }
}
