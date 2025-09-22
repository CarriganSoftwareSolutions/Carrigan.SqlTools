using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Tags;
using Carrigan.Core.Attributes;

namespace Carrigan.SqlTools.SqlGenerators;

/// <summary>
/// Generates SQL for the data model type <typeparamref name="T"/>.
/// Provides helper methods to build statements and clauses based on the model’s
/// public metadata.
/// </summary>
/// <remarks>
/// The data model type must be <c>public</c>, and any properties intended to be used
/// as columns must be public instance properties with a public getter.
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
    /// Validates that encryption prerequisites are met for the current table.
    /// Throws an exception if encrypted columns exist but no encryption service or key version attribute
    /// has not been set.
    /// </summary>
    /// <exception cref="NullReferenceException">
    /// Thrown when encrypted columns are detected but either
    /// the <c>_Encryption</c> service is <c>null</c> or the
    /// <c>KeyVersionAttribute</c> has not been set on a property in <see cref="KeyVersionAttribute"/>.
    /// </exception>
    private void EncryptionChecks()
    {
        if (HasEncryptedColumns())
        {
            if (_Encryption is null)
                throw new NullReferenceException($"No encryption key data model, {Table}, with encrypted properties.");
            if (KeyVersionColumn is null)
                throw new NullReferenceException($"KeyVersion attribute of type int not set on data model, {Table}, with encrypted properties. ");
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlGenerator{T}"/> class
    /// with no encryption service configured.
    /// </summary>
    /// <remarks>
    /// Sets <c>_Encryption</c> to <c>null</c> and immediately calls
    /// <see cref="EncryptionChecks"/> to validate that encryption
    /// requirements (if any) are satisfied.
    /// </remarks>
    /// <exception cref="NullReferenceException">
    /// Thrown when encrypted columns are detected but either
    /// the <c>_Encryption</c> service is <c>null</c> or the
    /// <c>KeyVersionAttribute</c> has not been set on a property in <see cref="KeyVersionAttribute"/>.
    /// </exception>
    public SqlGenerator()
    {
        _Encryption = null;
        EncryptionChecks();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlGenerator{T}"/> class
    /// with the specified encryption service.
    /// </summary>
    /// <param name="encryption">
    /// The <see cref="IEncryption"/> implementation to use for encrypting
    /// and decrypting columns in SQL operations. Must not be <c>null</c>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="encryption"/> is <c>null</c>.
    /// </exception>
    /// <remarks>
    /// Assigns the provided <paramref name="encryption"/> to <c>_Encryption</c>
    /// and calls <see cref="EncryptionChecks"/> to ensure all encryption
    /// prerequisites are satisfied.
    /// </remarks>
    /// <exception cref="NullReferenceException">
    /// Thrown when encrypted columns are detected but either
    /// the <see <paramref name="encryption"/> is <c>null</c> or the
    /// <c>KeyVersionAttribute</c> has not been set on a property in <see cref="KeyVersionAttribute"/>.
    /// </exception>
    public SqlGenerator(IEncryption encryption)
    {
        _Encryption = encryption ?? throw new ArgumentNullException(nameof(encryption));
        EncryptionChecks();
    }

    /// <summary>
    /// Builds an <see cref="And"/> predicate for selecting a record by its key columns.
    /// </summary>
    /// <param name="entity">
    /// The entity instance containing the key field values used to construct the predicate.
    /// </param>
    /// <returns>
    /// An <see cref="And"/> predicate that matches the entity’s key column values,
    /// useful for selecting a record by its primary key.
    /// </returns>
    private static And GetByKeyPredicates(T entity) =>
        new (KeyColumns.Select(key => new Equal(new Columns<T>(key._columnName), new Parameters(key._columnName, key._propertyInfo.GetValue(entity)))));

    /// <summary>
    /// Creates a SQL parameter as a key–value pair for the specified column and entity instance.
    /// </summary>
    /// <param name="column">
    /// The <see cref="ColumnTag"/> that identifies the column for which to create a parameter.
    /// </param>
    /// <param name="entity">
    /// The entity instance (corresponding to a database row) that provides the column’s value.
    /// </param>
    /// <param name="entityIndex">
    /// Optional index of the entity in an enumerated collection.  
    /// If supplied, it is appended to the parameter name to ensure uniqueness.
    /// </param>
    /// <param name="parameterPrepend">
    /// Optional prefix to prepend to the parameter name.
    /// </param>
    /// <returns>
    /// A <see cref="KeyValuePair{TKey,TValue}"/> where  
    /// <typeparamref name="TKey"/> is <see cref="ParameterTag"/> (the SQL parameter name) and  
    /// <typeparamref name="TValue"/> is <see cref="object"/> (the parameter value or <see cref="DBNull.Value"/> if null).
    /// </returns>
    /// <remarks>
    /// If encryption is configured and the column is marked as encrypted,
    /// the column’s value is transparently encrypted before being stored in the parameter value.
    /// If the column represents a key version, the encryption version is used instead of the property value.
    /// </remarks>
    /// <exception cref="NullReferenceException">
    /// Thrown when the <paramref name="column"/> has no associated <see cref="ParameterTag"/>.
    /// </exception>
    private KeyValuePair<ParameterTag, object> GetSqlParameterKeyValue(ColumnTag column, T entity, int? entityIndex = null, string? parameterPrepend = null)
    {
        ParameterTag? key = (column._parameterTag ?? throw new NullReferenceException()).PrefixPrepend(parameterPrepend).AddIndex(entityIndex?.ToString() ?? null);

        if (_Encryption is not null && KeyVersionColumn is not null && KeyVersionColumn.Equals(column))
            return new (key, ((object?)_Encryption?.Version) ?? DBNull.Value);
        if (_Encryption is not null && ContainsEncryptedProperty(column))
            //the explicit conversion of _Encryption?.Encrypt(property.GetValue(entity)?.ToString()) to an object is required to avoid a compiler error.
            return new (key, ((object?)_Encryption?.Encrypt(column._propertyInfo.GetValue(entity)?.ToString())) ?? DBNull.Value);
        else
            return new (key, column._propertyInfo.GetValue(entity) ?? DBNull.Value);
    }

    /// <summary>
    /// Generates a sequence of SQL parameters as key–value pairs for the specified entity.
    /// </summary>
    /// <param name="entity">
    /// The entity instance (corresponding to a database row) that provides the parameter values.
    /// </param>
    /// <param name="entityIndex">
    /// Optional index of the entity in an enumerated collection.  
    /// If provided, it is appended to parameter names to ensure uniqueness.
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{TKey,TValue}"/> objects,
    /// where each key is a <see cref="ParameterTag"/> (the parameter name)
    /// and each value is the parameter value or <see cref="DBNull.Value"/> if null.
    /// </returns>
    private IEnumerable<KeyValuePair<ParameterTag, object>> GetSqlParameterKeyValuePairs(T entity, int? entityIndex = null) =>
        Columns.Select(column => GetSqlParameterKeyValue(column, entity, entityIndex));

    /// <summary>
    /// Generates a combined sequence of SQL parameters as key–value pairs
    /// for a collection of entities.
    /// </summary>
    /// <param name="entities">
    /// The collection of entities (each corresponding to a database row) that provide parameter values.
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{TKey,TValue}"/> objects
    /// aggregated from all specified entities, where each key is a <see cref="ParameterTag"/>
    /// and each value is the parameter value or <see cref="DBNull.Value"/> if null.
    /// </returns>
    /// <remarks>
    /// Each entity’s position in the collection is used as an index and appended to its parameter names
    /// to maintain uniqueness across multiple rows.
    /// </remarks>
    private IEnumerable<KeyValuePair<ParameterTag, object>> GetSqlParameterKeyValuePairs(params IEnumerable<T> entities) =>
        entities.SelectMany((entity, index) => GetSqlParameterKeyValuePairs(entity, index));
}
