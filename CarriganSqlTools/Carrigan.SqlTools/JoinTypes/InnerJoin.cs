using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;

//TODO: REDO Documentation, Unit Tests, Examples
namespace Carrigan.SqlTools.JoinTypes;

/// <summary>
/// Represents an SQL <c>INNER JOIN</c> operation.
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
/// InnerJoin&lt;Customer, Order&gt; join = new(predicate);
///
/// SqlQuery query = customerGenerator.Select(join, null, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* FROM [Customer] 
/// INNER JOIN [Order]  ON 
/// ([Customer].[Id] = [Order].[CustomerId])
/// ]]></code>
/// </example>
public class InnerJoin<rightT> : Relation
{
    private readonly string _sql;
    /// <summary>
    /// Initializes a new instance of the <see cref="InnerJoin{rightT}"/> class.
    /// </summary>
    /// <param name="predicates">
    /// The condition that defines the <c>ON</c> clause of the SQL <c>INNER JOIN</c>.
    /// </param>
    /// <exception cref="AmbiguousColumnException">
    /// Thrown when a <see cref="ColumnTag"/>  referenced in a <c>JOIN</c> clause belongs to a table
    /// that is not included in the <c>JOIN</c>.
    /// </exception>
    public InnerJoin(Predicates predicates) : base(predicates) => 
        _sql = $"INNER JOIN {TableTag} ON {predicates.ToSql()}";

    //TODO: Documentation, Unit Tests, Examples
    //Note: the deceleration may seem counter intuitive, however this gets called like this:
    //Joins<leftT>.InnerJoin<rightT>(predicate);
    public static Joins<leftT> Joins<leftT>(Predicates predicate) =>
        new(new InnerJoin<rightT>(predicate));

    //TODO: Documentation, Unit Tests, Examples
    public Joins<leftT> AsJoins<leftT>() =>
        new(this);

    //TODO: documentation, unit tests.
    internal override TableTag TableTag =>
        SqlToolsReflectorCache<rightT>.Table;

    /// Converts the current <see cref="InnerJoin{T,J}"/> instance to its SQL representation.
    /// </summary>
    /// <returns>A SQL string representing the <c>INNER JOIN</c> clause.</returns>
    internal override string ToSql() =>
        _sql;
}
