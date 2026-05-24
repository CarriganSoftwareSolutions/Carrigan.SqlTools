using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Base.Tests.TestEntities.Exceptionals.Table;

[Table("")]
internal class TableNameFromEmptyAnnotation
{
    public int SomeColumn { get; set; }
}
