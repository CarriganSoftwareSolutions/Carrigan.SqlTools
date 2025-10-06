using Carrigan.SqlTools.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Tests.TestEntities.Exceptionals.Aliases;
internal class AliasNameNull
{
    [Alias(null!)]
    public int ExceptionColumn { get; set; }
}
