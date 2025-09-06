using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Exceptions;

public class SqlIdentifierException : Exception
{
    /// <summary>
    /// Constructs an exception for one or more invalid SQL identifiers provided as ColumnTag instances.
    /// </summary>
    /// <param name="invalidColumns">A collection of invalid ColumnTag values.</param>
    public SqlIdentifierException(IEnumerable<ColumnTag> invalidColumns)
        : base(CreateMessage(invalidColumns))
    {
    }

    /// <summary>
    /// Constructs an exception for one or more invalid column names for a specific table.
    /// </summary>
    /// <param name="tableTag">The TableTag for the table in question.</param>
    /// <param name="invalidColumnNames">A collection of invalid column names.</param>
    private SqlIdentifierException(TableTag tableTag, params IEnumerable<string> invalidColumnNames)
        : base(CreateMessage(tableTag, invalidColumnNames))
    {
    }

    /// <summary>
    /// Constructs an exception for one or more invalid table.
    /// </summary>
    /// <param name="invalidTables">A collection of invalid column names.</param>
    public SqlIdentifierException(params IEnumerable<TableTag> invalidTables)
        : base(CreateMessage(invalidTables))
    {
    }

    /// <summary>
    /// Constructs an exception for one or more invalid table.
    /// </summary>
    /// <param name="invalidTables">A collection of invalid column names.</param>
    public SqlIdentifierException(IEnumerable<RoleTag> invalidRoles)
        : base(CreateMessage(invalidRoles))
    {
    }

    /// <summary>
    /// A helper method to create an exception using the table tag derived from ReflectorCache&lt;T&gt;.
    /// </summary>
    /// <typeparam name="T">A type whose associated TableTag is obtained from ReflectorCache&lt;T&gt;.TableTag.</typeparam>
    /// <param name="invalidColumnNames">A collection of invalid column names.</param>
    /// <returns>An InvalidSqlIdentifier exception instance.</returns>
    public static SqlIdentifierException FromInvalidColumnNames<T>(params IEnumerable<string> invalidColumnNames)
    {
        TableTag tableTag = SqlToolsReflectorCache<T>.TableTag;
        return new SqlIdentifierException(tableTag, invalidColumnNames);
    }

    // Builds the exception message from a collection of ColumnTag values.
    private static string CreateMessage(IEnumerable<ColumnTag> invalidColumns)
    {
        IEnumerable<string> columnStrings = invalidColumns.Select(column => column.ToString());
        string columns = columnStrings.JoinAnd();
        return "Invalid SQL identifier(s): " + columns;
    }

    // Builds the exception message for a given table and invalid column names.
    private static string CreateMessage(TableTag tableTag, IEnumerable<string> invalidColumnNames)
    {
        IEnumerable<string> columnStrings = invalidColumnNames.Select(column => tableTag.ToString() + ".[" + column + "]");
        string columns = columnStrings.JoinAnd();
        return "Invalid SQL identifier(s) for table " + tableTag.ToString() + ": " + columns;
    }

    // Builds the exception message from a collection of ColumnTag values.
    private static string CreateMessage(IEnumerable<TableTag> invalidColumns)
    {
        IEnumerable<string> tableStrings = invalidColumns.Select(table => table.ToString());
        string tables = tableStrings.JoinAnd();
        return "Invalid SQL identifier(s): " + tables;
    }

    // Builds the exception message from a collection of ColumnTag values.
    private static string CreateMessage(IEnumerable<RoleTag> inavlidRoles)
    {
        IEnumerable<string> rolesStrings = inavlidRoles.Select(role => role.ToString());
        string roles = rolesStrings.JoinAnd();
        return "Invalid SQL identifier(s): " + roles;
    }
}
