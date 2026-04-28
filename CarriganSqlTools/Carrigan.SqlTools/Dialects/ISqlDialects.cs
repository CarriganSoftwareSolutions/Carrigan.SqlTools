using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

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
    /// <param name="schema">The optional schema name that qualifies the table. May be null to omit the schema.</param>
    /// <param name="table">The name of the table to render. Cannot be null or empty.</param>
    /// <returns>A string containing the rendered representation of the specified table.</returns>
    string RenderTable(string? schema, string table);
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
    /// Generates a parameter string by combining the specified prefix, name, and optional index.
    /// </summary>
    /// <param name="prefix">An optional string to prepend to the parameter name. May be null or empty if no prefix is required.</param>
    /// <param name="name">The name of the parameter to render. Cannot be null or empty.</param>
    /// <param name="index">An optional index to append to the parameter name. May be null or empty if no index is required.</param>
    /// <returns>A string representing the rendered parameter, including the prefix and index if provided.</returns>
    string RenderParameter(string? prefix, string name, string? index);
    /// <summary>
    /// Generates a paging clause for a SQL query based on the specified offset and fetch values.
    /// </summary>
    /// <param name="offset">The number of rows to skip before starting to return rows. Must be greater than or equal to 0.</param>
    /// <param name="fetch">The maximum number of rows to return. Must be greater than 0.</param>
    /// <returns>A string containing the SQL paging clause representing the specified offset and fetch values.</returns>
    string RenderPaging(int offset, int fetch);
    /// <summary>
    /// Generates a SQL statement that appends a RETURNING clause to the specified INSERT statement, allowing retrieval
    /// of specified columns after insertion.
    /// </summary>
    /// <param name="insertSql">The base SQL INSERT statement to which the RETURNING clause will be appended. Must be a valid INSERT statement.</param>
    /// <param name="returnColumns">A read-only list of tuples specifying the columns to return. Each tuple contains the column name, its SQL type
    /// declaration, and an optional alias to use in the RETURNING clause.</param>
    /// <returns>A SQL string representing the original INSERT statement with an appended RETURNING clause that returns the
    /// specified columns.</returns>
    string RenderInsertReturning(string insertSql, IReadOnlyList<(string ColumnName, string TypeDeclaration, string? Alias)> returnColumns);
}
