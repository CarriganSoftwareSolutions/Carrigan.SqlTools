using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.JoinTypes;
/// <summary>
/// Represents an SQL <c>LEFT JOIN</c>.
/// </summary>
/// <typeparam name="T">
/// The data model for the primary (left or base) table from which records are selected,
/// updated, or deleted.
/// </typeparam>
/// <typeparam name="J">
/// The data model for the secondary (right or joined) table that is joined to the primary table.
/// </typeparam>
/// <example>
/// <para>
/// Note: <c>ColumnEqualsColumn&lt;lefT, rightT&gt;</c> validates property names and throws an exception if a property name is invalid.
/// </para>
/// <code language="csharp"><![CDATA[
/// ColumnEqualsColumn&lt;Customer, Order&gt; predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
/// LeftJoin&lt;Customer, Order&gt; join = new(predicate);
///
/// SqlQuery query = customerGenerator.Select(join, null, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* FROM [Customer] 
/// LEFT JOIN [Order]  ON 
/// ([Customer].[Id] = [Order].[CustomerId])
/// ]]></code>
/// </example>
public class LeftJoin<T, J> : JoinBaseClass
{
    private readonly IEnumerable<Parameter> _parameters;
    private readonly string _sql;

    /// <summary>
    /// Initializes a new instance of the <see cref="LeftJoin{T,J}"/> class.
    /// </summary>
    /// <param name="predicate">
    /// The condition that defines the <c>ON</c> clause of the SQL <c>LEFT JOIN</c>.
    /// </param>
    /// <exception cref="AmbiguousColumnException">
    /// Thrown when a <see cref="ColumnTag"/>  referenced in a <c>JOIN</c> clause belongs to a table
    /// that is not included in the <c>JOIN</c>.
    /// </exception>

    public LeftJoin(PredicatesLogic.Predicates predicate)
    {
        TableTag leftTableTag = SqlToolsReflectorCache<T>.Table;
        TableTag rightTableTag = SqlToolsReflectorCache<J>.Table;
        IEnumerable<ColumnInfo> invalidTags =
            predicate
                .Columns
                .Where(column => column.TableTag != leftTableTag && column.TableTag != rightTableTag)
                .Select(column => column.ColumnInfo);

        if (invalidTags.Any())
            throw new InvalidColumnException(invalidTags);

        _sql = $"LEFT JOIN {rightTableTag} ON {predicate.ToSql()}";
        _tableTags = [leftTableTag, rightTableTag];
        _parameters = predicate.Parameters;
    }

    /// <summary>
    /// Enumerates all possible columns included in <see cref="Joints"/>
    /// providing a quick way to determine whether a given column
    /// participates in a table that participates in any join operation.
    /// </summary>
    public override IEnumerable<ColumnInfo> ColumnInfo =>
        SqlToolsReflectorCache<T>.ColumnInfo.Concat(SqlToolsReflectorCache<J>.ColumnInfo);

    public override Dictionary<ParameterTag, object> Parameters => new(_parameters.SelectMany(parameter => parameter.GetParameters()));

    /// <summary>
    /// Generates the SQL representation of the <c>LEFT JOIN</c> clause.
    /// </summary>
    /// <returns>A SQL string representing the <c>LEFT JOIN</c> clause.</returns>
    public override string ToSql() =>
        _sql;
}
