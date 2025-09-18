namespace Carrigan.SqlTools.Predicates;

/// <summary>
/// Predicates control the boolean logic for join and where clauses.
/// This class represents SQL's logical IS NULL operator.
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// Columns&lt;Customer&gt; columnName = new(nameof(Customer.Name));
/// IsNull isNull = new(columnName);
/// SqlQuery query = customerGenerator.Select(null, isNull, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* FROM [Customer] WHERE ([Customer].[Name] IS NULL)
/// ]]></code>
/// </example>
public class IsNull : PredicatesBase
{
    private readonly PredicatesBase _someValue;

    /// <summary>
    /// This is the constructor for the classes that represents SQL's IS NOT NULL operator
    /// </summary>
    /// <param name="someValue">should represent something that may or may not be a null value in SQL</param>
    public IsNull(PredicatesBase someValue) => 
        _someValue = someValue;

    /// <summary>
    ///  Recursively get all the columns associated with the logic.
    /// </summary>
    internal override IEnumerable<Parameters> Parameter =>
       _someValue.Parameter;

    internal override IEnumerable<IColumnValue> Column =>
       _someValue.Column;

    internal override string ToSql(string prefix, IEnumerable<string> duplicates) =>
        $"({_someValue.ToSql(prefix, duplicates)} IS NULL)";
    internal override IEnumerable<KeyValuePair<string, object>> GetParameters(string prefix, IEnumerable<string> duplicates) =>
        _someValue.GetParameters(prefix, duplicates);
}
