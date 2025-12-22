using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.JoinTypes;

/// <summary>
/// Represents an SQL <c>FULL JOIN</c> operation.
/// </summary>
/// <typeparam name="rightT">
/// The data model representing the right-side table being joined.
/// </typeparam>
/// <example>
/// <para>
/// Note: <see cref="ColumnEqualsColumn{leftT, rightT}"/> validates property names and throws an exception if a property name is invalid.
/// </para>
/// <code language="csharp"><![CDATA[
/// ColumnEqualsColumn<Customer, Order> predicate = new(nameof(Customer.Id), nameof(Order.CustomerId));
/// JoinsBase join = Joins<Customer>.FullJoin<Order>(predicate);
///
/// SqlQuery query = customerGenerator.Select(null, join, null, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].*
/// FROM [Customer]
/// FULL JOIN [Order]
/// ON ([Customer].[Id] = [Order].[CustomerId])
/// ]]></code>
/// </example>
public class FullJoin<rightT> : JoinBase
{
    private readonly string _sql;

    /// <summary>
    /// Initializes a new instance of the <see cref="FullJoin{rightT}"/> class.
    /// </summary>
    /// <param name="predicate">
    /// The predicate(s) that define the <c>ON</c> clause of the SQL <c>FULL JOIN</c>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="predicate"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="TypeInitializationException">
    /// Thrown when the SQL reflection cache for <typeparamref name="rightT"/> fails to initialize.
    /// </exception>
    public FullJoin(Predicates predicate)
        : base(predicate ?? throw new ArgumentNullException(nameof(predicate))) =>
        _sql = $"FULL JOIN {TableTag} ON {_predicates.ToSql()}";

    /// <summary>
    /// Creates and returns a new <see cref="Joins{leftT}"/> object that contains
    /// a newly created <see cref="FullJoin{rightT}"/> operation.
    /// </summary>
    /// <typeparam name="leftT">
    /// The data model representing the left (base) table being joined onto.
    /// </typeparam>
    /// <param name="predicate">
    /// The predicate(s) that define the <c>ON</c> clause of the SQL <c>FULL JOIN</c>.
    /// </param>
    /// <returns>
    /// A new <see cref="Joins{leftT}"/> object containing a single <see cref="FullJoin{rightT}"/> instance.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="predicate"/> is <c>null</c>.
    /// </exception>
    public static Joins<leftT> Joins<leftT>(Predicates predicate) =>
        new(new FullJoin<rightT>(predicate));

    /// <summary>
    /// Creates and returns a new <see cref="Joins{leftT}"/> object that contains
    /// the current <see cref="FullJoin{rightT}"/> instance.
    /// </summary>
    /// <typeparam name="leftT">
    /// The data model representing the left (base) table being joined onto.
    /// </typeparam>
    /// <returns>
    /// A new <see cref="Joins{leftT}"/> object containing the current <see cref="FullJoin{rightT}"/> instance.
    /// </returns>
    public Joins<leftT> AsJoins<leftT>() =>
        new(this);

    /// <summary>
    /// Gets the <see cref="TableTag"/> associated with the right-side table in the <c>FULL JOIN</c> operation.
    /// </summary>
    /// <exception cref="TypeInitializationException">
    /// Thrown when the SQL reflection cache for <typeparamref name="rightT"/> fails to initialize.
    /// </exception>
    internal override TableTag TableTag =>
        SqlToolsReflectorCache<rightT>.Table;

    /// <summary>
    /// Converts the current <see cref="FullJoin{rightT}"/> instance to its SQL representation.
    /// </summary>
    /// <returns>
    /// A SQL string representing the <c>FULL JOIN</c> clause.
    /// </returns>
    internal override string ToSql() =>
        _sql;
}
