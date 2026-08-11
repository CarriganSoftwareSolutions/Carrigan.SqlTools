using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents a SQL parameter and its corresponding value as a leaf node within a SQL expression tree.
/// </summary>
/// <typeparam name="T">The model type whose C# properties supply parameter metadata.</typeparam>
/// <example>
/// <code language="csharp"><![CDATA[
/// Parameter<Customer> parameterName = new(nameof(Customer.Name), "Hank");
/// Column<Customer> columnName = new(nameof(Customer.Name));
/// Equal equalName = new(columnName, parameterName);
/// SelectBuilder<Customer> selectBuilder = new()
/// {
///     Where = equalName
/// };
/// 
/// SqlQuery query = customerGenerator.Select(selectBuilder);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].*
/// FROM [Customer]
/// WHERE ([Customer].[Name] = @Name_1)
/// ]]></code>
/// </example>
public class Parameter<T> : Parameter where T : class
{
    /// <summary>
    /// Initializes a new instance of <see cref="Parameter"/> that resolves parameter metadata from the specified model property and uses the provided value.
    /// </summary>
    /// <param name="propertyName">The property name that identifies the model property represented by the parameter.</param>
    /// <param name="value">Value to assign to the parameter; may be null.</param>
    public Parameter(PropertyName propertyName, object? value) : base(value, SqlToolsReflectorCache<T>.GetColumnsFromProperty(DialectStatics.SupportedTypes, propertyName))
    {
    }
    /// <summary>
    /// Initializes a new instance of the Parameter class with the specified property name and value.
    /// </summary>
    /// <remarks>Forwards to the overload that accepts a PropertyName by creating a new PropertyName from the
    /// provided string.</remarks>
    /// <param name="propertyName">The C# property name that identifies the model property represented by the parameter.</param>
    /// <param name="value">The value to bind; may be null.</param>
    [ExternalOnly]
    public Parameter(string propertyName, object? value) : this(new PropertyName(propertyName), value)
    {
    }
}
