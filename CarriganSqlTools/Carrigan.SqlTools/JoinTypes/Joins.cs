using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;

//IGNORE SPELLING: joins

namespace Carrigan.SqlTools.JoinTypes;

/// <summary>
/// Defines a class that represent one or more SQL join operations.
/// </summary>
/// <example>
/// <para>
/// Note: <c>ColumnEqualsColumn&lt;lefT, rightT&gt;</c> validates property names and throws an exception if a property name is invalid.
/// </para>
/// <code language="csharp"><![CDATA[
/// ColumnEqualsColumn&lt;Customer, Order&gt; customerIdEquals = new(nameof(Customer.Id), nameof(Order.CustomerId));
/// InnerJoin&lt;Customer, Order&gt; join1 = new(customerIdEquals);
/// 
/// ColumnEqualsColumn&lt;Order, PaymentMethod&gt; paymentMethodIdEquals = new(nameof(Order.PaymentMethodId), nameof(PaymentMethod.Id));
/// InnerJoin&lt;Order, PaymentMethod&gt; join2 = new(paymentMethodIdEquals);
/// 
/// Joins joins = new(join1, join2);
/// 
/// SqlQuery query = customerGenerator.Select(joins, null, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* FROM [Customer] 
/// INNER JOIN [Order] ON ([Customer].[Id] = [Order].[CustomerId]) 
/// INNER JOIN [PaymentMethod] ON ([Order].[PaymentMethodId] = [PaymentMethod].[Id])
/// ]]></code>
/// </example>
public class Joins : IJoins
{
    /// <summary>
    /// Represents a collection of classes where each class defines a single SQL join operation.
    /// The name differs from the preferred “Joins” to avoid a naming conflict (e.g., Joins.Joins),
    /// which would result in a compiler error.
    /// </summary>
    public IEnumerable<IJoins> Joints { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Joins"/> class.
    /// </summary>
    /// <param name="joins">
    /// One or more sequences of <see cref="IJoins"/> objects, where each object represents a single SQL join operation.
    /// </param>
    public Joins(params IEnumerable<IJoins> joins) =>
        Joints = joins;

    /// <summary>
    /// Enumerates all tables included in <see cref="Joints"/>
    /// providing a quick way to determine whether a given table
    /// participates in any join operation.
    /// </summary>
    public IEnumerable<TableTag> TableTags =>
        Joints.SelectMany(join => join.TableTags);

    /// <summary>
    /// Enumerates all possible columns included in <see cref="Joints"/>
    /// providing a quick way to determine whether a given column
    /// participates in a table that participates in any join operation.
    /// </summary>
    public IEnumerable<ColumnInfo> ColumnInfo =>
        Joints.SelectMany(join => join.ColumnInfo);

    /// <summary>
    /// Generates the SQL fragment for the JOIN clause represented by <see cref="Joints"/>.
    /// </summary>
    /// <returns>The SQL fragment for the JOIN clause represented by <see cref="Joints"/>.</returns>
    public string ToSql() =>
        string.Join(" ", Joints.Select(join => join.ToSql()));

    /// <summary>
    /// Recursively get all the parameters associated with the logic.
    /// </summary>
    public Dictionary<ParameterTag, object> Parameters { get => new (Joints.SelectMany(join => join.Parameters)); }
}
