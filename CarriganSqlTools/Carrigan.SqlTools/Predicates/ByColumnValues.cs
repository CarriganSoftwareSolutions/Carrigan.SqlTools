using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Predicates;

/// <summary>
/// Predicates control the boolean logic for join and where clauses.
/// This class is essentially an alias for Column = Value
/// The intent is to reduce the amount of code needed to perform a routine task.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// ColumnValues&lt;Customer&gt; coumnValue = new(nameof(Customer.Name), "Hank");
/// SqlQuery query = customerGenerator.Select(null, coumnValue, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] = @Parameter_Name)
/// ]]></code>
/// </example>
[Obsolete("Use ColumnValues<T> instead.")]
public class ByColumnValues<T> : ColumnValues<T>
{
    public ByColumnValues(PropertyName propertyName, object value) : base(propertyName, value)
    {
    }
}
