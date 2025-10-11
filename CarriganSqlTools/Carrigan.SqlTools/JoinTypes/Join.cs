using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.JoinTypes;

//TODO: REDO Documentation, Unit Tests, Examples
/// <summary>
/// Represents an SQL <c>JOIN</c> operation. This class functions as an alias
/// for the <see cref="Join{rightT}"/> class.
/// </summary>
/// <typeparam name="T">
/// The data model for the primary (left or base) table from which records are
/// selected, updated, or deleted.
/// </typeparam>
/// <typeparam name="J">
/// The data model for the secondary (right or joined) table that is joined to
/// the primary table.
/// </typeparam>
/// <example>
/// <para>
/// Note: <c>ColumnEqualsColumn&lt;lefT, rightT&gt;</c> validates property names and throws an exception if a property name is invalid.
/// </para>
/// <code language="csharp"><![CDATA[
/// ColumnEqualsColumn&lt;Customer, Order&gt; predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
/// Join&lt;Customer, Order&gt; join = new(predicate);
/// Join<Customer, Order> join = new(equals);
///
/// SqlQuery query = customerGenerator.Select(join, null, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* FROM [Customer] 
/// JOIN [Order]  ON 
/// ([Customer].[Id] = [Order].[CustomerId])
/// ]]></code>
/// </example>
public class Join<rightT> : Relation
{
    protected readonly string _sql;

    /// <summary>
    /// Initializes a new instance of the <see cref="Join{rightT}"/> class.
    /// </summary>
    /// <param name="predicate">
    /// The condition that defines the <c>ON</c> clause of the SQL <c>JOIN</c>.
    /// </param>
    /// <exception cref="AmbiguousColumnException">
    /// Thrown when a <see cref="ColumnTag"/>  referenced in a <c>JOIN</c> clause belongs to a table
    /// that is not included in the <c>JOIN</c>.
    /// </exception>

    public Join(Predicates predicate) : base(predicate) =>
        _sql = $"JOIN {TableTag} ON {predicate.ToSql()}";

    //TODO: Documentation, Unit Tests, Examples
    //Note: the deceleration may seem counter intuitive, however this gets called like this:
    //Joins<leftT>.Join<rightT>(predicate);
    public static Joins<leftT> Joins<leftT>(Predicates predicate) =>
        new(new Join<rightT>(predicate));

    //TODO: Documentation, Unit Tests, Examples
    public Joins<leftT> AsJoins<leftT>() =>
        new(this);

    /// <summary>
    /// Enumerates all possible columns included in <see cref="Joints"/>
    /// providing a quick way to determine whether a given column
    /// participates in a table that participates in any join operation.
    /// </summary>
    //public override IEnumerable<ColumnInfo> ColumnInfo =>
    //    SqlToolsReflectorCache<T>.ColumnInfo.Concat(SqlToolsReflectorCache<rightT>.ColumnInfo);

    internal override TableTag TableTag =>
        SqlToolsReflectorCache<rightT>.Table;

    /// <summary>
    /// Generates the SQL representation of the <c>LEFT JOIN</c> clause.
    /// </summary>
    /// <returns>A SQL string representing the <c>LEFT JOIN</c> clause.</returns>
    internal override string ToSql() =>
        _sql;
}
