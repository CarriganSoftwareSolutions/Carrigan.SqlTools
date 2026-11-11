using Carrigan.Core.Extensions;
using Carrigan.SqlTools.IdentifierTypes;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Dialects;

/// <summary>
/// Exposes default <see cref="ISqlDialects"/> rendering methods to callers using concrete dialect types.
/// Each extension delegates to the interface implementation so dialect-specific overrides are respected.
/// </summary>
public static class ISqlDialectsExtensions
{
    /// <summary>
    /// Generates a string representation of the specified database procedure, optionally within a given schema.
    /// </summary>
    /// <param name="dialect">
    /// The dialect to use for rendering the table alias.
    /// </param>
    /// <param name="procedure">The procedure tag to render. Cannot be null or empty.</param>
    /// <returns>A string containing the rendered representation of the specified procedure.</returns>
    public static string RenderProcedureTag(this ISqlDialects dialect, ProcedureTag procedure) =>
        procedure.SchemaName.IsNotNullOrEmpty()
            ? $"{dialect.QuoteIdentifier(procedure.SchemaName.ToString())}.{dialect.QuoteIdentifier(procedure.ProcedureName.ToString())}"
            : dialect.QuoteIdentifier(procedure.ProcedureName.ToString());

    /// <summary>
    /// Generates a string representation of the specified database table alias
    /// </summary>
    /// <param name="dialect">
    /// The dialect to use for rendering the table alias.
    /// </param>
    /// <param name="tableAlias">The alias tag for the table. May be null to omit the alias.</param>
    /// <returns>A string containing the rendered representation of the specified table alias.</returns>
    public static string RenderTableAlias(this ISqlDialects dialect, TableAlias? tableAlias) =>
        tableAlias.IsNotNullOrEmpty()
                ? dialect.QuoteIdentifier(tableAlias.ToString())
                : string.Empty;

    /// <summary>
    /// Generates a string representation of the specified database column alias
    /// </summary>
    /// <param name="dialect">
    /// The dialect to use for rendering the table alias.
    /// </param>
    /// <param name="columnAlias">The alias tag for the column. May be null to omit the alias.</param>
    /// <returns>A string containing the rendered representation of the specified column alias.</returns>
    public static string RenderColumnAlias(this ISqlDialects dialect, ColumnAlias? columnAlias) =>
        columnAlias.IsNotNullOrEmpty()
                ? dialect.QuoteIdentifier(columnAlias.ToString())
                : string.Empty;

    /// <summary>
    /// Renders the fully qualified name of a database column, optionally including the table name and schema.
    /// </summary>
    /// <param name="dialect">
    /// The dialect to use for rendering the table alias.
    /// </param>
    /// <param name="includeTable">true to include the table name in the rendered output; otherwise, false.</param>
    /// <param name="tableTag">The tableTag value.</param>
    /// <param name="columnName">The SQL column name to apply.</param>
    /// <returns>A string representing the fully qualified column name, formatted according to the specified parameters.</returns>
    public static string RenderColumn(this ISqlDialects dialect, TableTag? tableTag, ColumnName columnName, bool includeTable = true) =>
        includeTable && (tableTag?.ToString()).IsNotNullOrEmpty()
                ? $"{tableTag.ToSql(dialect)}.{dialect.QuoteIdentifier(columnName.ToString())}"
                : dialect.QuoteIdentifier(columnName.ToString());

    /// <summary>
    /// Generates a string representation of the specified database table, optionally qualified by schema.
    /// </summary>
    /// <param name="dialect">
    /// The dialect to use for rendering the table alias.
    /// </param>
    /// <param name="schemaName">The name of the schema to which the table belongs, or null to omit the schema from the rendered output.</param>
    /// <param name="tableName">The name of the table to render. Cannot be null or empty.</param>
    /// <returns>A string containing the rendered representation of the specified table.</returns>
    public static string RenderTable(this ISqlDialects dialect, SchemaName? schemaName, TableName tableName) =>
        schemaName.IsNotNullOrEmpty()
            ? $"{dialect.QuoteIdentifier(schemaName!.ToString())}.{dialect.QuoteIdentifier(tableName.ToString())}"
            : dialect.QuoteIdentifier(tableName.ToString());
}
