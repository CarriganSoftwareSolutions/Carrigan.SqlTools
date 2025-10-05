using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Tests.TestEntities.Exceptionals;
internal class ColumnNameFromInvalidAnnotation
{
    [Column("123")]
    public int ExceptionColumn { get; set; }
}
