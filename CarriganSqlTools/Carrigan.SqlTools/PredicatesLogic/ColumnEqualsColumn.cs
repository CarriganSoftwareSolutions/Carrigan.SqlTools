using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents a predicate that compares two columns for equality (i.e., <c>Column1 = Column2</c>).
/// </summary>
/// <typeparam name="leftT">
/// The data model representing the left-hand table in the comparison.
/// </typeparam>
/// <typeparam name="rightT">
/// The data model representing the right-hand table in the comparison.
/// </typeparam>
/// <remarks>
/// This class simplifies constructing SQL expressions such as
/// <c>[Customer].[Id] = [Order].[CustomerId]</c>.
/// </remarks>
/// <example>
/// <para>
/// <see cref="ColumnEqualsColumn{leftT, righT}"/> validates the names of the properties, and throws an error if the property isn't valid
/// </para>
/// <code language="csharp"><![CDATA[
/// ColumnEqualsColumn<Customer, Order> columnValue = new(nameof(Customer.Id), nameof(Order.CustomerId));
/// Joins<Customer> joins = LeftJoin<Order>.Joins<Customer>(columnValue);
/// SqlQuery query = customerGenerator.Select(null, joins, null, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* 
/// FROM [Customer] 
/// LEFT JOIN [Order] 
///   ON ([Customer].[Id] = [Order].[CustomerId])
/// ]]></code>
/// </example>
public class ColumnEqualsColumn<leftT, rightT> : ComparisonOperator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnEqualsColumn{leftT, rightT}"/> class,
    /// representing an equality comparison (<c>=</c>) between two columns.
    /// </summary>
    /// <param name="leftPropertyName">The property on the left-hand entity (<typeparamref name="leftT"/>).</param>
    /// <param name="rightPropertyName">The property on the right-hand entity (<typeparamref name="rightT"/>).</param>
    public ColumnEqualsColumn(PropertyName leftPropertyName, PropertyName rightPropertyName) : base(new Column<leftT>(leftPropertyName), new Column<rightT>(rightPropertyName), "=")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnEqualsColumn{leftT, rightT}"/> class,
    /// representing an equality comparison (<c>=</c>) between two columns.
    /// </summary>
    /// <param name="leftPropertyName">The property name on the left-hand entity (<typeparamref name="leftT"/>).</param>
    /// <param name="rightPropertyName">The property name on the right-hand entity (<typeparamref name="rightT"/>).</param>
    [ExternalOnly]
    public ColumnEqualsColumn(string leftPropertyName, string rightPropertyName) : this (new PropertyName(leftPropertyName), new PropertyName(rightPropertyName))
    {
    }
}
