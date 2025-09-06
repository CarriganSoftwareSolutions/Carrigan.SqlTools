using Carrigan.Core.Attributes;
using Carrigan.Core.ReflectionCaching;
using SqlTools.Tags;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace SqlTools;

internal static class SqlToolsReflectorCache<T>
{
    internal static readonly Type Type;
    internal static string ProcedureName => _LazyProcedureName.Value;
    internal static IEnumerable<PropertyInfo> Key => _LazyKey.Value;
    internal static IEnumerable<PropertyInfo> Properties => _LazyProperties.Value;
    internal static IEnumerable<PropertyInfo> PropertiesLessKeys => _LazyPropertiesLessKeys.Value;
    public static IEnumerable<string> ColumnNames => _LazyColumnNames.Value;
    public static HashSet<string> ColumnNamesHashSet => _LazyColumnNamesHashSet.Value;
    public static string TableName => _LazyTableName.Value;
    public static string TableSchema => _LazyTableSchema.Value;
    public static TableTag TableTag => (new TableTag(TableSchema, TableName));
    public static HashSet<string> EncryptedProperties => _LazyEncryptedProperties.Value;
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

        _LazyTableAttribute = new Lazy<TableAttribute?> (() => Type.GetCustomAttribute<TableAttribute>());

        _LazyTableName = new Lazy<string> (() => _LazyTableAttribute.Value is null ? Type.Name : _LazyTableAttribute.Value.Name);

        _LazyTableSchema = new Lazy<string> 
            (() => _LazyTableAttribute.Value is null || string.IsNullOrEmpty(_LazyTableAttribute.Value.Schema) ? string.Empty : _LazyTableAttribute.Value.Schema);

        _LazyProcedureName = new Lazy<string>
            (() =>
            {
                // Check if the Procedure attribute is present, use it if available
                ProcedureAttribute procedureAttribute = Type.GetCustomAttribute<ProcedureAttribute>();
                return procedureAttribute != null
                    ? string.IsNullOrEmpty(procedureAttribute.Schema)
                        ? $"[{procedureAttribute.Name}]"
                        : $"[{procedureAttribute.Schema}].[{procedureAttribute.Name}]"
                    : $"[{Type.Name}]";
            }
            );

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

}
