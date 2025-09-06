using System.Data;

namespace SqlTools.SqlGenerators;

public class SqlQuery
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public SqlQuery()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public SqlQuery(string query, Dictionary<string, object> parameters)
    {
        QueryText = query;
        Parameters = parameters;
    }

    public string QueryText { get; set; }
    public Dictionary<string, object> Parameters { get; set; }

    public CommandType CommandType { get; set; }
}
