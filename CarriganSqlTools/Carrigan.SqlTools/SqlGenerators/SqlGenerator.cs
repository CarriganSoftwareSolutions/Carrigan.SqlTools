using Carrigan.Core.Enums;
using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.RegularExpressions;
using Carrigan.SqlTools.Tags;
using System.Data;
using System.Reflection;
//IGNORE SPELLING: parameterization
namespace Carrigan.SqlTools.SqlGenerators;

/// <summary>
/// Generates SQL for the data model type <typeparamref name="T"/>.
/// Provides helper methods for building statements and clauses based on the model’s
/// publicly accessible metadata.
/// </summary>
/// <remarks>
/// When generating SQL, only properties that are publicly readable from accessible types
/// are considered. Members that are not visible outside their defining assembly are ignored.
/// </remarks>
/// <typeparam name="T">
/// The entity or data model type that defines the target table and columns.
/// </typeparam>
public partial class SqlGenerator<T> : SqlToolsReflectorCache<T> where T : class
{
    /// <summary>
    /// Stores the encryption service used for SQL generation,
    /// or <c>null</c> if encryption is not required or no encrypter was provided.
    /// </summary>
    private readonly IEncryption? _Encryption;

    /// <summary>
    /// Validates identifier formats, SQL type compatibility, and encryption prerequisites
    /// for the data model.
    /// </summary>
    /// <exception cref="AggregateException">
    /// Thrown when multiple validation errors are detected. Possible inner exceptions include:
    /// <see cref="InvalidSqlIdentifierException"/>,
    /// <see cref="SqlTypeMismatchException"/>,
    /// <see cref="EncrypterNotProvidedException{T}"/>,
    /// <see cref="NoKeyVersionException{T}"/>,
    /// <see cref="InvalidKeyVersionPropertyTypeException{T}"/>, and
    /// <see cref="MultipleKeyVersionsException{T}"/>.
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
                .Where(static column => SqlIdentifierPattern.Fails(column.ColumnName))
                .Select(static column => new Tuple<PropertyInfo, ColumnName>(column.PropertyInfo, column.ColumnName))
                .Materialize(NullOptionsEnum.Exception);

        if (invalidColumns.Any())
            exceptions.Add(new InvalidSqlIdentifierException(invalidColumns));

        ColumnInfo
            .Select(static column => new Tuple<PropertyInfo, SqlTypeAttribute?>(column.PropertyInfo, SqlTypeAttribute.GetSqlTypeAttribute(column.PropertyInfo)))
            .Select(static tuple => SqlTypeMismatchException.Validate(tuple.Item1, tuple.Item2))
            .ForEach(sqlTypeMismatchException =>
            {
                if (sqlTypeMismatchException is not null)
                    exceptions.Add(sqlTypeMismatchException);
            });

       invalidAliases =
            ColumnInfo
                .Where(static column => column.AliasName is not null && SqlIdentifierPattern.Fails(column.AliasName))
                .Select(static column =>
                    column.AliasName is not null
                        ? new Tuple<PropertyInfo, AliasName>(column.PropertyInfo, column.AliasName)
                        : null)
                .OfType<Tuple<PropertyInfo, AliasName>>()
                .Materialize(NullOptionsEnum.Exception);

        if (invalidAliases.Any())
            exceptions.Add(new InvalidSqlIdentifierException(invalidAliases));

        invalidParameters =
            ColumnInfo
                .Where(static column => SqlIdentifierPattern.Fails(column.ParameterTag))
                .Select(static column => new Tuple<PropertyInfo, ParameterTag>(column.PropertyInfo, column.ParameterTag))
                .Materialize(NullOptionsEnum.Exception);

        if (invalidParameters.Any())
            exceptions.Add(new InvalidSqlIdentifierException(invalidParameters));

        if (HasEncryptedColumns())
        {
            if (_Encryption is null)
                exceptions.Add(new EncrypterNotProvidedException<T>());
            if (KeyVersionColumnInfo is null)
                exceptions.Add(new NoKeyVersionException<T>());
            else if ((Nullable.GetUnderlyingType(KeyVersionColumnInfo.PropertyInfo.PropertyType) ?? KeyVersionColumnInfo.PropertyInfo.PropertyType) != typeof(int))
                exceptions.Add(new InvalidKeyVersionPropertyTypeException<T>(new PropertyName(KeyVersionColumnInfo.PropertyInfo.Name)));
            if (KeyVersionColumnsInfo.Count() > 1)
                exceptions.Add(new MultipleKeyVersionsException<T>(KeyVersionColumnsInfo.Select(static column => column.PropertyName)));
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
    /// <exception cref="NoKeyVersionException{T}">
    /// if encrypted columns exist but no key-version property was designated.
    /// </exception>
    /// <exception cref="InvalidKeyVersionPropertyTypeException{T}">
    /// if the key-version property is not an <see cref="int"/> (nullable allowed).
    /// </exception>
    /// <exception cref="MultipleKeyVersionsException{T}">
    /// if more than one key-version property is present.
    /// </exception>
    public SqlGenerator()
    {
        _Encryption = null;
        ValidationChecks();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlGenerator{T}"/> class
    /// using the specified encryption service.
    /// </summary>
    /// <param name="encryption">
    /// The <see cref="IEncryption"/> implementation used to encrypt and decrypt values.
    /// Must not be <c>null</c>.
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
    /// <exception cref="NoKeyVersionException{T}">
    /// if encrypted columns exist but no key-version property was designated.
    /// </exception>
    /// <exception cref="InvalidKeyVersionPropertyTypeException{T}">
    /// if the key-version property is not an <see cref="int"/> (nullable allowed).
    /// </exception>
    /// <exception cref="MultipleKeyVersionsException{T}">
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
    /// The entity instance containing the key values used to construct the predicate.
    /// </param>
    /// <returns>
    /// An <see cref="And"/> predicate that matches the entity’s key column values.  
    /// Useful for selecting a record by primary key.
    /// </returns>
    private static And GetByKeyPredicates(T entity) =>
        new(KeyColumnInfo.Select(key => new Equal(new Column<T>(key.PropertyName), new Parameter(key.ParameterTag, key.PropertyInfo.GetValue(entity)))));


    /// <summary>
    /// Creates a SQL parameter key–value pair for the specified column and entity instance.
    /// </summary>
    /// <param name="column">The <see cref="ColumnInfo"/> describing the target column.</param>
    /// <param name="entity">The entity instance supplying the column value.</param>
    /// <param name="entityIndex">
    /// Optional zero-based index used when batching operations; appended to the parameter name.
    /// </param>
    /// <param name="parameterPrepend">
    /// Optional prefix to prepend to the parameter name.
    /// </param>
    /// <returns>
    /// A <see cref="KeyValuePair{TKey, TValue}"/> whose key is the computed <see cref="ParameterTag"/>
    /// and whose value is either the encrypted/unencrypted value or <see cref="DBNull.Value"/>.
    /// </returns>
    /// <remarks>
    /// If the column is encrypted, the value is encrypted.  
    /// If the column is the key-version column, the encrypter's version is used instead of the entity value.  
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="column"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="entity"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// Thrown if <paramref name="column"/> does not expose a <see cref="ParameterTag"/>.
    /// </exception>
    /// <exception cref="InvalidParameterIdentifierException">
    /// Thrown if <paramref name="parameterPrepend"/> produces an invalid parameter identifier.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when an index is applied to a parameter that already has an index.
    /// </exception>
    private KeyValuePair<ParameterTag, object> GetSqlParameterKeyValue(ColumnInfo column, T entity, int? entityIndex = null, string? parameterPrepend = null)
    {
        ArgumentNullException.ThrowIfNull(column);
        ArgumentNullException.ThrowIfNull(entity);

        if (column.ParameterTag is null)
            throw new NullReferenceException();

        ParameterTag parameter = column.ParameterTag.PrefixPrepend(parameterPrepend).AddIndex(entityIndex?.ToString() ?? null);

        if (_Encryption is not null && KeyVersionColumnInfo is not null && KeyVersionColumnInfo.Equals(column))
            return parameter.GetParameter(_Encryption.Version);
        else if (_Encryption is not null && IsEncrypted(column))
            return parameter.GetParameter(_Encryption, column, entity);
        else
            return parameter.GetParameter(column, entity);
    }


    /// <summary>
    /// Generates SQL parameter key–value pairs for all mapped columns of a single entity.
    /// </summary>
    /// <param name="columns">The columns that require parameterization.</param>
    /// <param name="entity">The entity instance.</param>
    /// <param name="entityIndex">Optional index for batch parameter naming.</param>
    /// <returns>A sequence of SQL parameter key–value pairs.</returns>
    private IEnumerable<KeyValuePair<ParameterTag, object>> GetSqlParameterKeyValuePairs(IEnumerable<ColumnInfo> columns, T entity, int? entityIndex = null) =>
        columns.Select(column => GetSqlParameterKeyValue(column, entity, entityIndex));

    /// <summary>
    /// Generates a flattened sequence of SQL parameter key–value pairs for a collection of entities,
    /// automatically indexing parameter names to ensure uniqueness in batched operations.
    /// </summary>
    /// <param name="columns">Columns requiring parameters.</param>
    /// <param name="entities">The collection of entities.</param>
    /// <returns>A combined sequence of SQL parameter key–value pairs.</returns>
    private IEnumerable<KeyValuePair<ParameterTag, object>> GetSqlParameterKeyValuePairs(IEnumerable<ColumnInfo> columns, params IEnumerable<T> entities) =>
        entities.SelectMany((entity, index) => GetSqlParameterKeyValuePairs(columns, entity, index));
}
