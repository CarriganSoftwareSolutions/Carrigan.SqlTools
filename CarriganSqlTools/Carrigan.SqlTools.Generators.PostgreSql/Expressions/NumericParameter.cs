using Carrigan.Core.Attributes;
using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;
using System.Numerics;

//IGNORE SPELLING: unrendered

//TODO: Unit tests.

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Initializes a new instance of <see cref="NumericParameter{T}"/> with an auto-generated name.
/// The name is generated as "Parameter" followed by a unique suffix or prefix, depending on the dialect to ensure it does not collide with any
/// user-supplied parameter names within the same predicate tree or query.
/// </summary>
/// <typeparam name="modelT">
/// The model type whose C# properties supply parameter name from the class metadata.
/// </typeparam>
/// <typeparam name="T">
/// The numeric type of the parameter value, constrained to types implementing <see cref="INumber{T}"/>.
/// </typeparam>
public class NumericParameter<modelT, T> : NumericParameter<T>
    where modelT : class
    where T : INumber<T>

{
    /// <summary>
    /// Initializes a new instance of <see cref="NumericParameter{T}"/> using a <see cref="PropertyName"/> wrapper.
    /// </summary>
    /// <param name="value">
    /// The numeric value to bind to the parameter; may be null.
    /// </param>
    /// <param name="propertyName">
    /// The name of the property for which to create a parameter.
    /// </param>
    public NumericParameter(T? value, PropertyName propertyName) : this(value, GetColumnInfo(propertyName))
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="NumericParameter{T}"/> using reflected column metadata.
    /// </summary>
    /// <param name="value">
    /// The numeric value to bind to the parameter; may be null.
    /// </param>
    /// <param name="columnInfo">
    /// The <see cref="ColumnInfo"/> that contains metadata about the model property represented by the parameter.
    /// </param>
    private NumericParameter(T? value, ColumnInfo columnInfo) : base(value, columnInfo.ParameterTag, columnInfo.FieldPropertiesOrDefault(new PostgreSqlDialect()))
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="NumericParameter{T}"/> using a property name string.
    /// </summary>
    /// <param name="value">
    /// The numeric value to bind to the parameter; may be null.
    /// </param>
    /// <param name="propertyName">
    /// The name of the property for which to create a parameter.
    /// </param>
    [ExternalOnly]
    public NumericParameter(T? value, string propertyName) : this(value, new PropertyName(propertyName))
    {
    }

    /// <summary>
    /// Retrieves and validates the reflected column metadata for the specified model property.
    /// </summary>
    private static ColumnInfo GetColumnInfo(PropertyName propertyName)
    {
        ColumnInfo columnInfo = SqlToolsReflectorCache<modelT>.GetColumnsFromProperty(DialectBaseStatics.SupportedTypes, propertyName);
        if (columnInfo.Type.IsNumericType() is false)
            throw new NonNumericValueException($"{columnInfo.PropertyName} must represent a numeric property on {typeof(modelT).Name} to be used as a numeric parameter.");
        return columnInfo;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="NumericParameter{T}"/> from an existing <see cref="Parameter"/> instance.
    /// </summary>
    /// <param name="parameter">
    /// The existing <see cref="Parameter"/> instance from which to create the new <see cref="NumericParameter{T}"/>.
    /// </param>
    protected NumericParameter(Parameter parameter) : base(parameter)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="NumericParameter{T}"/> from an existing <see cref="Parameter{modelT}"/> instance.
    /// </summary>
    /// <param name="parameter">
    /// The existing <see cref="Parameter{modelT}"/> instance from which to create the new <see cref="NumericParameter{T}"/>.
    /// </param>
    protected NumericParameter(Parameter<modelT> parameter) : base(parameter)
    {
    }

    /// <summary>
    /// Defines an implicit conversion from a <see cref="Parameter"/> to a <see cref="NumericParameter{modelT, T}"/>.
    /// </summary>
    /// <param name="parameter">
    /// The <see cref="Parameter"/> instance to convert.
    /// </param>
    //TODO: unite tests
    [TypeSafetyLoss]
    public static implicit operator NumericParameter<modelT, T>(Parameter parameter) =>
        new (parameter);

    /// <summary>
    /// Defines an implicit conversion from a <see cref="Parameter{modelT}"/> to a <see cref="NumericParameter{modelT, T}"/>.
    /// </summary>
    /// <param name="parameter">
    /// The <see cref="Parameter{modelT}"/> instance to convert.
    /// </param>
    //TODO: unite tests
    [TypeSafetyLoss]
    public static implicit operator NumericParameter<modelT, T>(Parameter<modelT> parameter) =>
        new (parameter);

    /// <summary>
    /// Defines an implicit conversion from a <see cref="NumericParameter{modelT, T}"/> to a <see cref="Parameter"/>.
    /// </summary>
    /// <param name="parameter">
    /// The <see cref="NumericParameter{modelT, T}"/> instance to convert.
    /// </param>
    public static implicit operator Parameter(NumericParameter<modelT, T> parameter) =>
        new(parameter.Value, parameter.Name, parameter.FieldProperties);

    /// <summary>
    /// Defines an implicit conversion from a <see cref="NumericParameter{modelT, T}"/> to a <see cref="Parameter{modelT}"/>.
    /// </summary>
    /// <param name="parameter">
    /// The <see cref="NumericParameter{modelT, T}"/> instance to convert.
    /// </param>
    public static implicit operator Parameter<modelT> (NumericParameter<modelT, T> parameter) =>
        new (parameter);
}

