using SqlTools.Tags;

namespace SqlTools.Joins;

public class Joins : IJoins
{
    public IEnumerable<ISingleJoin> Joints { get; private set; }
    public Joins(params IEnumerable<ISingleJoin> joins) =>
        Joints = joins;

    public IEnumerable<TableTag> TableTags =>
        Joints.SelectMany(join => join.TableTags);

    public string ToSql() =>
        string.Join(" ", Joints.Select(join => join.ToSql()));
}
