using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Tests.TestEntities.Attributes;
internal class TableWithAliases
{
    [PrimaryKey]
    [Alias("TableId")]
    public int Id { get; set; }
    public string? Name { get; set; }
}
