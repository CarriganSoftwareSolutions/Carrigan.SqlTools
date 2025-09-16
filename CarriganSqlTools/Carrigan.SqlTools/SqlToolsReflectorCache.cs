using Carrigan.Core.Attributes;
using Carrigan.Core.Extensions;
using Carrigan.Core.ReflectionCaching;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Tags;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Xml.Linq;

namespace Carrigan.SqlTools;

internal static class SqlToolsReflectorCache<T>
{
    internal static readonly Type Type;
    internal static string ProcedureName => _LazyProcedureName.Value;
    internal static IEnumerable<PropertyInfo> Key => _LazyKey.Value;
    internal static IEnumerable<PropertyInfo> Properties => _LazyProperties.Value;
    internal static IEnumerable<PropertyInfo> PropertiesLessKeys => _LazyPropertiesLessKeys.Value;
    internal static IEnumerable<string> ColumnNames => _LazyColumnNames.Value;
    internal static HashSet<string> ColumnNamesHashSet => _LazyColumnNamesHashSet.Value;
    internal static string TableName => _LazyTableName.Value;
    internal static string TableSchema => _LazyTableSchema.Value;
    internal static TableTag TableTag => new (TableSchema, TableName);
    internal static ProcedureTag ProcedureTag => new (TableSchema, TableName);
    internal static HashSet<string> EncryptedProperties => _LazyEncryptedProperties.Value;
    internal static PropertyInfo? KeyVersionProperty => _LazyKeyVersionProperty.Value;

    private static readonly Lazy<string> _LazyProcedureName;
    private static readonly Lazy<IEnumerable<PropertyInfo>> _LazyKey;
    private static readonly Lazy<IEnumerable<PropertyInfo>> _LazyProperties;
    private static readonly Lazy<IEnumerable<PropertyInfo>> _LazyPropertiesLessKeys;
    private static readonly Lazy<IEnumerable<string>> _LazyColumnNames;
    private static readonly Lazy<HashSet<string>> _LazyColumnNamesHashSet;
    private static readonly Lazy<HashSet<string>> _LazyEncryptedProperties;
    private static readonly Lazy<TableAttribute?> _LazyTableAttribute;
    private static readonly Lazy<string> _LazyTableName;
    private static readonly Lazy<string> _LazyTableSchema;
    private static readonly Lazy<PropertyInfo?> _LazyKeyVersionProperty;


    static SqlToolsReflectorCache()
    {
        Type = typeof(T);

        _LazyTableAttribute = new Lazy<TableAttribute?>(() => Type.GetCustomAttribute<TableAttribute>());

        _LazyTableName = new Lazy<string>
            (GetTableName());

        _LazyTableSchema = new Lazy<string>
            (GetSchemaName());

        _LazyProcedureName = new Lazy<string>
            (GetProcedureName());

        _LazyKey = new Lazy<IEnumerable<PropertyInfo>>
            (() =>
                ReflectorCache<T>
                    .ReadablePublicInstanceProperties
                    .Where(property => property.GetCustomAttribute<KeyAttribute>() != null)
            );

        // Get properties that are public, not marked with [NotMapped], and are either value types or strings
        _LazyProperties = new Lazy<IEnumerable<PropertyInfo>>
            (() =>
                ReflectorCache<T>
                    .ReadablePublicInstanceProperties
                    .Where(property => property.GetCustomAttribute<NotMappedAttribute>() == null
                                && (property.PropertyType == typeof(int) ||              // SQL INT
                                    property.PropertyType == typeof(int?) ||             // SQL INT (nullable)
                                    property.PropertyType == typeof(long) ||             // SQL BIGINT
                                    property.PropertyType == typeof(long?) ||            // SQL BIGINT (nullable)
                                    property.PropertyType == typeof(short) ||            // SQL SMALLINT
                                    property.PropertyType == typeof(short?) ||           // SQL SMALLINT (nullable)
                                    property.PropertyType == typeof(byte) ||             // SQL TINYINT
                                    property.PropertyType == typeof(byte?) ||            // SQL TINYINT (nullable)
                                    property.PropertyType == typeof(bool) ||             // SQL BIT
                                    property.PropertyType == typeof(bool?) ||            // SQL BIT (nullable)
                                    property.PropertyType == typeof(decimal) ||          // SQL DECIMAL
                                    property.PropertyType == typeof(decimal?) ||         // SQL DECIMAL (nullable)
                                    property.PropertyType == typeof(float) ||            // SQL FLOAT
                                    property.PropertyType == typeof(float?) ||           // SQL FLOAT (nullable)
                                    property.PropertyType == typeof(double) ||           // SQL REAL
                                    property.PropertyType == typeof(double?) ||          // SQL REAL (nullable)
                                    property.PropertyType == typeof(string) ||           // SQL NVARCHAR, VARCHAR, TEXT
                                    property.PropertyType == typeof(DateTime) ||         // SQL DATETIME, DATETIME2, SMALLDATETIME
                                    property.PropertyType == typeof(DateTime?) ||        // SQL DATETIME, DATETIME2, SMALLDATETIME (nullable)
                                    property.PropertyType == typeof(Guid) ||             // SQL UNIQUEIDENTIFIER
                                    property.PropertyType == typeof(Guid?) ||            // SQL UNIQUEIDENTIFIER (nullable)
                                    property.PropertyType == typeof(byte[]) ||           // SQL VARBINARY
                                    property.PropertyType == typeof(char) ||             // SQL CHAR
                                    property.PropertyType == typeof(char?) ||            // SQL CHAR (nullable)
                                    property.PropertyType == typeof(TimeOnly) ||         // SQL Time
                                    property.PropertyType == typeof(TimeOnly?) ||        // SQL Time (nullable)
                                    property.PropertyType == typeof(DateOnly) ||         // SQL Date
                                    property.PropertyType == typeof(DateOnly?) ||        // SQL Date (nullable)
                                    property.PropertyType == typeof(DateTimeOffset) ||   // SQL DateTimeOffset
                                    property.PropertyType == typeof(DateTimeOffset?))    // SQL DateTimeOffset (nullable)
                        )
            );

        _LazyPropertiesLessKeys = new Lazy<IEnumerable<PropertyInfo>>
            (() =>
                _LazyProperties
                    .Value
                    .Where(property => property.GetCustomAttribute<KeyAttribute>() == null)
            );

        _LazyEncryptedProperties = new Lazy<HashSet<string>>
            (() =>
                [.. _LazyPropertiesLessKeys
                        .Value
                        .Where(property => property.GetCustomAttribute<EncryptedAttribute>() != null)
                        .Select(property => property.Name)]
            );
        _LazyColumnNames = new Lazy<IEnumerable<string>>
            (() =>
                _LazyProperties
                    .Value
                    .Select(property => property.Name)
            );
        _LazyColumnNamesHashSet = new Lazy<HashSet<string>>
            (() =>
                [.. _LazyColumnNames
                    .Value]
            );

        _LazyKeyVersionProperty = new Lazy<PropertyInfo?>
            (() => Properties
                    .Where(property => property.GetCustomAttribute<KeyVersionAttribute>() != null)
                    .FirstOrDefault()
            );
    }

    //TODO: write unit test
    private static string GetColumnName(PropertyInfo property)
    {
        IdentifierAttribute? identifier = property.GetCustomAttribute<IdentifierAttribute>();
        if (identifier != null && identifier.Name.IsNotNullOrWhiteSpace())
            if (SqlIdentifierPattern.Fails(identifier.Name))
                throw new SqlNamePatternException(identifier.Name);
            else
                return identifier.Name;
        else
        {
            ColumnAttribute? columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
            if(columnAttribute != null && columnAttribute.Name.IsNotNullOrWhiteSpace())
                if (SqlIdentifierPattern.Fails(columnAttribute.Name))
                    throw new SqlNamePatternException(columnAttribute.Name);
                else
                    return columnAttribute.Name;
            else  if (SqlIdentifierPattern.Fails(property.Name))
                throw new SqlNamePatternException(property.Name);
            else
                return property.Name;
        }
    }

    //TODO: write unit test
    private static string GetTableName()
    {
        IdentifierAttribute? identifier = Type.GetCustomAttribute<IdentifierAttribute>();
        if (identifier != null && identifier.Name.IsNotNullOrWhiteSpace())
            if (SqlIdentifierPattern.Fails(identifier.Name))
                throw new SqlNamePatternException(identifier.Name);
            else
                return identifier.Name;
        else
        {
            if (_LazyTableAttribute.Value != null && _LazyTableAttribute.Value.Name.IsNotNullOrWhiteSpace())
                if (SqlIdentifierPattern.Fails(_LazyTableAttribute.Value.Name))
                    throw new SqlNamePatternException(_LazyTableAttribute.Value.Name);
                else
                    return _LazyTableAttribute.Value.Name;
            else if (SqlIdentifierPattern.Fails(Type.Name))
                throw new SqlNamePatternException(Type.Name);
            else
                return Type.Name;
        }
    }

    //TODO: write unit test
    private static string GetProcedureName()
    {
        IdentifierAttribute? identifier = Type.GetCustomAttribute<IdentifierAttribute>();
        if (identifier != null && identifier.Name.IsNotNullOrWhiteSpace())
            if (SqlIdentifierPattern.Fails(identifier.Name))
                throw new SqlNamePatternException(identifier.Name);
            else
                return identifier.Name;
        else if (SqlIdentifierPattern.Fails(Type.Name))
                throw new SqlNamePatternException(Type.Name);
        else
            return Type.Name;
    }


    //TODO: write unit test
    private static string GetSchemaName()
    {
        IdentifierAttribute? identifier = Type.GetCustomAttribute<IdentifierAttribute>();
        if (identifier != null && identifier.Schema.IsNotNullOrWhiteSpace())
            if (SqlIdentifierPattern.Fails(identifier.Schema))
                throw new SqlNamePatternException(identifier.Schema);
            else
                return identifier.Schema;
        else
        {
            if (_LazyTableAttribute.Value != null && _LazyTableAttribute.Value.Schema.IsNotNullOrWhiteSpace())
                if (SqlIdentifierPattern.Fails(_LazyTableAttribute.Value.Schema))
                    throw new SqlNamePatternException(_LazyTableAttribute.Value.Schema);
                else
                    return _LazyTableAttribute.Value.Schema;
            else
                return string.Empty;
        }
    }
}
