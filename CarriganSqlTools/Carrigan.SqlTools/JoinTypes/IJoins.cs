using Carrigan.SqlTools.Tags;
namespace Carrigan.SqlTools.JoinTypes;

/// <summary>
/// Interface for a class that represents a series of SQL join.
/// </summary>
public interface IJoins
{
    /// <summary>
    /// An enumeration of classes that each represent single join.
    /// 
    /// Admittedly this is a terrible name, but the name I wanted, Joins, couldn't be used, 
    /// because then there has to be a Joins.Joins, which causes a compiler error.
    /// In the highly unlikely chance anyone ever takes any interest in this project, I want to state for the record,
    /// this was never intended to be reference to recreational substances. I was thinking more like joins of a frame.
    /// Get your minds of out of the gutter.
    /// </summary>
    IEnumerable<IJoins> Joints { get; }
    public string ToSql();

    /// <summary>
    /// This enumeration represents all the tables that are contained in the enumeration of joins.
    /// This provides a quick means of determining if a given table is part of one of the joins.
    /// </summary>
    public IEnumerable<TableTag> TableTags { get; }
}
