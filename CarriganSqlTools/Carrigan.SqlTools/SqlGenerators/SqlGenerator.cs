using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.RegularExpressions;
using Carrigan.SqlTools.Tags;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace Carrigan.SqlTools.SqlGenerators;

/// <summary>
/// Generates SQL for the data model type <typeparamref name="T"/>.
/// Provides helper methods to build statements and clauses based on the model’s
/// public metadata.
/// </summary>
/// <remarks>
/// When generating SQL, only properties that can be publicly read from accessible types are considered. 
/// Members not visible outside their defining assembly are ignored.
/// </remarks>
/// <typeparam name="T">
/// The entity or data model type that defines the target table and columns.
/// </typeparam>
public partial class SqlGenerator<T> : SqlToolsReflectorCache<T> where T : class
{
    /// <summary>
    /// Stores the encryption service used for SQL generation,
    /// or <c>null</c> if encryption is not required or no encrypter is provided.
    /// </summary>
    private readonly IEncryption? _Encryption;

    /// <summary>
    /// Validates identifier formats, SQL type compatibility, and encryption prerequisites for the model.
    /// </summary>
    /// <exception cref="AggregateException">
    /// Contains multiple exceptions when more than one validation error is detected. Possible inner exceptions include
    /// <see cref="InvalidSqlIdentifierException"/>, 
    /// <see cref="SqlTypeMismatchException"/>,
    /// <see cref="EncrypterNotProvidedException{T}"/>, 
    /// <see cref="NoKeyVersionPropertyException{T}"/>,
    /// <see cref="InvalidKeyVersionPropertyTypeException{T}"/>, and 
    /// <see cref="MultipleKeyVersionProperties{T}"/>.
    /// </exception>
    private void ValidationChecks()
    {
        List<Exception> exceptions = [];
        IEnumerable<Tuple<PropertyInfo, ColumnName>> invalidColumns = [];
        IEnumerable<Tuple<PropertyInfo, AliasName>> invalidAliases = [];
        IEnumerable<Tuple<PropertyInfo, ParameterTag>> invalidParameters = [];

        if (SqlIdentifierPattern.Fails(TableName))
            exceptions.Add(new InvalidSqlIdentifierException(Type, TableName));
        if (SchemaName is not null && SqlIdentifierPattern.Fails(SchemaName))
            exceptions.Add(new InvalidSqlIdentifierException(Type, SchemaName));
        if (SqlIdentifierPattern.Fails(ProcedureName))
            exceptions.Add(new InvalidSqlIdentifierException(Type, ProcedureName));

        invalidColumns =
            ColumnInfo
                .Where(column => SqlIdentifierPattern.Fails(column.ColumnName))
                .Select(column => new Tuple<PropertyInfo, ColumnName>(column.PropertyInfo, column.ColumnName));
        if (invalidColumns.Any())
            exceptions.Add(new InvalidSqlIdentifierException(invalidColumns));

        ColumnInfo
            .Select(column => new Tuple<PropertyInfo, SqlTypeAttribute?>(column.PropertyInfo, SqlTypeAttribute.GetSqlTypeAttribute(column.PropertyInfo)))
            .Select(tuple => SqlTypeMismatchException.Validate(tuple.Item1, tuple.Item2))
            .ForEach(sqlTypeMismatchException =>
            {
                if (sqlTypeMismatchException is not null)
                    exceptions.Add(sqlTypeMismatchException);
            });

        invalidAliases =
            ColumnInfo
                .Where(column => column.AliasName is not null && SqlIdentifierPattern.Fails(column.AliasName))
                .Select(column => column.AliasName is not null ? new Tuple<PropertyInfo, AliasName>(column.PropertyInfo, column.AliasName) : null)
                .OfType<Tuple<PropertyInfo, AliasName>>();
        if (invalidAliases.Any())
            exceptions.Add(new InvalidSqlIdentifierException(invalidAliases));

        invalidParameters =
            ColumnInfo
                .Where(column => SqlIdentifierPattern.Fails(column.ParameterTag))
                .Select(column => new Tuple<PropertyInfo, ParameterTag>(column.PropertyInfo, column.ParameterTag));
        if (invalidParameters.Any())
            exceptions.Add(new InvalidSqlIdentifierException(invalidParameters));

        if (HasEncryptedColumns())
        {
            if (_Encryption is null)
                exceptions.Add(new EncrypterNotProvidedException<T>());
            if (KeyVersionColumnInfo is null)
                exceptions.Add(new NoKeyVersionPropertyException<T>());
            else if ((Nullable.GetUnderlyingType(KeyVersionColumnInfo.PropertyInfo.PropertyType) ?? KeyVersionColumnInfo.PropertyInfo.PropertyType) != typeof(int))
                exceptions.Add(new InvalidKeyVersionPropertyTypeException<T>(new PropertyName(KeyVersionColumnInfo.PropertyInfo.Name)));
            if (KeyVersionColumnsInfo.Count() > 1)
                exceptions.Add(new MultipleKeyVersionProperties<T>(KeyVersionColumnsInfo.Select(column => column.PropertyName)));
        }

        if (exceptions.Count == 1)
            throw exceptions.First();
        else if (exceptions.Count > 1)
            throw new AggregateException(exceptions);
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="SqlGenerator{T}"/> class without an encrypter.
    /// </summary>
    /// <exception cref="AggregateException">
    /// containing multiple exceptions. Potential exceptions include the exceptions listed below.
    /// </exception>
    /// <exception cref="InvalidSqlIdentifierException">
    /// for invalid table, schema, procedure, column, alias, or parameter identifiers.
    /// </exception>
    /// <exception cref="EncrypterNotProvidedException{T}">
    /// if encrypted columns exist but no encrypter was supplied
    /// </exception>
    /// <exception cref="NoKeyVersionPropertyException{T}">
    /// if encrypted columns exist but no key-version property was designated.
    /// </exception>
    /// <exception cref="InvalidKeyVersionPropertyTypeException{T}">
    /// if the key-version property is not an <see cref="int"/> (nullable allowed).
    /// </exception>
    /// <exception cref="MultipleKeyVersionProperties{T}">
    /// if more than one key-version property is present.
    /// </exception>
    public SqlGenerator()
    {
        _Encryption = null;
        ValidationChecks();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlGenerator{T}"/> class
    /// with the specified encryption service.
    /// </summary>
    /// <param name="encryption">
    /// The <see cref="IEncryption"/> implementation to use for encrypting
    /// and decrypting columns in SQL operations. Must not be <c>null</c>.
    /// </param>
    /// <exception cref="AggregateException">
    /// containing multiple exceptions. Potential exceptions include the exceptions listed below.
    /// </exception>
    /// <exception cref="InvalidSqlIdentifierException">
    /// for invalid table, schema, procedure, column, alias, or parameter identifiers.
    /// </exception>
    /// <exception cref="EncrypterNotProvidedException{T}">
    /// if encrypted columns exist but no encrypter was supplied
    /// </exception>
    /// <exception cref="NoKeyVersionPropertyException{T}">
    /// if encrypted columns exist but no key-version property was designated.
    /// </exception>
    /// <exception cref="InvalidKeyVersionPropertyTypeException{T}">
    /// if the key-version property is not an <see cref="int"/> (nullable allowed).
    /// </exception>
    /// <exception cref="MultipleKeyVersionProperties{T}">
    /// if more than one key-version property is present.
    /// </exception>
    public SqlGenerator(IEncryption encryption)
    {
        _Encryption = encryption ?? throw new ArgumentNullException(nameof(encryption));
        ValidationChecks();
    }

    /// <summary>
    /// Builds an <see cref="And"/> predicate for selecting a record by its key columns.
    /// </summary>
    /// <param name="entity">
    /// The entity instance containing the key property values used to construct the predicate.
    /// </param>
    /// <returns>
    /// An <see cref="And"/> predicate that matches the entity’s key column values,
    /// useful for selecting a record by its primary key.
    /// </returns>
    private static And GetByKeyPredicates(T entity) =>
        new (KeyColumnInfo.Select(key => new Equal(new Column<T>(key.PropertyName), new Parameter(key.ParameterTag, key.PropertyInfo.GetValue(entity)))));

    /// <summary>
    /// Creates a SQL parameter key–value pair for a specific column and entity instance.
    /// </summary>
    /// <param name="column">The <see cref="ColumnInfo"/> describing the target column.</param>
    /// <param name="entity">The entity instance supplying the column value.</param>
    /// <param name="entityIndex">
    /// Optional zero-based index for the entity when batching; appended to the parameter name to keep it unique.
    /// </param>
    /// <param name="parameterPrepend">
    /// Optional prefix to prepend to the parameter name (e.g., to namespace parameters).
    /// </param>
    /// <returns>
    /// A <see cref="KeyValuePair{TKey, TValue}"/> whose key is the final <see cref="ParameterTag"/> and whose value is the
    /// (possibly encrypted) column value or <see cref="DBNull.Value"/> if <c>null</c>.
    /// </returns>
    /// <remarks>
    /// If an encrypter is configured and the column is marked encrypted, the value is encrypted.
    /// If the column is the key-version column, the encrypter’s <c>Version</c> is used instead of the entity value.
    /// </remarks>
    /// <exception cref="NullReferenceException">
    /// Thrown if <paramref name="column"/> does not expose a <see cref="ParameterTag"/>.
    /// </exception>
    private KeyValuePair<ParameterTag, object> GetSqlParameterKeyValue(ColumnInfo column, T entity, int? entityIndex = null, string? parameterPrepend = null)
    {
        if (column.ParameterTag is null)
            throw new NullReferenceException();
        ParameterTag? parameter = column.ParameterTag.PrefixPrepend(parameterPrepend).AddIndex(entityIndex?.ToString() ?? null);

        if (_Encryption is not null && KeyVersionColumnInfo is not null && KeyVersionColumnInfo.Equals(column))
            return parameter.GetParameter(_Encryption?.Version);
        else if (_Encryption is not null && IsEncrypted(column))
            return parameter.GetParameter(_Encryption, column, entity);
        else
            return parameter.GetParameter(column, entity);
    }
    //TODO: Update documentation for new parameter IEnumerable<ColumnInfo> columns
    /// <summary>
    /// Generates SQL parameter key–value pairs for all mapped columns of a single entity.
    /// </summary>
    /// <param name="entity">The entity instance.</param>
    /// <param name="entityIndex">
    /// Optional zero-based index for the entity when batching; appended to parameter names to keep them unique.
    /// </param>
    /// <returns>
    /// A sequence of parameter key–value pairs for the entity.
    /// </returns>
    private IEnumerable<KeyValuePair<ParameterTag, object>> GetSqlParameterKeyValuePairs(IEnumerable<ColumnInfo> columns, T entity, int? entityIndex = null) =>
        columns.Select(column => GetSqlParameterKeyValue(column, entity, entityIndex));

    //TODO: Update documentation for new parameter IEnumerable<ColumnInfo> columns
    /// <summary>
    /// Generates a flattened sequence of SQL parameter key–value pairs for a collection of entities,
    /// automatically indexing parameter names per entity to ensure uniqueness.
    /// </summary>
    /// <param name="entities">The collection of entities to materialize as parameters.</param>
    /// <returns>A combined sequence of parameter key–value pairs for all entities.</returns>
    private IEnumerable<KeyValuePair<ParameterTag, object>> GetSqlParameterKeyValuePairs(IEnumerable<ColumnInfo> columns, params IEnumerable<T> entities) =>
        entities.SelectMany((entity, index) => GetSqlParameterKeyValuePairs(columns, entity, index));
}
