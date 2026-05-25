using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.PredicatesLogic;
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
    public readonly ParameterTag ParameterTag;
    public readonly FieldProperties? FieldProperties;
    public readonly object? Value;

    internal static SqlFragmentParameter GetEncryptedParameter<T>(IEncryption? encryption, ColumnInfo column, T entity) =>
        new(column.ParameterTag, encryption?.Encrypt(column.PropertyInfo.GetValue(entity)?.ToString()));

    internal static SqlFragmentParameter GetParameter<T>(ColumnInfo column, FieldProperties fieldProperties, T entity) =>
        new(column, fieldProperties, column.PropertyInfo.GetValue(entity));

    internal SqlFragmentParameter(ParameterTag parameterTag, FieldProperties fieldProperties, object? value)
    {
        ParameterTag = parameterTag;
        FieldProperties = fieldProperties;
        Value = value;
    }
    internal SqlFragmentParameter(ParameterTag parameterTag, object? value)
    {
        ParameterTag = parameterTag;
        FieldProperties = null;
        Value = value;
    }

    internal SqlFragmentParameter(ColumnInfo columnInfo, FieldProperties fieldProperties, object? value)
    {
        ParameterTag = columnInfo.ParameterTag;
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
    internal SqlFragmentParameter(Parameter parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter);
        ParameterTag = parameter.Name;
        FieldProperties = null;
        Value = parameter.Value;
    }

    internal SqlFragmentParameter(SqlFragmentParameter sqlFragmentParameter, ParameterTag newTag)
    {
        ParameterTag = newTag;
        FieldProperties = sqlFragmentParameter.FieldProperties;
        Value = sqlFragmentParameter.Value;
    }

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
