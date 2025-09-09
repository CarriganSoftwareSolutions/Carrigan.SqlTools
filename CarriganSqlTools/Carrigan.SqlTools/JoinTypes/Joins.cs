using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.JoinTypes;

/// <summary>
/// This class represents multiple joins to be strung together.
/// </summary>
public class Joins : IJoins
{
    /// <summary>
    /// An enumeration of all the joins.
    /// </summary>
    public IEnumerable<IJoins> Joints { get; private set; }
    public Joins(params IEnumerable<IJoins> joins) =>
        Joints = joins;

    /// <summary>
    /// This enumeration provides a quick way to determine what all tables are involved in all of the Joins.
    /// </summary>
    public IEnumerable<TableTag> TableTags =>
        Joints.SelectMany(join => join.TableTags);

    /// <summary>
    /// This generates the SQL for the Joins as a string.
    /// </summary>
    /// <returns>A string for the Joins' SQL</returns>
    public string ToSql() =>
        string.Join(" ", Joints.Select(join => join.ToSql()));
}
