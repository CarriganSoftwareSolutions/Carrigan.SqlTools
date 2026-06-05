using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.SqlServer;
using Carrigan.SqlTools.Tags;

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
/// <see cref="ColumnBase{T}"/> validates property names and throws an exception if the property isn't valid.
/// </para>
/// <code language="csharp"><![CDATA[
/// Parameter parameterEmail = new("@example.", "Email");
/// Column<Customer> columnEmail = new(nameof(Customer.Email));
/// Contains<Customer> predicate = new(columnEmail, parameterEmail);
/// SelectBuilder<Customer> selectBuilder = new()
/// {
///     Where = predicate
/// };
/// 
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].*
/// FROM [Customer]
/// WHERE CONTAINS([Customer].[Email], @Email_1)
/// ]]></code>
/// </example>
public class Contains<T> : Predicates where T : class
{
    /// <summary>
    /// The column expression searched by the full-text predicate.
    /// </summary>
    private readonly ColumnBase<T> _column;

    /// <summary>
    /// The search-condition parameter rendered as the second argument to <c>CONTAINS</c>.
    /// </summary>
    private readonly Parameter _parameter;

    /// <summary>
    /// Initializes a new instance of the <see cref="Contains{T}"/> predicate.
    /// </summary>
    /// <param name="column">
    /// The left-hand operand, representing the full-text indexed column (<see cref="ColumnBase{T}"/>).
    /// </param>
    /// <param name="parameter">
    /// The right-hand operand, representing the search term parameter (<see cref="Predicates.Parameter"/>).
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="column"/> or <paramref name="parameter"/> is <c>null</c>.
    /// </exception>
    public Contains(ColumnBase<T> column, Parameter parameter) : base([column, parameter])
    {
        ArgumentNullException.ThrowIfNull(column, nameof(column));
        ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

        _column = column;
        _parameter = parameter;
    }

    /// <summary>
    /// Produces the SQL fragment represented by this predicate.
    /// </summary>
    /// <returns>
    /// A SQL fragment of the form <c>CONTAINS(&lt;column&gt;, &lt;parameter&gt;)</c>.
    /// </returns>
    internal override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        yield return new SqlFragmentText("CONTAINS(");

        foreach (ISqlFragment fragment in _column.ToSqlFragments(dialect))
            yield return fragment;

        yield return new SqlFragmentText(", ");

        foreach (ISqlFragment fragment in _parameter.ToSqlFragments(dialect))
            yield return fragment;

        yield return new SqlFragmentText(")");
    }
}
