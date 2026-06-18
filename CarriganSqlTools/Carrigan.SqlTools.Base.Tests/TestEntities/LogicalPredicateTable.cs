namespace Carrigan.SqlTools.Base.Tests.TestEntities;

internal class LogicalPredicateTable
{
    public bool IsActive { get; set; }

    public bool IsEnabled { get; set; }

    public bool? IsVisible { get; set; }

    public bool? IsArchived { get; set; }

    public string Name { get; set; } = string.Empty;
}
