using Carrigan.SqlTools.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carrigan.SqlTools.Base.Tests.TestEntities.Exceptionals.Parameters;
internal class ParameterNameNull
{
    [Parameter(null!)]
    public int ExceptionColumn { get; set; }
}
