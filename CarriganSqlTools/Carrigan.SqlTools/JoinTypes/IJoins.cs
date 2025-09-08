using Carrigan.SqlTools.Tags;
namespace Carrigan.SqlTools.JoinTypes;

/// <summary>
/// Interface for a class that represents a series of SQL join.
/// </summary>
public interface IJoins
{
    /// <summary>
    /// An enumeration of classes that represent each single join.
    /// </summary>
    IEnumerable<ISingleJoin> Joints { get; }
    public string ToSql();

    /// <summary>
    /// This enumeration represents all the tables that are contained in the enumeration of joins.
    /// This provides a quick means of determining if a given table is part of one of the joins.
    /// </summary>
    public IEnumerable<TableTag> TableTags { get; }
}
