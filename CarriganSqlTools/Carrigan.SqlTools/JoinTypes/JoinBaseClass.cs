using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.JoinTypes;

/// <summary>
/// Base class for join classes.
/// </summary>
public abstract class JoinBaseClass : IJoins
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    /// <summary>
    /// Base constructor.
    /// </summary>
    public JoinBaseClass() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    /// <summary>
    /// Derived classes should set this in the constructor, or <see cref="TableTags"/> should be overwritten. 
    /// </summary>
    protected IEnumerable<TableTag> _tableTags;

    /// <summary>
    /// An enumeration of classes that each represent single join.
    /// 
    /// As I already explained in the IJoins interface:
    /// Admittedly this is a terrible name, but the name I wanted, Joins, couldn't be used, 
    /// because then there has to be a Joins.Joins, which causes a compiler error.
    /// In the highly unlikely chance anyone ever takes any interest in this project, I want to state for the record,
    /// this was never intended to be reference to recreational substances. I was thinking more like joins of a frame.
    /// Get your minds of out of the gutter.
    /// </summary>
    public IEnumerable<IJoins> Joints => 
        [this];

    /// <summary>
    /// This enumeration provides a quick way to determine what all tables are involved in the Join.
    /// </summary>
    public IEnumerable<TableTag> TableTags => 
        _tableTags;

    /// <summary>
    /// This generates the SQL for the Join as a string.
    /// </summary>
    /// <returns>A string for the Join's SQL</returns>
    public abstract string ToSql();
}
