using Carrigan.SqlTools.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Tests.TestEntities.Exceptionals.Aliases;

internal class AliasNameEmpty
{
    [Alias("")]
    public int ExceptionColumn { get; set; }
}
