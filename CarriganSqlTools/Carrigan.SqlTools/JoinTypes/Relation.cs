using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.JoinTypes;

/// <summary>
/// Serves as the base class for all SQL <c>JOIN</c> operation classes.
/// </summary>
public abstract class Relation
{
    protected readonly Predicates _predicates;
    /// <summary>
    /// Base constructor.
    /// </summary>
   public Relation(Predicates predicates) =>
        _predicates = predicates;

    /// <summary>
    /// Table Tag associated with the right side of the relationship.
    /// </summary>
    internal abstract TableTag TableTag { get; }

    internal IEnumerable<TableTag> JoinsOn =>
        _predicates.Columns.Select(column => column.ColumnInfo.ColumnTag.TableTag);

    /// <summary>
    /// Generates the SQL fragment for the JOIN clause represented by the Relationship.
    /// </summary>
    /// <returns>Generates the SQL fragment for the JOIN clause represented by the Relationship.</returns>
    internal abstract string ToSql();

    /// <summary>
    /// Recursively get all the parameters associated with the logic.
    /// </summary>
    internal Dictionary<ParameterTag, object> Parameters =>
        _predicates.GetParameters();
}
