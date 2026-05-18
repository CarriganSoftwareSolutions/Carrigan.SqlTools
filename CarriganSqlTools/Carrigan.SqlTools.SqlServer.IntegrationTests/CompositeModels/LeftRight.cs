using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.SqlServer.IntegrationTests.Models;

namespace Carrigan.SqlTools.SqlServer.IntegrationTests.CompositeModels;

public sealed class LeftRight
{
    [SelectTag<Left>(nameof(Left.Id), nameof(LeftId))]
    public int? LeftId { get; set; }

    [SelectTag<Left>(nameof(Left.LeftWord))]
    public string? LeftWord { get; set; } = string.Empty;

    [SelectTag<Right>(nameof(Right.Id), nameof(RightId))]
    public int? RightId { get; set; }

    [SelectTag<Right>(nameof(Right.RightWord))]
    public string? RightWord { get; set; } = string.Empty;
}