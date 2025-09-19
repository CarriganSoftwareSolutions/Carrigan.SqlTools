using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Tags;
using System.Data;

namespace Carrigan.SqlTools.SqlGenerators;

public class SqlQuery
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public SqlQuery()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public SqlQuery(string query, Dictionary<ParameterTag, object> parameters)
    {
        QueryText = query;
        Parameters = parameters;
    }

    public string QueryText { get; set; }
    public Dictionary<ParameterTag, object> Parameters { get; set; }

    public CommandType CommandType { get; set; }

    /// <summary>
    /// Gets the value of a parameter. This is intended for testing only.
    /// </summary>
    internal T GetParameterValue<T>(string parameterTestName) =>
        (T)Parameters.Where(param => param.Key == parameterTestName).Single().Value;
}
