using Carrigan.SqlTools.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Base.Tests.TestEntities.Exceptionals.Table;

[Identifier("123")]
internal class TableNameFromInvalidIdentifier
{
    public int SomeColumn { get; set; }
}
