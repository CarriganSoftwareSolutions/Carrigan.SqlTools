using Carrigan.SqlTools.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Tests.TestEntities.Attributes;
[Identifier("SomeTable", "SomeSchema")]
internal class TableAndColumnIdentifiers
{
    [Identifier("SomeId")]
    [Parameter("SomeIdParameter")]
    [PrimaryKey]
    public int Id { get; set; }
    [Identifier("SomeColumn")]
    [Parameter("SomeColumnParameter")]
    public int Column { get; set; }
}
