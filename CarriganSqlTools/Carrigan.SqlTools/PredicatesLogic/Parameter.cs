using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;
using System.Data;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents a SQL parameter and its corresponding value for use in predicate expressions
/// (e.g., <c>WHERE</c> or <c>JOIN</c> clauses).
/// </summary>
/// <example>
/// <para>
/// <see cref="Column{T}"/> validates the names of the property, and throws an error if the property isn't valid
/// </para>
/// <code language="csharp"><![CDATA[
/// Parameter parameterName = new("Name", "Hank");
/// Column<Customer> columnName = new(nameof(Customer.Name));
/// Equal equalName = new(columnName, parameterName);
/// SqlQuery query = customerGenerator.Select(null, null, equalName, null, null);
/// ]]></code>
/// <para>Resulting SQL:</para>
/// <code><![CDATA[
/// SELECT [Customer].* 
/// FROM [Customer] 
/// WHERE ([Customer].[Name] = @Parameter_Name)
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
    /// Initializes a new instance of <see cref="Parameter"/> with a validated <see cref="ParameterTag"/>.
    /// </summary>
    /// <remarks>
    /// The parameter tag is used as the base name; a unique prefix may be added during SQL generation
    /// when duplicate names are detected within a predicate tree.
    /// </remarks>
    /// <param name="parameter">The base parameter tag (name + metadata).</param>
    /// <param name="value">The value to bind; <c>null</c> becomes <see cref="DBNull.Value"/> at materialization time.</param>
    internal Parameter(ParameterTag parameter, object? value)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));
        Name = parameter;
        Value = value;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="Parameter"/> with a raw name.
    /// </summary>
    /// <remarks>
    /// A unique prefix may be added during SQL generation when duplicate names are detected within a predicate tree.
    /// </remarks>
    /// <param name="parameter">The base parameter name (do not include the leading <c>@</c>).</param>
    /// <param name="value">The value to bind; <c>null</c> becomes <see cref="DBNull.Value"/> at materialization time.</param>
    /// <exception cref="InvalidSqlIdentifierException">
    /// Thrown by <see cref="ParameterTag"/> if <paramref name="parameter"/> is empty or fails identifier validation.
    /// </exception>
    [ExternalOnly]
    public Parameter(string parameter, object? value, SqlTypeDefinition? sqlType = null)
    {
        SqlTypeDefinition sqlTypeDefinition = sqlType ?? new(value);
        SqlTypeMismatchException? exception = null;
        if(value is not null)
        {
            exception = SqlTypeMismatchException.Validate(value, sqlTypeDefinition.Type);
        }
        if (exception is not null)
            throw exception;

        Name = new ParameterTag(null, parameter, null, sqlTypeDefinition);
        Value = value;
    }

    /// <summary>
    /// Leaf node: returns this parameter instance as a single-item sequence for recursive enumeration.
    /// </summary>
    internal override IEnumerable<Parameter> Parameters =>
       [this];

    /// <summary>
    /// Leaf node: parameters do not contribute columns; returns an empty sequence.
    /// </summary>
    internal override IEnumerable<ColumnBase> Columns =>
       [];

    /// <summary>
    /// Produces the SQL fragment for this parameter (its final, possibly prefixed name).
    /// </summary>
    /// <param name="prefix">
    /// The recursion-built prefix used to disambiguate duplicate user-supplied parameter names.
    /// </param>
    /// <param name="duplicates">
    /// The set of user-supplied parameter tags identified as duplicates within the predicate tree.
    /// </param>
    /// <returns>
    /// The SQL parameter name (e.g., <c>@Parameter_Name</c> or a prefixed variant).
    /// </returns>
    internal override string ToSql(string prefix, IEnumerable<ParameterTag> duplicates) =>
        GetFinalParameterName(prefix, duplicates);

    /// <summary>
    /// Recursively returns this parameter as a key–value pair suitable for command binding.
    /// </summary>
    /// <param name="prefix">
    /// The recursion-built prefix used to disambiguate duplicate user-supplied parameter names.
    /// </param>
    /// <param name="duplicates">
    /// The set of user-supplied parameter tags identified as duplicates within the predicate tree.
    /// </param>
    /// <returns>
    /// A single <see cref="KeyValuePair{TKey, TValue}"/> mapping the final parameter tag to its value
    /// (with <c>null</c> normalized to <see cref="DBNull.Value"/>).
    /// </returns>
    internal override IEnumerable<KeyValuePair<ParameterTag, object>> GetParameters(string prefix, IEnumerable<ParameterTag> duplicates)
    {
        ParameterTag key = GetFinalParameterName(prefix, duplicates);
        object value = Value ?? DBNull.Value;
        KeyValuePair<ParameterTag, object> keyValuePair = new(key, value);
        return (new KeyValuePair<ParameterTag, object>[] { keyValuePair }).AsEnumerable();
    }

    /// <summary>
    /// Computes the final parameter tag used in SQL, adding a disambiguating prefix when required.
    /// </summary>
    /// <param name="prefix">
    /// The recursion-built prefix used for disambiguation when the base name is a duplicate.
    /// </param>
    /// <param name="duplicates">
    /// The set of user-supplied parameter tags identified as duplicates within the predicate tree.
    /// </param>
    /// <returns>
    /// The final <see cref="ParameterTag"/> to be emitted into SQL and used as the binding key.
    /// </returns>
    private ParameterTag GetFinalParameterName(string prefix, IEnumerable<ParameterTag> duplicates) =>
        duplicates.Contains(Name) ? Name.PrefixPrepend($"@Parameter{prefix}") : Name.PrefixPrepend($"@Parameter");
}
