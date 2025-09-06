using SqlTools.Tags;
namespace SqlTools.Joins;

public interface IJoins
{
    IEnumerable<ISingleJoin> Joints { get; }
    public string ToSql();

    public IEnumerable<TableTag> TableTags { get; }
}
