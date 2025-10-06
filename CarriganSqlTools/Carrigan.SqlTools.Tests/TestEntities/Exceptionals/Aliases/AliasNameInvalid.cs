using Carrigan.SqlTools.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Tests.TestEntities.Exceptionals.Aliases;
internal class AliasNameInvalid
{
    [Alias("123")]
    public int ExceptionColumn { get; set; }
}
