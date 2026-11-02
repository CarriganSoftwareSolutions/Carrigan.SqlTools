using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.SqlGenerators;

public interface ISelectable
{
    public SelectTags SelectTags { get; }
}