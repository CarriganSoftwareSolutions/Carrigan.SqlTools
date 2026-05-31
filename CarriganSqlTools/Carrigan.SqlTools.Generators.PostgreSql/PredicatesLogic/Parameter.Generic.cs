using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents a SQL parameter and its corresponding value for use in predicate expressions
/// (e.g., <c>WHERE</c> or <c>JOIN</c> clauses).
/// </summary>
/// <example>
/// <code language="csharp"><![CDATA[
/// Parameter<Customer> parameterName = new(nameof(Customer.Name), "Hank");
/// Column<Customer> columnName = new(nameof(Customer.Name));
/// Equal equalName = new(columnName, parameterName);
/// SqlQuery query = customerGenerator.Select(null, null, null, null, equalName, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* 
/// FROM [Customer] 
/// WHERE ([Customer].[Name] = @Parameter_Name)
/// ]]></code>
/// </example>
public class Parameter<T> : Parameter where T : class
{
    /// <summary>
    /// Initializes a new instance of <see cref="Parameter"/> that resolves columns from the specified property and uses the provided
    /// value.
    /// </summary>
    /// <param name="propertyName">PropertyName that identifies the property from which columns are resolved.</param>
    /// <param name="value">Value to assign to the parameter; may be null.</param>
    public Parameter(PropertyName propertyName, object? value) : base(value, SqlToolsReflectorCache<T>.GetColumnsFromProperty(DialectStatics.SupportedTypes, propertyName))
    {
    }
    /// <summary>
    /// Initializes a new instance of the Parameter class with the specified property name and value.
    /// </summary>
    /// <remarks>Forwards to the overload that accepts a PropertyName by creating a new PropertyName from the
    /// provided string.</remarks>
    /// <param name="propertyName">The name of the property.</param>
    /// <param name="value">The value of the property; may be null.</param>
    [ExternalOnly]
    public Parameter(string propertyName, object? value) : this(new PropertyName(propertyName), value)
    {
    }
}
