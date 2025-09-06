using SqlTools.Tags;

namespace SqlTools.Joins;

public abstract class JoinBaseClass : ISingleJoin
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public JoinBaseClass() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    protected IEnumerable<TableTag> _tableTags;

    public IEnumerable<ISingleJoin> Joints => 
        [this];

    IEnumerable<TableTag> IJoins.TableTags => 
        _tableTags;

    public abstract string ToSql();
}
