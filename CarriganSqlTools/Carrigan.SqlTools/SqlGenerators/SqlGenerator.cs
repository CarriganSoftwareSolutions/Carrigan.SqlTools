using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces;
using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Tags;
using System.Reflection;

namespace Carrigan.SqlTools.SqlGenerators;

public partial class SqlGenerator<T>
{
#pragma warning disable IDE1006 // Naming Styles
    private static string _ProcedureName => SqlToolsReflectorCache<T>.ProcedureName;
    private static IEnumerable<PropertyInfo> _Key => SqlToolsReflectorCache<T>.Key;
    private static IEnumerable<PropertyInfo> _Properties => SqlToolsReflectorCache<T>.Properties;
    private static IEnumerable<PropertyInfo> _PropertiesLessKeys => SqlToolsReflectorCache<T>.PropertiesLessKeys;
    private static HashSet<string> _EncryptedProperties => SqlToolsReflectorCache<T>.EncryptedProperties;
    private static PropertyInfo? _KeyVersionProperty => SqlToolsReflectorCache<T>.KeyVersionProperty;

#pragma warning restore IDE1006 // Naming Styles
    private readonly IEncryption? _Encryption;
    public static TableTag TableTag => SqlToolsReflectorCache<T>.TableTag;

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

    public SqlGenerator()
    {
        _Encryption = null;
        EncryptionChecks();
    }

    public SqlGenerator(IEncryption encryption)
    {
        //TODO: pull the encryption logic out to a separate class.

        _Encryption = encryption ?? throw new ArgumentNullException(nameof(encryption));
        EncryptionChecks();
    }

    private static And GetByKeyPredicates(T entity) =>
        new (_Key.Select(key => new Equal(new Columns<T>(key.Name), new Parameters(key.Name, key.GetValue(entity)))));

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

    private IEnumerable<KeyValuePair<string, object>> GetSqlParameterKeyValuePairs(bool useEncryption, T entity, int? entityIndex = null) =>
        _Properties.Select(property => GetSqlParameterKeyValue(property, useEncryption, entity, entityIndex));

    private IEnumerable<KeyValuePair<string, object>> GetSqlParameterKeyValuePairs(bool useEncryption, params IEnumerable<T> entities) =>
        entities.SelectMany((entity, index) => GetSqlParameterKeyValuePairs(useEncryption, entity, index));
}
