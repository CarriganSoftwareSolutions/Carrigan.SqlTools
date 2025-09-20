using Carrigan.Core.Attributes;
using Carrigan.Core.Extensions;
using Carrigan.Core.ReflectionCaching;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Tags;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Carrigan.SqlTools;

public class SqlToolsReflectorCache<T>
{
    internal static readonly Type Type;
    internal static IEnumerable<ColumnTag> KeyColumns =>
        _LazyKeyColumns.Value;

    internal static IEnumerable<ColumnTag> Columns =>
        _LazyColumnsDictionary.Value.Values;

    internal static IEnumerable<ColumnTag> ColumnsLessKeys =>
        _LazyColumnsLessKeys.Value;

    internal static TableTag Table => 
        _LazyTableTag.Value;

    internal static ProcedureTag ProcedureTag => 
        _LazyProcedureTag.Value;
    internal static ColumnTag? KeyVersionColumn =>
        _LazyKeyVersionColumn.Value;

    internal static bool ContainsEncryptedProperty(ColumnTag column) =>
        _LazyEncryptedColumnsHashSet.Value.Contains(column);

    internal static bool HasEncryptedColumns() =>
        _LazyEncryptedColumnsHashSet.Value.Count != 0;

    internal static IEnumerable<ColumnTag> GetColumnsFromProperties(params IEnumerable<string> propertyNames)
    {
        List<Exception> exceptions = [];

        IEnumerable<string> invalidPropertyNames = 
            propertyNames
                .Where(propertyName => ContainsProperty(propertyName) is false);

        IEnumerable<ColumnTag> columns =
            propertyNames
                .Where(propertyName => ContainsProperty(propertyName)) //we are testing column names here, so filter out property names with no corresponding column
                .Select(propertyName => GetColumnByProperty(propertyName))
                .OfType<ColumnTag>(); //filter nulls, clear null reference warning

        IEnumerable<ColumnTag> invalidColumns =
            columns
                .Where(column => SqlIdentifierPattern.Fails(column?._columnName ?? string.Empty))
                .OfType<ColumnTag>();

        if (invalidPropertyNames.Any())
            exceptions.Add(new ArgumentException($"The property names {invalidPropertyNames.JoinAnd()} do not have an associated column in {Table}.", nameof(propertyNames)));
        if (invalidColumns.Any())
            exceptions.Add(new SqlNamePatternException(invalidPropertyNames));

        if (exceptions.None())
            return columns; 
        else if (exceptions.Count == 1)
            throw exceptions.Single();
        else
            throw new AggregateException(exceptions);
    }

    private static bool ContainsProperty(string propertyName) =>
        _LazyColumnsDictionary.Value.ContainsKey(propertyName);

    private static ColumnTag? GetColumnByProperty(string propertyName) =>
        ContainsProperty(propertyName) ? _LazyColumnsDictionary.Value[propertyName] : null;

    private static readonly Lazy<TableTag> _LazyTableTag;
    private static readonly Lazy<ProcedureTag> _LazyProcedureTag;
    private static readonly Lazy<ColumnTag?> _LazyKeyVersionColumn;
    private static readonly Lazy<TableAttribute?> _LazyTableAttribute;

    private static readonly Lazy<IEnumerable<PropertyInfo>> _LazyProperties;
    private static readonly Lazy<IEnumerable<PropertyInfo>> _LazyPropertiesLessKeys;
    private static readonly Lazy<IEnumerable<ColumnTag>> _LazyKeyColumns;
    private static readonly Lazy<IEnumerable<ColumnTag>> _LazyColumnsLessKeys;

    private static readonly Lazy<HashSet<ColumnTag>> _LazyEncryptedColumnsHashSet;

    private static readonly Lazy<Dictionary<string, ColumnTag>> _LazyColumnsDictionary;

    static SqlToolsReflectorCache()
    {
        Type = typeof(T);

        _LazyTableAttribute = new (() => Type.GetCustomAttribute<TableAttribute>());

        //Procedure Tags self validate the SQL Identifier.
        _LazyProcedureTag = new (new ProcedureTag(GetSchemaName(), GetProcedureName()));

        //Table Tags self validate the SQL Identifier.
        _LazyTableTag = new(new TableTag(GetSchemaName(), GetTableName()));

        // Get properties that are public, not marked with [NotMapped], and are either value types or strings
        _LazyProperties = new
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

        _LazyPropertiesLessKeys = new 
            (() =>
                _LazyProperties
                    .Value
                    .Where(property => property.GetCustomAttribute<KeyAttribute>() == null)
            );

        _LazyEncryptedColumnsHashSet = new
            (() =>
                [.. _LazyPropertiesLessKeys
                        .Value
                        .Where(property => property.GetCustomAttribute<EncryptedAttribute>() != null)
                        .Select(property => new ColumnTag(_LazyTableTag.Value, GetColumnName(property), property, GetParameterName(property)))]
            );

        _LazyColumnsDictionary = new Lazy<Dictionary<string, ColumnTag>>
            (() =>
                [.. _LazyProperties
                    .Value
                    .Select(property => new KeyValuePair<string, ColumnTag>(property.Name, new ColumnTag(_LazyTableTag.Value, GetColumnName(property), property, GetParameterName(property))))]
            );

        _LazyColumnsLessKeys = new
            (() =>
                _LazyProperties
                    .Value
                    .Where(property => property.GetCustomAttribute<KeyAttribute>() == null)
                    .Select(property => new ColumnTag(_LazyTableTag.Value, GetColumnName(property), property, GetParameterName(property)))
            );

        _LazyKeyColumns = new
            (() =>
                ReflectorCache<T>
                    .ReadablePublicInstanceProperties
                    .Where(property => property.GetCustomAttribute<KeyAttribute>() != null)
                    .Select(property => new ColumnTag(_LazyTableTag.Value, GetColumnName(property), property, GetParameterName(property)))
            );

        _LazyKeyVersionColumn = new
            (() => _LazyProperties
                            .Value
                            .Where(property => property.GetCustomAttribute<KeyVersionAttribute>() != null)
                            .Where(property => ((Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType) == typeof(int)))
                            .Select(property => new ColumnTag(_LazyTableTag.Value, GetColumnName(property), property, GetParameterName(property)))
                            .FirstOrDefault()
            );
    }

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

    private static ParameterTag GetParameterName(PropertyInfo property)
    {
        ParameterAttribute? parameterName = property.GetCustomAttribute<ParameterAttribute>();

        if (parameterName != null && parameterName.Name.IsNotNullOrWhiteSpace())
        {
            if (SqlIdentifierPattern.Fails(parameterName.Name))
                throw new SqlNamePatternException(parameterName.Name);
            else
                return new(null, parameterName.Name, null);
        }
        else
        {
            return new(null, GetColumnName(property), null);
        }
    }

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
