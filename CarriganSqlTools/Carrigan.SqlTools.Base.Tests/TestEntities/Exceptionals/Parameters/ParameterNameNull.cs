using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Base.Tests.TestEntities.Exceptionals.Parameters;
internal class ParameterNameNull
{
    [Parameter(null!)]
    public int ExceptionColumn { get; set; }
}
