using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Represents a SQL parameter and its corresponding value as a leaf node within a SQL expression tree.
/// </summary>
/// <typeparam name="modelT">The model type whose C# properties supply parameter metadata.</typeparam>
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
//TODO: unit tests
public class Parameter<modelT> : Parameter where modelT : class
{
    /// <summary>
    /// Initializes a new instance of <see cref="Parameter"/> that resolves parameter metadata from the specified model property and uses the provided value.
    /// </summary>
    /// <param name="propertyName">The property name that identifies the model property represented by the parameter.</param>
    /// <param name="value">Value to assign to the parameter; may be null.</param>
    public Parameter(PropertyName propertyName, object? value) : base(value, SqlToolsReflectorCache<modelT>.GetColumnsFromProperty(DialectStatics.SupportedTypes, propertyName))
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

    /// <summary>
    /// Initializes a new instance of <see cref="Parameter"/> from an existing <see cref="NumericParameter"/> instance.
    /// </summary>
    /// <param name="numericParameter">
    /// The <see cref="NumericParameter"/> instance from which to initialize the new <see cref="Parameter"/> instance.
    /// </param>
    /// <remarks>
    /// Only use for implicit operator.
    /// </remarks>
    internal Parameter(NumericParameter numericParameter) : base (numericParameter.Value, numericParameter.Name, numericParameter.FieldProperties) 
    { }

    /// <summary>
    /// Initializes a new instance of <see cref="Parameter"/> from an existing <see cref="BooleanParameter"/> instance.
    /// </summary>
    /// <param name="booleanParameter">
    /// The <see cref="BooleanParameter"/> instance from which to initialize the new <see cref="Parameter"/> instance.
    /// </param>
    internal Parameter(BooleanParameter booleanParameter) : base (booleanParameter.Value, booleanParameter.Name, booleanParameter.FieldProperties)
    { }

    /// <summary>
    /// Defines an implicit conversion from a <see cref="Parameter{modelT}"/> to a <see cref="NumericParameter"/>.
    /// </summary>
    /// <param name="parameter">
    /// The <see cref="Parameter{modelT}"/> instance to convert.
    /// </param>
    public static implicit operator NumericParameter(Parameter<modelT> parameter) =>
        new(parameter);

    /// <summary>
    /// Defines an implicit conversion from a <see cref="Parameter{modelT}"/> to a <see cref="NumericExpression"/>.
    /// </summary>
    /// <param name="parameter">
    /// The <see cref="Parameter{modelT}"/> instance to convert.
    /// </param>
    public static implicit operator NumericExpression(Parameter<modelT> parameter) =>
        new NumericParameter(parameter);
}
