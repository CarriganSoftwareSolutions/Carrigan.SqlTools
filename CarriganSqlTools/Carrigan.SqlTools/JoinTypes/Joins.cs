using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;
using System;
using System.Linq;

//IGNORE SPELLING: joins

namespace Carrigan.SqlTools.JoinTypes;

//TODO: REDO Documentation, Unit Tests, Examples
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
public class Joins<leftT> : Relations
{
    /// <summary>
    /// Represents a collection of classes where each class defines a single SQL join operation.
    /// The name differs from the preferred “Joins” to avoid a naming conflict (e.g., Joins.Joins),
    /// which would result in a compiler error.
    /// </summary>
    protected override IEnumerable<Relation> Joints { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Joins"/> class.
    /// </summary>
    /// <param name="joins">
    /// One or more sequences of <see cref="Joins"/> objects, where each object represents a single SQL join operation.
    /// </param>
    public Joins(params IEnumerable<Relation> joins)
    {
        Joints = [];
        IEnumerable<TableTag> invalids; ;

        //validate each join to ensure it is joined in the proper order for column participation.
        foreach (Relation join in joins)
        {
            IEnumerable<TableTag> valid = TableTags.Append(join.TableTag);
            //ensure each column involved in the join comes from either an earlier table or the table being joined on 
            invalids = join.JoinsOn.Where(table => valid.DoesNotContain(table));
            if (invalids.IsNullOrEmpty())
                Joints = Joints.Append(join);
            else
                throw new InvalidTableException(invalids);
        }
    }

    public static Joins<leftT> LeftJoin<rightT>(Predicates predicates) =>
        JoinTypes.LeftJoin<rightT>.Joins<leftT>(predicates);

    public static Joins<leftT> Join<rightT>(Predicates predicates) =>
        JoinTypes.Join<rightT>.Joins<leftT>(predicates);

    public static Joins<leftT> InnerJoin<rightT>(Predicates predicates) =>
        JoinTypes.InnerJoin<rightT>.Joins<leftT>(predicates);

    internal override TableTag TableTag =>
        SqlToolsReflectorCache<leftT>.Table;
}
