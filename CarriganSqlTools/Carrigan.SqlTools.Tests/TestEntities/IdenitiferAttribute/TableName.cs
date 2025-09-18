using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Tests.TestEntities.IdenitiferAttribute;
[Table("TableNameTable")]
public class TableName
{
    public Guid Id { get; set; }
    public string? Text { get; set; }
}
