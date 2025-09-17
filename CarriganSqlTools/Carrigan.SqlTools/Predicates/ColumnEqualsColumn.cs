namespace Carrigan.SqlTools.Predicates;

/// <summary>
/// Predicates control the boolean logic for join and where clauses.
/// This is the class is a combination of predicates.
/// It allows you to generate SQL Column1 = Column2 with fewer lines of C#.
/// </summary>
/// <example>
/// <para>ColumnEqualsColumn&lt;leftT, rightT&gt; validates the names of the properties, and throws an error if the property isn't valid</para>
/// <code language="csharp"><![CDATA[
/// ColumnEqualsColumn&lt;Customer, Order&gt; columnValue = new(nameof(Customer.Id), nameof(Order.CustomerId));
/// LeftJoin&lt;Customer, Order&gt; join = new(columnValue);
/// SqlQuery query = customerGenerator.Select(join, null, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* 
/// FROM [Customer] 
/// LEFT JOIN [Order] 
/// ON ([Customer].[Id] = [Order].[CustomerId])
/// ]]></code>
/// </example>
public class ColumnEqualsColumn<leftT, rightT> : ComparisonOperators
{
    /// <summary>
    /// This is the constructor for the classes that represents SQL's Equality, =, comparison operators
    /// </summary>
    /// <param name="leftColumn">left value</param>
    /// <param name="rightColumn">right value</param>
    public ColumnEqualsColumn(string leftColumn, string rightColumn)
    {
        Columns<leftT> left = new(leftColumn);
        Columns<rightT> right = new(rightColumn);

        Initialize(left, right, "=");
    }
}
