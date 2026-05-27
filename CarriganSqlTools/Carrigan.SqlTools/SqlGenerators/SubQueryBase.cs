using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using System.Text;

//IGNORE SPELLING: subquery, subqueries

namespace Carrigan.SqlTools.SqlGenerators;

/// <summary>
/// Represents a base class for subqueries, encapsulating common logic for rendering and parameter management.
/// </summary>
public class SubqueryBase : ISqlFragment
{
    /// <summary>
    /// The SQL dialect to use for rendering the fragments of this subquery. 
    /// This ensures that the generated SQL is compatible with the target database system.
    /// </summary>
    private readonly ISqlDialects Dialect;
    /// <summary>
    /// The sequence of SQL fragments that make up the subquery. These fragments will be rendered 
    /// together to form the complete SQL text of the subquery  when it is consumed by a predicate 
    /// or included in a larger query.
    /// </summary>
    private readonly IEnumerable<ISqlFragment> SqlFragments;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubqueryBase"/> class with the specified SQL fragments and dialect.
    /// </summary>
    /// <param name="sqlFragments">
    /// The sequence of SQL fragments that make up the subquery. These fragments will be rendered 
    /// together to form the complete SQL text of the subquery when it is consumed  by a predicate
    /// or included in a larger query.
    /// </param>
    /// <param name="dialect">
    /// The SQL dialect to use for rendering the fragments of this subquery. 
    /// This ensures that the generated SQL is compatible with the target database system.
    /// </param>
    public SubqueryBase(IEnumerable<ISqlFragment> sqlFragments, ISqlDialects dialect)
    {
        IEnumerable<ISqlFragment> GetFragments()
        {
            yield return new SqlFragmentText("(");
            foreach (ISqlFragment fragment in sqlFragments)
                yield return fragment;
            yield return new SqlFragmentText(")");
        }
        SqlFragments = GetFragments();
        Dialect = dialect;
    }

    /// <summary>
    /// Flattens the nested structure of SQL fragments into a single enumerable sequence. This
    /// is necessary because the SQL fragments may be composed of other fragments, and we need 
    /// to ensure that we have a single level of fragments when rendering the SQL text or extracting parameters. 
    /// </summary>
    /// <returns></returns>
    public IEnumerable<ISqlFragment> Flatten() =>
        SqlFragments.Flatten();

    /// <summary>
    /// Extracts the parameters from the SQL fragments of this subquery. This method traverses the fragments
    /// and collects any parameters that are defined within those fragments, returning them as a single 
    /// enumerable sequence.
    /// </summary>
    /// <returns>
    /// An enumerable sequence of <see cref="SqlFragmentParameter"/> objects that represent the parameters defined
    /// within the SQL fragments of this subquery. 
    /// </returns>
    public IEnumerable<SqlFragmentParameter> GetSqlFragmentParameters() =>
        SqlFragments.GetSqlFragmentParameters(Dialect);

    /// <summary>
    /// Renders the SQL fragments of this subquery into a complete SQL string. This method concatenates the SQL
    /// text of each fragment, using the provided SQL dialect to ensure that any database-specific syntax is 
    /// correctly applied.
    /// </summary>
    /// <param name="dialect">
    /// The SQL dialect to use for rendering the fragments of this subquery. 
    /// This ensures that the generated SQL is compatible with the target database system.
    /// </param>
    /// <returns>
    /// A string that represents the complete SQL text of this subquery, rendered according
    /// to the specified SQL dialect.
    /// </returns>
    public string ToSql(ISqlDialects dialect)
    {
        StringBuilder stringBuilder = new();
        foreach (ISqlFragment fragment in SqlFragments.Flatten())
        {
            stringBuilder.Append(fragment.ToSql(dialect));
        }
        return stringBuilder.ToString();
    }
}