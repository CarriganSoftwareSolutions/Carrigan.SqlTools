using Carrigan.SqlTools.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Tests.TestEntities.Attributes;
internal class AliasEntity
{
    public int Id { get; set; }
    [Alias("AnAlias")]
    public string? TestColumn { get; set; }
    public string? NoAlias { get; set; }
}
