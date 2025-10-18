using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.JoinTypes;

public abstract class JoinsBase
{
    /// <summary>
    /// Represents a collection of classes where each class defines a single SQL join operation.
    /// The name differs from the preferred “Joins” to avoid a naming conflict (e.g., Joins.Joins),
    /// which would result in a compiler error.
    /// </summary>
    protected abstract IEnumerable<JoinBase> Joints { get; set; }

    /// <summary>
    /// Recursively get all the parameters associated with the logic.
    /// </summary>
    internal Dictionary<ParameterTag, object> Parameters =>
        [.. Joints.SelectMany(join => join.Parameters)];

    internal abstract TableTag TableTag { get; }

    /// <summary>
    /// Enumerates all tables included in <see cref="Joints"/> and <typeparamref name="leftT"/>
    /// providing a quick way to determine whether a given table
    /// participates in any join operation.
    /// </summary>
    internal IEnumerable<TableTag> TableTags =>
        Joints.Select(join => join.TableTag).Append(TableTag);

    /// <summary>
    /// Generates the SQL fragment for the JOIN clause represented by <see cref="Joints"/>.
    /// </summary>
    /// <returns>The SQL fragment for the JOIN clause represented by <see cref="Joints"/>.</returns>
    internal string ToSql() =>
        string.Join(" ", Joints.Select(join => join.ToSql()));

    /// <summary>
    /// Determines if the Joins object is empty and contains no joins.
    /// </summary>
    /// <returns>True is empty, false it not empty.</returns>
    internal bool IsEmpty() =>
        Joints.None();
}