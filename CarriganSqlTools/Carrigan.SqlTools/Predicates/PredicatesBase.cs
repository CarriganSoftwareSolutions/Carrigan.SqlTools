using Carrigan.SqlTools.Predicates;

namespace Carrigan.SqlTools.Predicates;

public abstract class PredicatesBase
{
    internal abstract IEnumerable<Parameters> Parameter { get; }
    internal abstract IEnumerable<IColumnValue> Column { get; }

    public string ToSql()
    {
        //get an IEnumerable of all the duplicate parameter names
        IEnumerable<string> duplicates = Parameter.Select(parameter => parameter.Name)
            .GroupBy(name => name)
            .Where(nameGroup => nameGroup.Count() > 1)
            .Select(nameGroup => nameGroup.Key);

        return ToSql(string.Empty, duplicates);
    }
    //Get the SQL recursively, while also renaming the parameters to ensure they are unique and match the names from GetParameters.
    internal abstract string ToSql(string prefix, IEnumerable<string> duplicates);

    public Dictionary<string,object> GetParameters()
    {
        //get an IEnumerable of all the duplicate parameter names
        IEnumerable<string> duplicates = Parameter.Select(parameter => parameter.Name)
            .GroupBy(name => name)
            .Where(nameGroup => nameGroup.Count() > 1)
            .Select(nameGroup => nameGroup.Key);

        return GetParameters(string.Empty, duplicates)
               .ToDictionary(pair => pair.Key, pair => pair.Value);
    }
    //This method recursively adds prefixes to the parameter names to ensure unique parameter names
    internal abstract IEnumerable<KeyValuePair<string, object>> GetParameters(string prefix, IEnumerable<string> duplicates);
}
