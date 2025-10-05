using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Tests.TestEntities.Exceptionals;
internal class ColumnNameFromNullAnnotation
{
    [Column(null!)]
    public int ExceptionColumn { get; set; }
}
