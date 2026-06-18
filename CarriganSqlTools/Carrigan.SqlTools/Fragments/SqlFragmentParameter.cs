using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Expressions;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Fragments;

/// <summary>
/// Represents an SQL fragment that renders a single parameter reference.
/// </summary>
/// <remarks>
/// This fragment wraps a <see cref="Parameter"/> so it can participate in fragment concatenation
/// while preserving access to the parameter’s tag and bound value for later materialization.
/// </remarks>
public class SqlFragmentParameter : ISqlFragment
{
    /// <summary>
    /// Gets the <see cref="ParameterTag"/> associated with this parameter fragment, which serves as a unique identifier for the parameter during SQL
    /// generation and materialization.
    /// </summary>
    public readonly ParameterTag ParameterTag;
    /// <summary>
    /// Gets the <see cref="FieldProperties"/> associated with this parameter fragment, which can be used to validate the parameter value before SQL generation
    /// </summary>
    public readonly FieldProperties? FieldProperties;
    /// <summary>
    /// Gets the runtime value associated with this parameter fragment, which will be materialized as a SQL parameter value during query execution.
    /// </summary>
    public readonly object? Value;

    /// <summary>
    /// Creates a new <see cref="SqlFragmentParameter"/> for the specified column and entity, applying encryption if an <see cref="IEncryption"/> instance is provided.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the entity from which the parameter value will be extracted. This type is used to access the property value via reflection based on the provided <see cref="ColumnInfo"/>.
    /// </typeparam>
    /// <param name="encryption">
    /// An optional <see cref="IEncryption"/> instance used to encrypt the parameter value if encryption is required. If this parameter is <c>null</c>, the value will be used as-is without encryption.
    /// </param>
    /// <param name="column">
    /// The <see cref="ColumnInfo"/> object that contains metadata about the column for which the parameter is being created, including the property information needed to extract the value from the entity and any encryption requirements.
    /// </param>
    /// <param name="entity">
    /// The entity from which to extract the parameter value.
    /// </param>
    /// <returns>
    /// A new <see cref="SqlFragmentParameter"/> instance representing the encrypted parameter.
    /// </returns>
    internal static SqlFragmentParameter GetEncryptedParameter<T>(IEncryption? encryption, ColumnInfo column, T entity) =>
        new(column, encryption?.Encrypt(column.PropertyInfo.GetValue(entity)?.ToString()));

    /// <summary>
    /// Creates a new <see cref="SqlFragmentParameter"/> for the specified column and entity, extracting the parameter value directly without encryption.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="column">
    /// The <see cref="ColumnInfo"/> object that contains metadata about the column for which the parameter is being created.
    /// </param>
    /// <param name="fieldProperties">
    /// The <see cref="FieldProperties"/> associated with the parameter, used for validation.
    /// </param>
    /// <param name="entity">
    /// The entity from which to extract the parameter value.
    /// </param>
    /// <returns>
    /// A new <see cref="SqlFragmentParameter"/> instance representing the parameter.
    /// </returns>
    internal static SqlFragmentParameter GetParameter<T>(ColumnInfo column, FieldProperties fieldProperties, T entity) =>
        new(column, fieldProperties, column.PropertyInfo.GetValue(entity));

    /// <summary>
    /// Initializes a new instance of <see cref="SqlFragmentParameter"/> using the provided <see cref="ColumnInfo"/> to determine the parameter tag and field properties, and the provided value as the parameter value.
    /// </summary>
    /// <param name="columnInfo">The <see cref="ColumnInfo"/> object that contains metadata about the column for which the parameter is being created.</param>
    /// <param name="value">The value for the parameter.</param>
    internal SqlFragmentParameter(ColumnInfo columnInfo, object? value)
        : this (columnInfo, columnInfo.FieldProperties, value)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="SqlFragmentParameter"/> using the provided <see cref="ColumnInfo"/> to determine the parameter tag, the provided <see cref="FieldProperties"/> for validation, and the provided value as the parameter value.
    /// </summary>
    /// <param name="columnInfo">The <see cref="ColumnInfo"/> object that contains metadata about the column for which the parameter is being created.</param>
    /// <param name="fieldProperties">The <see cref="FieldProperties"/> associated with the parameter, used for validation.</param>
    /// <param name="value">The value for the parameter.</param>
    internal SqlFragmentParameter(ColumnInfo columnInfo, FieldProperties? fieldProperties, object? value)
        : this(columnInfo.ParameterTag, columnInfo.FieldProperties ?? fieldProperties, value)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="SqlFragmentParameter"/> with the provided parameter tag, field properties, and value.
    /// </summary>
    /// <param name="parameterTag">The tag for the parameter.</param>
    /// <param name="fieldProperties">The field properties for the parameter.</param>
    /// <param name="value">The value for the parameter.</param>
    internal SqlFragmentParameter(ParameterTag parameterTag, FieldProperties? fieldProperties, object? value)
    {
        ParameterTag = parameterTag;
        FieldProperties = fieldProperties;
        Value = value;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="SqlFragmentParameter"/> with the provided parameter.
    /// </summary>
    /// <param name="parameter">The parameter to wrap.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="parameter"/> is <c>null</c>.
    /// </exception>
    internal SqlFragmentParameter(Parameter parameter) : this(parameter.Name, parameter.FieldProperties, parameter.Value)
    { }

    /// <summary>
    /// Initializes a new instance of <see cref="SqlFragmentParameter"/> by copying the properties of an existing instance and applying a new parameter tag.
    /// </summary>
    /// <param name="sqlFragmentParameter">The existing parameter instance to copy.</param>
    /// <param name="newTag">The new tag for the parameter.</param>
    internal SqlFragmentParameter(SqlFragmentParameter sqlFragmentParameter, ParameterTag newTag)
    {
        ParameterTag = newTag;
        FieldProperties = sqlFragmentParameter.FieldProperties;
        Value = sqlFragmentParameter.Value;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="SqlFragmentParameter"/> by copying the properties of an existing instance and applying a new value.
    /// </summary>
    /// <param name="sqlFragmentParameter">The existing parameter instance to copy.</param>
    /// <param name="recastValue">The new value for the parameter.</param>
    internal SqlFragmentParameter(SqlFragmentParameter sqlFragmentParameter, object? recastValue)
    {
        ParameterTag = sqlFragmentParameter.ParameterTag;
        FieldProperties = sqlFragmentParameter.FieldProperties;
        Value = recastValue;
    }

    /// <summary>
    /// Converts this fragment into its SQL representation.
    /// </summary>
    /// <param name="dialect">The SQL dialect to use for rendering.</param>
    /// <returns>The SQL parameter name produced by the wrapped <see cref="Parameter"/>.</returns>
    /// <remarks>
    /// Any exception thrown by <see cref="Parameter.ParameterTag"/> will be propagated to the caller.
    /// </remarks>
    public string ToSql(ISqlDialects dialect) =>
        //Note: ToString will not render the final parameter text by itself.
        //When unit testing, you need to convert to final SqlQuery, then get the command text.
        //The command text should then have @'s added if needed, and index numbers added on.
        //Before that, it will just be the parameter name.
        ParameterTag;


    /// <summary>
    /// Retrieves the parameters contained within this fragment for later materialization.
    /// </summary>
    /// <returns>An enumerable collection containing the single <see cref="Parameter"/> wrapped by this fragment.</returns>
    public IEnumerable<SqlFragmentParameter> GetSqlFragmentParameters()
    {
        yield return this;
    }

    /// <summary>
    /// Returns a flattened sequence of all SQL fragments contained within this fragment and its descendants.
    /// </summary>
    /// <returns>An enumerable collection containing this fragment as a single element.</returns>
    public IEnumerable<ISqlFragment> Flatten() => [this];
}
