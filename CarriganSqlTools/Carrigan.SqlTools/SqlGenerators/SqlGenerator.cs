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
public partial class SqlGenerator<T>
{
#pragma warning disable IDE1006 // Naming Styles
    private static string _ProcedureName => SqlToolsReflectorCache<T>.ProcedureName;
    /// <summary>
    /// Uses reflection to determine which columns are part of the key field. Cached with LazyLoad
    /// </summary>
    private static IEnumerable<PropertyInfo> _Key => SqlToolsReflectorCache<T>.Key;
    /// <summary>
    /// Uses reflection to get an enumeration of all public, readable instance properties
    /// </summary>
    private static IEnumerable<PropertyInfo> _Properties => SqlToolsReflectorCache<T>.Properties;
    /// <summary>
    /// Uses reflection to get an enumeration of all public, readable instance properties that are not part of the key.
    /// </summary>
    private static IEnumerable<PropertyInfo> _PropertiesLessKeys => SqlToolsReflectorCache<T>.PropertiesLessKeys;
    /// <summary>
    /// Uses reflection to get an enumeration of all public, readable instance properties that are marked for encryption.
    /// </summary>
    private static HashSet<string> _EncryptedProperties => SqlToolsReflectorCache<T>.EncryptedProperties;
    /// <summary>
    /// Uses reflection to get an enumeration of all public, readable instance properties that marked as containing the encryption key version number.
    /// </summary>
    private static PropertyInfo? _KeyVersionProperty => SqlToolsReflectorCache<T>.KeyVersionProperty;

#pragma warning restore IDE1006 // Naming Styles
    /// <summary>
    /// Holds the encryptor, null if there is no encryption and no encryptor is provided.
    /// </summary>
    private readonly IEncryption? _Encryption;
    /// <summary>
    /// The table tag, identifier, for the data model.
    /// </summary>
    public static TableTag TableTag => SqlToolsReflectorCache<T>.TableTag;

    /// <summary>
    /// Throws exceptions if there are encrypted fields and encryption / decryption hasn't been provided.
    /// </summary>
    /// <exception cref="NullReferenceException">There are encrypted fields and you didn't provide the required encryption setups.</exception>
    private void EncryptionChecks()
    {
        if (_EncryptedProperties.Count != 0)
        {
            if (_Encryption is null)
                throw new NullReferenceException($"No encryption key data model, {TableTag}, with encrypted properties.");
            if (_KeyVersionProperty is null)
                throw new NullReferenceException($"KeyVersion attribute not set on data model, {TableTag}, with encrypted properties.");
            if ((Nullable.GetUnderlyingType(_KeyVersionProperty.PropertyType) ?? _KeyVersionProperty.PropertyType) != typeof(int))
                throw new NullReferenceException($"The KeyVersion, {_KeyVersionProperty.Name}, attribute is not a int for data model, {TableTag}");
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
        new (_Key.Select(key => new Equal(new Columns<T>(key.Name), new Parameters(key.Name, key.GetValue(entity)))));

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
        if(_Encryption is not null && useEncryption && _KeyVersionProperty is not null && _KeyVersionProperty.Name.Equals(property.Name))
            return new KeyValuePair<string, object>(key, _Encryption.Version);
        if (_Encryption is not null && useEncryption && _EncryptedProperties.Contains(property.Name))
            //the explicit conversion of _Encryption?.Encrypt(property.GetValue(entity)?.ToString()) to an object is required to avoid a compiler error.
            return new KeyValuePair<string, object>(key, ((object)_Encryption?.Encrypt(property.GetValue(entity)?.ToString())) ?? DBNull.Value);
        else
            return new KeyValuePair<string, object>(key, property.GetValue(entity) ?? DBNull.Value);
    }

    /// <summary>
    /// Get enumeration of key value pairs of parameters associated with the query
    /// </summary>
    /// <param name="useEncryption">Is it encrypted?</param>
    /// <param name="entity">An instance of the data model (corresponds to a row in the database)</param>
    /// <param name="entityIndex">the index value of the enumerated record in the collection</param>
    /// <returns>key value pairs of parameters associated with the query</returns>
    private IEnumerable<KeyValuePair<string, object>> GetSqlParameterKeyValuePairs(bool useEncryption, T entity, int? entityIndex = null) =>
        _Properties.Select(property => GetSqlParameterKeyValue(property, useEncryption, entity, entityIndex));

    /// <summary>
    /// Get enumeration of key value pairs of parameters associated with the query, from many
    /// </summary>
    /// <param name="entity">An instance of the data model (corresponds to a row in the database)</param>
    /// <param name="entityIndex">the index value of the enumerated record in the collection</param>
    /// <returns>enumeration of key value pairs of parameters associated with the query, from many</returns>
    private IEnumerable<KeyValuePair<string, object>> GetSqlParameterKeyValuePairs(bool useEncryption, params IEnumerable<T> entities) =>
        entities.SelectMany((entity, index) => GetSqlParameterKeyValuePairs(useEncryption, entity, index));
}
