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
/// Parameter parameterName = new("Hank", "Name");
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
/// --PostgreSql
/// SELECT "Customer".* 
/// FROM "Customer"
/// WHERE ("Customer"."Name" = $1)
/// 
/// --SqlServer
/// SELECT [Customer].*
/// FROM [Customer]
/// WHERE ([Customer].[Name] = @Name_1)
/// ]]></code>
/// </example>
public class Parameter : Predicates
{
    /// <summary>
    /// The value to bind to the parameter.
    /// </summary>
    internal readonly object? Value;

    /// <summary>
    /// The parameter’s tag (name + metadata). A prefix may be added during SQL generation
    /// to ensure uniqueness when duplicate user-supplied names occur.
    /// </summary>
    internal readonly ParameterTag Name;

    /// <summary>
    /// Optional field properties that can be used to validate the parameter value before SQL generation and/or to inform SQL type inference.
    /// </summary>
    internal readonly FieldProperties? FieldProperties;

    /// <summary>
    /// Initializes a new instance of <see cref="Parameter"/> with an auto-generated name.
    /// The name is generated as "Parameter" followed by a unique suffix or prefix, depending on the dialect to ensure it does not collide with any
    /// user-supplied parameter names within the same predicate tree or query.
    /// </summary>
    /// <param name="value">
    /// The value to bind.
    /// </param>
    /// <param name="fieldProperties">
    /// Optional field properties that can be used to validate the parameter value before SQL generation and/or to inform SQL type inference.
    /// </param>
    public Parameter(object? value, FieldProperties fieldProperties) :this(value, new ParameterTag("Parameter"), fieldProperties)
    {
    }

    /// <summary>
    /// Initializes a new Parameter instance with the specified value and a default ParameterTag named "Parameter".
    /// </summary>
    /// <param name="value">The value to associate with the parameter; may be null.</param>
    public Parameter(object? value) : this(value, new ParameterTag("Parameter"))
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="Parameter"/> with a validated <see cref="ParameterTag"/>.
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
    public Parameter(object? value, ParameterTag parameterTag, FieldProperties fieldProperties) : base([])
    {
        ArgumentNullException.ThrowIfNull(parameterTag, nameof(parameterTag));
        Name = new(parameterTag);
        Value = value;
        FieldProperties = fieldProperties;
    }
    /// <summary>
    /// Initializes a new Parameter with the specified value and parameter tag.
    /// </summary>
    /// <remarks>Name is constructed from parameterTag and FieldProperties is initialized to null.</remarks>
    /// <param name="value">The value to associate with the parameter.</param>
    /// <param name="parameterTag">The tag used to construct the parameter name; must not be null.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when a required argument is <c>null</c>.
    /// </exception>
    /// <exception cref="InvalidParameterIdentifierException">
    /// Thrown when a supplied or generated SQL parameter identifier is invalid.
    /// </exception>
    public Parameter(object? value, ParameterTag parameterTag) : base([])
    {
        ArgumentNullException.ThrowIfNull(parameterTag, nameof(parameterTag));
        Name = new(parameterTag);
        Value = value;
        FieldProperties = null;
    }

    /// <summary>
    /// Creates a Parameter using the provided value and ColumnInfo to set the parameter name and field properties.
    /// </summary>
    /// <remarks>Throws ArgumentNullException if columInfo is null.</remarks>
    /// <param name="value">The value for the parameter; may be null.</param>
    /// <param name="columInfo">ColumnInfo used to obtain the parameter tag and field properties; must not be null.</param>
    internal Parameter(object? value, ColumnInfo columInfo) : base([])
    {
        ArgumentNullException.ThrowIfNull(columInfo, nameof(columInfo));
        Name = new(columInfo.ParameterTag);
        Value = value;
        FieldProperties = columInfo.FieldProperties;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="Parameter"/> with a raw name.
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
    public Parameter(object? value, string parameter, FieldProperties fieldProperties) : this(value, new ParameterTag(parameter), fieldProperties)
    {
    }
    /// <summary>
    /// Initializes a new Parameter instance from the specified value and parameter name by creating a ParameterTag.
    /// </summary>
    /// <remarks>Delegates to the constructor that accepts a ParameterTag.</remarks>
    /// <param name="value">The value to associate with the parameter; may be null.</param>
    /// <param name="parameter">The parameter name used to create a ParameterTag.</param>
    [ExternalOnly]
    public Parameter(object? value, string parameter) : this(value, new ParameterTag(parameter))
    {
    }

    /// <summary>
    /// Produces the SQL fragment for this parameter using its base name (without any disambiguating prefix).
    /// </summary>
    /// <returns></returns>
    internal string ToSql() =>
        Name.ToString();

    /// <summary>
    /// Produces the SQL fragment for this parameter (its final, possibly prefixed name).
    /// </summary>
    /// <returns>
    /// The SQL parameter name (e.g., <c>@Parameter_Name</c> or a prefixed variant).
    /// </returns>
    internal override IEnumerable<ISqlFragment> ToSqlFragments(ISqlDialects dialect)
    {
        yield return new SqlFragmentParameter(this);
    }
}
