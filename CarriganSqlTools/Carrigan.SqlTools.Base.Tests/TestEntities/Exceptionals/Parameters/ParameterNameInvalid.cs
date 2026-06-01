using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Base.Tests.TestEntities.Exceptionals.Parameters;
internal class ParameterNameInvalid
{
    [Parameter("123")]
    public int ExceptionColumn { get; set; }
}
