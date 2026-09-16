using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents a SQL <c>COALESCE</c> expression that returns the first non-null value from a sequence of expressions.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// Coalesce coalesce = new(new Column<Customer>(nameof(Customer.Phone)), new Column<Customer>(nameof(Customer.Email)));
/// SelectTags selects = new(new SelectTag(coalesce, "Coalescence"));
/// 
/// SelectBuilder<Customer> selectBuilder = new()
/// {
///     Selects = selects
/// };
/// 
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// --SqlServer
/// SELECT COALESCE([Customer].[Phone], [Customer].[Email]) AS [Coalescence] FROM [Customer]
/// --PostgreSql
/// SELECT COALESCE(\"Customer\".\"Phone\", \"Customer\".\"Email\") AS \"Coalescence\" FROM \"Customer\"
/// ]]></code>
/// </example>
public class Coalesce : FunctionalExpression
{
    protected override string FunctionName =>
        "COALESCE";

    /// <summary>
    /// Initializes a new instance of the <see cref="Coalesce"/> class with the specified values.
    /// </summary>
    /// <param name="values">
    /// The expressions to evaluate in order. The sequence must contain at least two values.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="values"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="values"/> contains fewer than two expressions.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// Thrown when <paramref name="values"/> contains a <c>null</c> expression.
    /// </exception>
    public Coalesce(params IEnumerable<SqlExpression> values) : base(ValidateValues(values))
    {
    }

    /// <summary>
    /// Validates and materializes the values supplied to the <c>COALESCE</c> expression.
    /// </summary>
    /// <param name="values">The expressions to validate.</param>
    /// <returns>A materialized sequence containing the validated expressions.</returns>
    private static IEnumerable<SqlExpression> ValidateValues(IEnumerable<SqlExpression> values)
    {
        ArgumentNullException.ThrowIfNull(values, nameof(values));

        if (values.Count() < 2)
            throw new ArgumentException("Coalesce requires two or more values.", nameof(values));

        return values;
    }
}
