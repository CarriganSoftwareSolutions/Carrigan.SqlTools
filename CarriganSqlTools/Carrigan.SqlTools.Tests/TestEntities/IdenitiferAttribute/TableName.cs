using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Tests.TestEntities.IdenitiferAttribute;
[Table("TableNameTable")]
internal class TableName
{
    internal Guid Id { get; set; }
    internal string? Text { get; set; }
}
