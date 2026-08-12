using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;

//IGNORE SPELLING: unrendered

//TODO: Unit tests.

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Initializes a new instance of <see cref="BooleanParameter{modelT}"/> with an auto-generated name.
/// The name is generated as "Parameter" followed by a unique suffix or prefix, depending on the dialect to ensure it does not collide with any
/// user-supplied parameter names within the same predicate tree or query.
/// </summary>
/// <typeparam name="modelT">
/// The model type whose C# properties supply parameter name from the class metadata.
/// </typeparam>
public class BooleanParameter<modelT> : BooleanParameter
    where modelT : class

{
    /// <summary>
    /// Initializes a new instance of <see cref="BooleanParameter{modelT}"/> using a <see cref="PropertyName"/> wrapper.
    /// </summary>
    /// <param name="value">
    /// The numeric value to bind to the parameter; may be null.
    /// </param>
    /// <param name="propertyName">
    /// The name of the property for which to create a parameter.
    /// </param>
    public BooleanParameter(bool value, PropertyName propertyName) : base(value, GetParameterTag(propertyName), GetFieldProperties(propertyName))
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="BooleanParameter{modelT}"/> using a string property name.
    /// </summary>
    /// <param name="value">
    /// The numeric value to bind to the parameter; may be null.
    /// </param>
    /// <param name="propertyName">
    /// The name of the property for which to create a parameter.
    /// </param>
    [ExternalOnly]
    public BooleanParameter(bool value, string propertyName) : this(value, new PropertyName(propertyName))
    {
    }

    /// <summary>
    /// Retrieves the <see cref="ParameterTag"/> for the specified property name from the reflector cache.
    /// </summary>
    /// <param name="propertyName">
    /// The name of the property for which to retrieve the parameter tag.
    /// </param>
    /// <returns>
    /// The <see cref="ParameterTag"/> associated with the specified property name.
    /// </returns>
    private static ParameterTag GetParameterTag(PropertyName propertyName) =>
        SqlToolsReflectorCache<modelT>.GetColumnsFromProperty(DialectBaseStatics.SupportedTypes, propertyName).ParameterTag;

    /// <summary>
    /// Retrieves the <see cref="FieldProperties"/> for the specified property name from the reflector cache.
    /// </summary>
    /// <param name="propertyName">
    /// The name of the property for which to retrieve the field properties.
    /// </param>
    /// <returns>
    /// The <see cref="FieldProperties"/> associated with the specified property name.
    /// </returns>
    private static FieldProperties GetFieldProperties(PropertyName propertyName) =>
        SqlToolsReflectorCache<modelT>.GetColumnsFromProperty(DialectBaseStatics.SupportedTypes, propertyName).FieldPropertiesOrDefault(new PostgreSqlDialect());

    /// <summary>
    /// Initializes a new instance of <see cref="BooleanParameter{modelT}"/> from an existing <see cref="Parameter"/> instance.
    /// </summary>
    /// <param name="parameter">
    /// The existing <see cref="Parameter"/> instance from which to create the new <see cref="BooleanParameter{modelT}"/>.
    /// </param>
    protected BooleanParameter(Parameter parameter) : base(parameter)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="BooleanParameter{modelT}"/> from an existing <see cref="Parameter{modelT}"/> instance.
    /// </summary>
    /// <param name="parameter">
    /// The existing <see cref="Parameter{modelT}"/> instance from which to create the new <see cref="BooleanParameter{modelT}"/>.
    /// </param>
    protected BooleanParameter(Parameter<modelT> parameter) : base(parameter)
    {
    }

    /// <summary>
    /// Defines an implicit conversion from a <see cref="Parameter"/> to a <see cref="BooleanParameter{modelT}"/>.
    /// </summary>
    /// <param name="parameter">
    /// The <see cref="Parameter"/> instance to convert.
    /// </param>
    //TODO: create Roslyn analyzer to warn against using this.
    //TODO: unite tests
    public static implicit operator BooleanParameter<modelT>(Parameter parameter) =>
        new (parameter);

    /// <summary>
    /// Defines an implicit conversion from a <see cref="Parameter{modelT}"/> to a <see cref="BooleanParameter{modelT}"/>.
    /// </summary>
    /// <param name="parameter">
    /// The <see cref="Parameter{modelT}"/> instance to convert.
    /// </param>
    //TODO: create Roslyn analyzer to warn against using this.
    //TODO: unite tests
    public static implicit operator BooleanParameter<modelT>(Parameter<modelT> parameter) =>
        new (parameter);

    /// <summary>
    /// Defines an implicit conversion from a <see cref="BooleanParameter{modelT}"/> to a <see cref="Parameter"/>.
    /// </summary>
    /// <param name="parameter">
    /// The <see cref="BooleanParameter{modelT}"/> instance to convert.
    /// </param>
    public static implicit operator Parameter(BooleanParameter<modelT> parameter) =>
        new(parameter.Value, parameter.Name, parameter.FieldProperties);

    /// <summary>
    /// Defines an implicit conversion from a <see cref="BooleanParameter{modelT}"/> to a <see cref="Parameter{modelT}"/>.
    /// </summary>
    /// <param name="parameter">
    /// The <see cref="BooleanParameter{modelT}"/> instance to convert.
    /// </param>
    public static implicit operator Parameter<modelT> (BooleanParameter<modelT> parameter) =>
        new (parameter);
}

