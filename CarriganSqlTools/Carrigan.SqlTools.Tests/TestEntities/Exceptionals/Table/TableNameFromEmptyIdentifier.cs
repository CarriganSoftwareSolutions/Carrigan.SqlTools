using Carrigan.SqlTools.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Tests.TestEntities.Exceptionals.Table;

[Identifier("")]
internal class TableNameFromEmptyIdentifier
{
    public int SomeColumn { get; set; }
}
