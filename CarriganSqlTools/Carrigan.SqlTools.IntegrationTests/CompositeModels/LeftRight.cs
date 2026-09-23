using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.IntegrationTests.Models;

namespace Carrigan.SqlTools.IntegrationTests.CompositeModels;

public sealed class LeftRight
{
    [SelectTag<LeftWords>(nameof(LeftWords.Id), nameof(LeftId))]
    public int? LeftId { get; set; }

    [SelectTag<LeftWords>(nameof(LeftWords.LeftWord))]
    public string? LeftWord { get; set; } = string.Empty;

    [SelectTag<RightWords>(nameof(RightWords.Id), nameof(RightId))]
    public int? RightId { get; set; }

    [SelectTag<RightWords>(nameof(RightWords.RightWord))]
    public string? RightWord { get; set; } = string.Empty;
}