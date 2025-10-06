using Carrigan.SqlTools.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Tests.TestEntities.Exceptionals.Parameters;
internal class ParameterNameInvalid
{
    [Parameter("123")]
    public int ExceptionColumn { get; set; }
}
