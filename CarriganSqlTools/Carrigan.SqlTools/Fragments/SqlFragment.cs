using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.PredicatesLogic;

namespace Carrigan.SqlTools.Fragments;

/// <summary>
/// Represents a single fragment of an SQL statement.
/// </summary>
/// <remarks>
/// This type is the common base for fragment implementations such as:
/// <list type="bullet">
/// <item><description><see cref="SqlFragmentText"/> for literal SQL text.</description></item>
/// <item><description><see cref="SqlFragmentParameter"/> for parameter references.</description></item>
/// </list>
/// <para>
/// </remarks>
public abstract class SqlFragment
{
    /// <summary>
    /// Converts this fragment into its SQL representation.
    /// </summary>
    /// <returns>The SQL text for this fragment.</returns>
    internal abstract string ToSql();

    /// <summary>
    /// Gets the parameters that are referenced by this fragment.
    /// </summary>
    /// <returns>An enumerable collection of <see cref="Parameter"/> objects referenced by this fragment.</returns>
    internal abstract IEnumerable<Parameter> GetParameters();

    /// <summary>
    /// Returns an enumerable sequence containing the specified SQL fragments in order.
    /// </summary>
    /// <param name="fragment1">The first SQL fragment to include in the sequence.</param>
    /// <param name="fragment2">The second SQL fragment to include in the sequence.</param>
    /// <returns>An enumerable sequence of SQL fragments, with the first fragment followed by the second fragment.</returns>
    internal IEnumerable<SqlFragment> Append(SqlFragment fragment2)
    {
        yield return this;
        yield return fragment2;
    }

    /// <summary>
    /// Returns a sequence that contains the specified initial fragment followed by the elements of the provided
    /// fragment collection.
    /// </summary>
    /// <param name="fragment1">The first fragment to include in the resulting sequence.</param>
    /// <param name="fragments">A collection of fragments to append after the initial fragment. Cannot be null.</param>
    /// <returns>An <see cref="IEnumerable{SqlFragment}"/> containing the initial fragment followed by the elements of <paramref
    /// name="fragments"/>.</returns>
    internal IEnumerable<SqlFragment> Concat(IEnumerable<SqlFragment> fragments)
    {
        yield return this;
        foreach (SqlFragment f in fragments)
        {
            yield return f;
        }
    }

    /// <summary>
    /// Returns an enumerable sequence containing only this SQL fragment.
    /// </summary>
    /// <returns>An <see cref="IEnumerable{SqlFragment}"/> containing only this fragment.</returns>
    internal IEnumerable<SqlFragment> AsEnumerable()
    {
        yield return this;
    }

    /// <summary>
    /// Returns a flattened sequence of all child SQL fragments contained within this fragment.
    /// </summary>
    /// <remarks>Use this method to enumerate all nested SQL fragments in a single, flat sequence, regardless
    /// of their original hierarchical structure.</remarks>
    /// <returns>An enumerable collection of <see cref="SqlFragment"/> objects representing the flattened structure of this
    /// fragment. The collection may be empty if there are no child fragments.</returns>
    internal abstract IEnumerable<SqlFragment> Flaten();
}
