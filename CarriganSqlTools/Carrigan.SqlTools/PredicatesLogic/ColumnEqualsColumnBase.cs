using Carrigan.Core.Interfaces.IModels;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.JoinTypes;
using Carrigan.SqlTools.SqlGenerators;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents a predicate that compares two columns for equality (i.e., <c>Column1 = Column2</c>).
/// </summary>
/// <typeparam name="leftT">The data model representing the left-hand table in the comparison.</typeparam>
/// <typeparam name="rightT">The data model representing the right-hand table in the comparison.</typeparam>
/// <remarks>
/// This class simplifies constructing SQL expressions such as
/// <c>[Customer].[Id] = [Order].[CustomerId]</c>.
/// </remarks>
/// <example>
/// <para>
/// <see cref="ColumnEqualsColumnBase{leftT, rightT}"/> validates property names and throws an exception
/// if a property name is invalid.
/// </para>
/// <code language="csharp"><![CDATA[
/// ColumnEqualsColumn<Customer, Order> columnValue = new(nameof(Customer.Id), nameof(Order.CustomerId));
/// LeftJoin<Order> join = new(columnValue);
/// SqlQuery query = customerGenerator.Select(null, join, null, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* 
/// FROM [Customer] 
/// LEFT JOIN [Order] 
///   ON ([Customer].[Id] = [Order].[CustomerId])
/// ]]></code>
/// </example>
public abstract class ColumnEqualsColumnBase<leftT, rightT> : ComparisonOperator
    where leftT : class
    where rightT : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnEqualsColumnBase{leftT, rightT}"/> class,
    /// representing an equality comparison (<c>=</c>) between two columns.
    /// </summary>
    /// <param name="leftColumn">The column on the left-hand entity (<typeparamref name="leftT"/>).</param>
    /// <param name="rightColumn">The column on the right-hand entity (<typeparamref name="rightT"/>).</param>
    public ColumnEqualsColumnBase(ColumnBase<leftT> leftColumn, ColumnBase<rightT> rightColumn)
        : base(leftColumn, rightColumn, "=")
    {
    }
}
