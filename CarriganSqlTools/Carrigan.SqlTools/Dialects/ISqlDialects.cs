using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Carrigan.SqlTools.Dialects;

/// <summary>
/// This is the ISqlDialects interface, which defines the methods that a SQL dialect must implement. 
/// It includes methods for quoting identifiers, rendering table and column names, rendering parameters, 
/// rendering paging, and rendering insert statements with returning clauses. Each method takes the necessary.
/// The current implementation of this interface is the SqlServerDialect class, which implements the methods for the SQL Server dialect.
/// The current interface is just a best guess at the methods that will be needed, and may need to be updated as the library is developed further.
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
    /// Generates a string representation of the specified database table, optionally within a given schema.
    /// </summary>
    /// <remarks>If <paramref name="schema"/> is provided, the output includes the schema and table names. If
    /// <paramref name="includeTable"/> is false, only the column name is rendered.</remarks>
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
    IEnumerable<SqlFragment> GetInsertReturningFragments<T>(IEnumerable<SqlFragment> insertIntoFragments, IEnumerable<SqlFragment> insertValuesFragments, IEnumerable<ColumnInfo> columnInfo);

    /// <summary>
    /// Generates the final parameter name a dialect specific parameter delimiter, parameter index and base name if applicable in the dialect.
    /// </summary>
    /// <param name="baseParameterName">The base name to use for the parameter. Cannot be null or empty.</param>
    /// <param name="parameterIndex">The index to append to the base parameter name. Must be zero or greater.</param>
    /// <returns>A string representing the final parameter name, formed by combining the base name and the index.</returns>
    string RenderFinalParameterName(string baseParameterName, int parameterIndex);
    /// <summary>
    /// Generates a paging clause for a SQL query based on the specified offset and fetch values.
    /// </summary>
    /// <param name="offset">The number of rows to skip before starting to return rows. Must be greater than or equal to 0.</param>
    /// <param name="fetch">The maximum number of rows to return. Must be greater than 0.</param>
    /// <returns>A string containing the SQL paging clause representing the specified offset and fetch values.</returns>
    string RenderPaging(int offset, int fetch);

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

    SqlQuery RenderSqlQuery(IEnumerable<SqlFragment> sqlFragments);

    SqlQuery RenderStoredProcedureQuery(IEnumerable<SqlFragmentParameter> sqlFragments, ProcedureTag procedureTag);


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
    public object ValueConversion(object? value);
}
