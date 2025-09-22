using Carrigan.Core.Attributes;
using Carrigan.Core.Extensions;
using Carrigan.Core.ReflectionCaching;
using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Exceptions;
using Carrigan.SqlTools.Tags;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Carrigan.SqlTools;

/// <summary>
/// Provides lazy loaded reflection caching for the library.
/// </summary>
/// <typeparam name="T"></typeparam>
public class SqlToolsReflectorCache<T>
{
    /// <summary>
    /// Represents the C# <see cref="Type"/> for the class <see cref="T"/>
    /// </summary>
    internal static readonly Type Type;

    /// <summary>
    /// Represents an Enumeration  <see cref="ColumnTag"/> for the key fields for the given type <see cref="T"/>
    /// </summary>
    internal static IEnumerable<ColumnTag> KeyColumns =>
        _LazyKeyColumns.Value;

    /// <summary>
    /// Represents an Enumeration <see cref="ColumnTag"/>s for the given type <see cref="T"/>
    /// </summary>
    internal static IEnumerable<ColumnTag> Columns =>
        _LazyColumnsDictionary.Value.Values;

    /// <summary>
    /// Represents an Enumeration <see cref="ColumnTag"/>s for the given type <see cref="T"/> less the key fields
    /// </summary>
    internal static IEnumerable<ColumnTag> ColumnsLessKeys =>
        _LazyColumnsLessKeys.Value;

    /// <summary>
    /// Represents the <see cref="TableTag"/> for the given type <see cref="T"/>
    /// </summary>
    internal static TableTag Table => 
        _LazyTableTag.Value;

    /// <summary>
    /// Represents the <see cref="ProcedureTag"/> for the given type <see cref="T"/>
    /// </summary>
    internal static ProcedureTag ProcedureTag => 
        _LazyProcedureTag.Value;

    /// <summary>
    /// Represents the <see cref="ColumnTag"/> used to store the encryption version. This should be null if and only if the table doesn't contain encryption
    /// </summary>
    internal static ColumnTag? KeyVersionColumn =>
        _LazyKeyVersionColumn.Value;

    /// <summary>
    /// Determines if the <see cref="ColumnTag"/> contains a <see cref="PropertyInfo"/> flagged for encryption.
    /// </summary>
    /// <param name="column"></param>
    /// <returns>True if so, otherwise false.</returns>
    internal static bool ContainsEncryptedProperty(ColumnTag column) =>
        _LazyEncryptedColumnsHashSet.Value.Contains(column);

    /// <summary>
    /// Determines if a if table represented by T has encrypted columns
    /// </summary>
    /// <returns>True if so, false if not</returns>
    internal static bool HasEncryptedColumns() =>
        _LazyEncryptedColumnsHashSet.Value.Count != 0;


    /// <summary>
    /// Gets an enumeration of <see cref="ColumnTag"/>s one for each property name passed in.
    /// </summary>
    /// <param name="propertyNames">multiple property names </param>
    /// <returns>the <see cref="IEnumerable{ColumnTag}"/>  associated with a property name in <see cref="T"/></returns>
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

    /// <summary>
    /// Checks if there if a <see cref="ColumnTag"/> exists for a given property name.
    /// </summary>
    /// <param name="propertyName">PropertyInfo.Name</param>
    /// <returns>the <see cref="ColumnTag"/>  associated with a property name in <see cref="T"/></returns>
    private static bool ContainsProperty(string propertyName) =>
        _LazyColumnsDictionary.Value.ContainsKey(propertyName);

    /// <summary>
    /// Gets a nullable <see cref="ColumnTag"/> from a property name
    /// </summary>
    /// <param name="propertyName">PropertyInfo.Name</param>
    /// <returns>the <see cref="ColumnTag"/>  associated with a property name in <see cref="T"/></returns>
    private static ColumnTag? GetColumnByProperty(string propertyName) =>
        ContainsProperty(propertyName) ? _LazyColumnsDictionary.Value[propertyName] : null;

    /// <summary>
    /// Used for determining a table name associated with <see cref="T"/>
    /// USed to help set <see cref="Table"/>
    /// USed to help set <see cref="_LazyKeyColumns"/>
    /// USed to help set <see cref="_LazyEncryptedColumnsHashSet"/>
    /// USed to help set <see cref="_LazyColumnsDictionary"/>
    /// USed to help set <see cref="_LazyKeyVersionColumn"/>
    /// </summary>
    private static readonly Lazy<TableTag> _LazyTableTag;

    /// <summary>
    /// Used for determining a procedure name associated with <see cref="T"/>
    /// USed to help set <see cref="ProcedureTag"/>
    /// </summary>
    private static readonly Lazy<ProcedureTag> _LazyProcedureTag;

    /// <summary>
    /// Determines which ColumnTag is associated with the Encryption KeyVersion used in a record.
    /// USed to help set <see cref="KeyVersionColumn"/>
    /// </summary>
    private static readonly Lazy<ColumnTag?> _LazyKeyVersionColumn;

    /// <summary>
    /// Used to determine the Table name and Schema name, when the <see cref="TableAttribute"/> is used. 
    /// USed is in <see cref="GetTableName"/>
    /// USed is in <see cref="GetSchemaName"/>
    /// </summary>
    private static readonly Lazy<TableAttribute?> _LazyTableAttribute;

    /// <summary>
    /// LazyIEnumerable that contains all of the <see cref="PropertyInfo"/> for each qualifying property represented by <see cref="T"/>
    /// USed to help set <see cref="_LazyColumnsDictionary"/>
    /// USed to help set <see cref="_LazyKeyVersionColumn"/>
    /// </summary>
    private static readonly Lazy<IEnumerable<PropertyInfo>> _LazyProperties;

    /// <summary>
    /// LazyIEnumerable that contains all of the <see cref="PropertyInfo"/> foreach qualifying property represented by <see cref="T"/> that is not part of the key
    /// USed to help set <see cref="_LazyEncryptedColumnsHashSet"/>
    /// </summary>
    private static readonly Lazy<IEnumerable<PropertyInfo>> _LazyPropertiesLessKeys;

    /// <summary>
    /// LazyIEnumerable that contains all of the <see cref="ColumnTag"/> for each key column represented by <see cref="T"/>
    /// USed to help set <see cref="ColumnsLessKeys"/>
    /// USed to help set <see cref="_LazyKeyColumns"/>
    /// USed to help set <see cref="_LazyPropertiesLessKeys"/>
    /// USed to help set <see cref="_LazyColumnsLessKeys"/>
    /// </summary>
    private static readonly Lazy<IEnumerable<ColumnTag>> _LazyKeyColumns;

    /// <summary>
    /// LazyIEnumerable that contains all of the <see cref="ColumnTag"/> for each column represented by <see cref="T"/>
    /// USed to help set <see cref="KeyColumns"/>
    /// USed to help set <see cref="_LazyKeyVersionColumn"/>
    /// </summary>
    private static readonly Lazy<IEnumerable<ColumnTag>> _LazyColumnsLessKeys;

    /// <summary>
    /// Lazy HashSet used to determine if a column has been flagged for encryption.
    /// Used by <see cref="ContainsEncryptedProperty"/>
    /// Used by <see cref="HasEncryptedColumns"/>
    /// </summary>
    private static readonly Lazy<HashSet<ColumnTag>> _LazyEncryptedColumnsHashSet;

    /// <summary>
    /// Static Lazy Dictionary holds look up information to get a Column using a C# property name.
    /// Used by <see cref="Columns"/> 
    /// Used by <see cref="ContainsProperty"/>
    /// USed by <see cref="GetColumnByProperty"/>
    /// USed to help set <see cref="_LazyColumnsLessKeys"/>
    /// </summary>
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

        _LazyKeyColumns = new
            (() =>
                {
                    IEnumerable<PropertyInfo> keys =
                        ReflectorCache<T>
                            .ReadablePublicInstanceProperties
                            .Where(property => property.GetCustomAttribute<PrimaryKeyAttribute>() != null);
                    if(keys.None()) 
                        keys =
                            ReflectorCache<T>
                                .ReadablePublicInstanceProperties
                                .Where(property => property.GetCustomAttribute<KeyAttribute>() != null);

                    return keys
                        .Select(property => new ColumnTag(_LazyTableTag.Value, GetColumnName(property), property, GetParameterName(property)));
                }
            );

        _LazyPropertiesLessKeys = new 
            (() =>
                {
                    IEnumerable<PropertyInfo> keyProperties = 
                        _LazyKeyColumns
                            .Value
                            .Select(column => column._propertyInfo);
                    return _LazyProperties
                        .Value
                        .Where(property => keyProperties.DoesNotContain(property));
                }
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
                _LazyColumnsDictionary
                    .Value
                    .Values
                    .Where(column => _LazyKeyColumns.Value.DoesNotContain(column))
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

    /// <summary>
    /// Returns the column name associated with the <see cref="PropertyInfo"/> in class <see cref="T"/>
    /// </summary>
    /// <returns>The name of the column to be used in SQL generations</returns>
    /// <exception cref="SqlNamePatternException"></exception>
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

    /// <summary>
    /// Returns the parameter name associated with the <see cref="PropertyInfo"/> in class <see cref="T"/>
    /// </summary>
    /// <returns>The name of the parameter to be used in SQL generations</returns>
    /// <exception cref="SqlNamePatternException"></exception>
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

    /// <summary>
    /// Returns the table name associated with the class <see cref="T"/>
    /// </summary>
    /// <returns>The name of the table to be used in SQL generations</returns>
    /// <exception cref="SqlNamePatternException"></exception>
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

    /// <summary>
    /// Returns the procedure name associated with the class <see cref="T"/>
    /// </summary>
    /// <returns>The name of the procedure to be used in SQL generations</returns>
    /// <exception cref="SqlNamePatternException"></exception>
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

    /// <summary>
    /// Returns the schema name associated with the class <see cref="T"/>
    /// </summary>
    /// <returns>The name of the schema to be used in SQL generations</returns>
    /// <exception cref="SqlNamePatternException"></exception>
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
