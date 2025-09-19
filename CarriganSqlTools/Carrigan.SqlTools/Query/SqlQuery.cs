using Carrigan.SqlTools.Predicates;
using Carrigan.SqlTools.Tags;
using System.Collections.Immutable;
using System.Data;

namespace Carrigan.SqlTools.Query;

public class SqlQuery
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    internal SqlQuery()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    internal SqlQuery(string query, Dictionary<ParameterTag, object> parameters, CommandType commandType = CommandType.Text)
    {
        QueryText = query;
        Parameters = ImmutableDictionary.CreateRange(parameters.Keys.Select(key => new KeyValuePair<ParameterTag, object>(key, parameters[key])));
        CommandType = commandType;
    }

    public string QueryText { get; internal set; }

    public ImmutableDictionary<ParameterTag, object> Parameters { get; internal set; }

    public CommandType CommandType { get; internal set; }

    /// <summary>
    /// Gets the value of a parameter. This is intended for unit testing only.
    /// </summary>
    internal T GetParameterValue<T>(string parameterTestName) =>
        (T)Parameters.Where(param => param.Key == parameterTestName).Single().Value;

    /// <summary>
    /// Gets the value number of parameters. This is intended for unit testing only.
    /// </summary>
    internal int GetParameterCount() =>
        Parameters.Count;
}
