using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Tags;
namespace Carrigan.SqlTools.JoinTypes;

/// <summary>
/// Defines a interface for classes that represent one or more SQL join operations.
/// </summary>
public interface IJoins
{
    /// <summary>
    /// Represents a collection of classes where each class defines a single SQL join operation.
    /// The name differs from the preferred “Joins” to avoid a naming conflict (e.g., Joins.Joins),
    /// which would result in a compiler error.
    /// </summary>
    IEnumerable<IJoins> Joints { get; }

    /// <summary>
    /// Generates the SQL fragment for the JOIN clause represented by <see cref="Joints"/>.
    /// </summary>
    /// <returns>The SQL fragment for the JOIN clause represented by <see cref="Joints"/>.</returns>
    public string ToSql();

    /// <summary>
    /// Enumerates all tables included in <see cref="Joints"/>
    /// providing a quick way to determine whether a given table
    /// participates in any join operation.
    /// </summary>
    public IEnumerable<TableTag> TableTags { get; }

    /// <summary>
    /// Recursively get all the parameters associated with the logic.
    /// </summary>
    internal Dictionary<ParameterTag, object> Parameters { get; }
}
