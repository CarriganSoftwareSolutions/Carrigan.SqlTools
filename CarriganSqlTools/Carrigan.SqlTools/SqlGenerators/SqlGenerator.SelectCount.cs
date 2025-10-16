using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.Tags;
using System.Data;
using System.Text;

namespace Carrigan.SqlTools.SqlGenerators;

public partial class SqlGenerator<T>
{
    /// <summary>
    /// Builds an <see cref="SqlQuery"/> containing a parameterized SQL
    /// <c>SELECT COUNT(*)</c> from the table represented by <typeparamref name="T"/>,
    /// with optional <c>JOIN</c> and <c>WHERE</c> clauses.
    /// </summary>
    /// <param name="joins">
    /// Optional joins to include in the count query. Omit to count only rows from the base table.
    /// </param>
    /// <param name="predicates">
    /// Optional filter predicates to compose the <c>WHERE</c> clause for the count.
    /// </param>
    /// <returns>
    /// An <see cref="SqlQuery"/> whose <c>QueryText</c> is the generated count SQL and whose
    /// <c>Parameters</c> are derived from <paramref name="predicates"/>.
    /// </returns>
    /// <exception cref="InvalidTableException">
    /// Thrown if the generated query references invalid or unrecognized table identifiers.
    /// </exception>
    /// <param name="orderBy"></param>
    /// <example>
    /// <code language="csharp"><![CDATA[
    /// SqlQuery query = orderGenerator.SelectCount(null, null);
    /// ]]></code>
    /// <para>Columns&lt;T&gt; validates the names of the properties, and throws an error if the property isn't valid</para>
    /// <code><![CDATA[
    /// SELECT COUNT(*) FROM [Order]
    /// ]]></code>
    /// </example>
    /// <example>
    /// <code language="csharp"><![CDATA[
    /// Columns&lt;Order&gt; totalCol = new(nameof(Order.Total));
    /// Parameters minTotal = new("Total", 500m);
    /// GreaterThan greaterThan = new(totalCol, minTotal);
    /// 
    /// ColumnEqualsColumn&lt;Order, Customer> columnCompare = new(nameof(Order.CustomerId), nameof(Customer.Id));
    /// Join&lt;Order, Customer&gt; join = new(columnCompare);
    /// 
    /// SqlQuery query = orderGenerator.SelectCount(join, greaterThan);
    /// ]]></code>
    /// <para>Resulting SQL:</para>
    /// <code><![CDATA[
    /// SELECT COUNT(*) 
    /// FROM [Order] 
    /// LEFT JOIN [Customer] 
    /// ON ([Order].[CustomerId] = [Customer].[Id]) 
    /// WHERE ([Order].[Total] > @Parameter_Total)
    /// ]]></code>
    /// </example>
    public SqlQuery SelectCount(JoinsBase? joins, Predicates? predicates)
    {
        IEnumerable<TableTag> selectableTableTags = (joins?.TableTags ?? []).Append(Table).Distinct();
        IEnumerable<TableTag> predicateTableTags = [.. predicates?.Columns?.Select(col => col.TableTag)?.Distinct() ?? []];
        IEnumerable<TableTag> invalidTags = predicateTableTags.Except(selectableTableTags);
        StringBuilder queryBuilder = new($"SELECT COUNT({Table}.*) FROM {Table}");

        if (invalidTags.Any())
        {
            throw new InvalidTableException(invalidTags);
        }

        if (joins?.IsNotNullOrEmpty() ?? false)
        {
            queryBuilder.Append($" {joins.ToSql()}");
        }
        if (predicates is not null)
        {
            queryBuilder.Append($" WHERE {predicates.ToSql()}");
        }
        return new SqlQuery()
        {
            QueryText = queryBuilder.ToString(),
            Parameters = [.. (joins?.Parameters ?? []).Concat(predicates?.GetParameters() ?? [])],
            CommandType = CommandType.Text
        };
    }
}