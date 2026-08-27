using Carrigan.Core.Attributes;
using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.PredicatesLogic;

//TODO: Examples and Unit test

public class BooleanParameter : Predicates, IParameter
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
    /// Optional field properties that can be used to validate the boolean parameter value before SQL generation and/or to inform SQL type inference.
    /// </summary>
    public FieldProperties? FieldProperties { get; init; }


    /// <summary>
    /// Initializes a new instance of <see cref="BooleanParameter"/> from an existing <see cref="Parameter"/> instance. 
    /// </summary>
    /// <param name="parameter">
    /// The existing <see cref="Parameter"/> instance from which to create the new <see cref="BooleanParameter"/>.
    /// </param>
    protected BooleanParameter(Parameter parameter) : this(parameter.Value, parameter.Name, parameter.FieldProperties)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="BooleanParameter"/> with an auto-generated name.
    /// The name is generated as "Parameter" followed by a unique suffix or prefix, depending on the dialect to ensure it does not collide with any
    /// user-supplied parameter names within the same predicate tree or query.
    /// </summary>
    /// <param name="value">
    /// The value to bind.
    /// </param>
    /// <param name="fieldProperties">
    /// Optional field properties that can be used to validate the parameter value before SQL generation and/or to inform SQL type inference.
    /// </param>
    public BooleanParameter(bool? value, FieldProperties? fieldProperties = null) : this(value, new ParameterTag("Parameter"), fieldProperties)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="BooleanParameter"/> with a validated <see cref="ParameterTag"/>.
    /// </summary>
    /// <param name="value">
    /// The value to bind.
    /// </param>
    /// <param name="parameterTag">
    /// The base parameter tag (name + metadata).
    /// </param>
    /// <param name="fieldProperties">
    /// Optional field properties that can be used to validate the parameter value before SQL generation and/or to inform SQL type inference.
    /// </param>
    public BooleanParameter(bool? value, ParameterTag parameterTag, FieldProperties? fieldProperties = null) : this((object?)value, parameterTag, fieldProperties)  
    {

    }

    /// <summary>
    /// Initializes a new instance of <see cref="BooleanParameter"/> with a validated <see cref="ParameterTag"/>.
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
    internal BooleanParameter(object? value, ParameterTag parameterTag, FieldProperties? fieldProperties = null) : base([], parameterTag)
    {
        ArgumentNullException.ThrowIfNull(parameterTag, nameof(parameterTag));
        ValidateType(value);
        Name = new(parameterTag);
        Value = value;
        FieldProperties = fieldProperties;
    }

    /// <summary>
    /// Creates a Parameter using the provided value and ColumnInfo to set the parameter name and field properties.
    /// </summary>
    /// <remarks>Throws ArgumentNullException if columInfo is null.</remarks>
    /// <param name="value">The value for the parameter; may be null.</param>
    /// <param name="columInfo">ColumnInfo used to obtain the parameter tag and field properties; must not be null.</param>
    internal BooleanParameter(object? value, ColumnInfo columInfo) : base([], columInfo.ParameterTag)
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
    /// Validates that the provided value is of a bool type. If the value is not boolean, a <see cref="NonNumericValueException"/> is thrown.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <exception cref="NonNumericValueException">Thrown if the value is not a boolean type.</exception>
    private static void ValidateType(object? value)
    {
        if (value.GetUnderlyingType().IsNotBoolType())
            throw new NonBooleanValueException(value.GetUnderlyingType());
    }

    /// <summary>
    /// Defines an implicit conversion from a <see cref="Parameter"/> to a <see cref="BooleanParameter"/>.
    /// </summary>
    /// <param name="parameter">
    /// The <see cref="Parameter"/> instance to convert.
    /// </param>
    //TODO: unite tests
    [TypeSafetyLoss]
    public static implicit operator BooleanParameter(Parameter parameter) =>
        new(parameter);

    /// <summary>
    /// Defines an implicit conversion from a <see cref="BooleanParameter"/> to a <see cref="Parameter"/>.
    /// </summary>
    /// <param name="parameter">
    /// The <see cref="BooleanParameter"/> instance to convert.
    /// </param>
    public static implicit operator Parameter(BooleanParameter parameter) =>
        new(parameter.Value, parameter.Name, parameter.FieldProperties);
}
