using Carrigan.Core.Attributes;
using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;
using System.Numerics;

//IGNORE SPELLING: unrendered

//TODO: Unit tests.

namespace Carrigan.SqlTools.Expressions;

public class NumericParameter<T> : NumericParameter
    where T : INumber<T>
{
    /// <summary>
    /// Initializes a new instance of <see cref="NumericParameter{T}"/> from an existing <see cref="Parameter"/> instance.
    /// </summary>
    /// <param name="parameter">
    /// The existing <see cref="Parameter"/> instance from which to create the new <see cref="NumericParameter{T}"/>.
    /// </param>
    protected NumericParameter(Parameter parameter) : base (parameter)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="NumericParameter{T}"/> with an auto-generated name.
    /// The name is generated as "Parameter" followed by a unique suffix or prefix, depending on the dialect to ensure it does not collide with any
    /// user-supplied parameter names within the same predicate tree or query.
    /// </summary>
    /// <param name="value">
    /// The value to bind.
    /// </param>
    /// <param name="fieldProperties">
    /// Optional field properties that can be used to validate the parameter value before SQL generation and/or to inform SQL type inference.
    /// </param>
    public NumericParameter(T? value, FieldProperties fieldProperties) :base(value, fieldProperties)
    {
    }

    /// <summary>
    /// Initializes a new Parameter instance with the specified value and a default ParameterTag named "Parameter".
    /// </summary>
    /// <param name="value">The value to associate with the parameter; may be null.</param>
    public NumericParameter(T? value) : base(value)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="NumericParameter{T}"/> with a validated <see cref="ParameterTag"/>.
    /// </summary>
    /// <remarks>
    /// The parameter tag is used as the base name; a unique prefix may be added during SQL generation
    /// when duplicate names are detected within a predicate tree.
    /// </remarks>
    /// <param name="value">The value to bind.</param>
    /// <param name="parameterTag">The base parameter tag (name + metadata).</param>
    /// <param name="fieldProperties">
    /// Optional field properties that can be used to validate the parameter value before SQL generation and/or
    /// to inform SQL type inference.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when a required argument is <c>null</c>.
    /// </exception>
    public NumericParameter(T? value, ParameterTag parameterTag, FieldProperties? fieldProperties = null) : base(value, parameterTag, fieldProperties)
    {
    }

    /// <summary>
    /// Creates a Parameter using the provided value and ColumnInfo to set the parameter name and field properties.
    /// </summary>
    /// <remarks>Throws ArgumentNullException if columInfo is null.</remarks>
    /// <param name="value">The value for the parameter; may be null.</param>
    /// <param name="columInfo">ColumnInfo used to obtain the parameter tag and field properties; must not be null.</param>
    internal NumericParameter(T? value, ColumnInfo columInfo) : base(value, columInfo)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="NumericParameter{T}"/> with a raw name.
    /// </summary>
    /// <remarks>
    /// A unique prefix or suffix may be added during SQL generation when duplicate names are detected within a predicate tree.
    /// </remarks>
    /// <param name="value">The value to bind.</param>
    /// <param name="parameter">The base parameter name (do not include the leading <c>@</c>).</param>
    /// <param name="fieldProperties">
    /// Optional field properties that can be used to validate the parameter value before SQL generation and/or to inform SQL type inference.
    /// </param>
    /// <exception cref="InvalidParameterIdentifierException">
    /// Thrown when <paramref name="parameter"/> is invalid (including <c>null</c>, empty, or failing identifier validation).
    /// </exception>
    [ExternalOnly]
    public NumericParameter(T? value, string parameter, FieldProperties fieldProperties) : this(value, new ParameterTag(parameter), fieldProperties)
    {
    }

    /// <summary>
    /// Initializes a new Parameter instance from the specified value and parameter name by creating a ParameterTag.
    /// </summary>
    /// <remarks>Delegates to the constructor that accepts a ParameterTag.</remarks>
    /// <param name="value">The value to associate with the parameter; may be null.</param>
    /// <param name="parameter">The parameter name used to create a ParameterTag.</param>
    [ExternalOnly]
    public NumericParameter(T? value, string parameter) : this(value, new ParameterTag(parameter))
    {
    }

    /// <summary>
    /// Defines an implicit conversion from a <see cref="Parameter"/> to a <see cref="NumericParameter{T}"/>.
    /// </summary>
    /// <param name="parameter">
    /// The <see cref="Parameter"/> instance to convert.
    /// </param>
    //TODO: unite tests
    [TypeSafetyLoss]
    public static implicit operator NumericParameter<T>(Parameter parameter) =>
        new(parameter);

    /// <summary>
    /// Defines an implicit conversion from a <see cref="NumericParameter{T}"/> to a <see cref="Parameter"/>.
    /// </summary>
    /// <param name="parameter">
    /// The <see cref="NumericParameter{T}"/> instance to convert.
    /// </param>
    public static implicit operator Parameter(NumericParameter<T> parameter) =>
        new(parameter.Value, parameter.Name, parameter.FieldProperties);
}

public class NumericParameter : NumericExpression, IParameter
{
    /// <summary>
    /// The value to bind to the parameter.
    /// </summary>
    public object? Value { get; init; }

    /// <summary>
    /// The parameter’s tag (name + metadata). A prefix may be added during SQL generation
    /// to ensure uniqueness when duplicate user-supplied names occur.
    /// </summary>
    public ParameterTag Name { get; init; }

    /// <summary>
    /// Optional field properties that can be used to validate the numeric parameter value before SQL generation and/or to inform SQL type inference.
    /// </summary>
    public FieldProperties? FieldProperties { get; init; }


    /// <summary>
    /// Initializes a new instance of <see cref="NumericParameter"/> from an existing <see cref="Parameter"/> instance. 
    /// </summary>
    /// <param name="parameter">
    /// The existing <see cref="Parameter"/> instance from which to create the new <see cref="NumericParameter"/>.
    /// </param>
    //NOTE: This needs to be internal or it won't be called by the implicit operator
    internal NumericParameter(Parameter parameter) : this(parameter.Value, parameter.Name, parameter.FieldProperties)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="NumericParameter{T}"/> with an auto-generated name.
    /// The name is generated as "Parameter" followed by a unique suffix or prefix, depending on the dialect to ensure it does not collide with any
    /// user-supplied parameter names within the same predicate tree or query.
    /// </summary>
    /// <param name="value">
    /// The value to bind.
    /// </param>
    /// <param name="fieldProperties">
    /// Optional field properties that can be used to validate the parameter value before SQL generation and/or to inform SQL type inference.
    /// </param>
    internal NumericParameter(object? value, FieldProperties fieldProperties) : this(value, new ParameterTag("Parameter"), fieldProperties)
    {
    }

    /// <summary>
    /// Creates a new <see cref="NumericParameter{T}"/> instance with the specified numeric value and field properties.
    /// </summary>
    /// <typeparam name="T">
    /// The numeric type of the value, constrained to types that implement <see cref="INumber{T}"/>.
    /// </typeparam>
    /// <param name="value">
    /// The numeric value to bind to the parameter.
    /// </param>
    /// <param name="fieldProperties">
    /// Optional field properties that can be used to validate the parameter value before SQL generation and/or to inform SQL type inference.
    /// </param>
    /// <returns>
    /// A new instance of <see cref="NumericParameter{T}"/> containing the specified value and field properties.
    /// </returns>
    public static NumericParameter<T> New<T>(T? value, FieldProperties fieldProperties) where T : INumber<T> =>
        new(value, fieldProperties);

    /// <summary>
    /// Initializes a new Parameter instance with the specified value and a default ParameterTag named "Parameter".
    /// </summary>
    /// <param name="value">The value to associate with the parameter; may be null.</param>
    internal NumericParameter(object? value) : this(value, new ParameterTag("Parameter"))
    {
    }

    /// <summary>
    /// Creates a new <see cref="NumericParameter{T}"/> instance with the specified numeric value.
    /// </summary>
    /// <typeparam name="T">
    /// The numeric type of the value, constrained to types that implement <see cref="INumber{T}"/>.
    /// </typeparam>
    /// <param name="value">
    /// The numeric value to bind to the parameter.
    /// </param>
    /// <returns>
    /// A new instance of <see cref="NumericParameter{T}"/> containing the specified value.
    /// </returns>
    public static NumericParameter<T> New<T>(T value) where T : INumber<T> =>
        new (value);

    /// <summary>
    /// Initializes a new instance of <see cref="NumericParameter{T}"/> with a validated <see cref="ParameterTag"/>.
    /// </summary>
    /// <remarks>
    /// The parameter tag is used as the base name; a unique prefix may be added during SQL generation
    /// when duplicate names are detected within a predicate tree.
    /// </remarks>
    /// <param name="value">The value to bind.</param>
    /// <param name="parameterTag">The base parameter tag (name + metadata).</param>
    /// <param name="fieldProperties">
    /// Optional field properties that can be used to validate the parameter value before SQL generation and/or
    /// to inform SQL type inference.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when a required argument is <c>null</c>.
    /// </exception>
    internal NumericParameter(object? value, ParameterTag parameterTag, FieldProperties? fieldProperties = null) : base([], parameterTag)
    {
        ArgumentNullException.ThrowIfNull(parameterTag, nameof(parameterTag));
        ValidateType(value);
        Name = new(parameterTag);
        Value = value;
        FieldProperties = fieldProperties;
    }

    /// <summary>
    /// Creates a new <see cref="NumericParameter{T}"/> instance with the specified numeric value and a validated <see cref="ParameterTag"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The numeric type of the value, constrained to types that implement <see cref="INumber{T}"/>.
    /// </typeparam>
    /// <param name="value">
    /// The numeric value to bind to the parameter.
    /// </param>
    /// <param name="parmeterTag">
    /// The base parameter tag (name + metadata) to use for the parameter.
    /// </param>
    /// <param name="fieldProperties">
    /// Optional field properties that can be used to validate the parameter value before SQL generation and/or to inform SQL type inference.
    /// </param>
    /// <returns>
    /// A new instance of <see cref="NumericParameter{T}"/> containing the specified value, parameter tag, and optional field properties.
    /// </returns>
    public static NumericParameter<T> New<T>(T value, ParameterTag parmeterTag, FieldProperties? fieldProperties = null) where T : INumber<T> =>
        new (value, parmeterTag, fieldProperties);

    /// <summary>
    /// Creates a new <see cref="NumericParameter{T}"/> instance with the specified numeric value and a parameter name.
    /// </summary>
    /// <typeparam name="T">
    /// The numeric type of the value, constrained to types that implement <see cref="INumber{T}"/>.
    /// </typeparam>
    /// <param name="value">
    /// The numeric value to bind to the parameter.
    /// </param>
    /// <param name="parmeterTag">
    /// The base parameter tag (name + metadata) to use for the parameter.
    /// </param>
    /// <param name="fieldProperties">
    /// Optional field properties that can be used to validate the parameter value before SQL generation and/or to inform SQL type inference.
    /// </param>
    /// <returns>
    /// A new instance of <see cref="NumericParameter{T}"/> containing the specified value, parameter tag, and optional field properties.
    /// </returns>
    [ExternalOnly]
    public static NumericParameter<T> New<T>(T value, string parmeterTag, FieldProperties? fieldProperties = null) where T : INumber<T> =>
        new(value, new ParameterTag(parmeterTag), fieldProperties);

    /// <summary>
    /// Creates a Parameter using the provided value and ColumnInfo to set the parameter name and field properties.
    /// </summary>
    /// <remarks>Throws ArgumentNullException if columInfo is null.</remarks>
    /// <param name="value">The value for the parameter; may be null.</param>
    /// <param name="columInfo">ColumnInfo used to obtain the parameter tag and field properties; must not be null.</param>
    internal NumericParameter(object? value, ColumnInfo columInfo) : base([], columInfo.ParameterTag)
    {
        ArgumentNullException.ThrowIfNull(columInfo, nameof(columInfo));
        ValidateType(value);
        Name = new(columInfo.ParameterTag);
        Value = value;
        FieldProperties = columInfo.FieldProperties;
    }

    /// <summary>
    /// Produces the SQL fragment for this parameter using its base name (without any disambiguating prefix).
    /// </summary>
    /// <returns>The SQL parameter name without dialect-specific formatting.</returns>
    internal string ToSql() =>
        Name.ToString();

    /// <summary>
    /// Returns the unrendered parameter name.
    /// </summary>
    public override string ToString() =>
        Name.ToString();

    /// <summary>
    /// Produces the SQL fragment for this parameter expression (its final, possibly prefixed name).
    /// </summary>
    /// <returns>
    /// The SQL parameter name (e.g., <c>@Parameter_Name</c> or a prefixed variant).
    /// </returns>
    internal override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        yield return new SqlFragmentParameter(this);
    }

    /// <summary>
    /// Validates that the provided value is of a numeric type. If the value is not numeric, a <see cref="NonNumericValueException"/> is thrown.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <exception cref="NonNumericValueException">Thrown if the value is not a numeric type.</exception>
    private static void ValidateType(object? value)
    {
        if (value.GetUnderlyingType().IsNotNumericType())
            throw new NonNumericValueException(value.GetUnderlyingType());
    }

    /// <summary>
    /// Defines an implicit conversion from a <see cref="Parameter"/> to a <see cref="NumericParameter{T}"/>.
    /// </summary>
    /// <param name="parameter">
    /// The <see cref="Parameter"/> instance to convert.
    /// </param>
    //TODO: unite tests
    [TypeSafetyLoss]
    public static implicit operator NumericParameter(Parameter parameter) =>
        new(parameter);

    /// <summary>
    /// Defines an implicit conversion from a <see cref="NumericParameter"/> to a <see cref="Parameter"/>.
    /// </summary>
    /// <param name="parameter">
    /// The <see cref="NumericParameter"/> instance to convert.
    /// </param>
    public static implicit operator Parameter(NumericParameter parameter) =>
        new (parameter.Value, parameter.Name, parameter.FieldProperties);
}
