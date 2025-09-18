using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.Exceptions;

public class SqlNamePatternException : Exception
{
    /// <summary>
    /// Constructs an exception for one or more invalid SQL identifiers provided as ColumnTag instances.
    /// </summary>
    /// <param name="invalidColumns">A collection of invalid ColumnTag values.</param>
    public SqlNamePatternException(IEnumerable<ColumnTag> invalidColumns)
        : base(CreateMessage(invalidColumns))
    {
    }

    /// <summary>
    /// Constructs an exception for one or more invalid column names for a specific table.
    /// </summary>
    /// <param name="tableTag">The TableTag for the table in question.</param>
    /// <param name="invalidColumnNames">A collection of invalid column names.</param>
    private SqlNamePatternException(TableTag tableTag, params IEnumerable<string> invalidColumnNames)
        : base(CreateMessage(tableTag, invalidColumnNames))
    {
    }

    /// <summary>
    /// Constructs an exception for one or more invalid table.
    /// </summary>
    /// <param name="invalidTables">A collection of invalid table names.</param>
    public SqlNamePatternException(params IEnumerable<TableTag> invalidTables)
        : base(CreateMessage(invalidTables))
    {
    }

    /// <summary>
    /// Constructs an exception for one or more invalid procedure identifiers.
    /// </summary>
    /// <param name="invalidProcedures">A collection of invalid procedure names.</param>
    public SqlNamePatternException(params IEnumerable<ProcedureTag> invalidProcedures)
        : base(CreateMessage(invalidProcedures))
    {
    }

    /// <summary>
    /// Constructs an exception for one or more invalid role names.
    /// </summary>
    /// <param name="invalidRoles">A collection of invalid role names.</param>
    public SqlNamePatternException(params IEnumerable<RoleTag> invalidRoles)
        : base(CreateMessage(invalidRoles))
    {
    }

    /// <summary>
    /// Constructs an exception for one or more invalid table.
    /// </summary>
    /// <param name="invalidIdentifiers">A collection of invalid column names.</param>
    public SqlNamePatternException(params IEnumerable<string> invalidIdentifiers)
        : base(CreateMessage(invalidIdentifiers))
    {
    }

    /// <summary>
    /// A helper method to create an exception using the table tag derived from ReflectorCache&lt;T&gt;.
    /// </summary>
    /// <typeparam name="T">A type whose associated TableTag is obtained from ReflectorCache&lt;T&gt;.TableTag.</typeparam>
    /// <param name="invalidColumnNames">A collection of invalid column names.</param>
    /// <returns>An InvalidSqlIdentifier exception instance.</returns>
    public static SqlNamePatternException FromInvalidColumnNames<T>(params IEnumerable<string> invalidColumnNames)
    {
        TableTag tableTag = SqlToolsReflectorCache<T>.Table;
        return new SqlNamePatternException(tableTag, invalidColumnNames);
    }

    // Builds the exception message from a collection of ColumnTag values.
    private static string CreateMessage(IEnumerable<ColumnTag> invalidColumns)
    {
        IEnumerable<string> columnStrings = invalidColumns.Select(column => column.ToString());
        string columns = columnStrings.JoinAnd();
        return $"The following column names do not follow the SQL naming convention: {columns}";
    }

    // Builds the exception message for a given table and invalid column names.
    private static string CreateMessage(TableTag tableTag, IEnumerable<string> invalidColumnNames)
    {
        IEnumerable<string> columnStrings = invalidColumnNames.Select(column => tableTag.ToString() + ".[" + column + "]");
        string columns = columnStrings.JoinAnd();
        return $"The following column names for the table, {tableTag}, do not follow the SQL naming convention: {columns}";
    }

    // Builds the exception message from a collection of TableTag values.
    private static string CreateMessage(IEnumerable<TableTag> invalidColumns)
    {
        IEnumerable<string> tableStrings = invalidColumns.Select(table => table.ToString());
        string tables = tableStrings.JoinAnd();
        return $"The following table names do not follow the SQL naming convention: {tables}";
    }

    // Builds the exception message from a collection of ProcedureTag values.
    private static string CreateMessage(IEnumerable<ProcedureTag> invalidColumns)
    {
        IEnumerable<string> tableStrings = invalidColumns.Select(table => table.ToString());
        string tables = tableStrings.JoinAnd();
        return $"The following procedure names do not follow the SQL naming convention: {tables}";
    }

    // Builds the exception message from a collection of ColumnTag values.
    private static string CreateMessage(IEnumerable<RoleTag> invalidRoles)
    {
        IEnumerable<string> rolesStrings = invalidRoles.Select(role => role.ToString());
        string roles = rolesStrings.JoinAnd();
        return $"The following role names do not follow the SQL naming convention: {roles}";
    }

    // Builds the exception message from a collection of unspecified sql Identifier values. invalidRoles
    private static string CreateMessage(IEnumerable<string> invalidRoles)
    {
        IEnumerable<string> rolesStrings = invalidRoles.Select(role => role.ToString());
        string roles = rolesStrings.JoinAnd();
        return $"The following sql identifiers do not follow the SQL naming convention: {roles}";
    }
}
