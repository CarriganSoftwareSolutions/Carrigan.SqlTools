using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.Tags;
using System;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Predicate for SQL Server's full-text <c>CONTAINS</c> operator.
/// Combines a single column and a single parameter (search term) into a
/// <c>CONTAINS(column, parameter)</c> expression for use in <c>WHERE</c> or <c>JOIN</c> conditions.
/// </summary>
/// <typeparam name="T">
/// The model type that maps to the table containing the target column.
/// </typeparam>
/// <example>
/// <para>
/// <see cref="Column{T}"/> validates property names and throws an exception if the property isn't valid.
/// </para>
/// <code language="csharp"><![CDATA[
/// Parameter parameterEmail = new("Email", "@example.");
/// Column<Customer> columnEmail = new(nameof(Customer.Email));
/// Contains<Customer> predicate = new(columnEmail, parameterEmail);
/// SqlQuery query = customerGenerator.Select(null, null, predicate, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* 
/// FROM [Customer] 
/// WHERE CONTAINS([Customer].[Email], @Parameter_Email)
/// ]]></code>
/// </example>
public class Contains<T> : Predicates
{
    private readonly Column<T> _column;
    private readonly Parameter _parameter;

    /// <summary>
    /// Initializes a new instance of the <see cref="Contains{T}"/> predicate.
    /// </summary>
    /// <param name="column">
    /// The left-hand operand, representing the full-text indexed column (<see cref="Column{T}"/>).
    /// </param>
    /// <param name="parameter">
    /// The right-hand operand, representing the search term parameter (<see cref="Predicates.Parameter"/>).
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="column"/> or <paramref name="parameter"/> is <c>null</c>.
    /// </exception>
    public Contains(Column<T> column, Parameter parameter) : base([column, parameter])
    {
        ArgumentNullException.ThrowIfNull(column, nameof(column));
        ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

        _column = column;
        _parameter = parameter;
    }

    /// <summary>
    /// Produces the SQL fragment represented by this predicate.
    /// </summary>
    /// <param name="prefix">
    /// A disambiguation prefix accumulated during predicate-tree traversal.
    /// Used to ensure parameter names are unique when duplicates exist.
    /// </param>
    /// <param name="branchName">
    /// The branch prefix that is prepended to the beginning of all parameter names in this predicate tree.
    /// </param>
    /// <param name="duplicates">
    /// The set of user-supplied <see cref="ParameterTag"/> values detected as duplicates.
    /// Leaf nodes use this to decide if the <paramref name="prefix"/> should be applied.
    /// </param>
    /// <returns>
    /// A SQL fragment of the form <c>CONTAINS(&lt;column&gt;, &lt;parameter&gt;)</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="prefix"/> or <paramref name="branchName"/> or <paramref name="duplicates"/> is <c>null</c>.
    /// </exception>
    internal override IEnumerable<SqlFragment> ToSqlFragments()
    {
        yield return new SqlFragmentText("CONTAINS(");

        foreach (SqlFragment fragment in _column.ToSqlFragments())
            yield return fragment;

        yield return new SqlFragmentText(", ");

        foreach (SqlFragment fragment in _parameter.ToSqlFragments())
            yield return fragment;

        yield return new SqlFragmentText(")");
    }
}
