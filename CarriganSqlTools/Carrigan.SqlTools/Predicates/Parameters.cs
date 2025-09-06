namespace SqlTools.Predicates;

public class Parameters : PredicatesBase
{
    internal readonly object? Value;
    internal readonly string Name;

    public Parameters(string parameter, object? value)
    {
        if (parameter is null)
        {
            throw new ArgumentNullException(nameof(parameter), $"A value for {nameof(parameter)} is required.");
        }
        else if (parameter == string.Empty)
        {
            throw new ArgumentException($"A value for {nameof(parameter)} is required.", nameof(parameter));
        }
        else if(parameter.AsEnumerable().Where(ch => ch == '@' || ch == '$' || ch == '#').Any())
        {
            throw new ArgumentException($"Special character exists in {nameof(parameter)} name of SQL \"{parameter}\"", nameof(parameter));
        }
        else if(parameter.AsEnumerable().Where(ch => !(char.IsLetter(ch) || char.IsNumber(ch) || ch == '_')).Any())
        {
            throw new ArgumentException($"Invalid character exists in {nameof(parameter)} name of SQL \"{parameter}\"", nameof(parameter));
        }
        else if (parameter.AsEnumerable().Where(ch => char.IsWhiteSpace(ch)).Any())
        {
            throw new ArgumentException($"White space character exists in {nameof(parameter)} name of SQL \"{parameter}\"", nameof(parameter));
        }
        Name = parameter;
        Value = value;
    }

    internal override IEnumerable<Parameters> Parameter =>
       [this];

    internal override IEnumerable<IColumnValue> Column =>
       [];

    internal override string ToSql(string prefix, IEnumerable<string> duplicates) =>
        GetFinalParameterName(prefix, duplicates);
    internal override IEnumerable<KeyValuePair<string, object>> GetParameters(string prefix, IEnumerable<string> duplicates)
    {
        string key = GetFinalParameterName(prefix, duplicates);
        object value = Value ?? DBNull.Value;
        KeyValuePair<string, object> keyValuePair = new(key, value);
        return (new KeyValuePair<string, object>[] { keyValuePair }).AsEnumerable();
    }

    private string GetFinalParameterName(string prefix, IEnumerable<string> duplicates) =>
        duplicates.Contains(Name) ? $"@Parameter{prefix}_{Name}" : $"@Parameter_{Name}";
}
