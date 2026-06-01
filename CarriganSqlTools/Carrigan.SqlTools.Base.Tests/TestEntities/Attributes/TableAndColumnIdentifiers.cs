using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Base.Tests.TestEntities.Attributes;
[Identifier("SomeTable", "SomeSchema")]
internal class TableAndColumnIdentifiers
{
    [Identifier("SomeId")]
    [Parameter("SomeIdParameter")]
    [PrimaryKey]
    public int Id { get; set; }
    [Identifier("SomeColumn")]
    [Parameter("SomeColumnParameter")]
    public int Column { get; set; }
}
