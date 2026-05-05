using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.PredicatesLogic;
using Carrigan.SqlTools.SqlGenerators;
using Carrigan.SqlTools.Tags;
using System.Data;

namespace Carrigan.SqlTools.Dialects.SqlServer;

/// <summary>
/// Represents a parameterized SQL command, including the command text,
/// parameters, and the ADO.NET <see cref="CommandType"/>.
/// DON'T FORGET TO PARAMETERIZE YOUR SQL TO MITIGATE SQL INJECTION
/// </summary>
/// <remarks>
/// Intentionally left public to allow manual SQL.
/// Use at your own risk, and DON'T FORGET TO PARAMETERIZE YOUR SQL TO MITIGATE SQL INJECTION.
/// </remarks>
public class SqlServerQuery : SqlQuery
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServerQuery"/> class with the specified
    /// </summary>
    /// <param name="dialect">The SQL dialect to use for rendering the fragments.</param>
    /// <param name="fragments">The sequence of SQL fragments to render.</param>
    internal SqlServerQuery(ISqlDialects dialect, IEnumerable<SqlFragment> fragments)
    {
        ArgumentNullException.ThrowIfNull(fragments);
        ArgumentNullException.ThrowIfNull(dialect);

        QueryText = fragments.ToSql(dialect);
        Parameters = fragments.GetSqlFragmentParameters(dialect);
        CommandType = CommandType.Text;
        ParametersAsDictionary = new
        (
            Parameters
                .Select(parameter => new KeyValuePair<ParameterTag, object?>(parameter.ParameterTag, parameter.Value))
        );
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServerQuery"/> class for a stored procedure with the specified
    /// </summary>
    /// <param name="dialect">The SQL dialect to use for rendering the fragments.</param>
    /// <param name="fragments">The sequence of SQL fragments representing the parameters.</param>
    /// <param name="procedure">The stored procedure to execute.</param>
    internal SqlServerQuery(ISqlDialects dialect, IEnumerable<SqlFragmentParameter> fragments, ProcedureTag procedure)
    {
        ArgumentNullException.ThrowIfNull(procedure);
        ArgumentNullException.ThrowIfNull(dialect);
        ArgumentNullException.ThrowIfNull(fragments);
        QueryText = procedure;
        Parameters = fragments.GetSqlFragmentParameters(dialect);
        ParametersAsDictionary = new
        (
            Parameters 
                .Select(parameter => new KeyValuePair<ParameterTag, object?>(parameter.ParameterTag, parameter.Value))
        );
        CommandType = CommandType.StoredProcedure;
    }
}
