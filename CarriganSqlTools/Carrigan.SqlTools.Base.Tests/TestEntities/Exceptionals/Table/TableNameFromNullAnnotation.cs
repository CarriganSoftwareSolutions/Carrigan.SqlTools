using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Base.Tests.TestEntities.Exceptionals.Table;

[Table(null!)]
internal class TableNameFromNullAnnotation
{
    public int SomeColumn { get; set; }
}
