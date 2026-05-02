using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;
using System.Xml;
using System.Xml.Linq;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Represents a SQL parameter and its corresponding value for use in predicate expressions
/// (e.g., <c>WHERE</c> or <c>JOIN</c> clauses).
/// </summary>
/// <example>
/// <para>
/// <see cref="Column{T}"/> validates the names of the property, and throws an exception if the property isn't valid.
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
    /// <param name="parameterTag">The base parameter tag (name + metadata).</param>
    /// <param name="value">The value to bind; <c>null</c> becomes <see cref="DBNull.Value"/> at materialization time.</param>
    internal Parameter(ParameterTag parameterTag, object? value) : base([])
    {
        ArgumentNullException.ThrowIfNull(parameterTag, nameof(parameterTag));
        Name = new(parameterTag, value);
        Value = ConvertValue(value);
    }

    /// <summary>
    /// Creates a <see cref="Parameter"/> using an encrypted value for the given entity and column.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="parameterTag">The base parameter tag (name + metadata) to use for the parameter.</param>
    /// <param name="encryption">The encryption service, or <c>null</c> to skip encryption.</param>
    /// <param name="column">Column metadata used to locate the property and SQL type.</param>
    /// <param name="entity">The entity instance providing the value.</param>
    /// <returns>
    /// A <see cref="Parameter"/> whose value is the encrypted string
    /// (or <c>null</c>/<see cref="DBNull.Value"/> when no encryption or value is available).
    /// </returns>
    internal static Parameter GetParameter<T>(ParameterTag parameterTag, IEncryption? encryption, ColumnInfo column, T entity) =>
        new(parameterTag, encryption?.Encrypt(column.PropertyInfo.GetValue(entity)?.ToString()));


    /// <summary>
    /// Creates a <see cref="Parameter"/> for the supplied entity and column.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="parameterTag">The base parameter tag (name + metadata) to use for the parameter.</param>
    /// <param name="column">
    /// The <see cref="ColumnInfo"/> used to locate the property and supply its <see cref="SqlTypeDefinition"/>.
    /// </param>
    /// <param name="entity">The entity instance to read the value from.</param>
    /// <returns>
    /// A <see cref="Parameter"/> whose key is a cloned
    /// <see cref="ParameterTag"/> configured with <paramref name="column"/>'s
    /// <see cref="ColumnInfo.SqlType"/>, and whose value is the property value
    /// or <see cref="DBNull.Value"/>.
    /// </returns>
    internal static Parameter GetParameter<T>(ParameterTag parameterTag, ColumnInfo column, T entity)
    {
        object? value = column.PropertyInfo.GetValue(entity);
        ParameterTag parameterTagCopy = new(parameterTag, value);

        return new(parameterTagCopy, ConvertValue(value));
    }

    /// <summary>
    /// Initializes a new instance of <see cref="Parameter"/> with a raw name.
    /// </summary>
    /// <remarks>
    /// A unique prefix may be added during SQL generation when duplicate names are detected within a predicate tree.
    /// </remarks>
    /// <param name="parameter">The base parameter name (do not include the leading <c>@</c>).</param>
    /// <param name="value">The value to bind; <c>null</c> becomes <see cref="DBNull.Value"/> at materialization time.</param>
    /// <param name="sqlType">
    /// Optional explicit SQL type definition used to validate <paramref name="value"/> before SQL generation.
    /// When not provided, a type definition is inferred from <paramref name="value"/>.
    /// </param>
    /// <exception cref="InvalidParameterIdentifierException">
    /// Thrown when <paramref name="parameter"/> is invalid (including <c>null</c>, empty, or failing identifier validation).
    /// </exception>
    /// <exception cref="SqlTypeMismatchException">
    /// Thrown when <paramref name="value"/> is incompatible with the provided <paramref name="sqlType"/>.
    /// </exception>
    [ExternalOnly]
    public Parameter(string parameter, object? value, SqlTypeDefinition? sqlType = null) : base([])
    {
        SqlTypeDefinition sqlTypeDefinition = sqlType ?? new(value);

        SqlTypeMismatchException? exception = null;
        if (value is not null)
        {
            exception = SqlTypeMismatchException.Validate(value, sqlTypeDefinition.Type);
        }

        if (exception is not null)
            throw exception;

        Name = new ParameterTag(parameter, sqlTypeDefinition);
        Value = value;
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
    /// <param name="prefix">
    /// The recursion-built prefix used to disambiguate duplicate user-supplied parameter names.
    /// </param>
    /// <param name="branchName">
    /// The branch prefix that is prepended to the beginning of all of the parameter names in this predicate tree.
    /// </param>
    /// <param name="duplicates">
    /// The set of user-supplied parameter tags identified as duplicates within the predicate tree.
    /// </param>
    /// <returns>
    /// The SQL parameter name (e.g., <c>@Parameter_Name</c> or a prefixed variant).
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="prefix"/> or <paramref name="branchName"/> or <paramref name="duplicates"/> is <c>null</c>.
    /// </exception>
    internal override IEnumerable<SqlFragment> ToSqlFragments()
    {
        yield return new SqlFragmentParameter(this);
    }

    /// <summary>
    /// Performs the necessary conversions for a parameter value
    /// before it is passed to the database.
    /// </summary>
    /// <param name="value">
    /// The value to convert. A <c>null</c> value is converted to
    /// <see cref="DBNull.Value"/>.
    /// </param>
    /// <returns>
    /// The converted value suitable for database operations.
    /// </returns>
    private static object ConvertValue(object? value)
    {
        if (value == null)
            return DBNull.Value;
        else if (value is XDocument xDocument)
            return xDocument.ToString();
        else if (value is XmlDocument xmlDocument)
            return ((object?)xmlDocument.OuterXml) ?? DBNull.Value; //the compiler didn't like xmlDocument.ToString() ?? DBNull.Value, so I had to get creative.
        else return value;
    }
}
