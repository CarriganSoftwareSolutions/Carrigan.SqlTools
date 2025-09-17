using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Tags;
using System.Reflection;

namespace Carrigan.SqlTools.SqlGenerators;

/// <summary>
/// The SQL generator class. Provides SQL generation methods for type, <see cref="T"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public partial class SqlGenerator<T> : SqlToolsReflectorCache<T> where T : class
{
    /// <summary>
    /// Holds the encryptor, null if there is no encryption and no encryptor is provided.
    /// </summary>
    private readonly IEncryption? _Encryption;

    /// <summary>
    /// Throws exceptions if there are encrypted fields and encryption / decryption hasn't been provided.
    /// </summary>
    /// <exception cref="NullReferenceException">There are encrypted fields and you didn't provide the required encryption setups.</exception>
    private void EncryptionChecks()
    {
        if (HasEncryptedColumns())
        {
            if (_Encryption is null)
                throw new NullReferenceException($"No encryption key data model, {Table}, with encrypted properties.");
            if (KeyVersionProperty is null)
                throw new NullReferenceException($"KeyVersion attribute not set on data model, {Table}, with encrypted properties.");
            if ((Nullable.GetUnderlyingType(KeyVersionProperty.PropertyType) ?? KeyVersionProperty.PropertyType) != typeof(int))
                throw new NullReferenceException($"The KeyVersion, {KeyVersionProperty.Name}, attribute is not a int for data model, {Table}");
        }
    }

    /// <summary>
    /// Class constructor
    /// </summary>
    public SqlGenerator()
    {
        _Encryption = null;
        EncryptionChecks();
    }

    /// <summary>
    /// Class constructor with an IEncrytion interface.
    /// </summary>
    public SqlGenerator(IEncryption encryption)
    {
        _Encryption = encryption ?? throw new ArgumentNullException(nameof(encryption));
        EncryptionChecks();
    }

    /// <summary>
    /// Returns an And predicate useful for selecting a record by Id.
    /// </summary>
    /// <param name="entity">holder for the key fields values</param>
    /// <returns>an And predicate useful for selecting a record by Id.</returns>
    private static And GetByKeyPredicates(T entity) =>
        new (Key.Select(key => new Equal(new Columns<T>(key.Name), new Parameters(key.Name, key.GetValue(entity)))));

    /// <summary>
    /// Gets an SQL Parameters as a Key Value pair
    /// </summary>
    /// <param name="property">The property (which corresponds to a column in the database)</param>
    /// <param name="useEncryption">Is it encrypted?</param>
    /// <param name="entity">An instance of the data model (corresponds to a row in the database)</param>
    /// <param name="entityIndex">the index value of the enumerated record in the collection</param>
    /// <param name="parameterPrepend">The prefix for the parameter that corresponds the value</param>
    /// <returns>Gets an SQL Parameters as a Key Value pair</returns>
    private KeyValuePair<string, object> GetSqlParameterKeyValue(PropertyInfo property, bool useEncryption, T entity, int? entityIndex = null, string? parameterPrepend = null)
    {
        string key;
        if(parameterPrepend.IsNullOrEmpty())
            key = entityIndex == null ? property.Name : $"{property.Name}_{entityIndex}";
        else
            key = entityIndex == null ? $"{parameterPrepend}{property.Name}" : $"{parameterPrepend}{property.Name}_{entityIndex}";
        if(_Encryption is not null && useEncryption && KeyVersionProperty is not null && KeyVersionProperty.Name.Equals(property.Name))
            return new KeyValuePair<string, object>(key, ((object?)_Encryption?.Version) ?? DBNull.Value);
        if (_Encryption is not null && useEncryption && ContainsEncryptedProperty(property.Name))
            //the explicit conversion of _Encryption?.Encrypt(property.GetValue(entity)?.ToString()) to an object is required to avoid a compiler error.
            return new KeyValuePair<string, object>(key, ((object?)_Encryption?.Encrypt(property.GetValue(entity)?.ToString())) ?? DBNull.Value);
        else
            return new KeyValuePair<string, object>(key, property.GetValue(entity) ?? DBNull.Value);
    }
    /// <summary>
    /// Gets an SQL Parameters as a Key Value pair
    /// </summary>
    /// <param name="column">The property (which corresponds to a column in the database)</param>
    /// <param name="useEncryption">Is it encrypted?</param>
    /// <param name="entity">An instance of the data model (corresponds to a row in the database)</param>
    /// <param name="entityIndex">the index value of the enumerated record in the collection</param>
    /// <param name="parameterPrepend">The prefix for the parameter that corresponds the value</param>
    /// <returns>Gets an SQL Parameters as a Key Value pair</returns>
    private KeyValuePair<string, object> GetSqlParameterKeyValue(ColumnTag column, bool useEncryption, T entity, int? entityIndex = null, string? parameterPrepend = null)
    {
        string key;
        if (parameterPrepend.IsNullOrEmpty())
            key = entityIndex == null ? column._columnName : $"{column._columnName}_{entityIndex}";
        else
            key = entityIndex == null ? $"{parameterPrepend}{column._columnName}" : $"{parameterPrepend}{column._columnName}_{entityIndex}";
        if (_Encryption is not null && useEncryption && KeyVersionProperty is not null && KeyVersionProperty.Name.Equals(column._columnName))
            return new KeyValuePair<string, object>(key, ((object?)_Encryption?.Version) ?? DBNull.Value);
        if (_Encryption is not null && useEncryption && ContainsEncryptedProperty(column._columnName))
            //the explicit conversion of _Encryption?.Encrypt(property.GetValue(entity)?.ToString()) to an object is required to avoid a compiler error.
            return new KeyValuePair<string, object>(key, ((object?)_Encryption?.Encrypt(GetValue(column, entity)?.ToString())) ?? DBNull.Value);
        else
            return new KeyValuePair<string, object>(key, GetValue(column, entity) ?? DBNull.Value);
    }

    private object? GetValue(ColumnTag column, T entity) =>
        PropertyInfoDicitionary.TryGetValue(column, out PropertyInfo? value) ? value.GetValue(entity) : null;

    /// <summary>
    /// Get enumeration of key value pairs of parameters associated with the query
    /// </summary>
    /// <param name="useEncryption">Is it encrypted?</param>
    /// <param name="entity">An instance of the data model (corresponds to a row in the database)</param>
    /// <param name="entityIndex">the index value of the enumerated record in the collection</param>
    /// <returns>key value pairs of parameters associated with the query</returns>
    private IEnumerable<KeyValuePair<string, object>> GetSqlParameterKeyValuePairs(bool useEncryption, T entity, int? entityIndex = null) =>
        Properties.Select(property => GetSqlParameterKeyValue(property, useEncryption, entity, entityIndex));

    /// <summary>
    /// Get enumeration of key value pairs of parameters associated with the query, from many
    /// </summary>
    /// <param name="entity">An instance of the data model (corresponds to a row in the database)</param>
    /// <param name="entityIndex">the index value of the enumerated record in the collection</param>
    /// <returns>enumeration of key value pairs of parameters associated with the query, from many</returns>
    private IEnumerable<KeyValuePair<string, object>> GetSqlParameterKeyValuePairs(bool useEncryption, params IEnumerable<T> entities) =>
        entities.SelectMany((entity, index) => GetSqlParameterKeyValuePairs(useEncryption, entity, index));
}
