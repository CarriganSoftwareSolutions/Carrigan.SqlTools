using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Paging;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Dialects;

/// <summary>
/// Defines the rendering contract used by SQL generators to translate shared SQL fragments into a concrete database dialect.
/// Implementations quote identifiers, render table and column references, assign parameter names, render paging clauses,
/// normalize temporal values, and map CLR types to dialect-specific SQL type metadata.
/// </summary>
public interface ISqlDialects
{
    /// <summary>
    /// Quotes an identifier (such as a table name or column name) according to the rules of the SQL dialect.
    /// </summary>
    /// <param name="identifier">The identifier to quote.</param>
    /// <returns>The quoted identifier.</returns>
    string QuoteIdentifier(string identifier);

    /// <summary>
    /// Generates a string representation of the specified database procedure, optionally within a given schema.
    /// </summary>
    /// <param name="procedure">The procedure tag to render. Cannot be null or empty.</param>
    /// <returns>A string containing the rendered representation of the specified procedure.</returns>
    string RenderProcedureTag(ProcedureTag procedure);

    /// <summary>
    /// Generates a string representation of the specified database table, optionally within a given schema.
    /// </summary>
    /// <param name="schemaName">The optional schema name that qualifies the table. May be null to omit the schema.</param>
    /// <param name="tableName">The name of the table to render. Cannot be null or empty.</param>
    /// <returns>A string containing the rendered representation of the specified table.</returns>
    string RenderTable(SchemaName? schemaName, TableName tableName);

    /// <summary>
    /// Renders a fully qualified column name for use in SQL statements.
    /// </summary>
    /// <remarks>If <paramref name="schema"/> is provided, the output includes the schema and table names. If
    /// <paramref name="includeTable"/> is false, only the column name is rendered.</remarks>
    /// <param name="schema">The optional schema name that qualifies the table. May be null to omit the schema.</param>
    /// <param name="table">The name of the table containing the column. Cannot be null or empty.</param>
    /// <param name="includeTable">true to include the table name in the rendered output; otherwise, false.</param>
    /// <returns>A string containing the rendered column name, optionally qualified by table and schema as specified.</returns>
    string RenderColumn(TableTag tableTag, ColumnName columnName, bool includeTable = true);


    /// <summary>
    /// Generates the SQL fragments required to perform an INSERT statement with a RETURNING clause for the specified
    /// columns.
    /// </summary>
    /// <typeparam name="T">The type of the entity being inserted. Used to determine the context for the generated SQL fragments.</typeparam>
    /// <param name="insertIntoFragments">The SQL fragments representing the INSERT INTO clause, including the target table and columns.</param>
    /// <param name="insertValuesFragments">The SQL fragments representing the VALUES clause for the INSERT statement.</param>
    /// <param name="columnInfo">A collection of column metadata specifying which columns should be included in the RETURNING clause.</param>
    /// <returns>An enumerable collection of SQL fragments that, when combined, form a complete INSERT statement with a RETURNING
    /// clause for the specified columns.</returns>
    IEnumerable<ISqlFragment> GetInsertReturningFragments<T>(IEnumerable<ISqlFragment> insertIntoFragments, IEnumerable<ISqlFragment> insertValuesFragments, IEnumerable<ColumnInfo> columnInfo);

    /// <summary>
    /// Generates the final parameter name a dialect specific parameter delimiter, parameter index and base name if applicable in the dialect.
    /// </summary>
    /// <param name="baseParameterName">The base name to use for the parameter. Cannot be null or empty.</param>
    /// <param name="parameterIndex">The index to append to the base parameter name. Must be zero or greater.</param>
    /// <returns>A string representing the final parameter name, formed by combining the base name and the index.</returns>
    string RenderFinalParameterName(string baseParameterName, int parameterIndex);

    /// <summary>
    /// Generates a paging clause for a SQL query based on the specified offset and fetch next values aka limit offset.
    /// </summary>
    /// <param name="paging">The <see cref="PagingBase"/> object containing the offset and fetch next values aka limit and offset values.</param>
    /// <returns>A string containing the SQL paging clause representing the specified offset and fetch next values aka limit and offset.</returns>
    ISqlFragment RenderPaging(PagingBase paging);

    /// <summary>
    /// Returns the default <see cref="FieldProperties"/> for a given CLR type according to the SQL dialect's type mapping rules.
    /// </summary>
    /// <param name="type">The CLR type for which to retrieve the default field properties.</param>
    /// <returns>The default <see cref="FieldProperties"/> for the specified CLR type.</returns>
    FieldProperties GetDefaultFieldPropertiesByClrType(Type type);

    /// <summary>
    /// Renders the SQL declaration for a field based on the provided <see cref="FieldProperties"/>. This includes the provider type name,
    /// </summary>
    /// <param name="fieldProperties">
    /// The <see cref="FieldProperties"/> containing the necessary information to generate the SQL declaration for the field, such as provider type name,
    /// </param>
    /// <returns>
    /// A string representing the SQL declaration for the field, formatted according to the rules of the SQL dialect. This may include the provider type name,
    /// </returns>
    ///
    string RenderFieldProperties(FieldProperties fieldProperties);

    /// <summary>
    /// Performs the necessary conversions for a parameter value
    /// before it is passed to the database.
    /// </summary>
    /// <param name="value">
    /// The value to convert. A <c>null</c> value is converted to
    /// <see cref="DBNull.Value"/>.
    /// </param>
    /// <returns>
    /// The converted value suitable for database operations.
    /// </returns>
    object ValueConversion(object? value);

    /// <summary>
    /// Returns the character used by the SQL dialect to represent Xor operations, such as in WHERE clauses.
    /// </summary>
    /// <returns>The SQL fragment used to render the dialect-specific XOR operator.</returns>
    ISqlFragment GetXOrSymbol();

    /// <summary>
    /// Generates the appropriate SQL fragment for a LIKE operator in the SQL dialect, optionally considering case sensitivity.
    /// </summary>
    /// <param name="isCaseSensitive">
    /// Indicates whether the LIKE operation should be case-sensitive.
    /// If null, the default behavior of the dialect is used.
    /// </param>
    /// <returns>A <see cref="ISqlFragment"/> representing the LIKE operation in the SQL dialect.</returns>
    ISqlFragment GetDialectLike(bool? isCaseSensitive = null);

    /// <summary>
    /// Indicates whether the SQL dialect supports fully qualified sets in UPDATE statements.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the SQL dialect supports fully qualified sets in UPDATE statements; otherwise, <see langword="false"/>.
    /// </returns>
    public bool DoesUpdateSupportsFullyQualifiedSets();

    /// <summary>
    /// Normalizes a <see cref="DateTimeOffset"/> value to the appropriate time zone for the SQL dialect, if necessary.
    /// </summary>
    /// <param name="dateTimeOffset">
    /// The <see cref="DateTimeOffset"/> value to normalize. This may involve converting the value to a specific time zone or adjusting it
    /// according to the rules of the SQL dialect. If the dialect does not require normalization, the original value is returned unchanged.
    /// </param>
    /// <returns>The normalized <see cref="DateTimeOffset"/> value, or the original value when the dialect does not require conversion.</returns>
    public DateTimeOffset? NormalizeTimeZone(DateTimeOffset? dateTimeOffset);

    /// <summary>
    /// Normalizes a <see cref="DateTime"/> value to the appropriate time zone for the SQL dialect, if necessary. This may involve converting
    /// the value to a specific time zone or adjusting it according to the rules of the SQL dialect. If the dialect does not require normalization,
    /// the original value is returned unchanged.
    /// </summary>
    /// <param name="dateTime">
    /// The <see cref="DateTime"/> value to normalize. This may involve converting the value to a specific time zone or adjusting it according to
    /// the rules of the SQL dialect.
    /// </param>
    /// <returns>
    /// The normalized <see cref="DateTime"/> value suitable for database operations according to the SQL dialect's time zone handling rules.
    /// If the dialect does not require normalization, the original value is returned unchanged.
    /// </returns>
    public DateTime? NormalizeTimeZone(DateTime? dateTime);

    /// <summary>
    /// Returns a set of CLR types that are supported by the SQL dialect for parameterization and field mapping. This set may include common types such as
    /// <see cref="int"/>, <see cref="string"/>, <see cref="DateTime"/>, <see cref="DateTimeOffset"/>, and others, depending on the capabilities of the SQL dialect.
    /// </summary>
    /// <returns>
    /// A <see cref="HashSet{Type}"/> containing the CLR types that are supported by the SQL dialect for parameterization and field mapping. This set is used to
    /// determine which types can be directly mapped to SQL types and may influence how parameters are rendered and how field properties are determined for those types.
    /// </returns>
    public HashSet<Type> SupportedTypes();

    /// <summary>
    /// Determines the appropriate <see cref="FieldProperties"/> for a given CLR value based on the SQL dialect's type mapping rules. 
    /// </summary>
    /// <param name="value">
    /// The CLR value for which to determine the corresponding <see cref="FieldProperties"/>. 
    /// The method will analyze the type of the value and return the appropriate type.
    /// </param>
    /// <returns>
    /// A <see cref="FieldProperties"/> instance containing the properties that correspond to the CLR value according to the SQL dialect's type mapping rules.
    /// </returns>
    public FieldProperties FromClrValue(object? value);
}
