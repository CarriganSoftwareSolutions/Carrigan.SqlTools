using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Base.Tests.TestEntities.Exceptionals.Parameters;

internal class ParameterNameEmpty
{
    [Parameter("")]
    public int ExceptionColumn { get; set; }
}
