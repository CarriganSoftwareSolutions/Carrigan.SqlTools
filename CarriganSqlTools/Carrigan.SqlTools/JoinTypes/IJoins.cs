using Carrigan.SqlTools.Tags;
namespace Carrigan.SqlTools.JoinTypes;

public interface IJoins
{
    IEnumerable<ISingleJoin> Joints { get; }
    public string ToSql();

    public IEnumerable<TableTag> TableTags { get; }
}
