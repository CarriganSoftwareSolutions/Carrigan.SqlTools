using Carrigan.SqlTools.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Tests.TestEntities.Exceptionals.Parameters;

internal class ParameterNameEmpty
{
    [Parameter("")]
    public int ExceptionColumn { get; set; }
}
