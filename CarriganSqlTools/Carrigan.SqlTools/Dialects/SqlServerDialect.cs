using Carrigan.Core.Extensions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.ReflectorCache;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using System.Reflection.Emit;
using System.Text;

namespace Carrigan.SqlTools.Dialects;

/// <summary>
/// Provides SQL dialect-specific formatting and rendering logic for Microsoft SQL Server.
/// </summary>
/// <remarks>Implements the ISqlDialects interface to generate SQL fragments and identifiers according to SQL
/// Server syntax rules. This class is typically used by database abstraction layers or query builders that need to
/// support multiple SQL dialects.</remarks>
public class SqlServerDialect : ISqlDialects
{
    /// <summary>
    /// Initializes a new instance of the SqlServerDialect class.
    /// </summary>
    public SqlServerDialect()
    {
    }
    /// <summary>
    /// Encloses the specified identifier in delimiters appropriate for use in a SQL statement, ensuring that reserved
    /// words or special characters are handled correctly.
    /// </summary>
    /// <param name="identifier">The database identifier to be quoted. This can be a table name, column name, or other SQL identifier. Cannot be
    /// null.</param>
    /// <returns>A string containing the quoted identifier, suitable for safe inclusion in a SQL statement.</returns>
    /// <exception cref="NotImplementedException">Thrown in all cases as this method is not yet implemented.</exception>
    public string QuoteIdentifier(string identifier) => 
        $"[{identifier}]";
    /// <summary>
    /// Generates a string representation of the specified database table, optionally qualified by schema.
    /// </summary>
    /// <param name="schemaName">The name of the schema to which the table belongs, or null to omit the schema from the rendered output.</param>
    /// <param name="tableName">The name of the table to render. Cannot be null or empty.</param>
    /// <returns>A string containing the rendered representation of the specified table.</returns>
    /// <exception cref="NotImplementedException">Thrown in all cases as the method is not implemented.</exception>
    public string RenderTable(SchemaName? schemaName, TableName tableName) =>
        schemaName.IsNotNullOrEmpty()
            ? $"{QuoteIdentifier(schemaName)}.{QuoteIdentifier(tableName)}"
            : QuoteIdentifier(tableName);
    /// <summary>
    /// Renders the fully qualified name of a database column, optionally including the table name and schema.
    /// </summary>
    /// <param name="schema">The schema name of the table containing the column, or null to omit the schema.</param>
    /// <param name="table">The name of the table containing the column. Cannot be null or empty.</param>
    /// <param name="includeTable">true to include the table name in the rendered output; otherwise, false.</param>
    /// <returns>A string representing the fully qualified column name, formatted according to the specified parameters.</returns>
    /// <exception cref="NotImplementedException">Thrown in all cases as the method is not implemented.</exception>
    public string RenderColumn(TableTag tableTag, ColumnName columnName, bool includeTable = true) =>
        includeTable && tableTag.ToString().IsNotNullOrEmpty()
                ? $"{tableTag}.{QuoteIdentifier(columnName)}"
                : QuoteIdentifier(columnName);

    /// <summary>
    /// Renders an INSERT statement that captures inserted values for the specified columns using SQL Server's OUTPUT clause.
    /// </summary>
    /// <typeparam name="T">The type of the entity being inserted, used to determine the table and column information for rendering the SQL statement.</typeparam>
    /// <param name="insertIntoClause">The INSERT INTO clause of the SQL statement.</param>
    /// <param name="insertValuesClause">The VALUES clause of the SQL statement.</param>
    /// <param name="columnInfo">The collection of columns for which to capture inserted values.</param>
    /// <returns>A string containing the complete INSERT statement with OUTPUT clause.</returns>
    public string RenderInsertReturning<T>(string insertIntoClause, string insertValuesClause, IEnumerable<ColumnInfo> columnInfo)
    {
        StringBuilder queryBuilder = new();
        queryBuilder.AppendLine(ReturnTableDefinition(columnInfo));
        queryBuilder.AppendLine(insertIntoClause);
        queryBuilder.AppendLine(ReturnOutputColumns(columnInfo));
        queryBuilder.AppendLine(insertValuesClause);
        queryBuilder.AppendLine(ReturnSelectOutput<T>(columnInfo));
        return queryBuilder.ToString();
    }

    /// <summary>
    /// Generates a paging clause for a SQL query based on the specified offset and fetch values.
    /// </summary>
    /// <param name="offset">The number of rows to skip before starting to return rows. Must be greater than or equal to 0.</param>
    /// <param name="fetch">The maximum number of rows to return. Must be greater than 0.</param>
    /// <returns>A string containing the SQL paging clause corresponding to the specified offset and fetch values.</returns>
    /// <exception cref="NotImplementedException">Thrown in all cases as the method is not implemented.</exception>
    public string RenderPaging(int offset, int fetch) => throw new NotImplementedException();
    /// <summary>
    /// Generates a parameter string using the specified prefix, name, and optional index.
    /// </summary>
    /// <param name="prefix">An optional prefix to prepend to the parameter name. May be null or empty if no prefix is required.</param>
    /// <param name="name">The name of the parameter to render. Cannot be null or empty.</param>
    /// <param name="index">An optional index to append to the parameter name. May be null or empty if no index is required.</param>
    /// <returns>A string representing the rendered parameter, including the prefix and index if provided.</returns>
    /// <exception cref="NotImplementedException">Thrown in all cases as this method is not yet implemented.</exception>
    public string RenderParameter(string? prefix, string name, string? index) => throw new NotImplementedException();



    /// <summary>
    /// Generates SQL to declare an output table used to capture inserted values
    /// for the columns specified by <paramref name="columnInfo"/>.
    /// </summary>
    /// <param name="columnInfo">
    /// Columns for which the inserted values should be captured.
    /// </param>
    /// <returns>
    /// A SQL statement that declares <c>@OutputTable</c> with one column per entry in <paramref name="columnInfo"/>.
    /// </returns>
    private static string ReturnTableDefinition(IEnumerable<ColumnInfo> columnInfo) =>
        $"DECLARE @OutputTable TABLE ({string.Join(", ", columnInfo.Select(column => $"{column.ColumnName} {column.SqlType.TypeDeclaration}"))});";


    /// <summary>
    /// Generates SQL to output the inserted column values into <c>@OutputTable</c>.
    /// </summary>
    /// <param name="columnInfo">
    /// Columns for which the inserted values should be captured.
    /// </param>
    /// <returns>
    /// A SQL <c>OUTPUT</c> clause that writes the specified inserted columns into <c>@OutputTable</c>.
    /// </returns>
    private static string ReturnOutputColumns(IEnumerable<ColumnInfo> columnInfo) =>
       $"OUTPUT {string.Join(", ", columnInfo.Select(column => $"INSERTED.{column.ColumnName}"))} INTO @OutputTable";


    /// <summary>
    /// Gets the column expression to use when returning values inserted for the column
    /// described by <paramref name="columnInfo"/>.
    /// </summary>
    /// <remarks>
    /// Because the caller will expect column names as they would appear in a <c>SELECT</c>,
    /// this method ensures that the final projection from <c>@OutputTable</c> uses the
    /// invoker’s expected column name, applying an alias when necessary.
    /// </remarks>
    /// <param name="columnInfo">
    /// The column for which the returned column name should be resolved.
    /// </param>
    /// <returns>
    /// A string containing either the raw column name or a <c>ColumnName AS ResultName</c> expression.
    /// </returns>
    private static string ReturnSelectName<T>(ColumnInfo columnInfo)
    {
        string resultColumnName = InvocationReflectorCache<T>.GetResultColumnName(columnInfo.PropertyInfo);
        if (resultColumnName != columnInfo.ColumnName)
            return $"{columnInfo.ColumnName} AS {resultColumnName}";
        else
            return columnInfo.ColumnName;
    }

    /// <summary>
    /// Generates a <c>SELECT</c> statement that reads the captured values from <c>@OutputTable</c>.
    /// </summary>
    /// <param name="columnInfo">
    /// Columns for which the inserted values were captured.
    /// </param>
    /// <returns>
    /// A SQL <c>SELECT</c> statement projecting the requested columns from <c>@OutputTable</c>.
    /// </returns>
    private static string ReturnSelectOutput<T>(IEnumerable<ColumnInfo> columnInfo) =>
        $"SELECT {string.Join(", ", columnInfo.Select(column => ReturnSelectName<T>(column)))} FROM @OutputTable;";
}
