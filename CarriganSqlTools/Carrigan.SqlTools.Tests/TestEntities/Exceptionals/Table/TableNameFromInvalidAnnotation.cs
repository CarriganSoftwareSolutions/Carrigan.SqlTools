using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Tests.TestEntities.Exceptionals.Table;

[Table("123")]
internal class TableNameFromInvalidAnnotation
{
    public int SomeColumn { get; set; }
}
