using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.JoinTypes;

/// <summary>
/// Serves as the base class for all SQL <c>JOIN</c> operation classes.
/// </summary>
public abstract class JoinBaseClass : IJoins
{
    /// <summary>
    /// Base constructor.
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public JoinBaseClass() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    
    /// <summary>
    /// Derived classes should assign this property in their constructor
    /// or override <see cref="TableTags"/> to provide the required table information.
    /// </summary>
    protected IEnumerable<TableTag> _tableTags;

    /// <summary>
    /// Represents a collection of classes, each describing a single SQL join operation.
    /// The name differs from the preferred “Joins” to avoid a naming conflict
    /// (e.g., a Joins.Joins type), which would result in a compiler error.
    /// </summary>
    public IEnumerable<IJoins> Joints => 
        [this];

    /// <summary>
    /// Enumerates all tables included in <see cref="Joints"/>
    /// providing a quick way to determine whether a given table
    /// participates in any join operation.
    /// </summary>
    public IEnumerable<TableTag> TableTags => 
        _tableTags;

    /// <summary>
    /// Enumerates all possible columns included in <see cref="Joints"/>
    /// providing a quick way to determine whether a given column
    /// participates in a table that participates in any join operation.
    /// </summary>
    public abstract IEnumerable<ColumnTag> ColumnsTags { get; }

    /// <summary>
    /// Generates the SQL fragment for the JOIN clause represented by <see cref="Joints"/>.
    /// </summary>
    /// <returns>The SQL fragment for the JOIN clause represented by <see cref="Joints"/>.</returns>
    public abstract string ToSql();


    /// <summary>
    /// Recursively get all the parameters associated with the logic.
    /// </summary>
    public abstract Dictionary<ParameterTag, object> Parameters { get; }
}
