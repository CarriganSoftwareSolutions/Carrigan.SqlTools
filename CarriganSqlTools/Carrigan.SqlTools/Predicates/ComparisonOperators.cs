namespace Carrigan.SqlTools.Predicates;

/// <summary>
/// Predicates control the boolean logic for join and where clauses.
/// This is the base class that represents SQL's comparison operators.
/// </summary>
public abstract class ComparisonOperators : PredicatesBase
{
    /// <summary>
    /// Left value
    /// </summary>
    private PredicatesBase _left;
    /// <summary>
    /// Right value
    /// </summary>
    private PredicatesBase _right;
    /// <summary>
    /// string representing the comparison operator as the operator is represented in SQL.
    /// </summary>
    private string _operator;

    /// <summary>
    /// This is the base constructor for the classes that represents SQL's comparison operators
    /// </summary>
    /// <param name="left">left value</param>
    /// <param name="right">right value</param>
    /// <param name="op">SQL string representation of the operator</param>
    public ComparisonOperators(PredicatesBase left, PredicatesBase right, string op)
    {
        _operator = op;
        _left = left;
        _right = right;
    }

    /// <summary>
    /// Recursively get all the parameters associated with the logic.
    /// </summary>
    internal override IEnumerable<Parameters> Parameter =>
        _left.Parameter.Concat(_right.Parameter);

    /// <summary>
    ///  Recursively get all the columns associated with the logic.
    /// </summary>
    internal override IEnumerable<IColumnValue> Column =>
        _left.Column.Concat(_right.Column);

    /// <summary>
    /// Produces the SQL represented by this class.
    /// </summary>
    /// <param name="prefix">
    /// building a prefix as we drill down the logic tree, 
    /// this prefix is added to the names of parameters to ensure that each parameter has a unique name
    /// this is only used with parameters that have duplicate names
    /// </param>
    /// <param name="duplicates">
    /// keep track of all of the user supplied parameter names that are duplicates
    /// this will be use in the leaf parameter node to determine if a prefix is needed or not.
    /// </param>
    /// <returns>Returns a SQL string represented by this class.</returns>
    internal override string ToSql(string prefix, IEnumerable<string> duplicates) =>
        $"({_left.ToSql($"{prefix}_L", duplicates)} {_operator} {_right.ToSql($"{prefix}_R", duplicates)})";

    /// <summary>
    /// Recursively get all the parameters associated with the logic, as key value pairs.
    /// </summary>
    /// <param name="prefix">
    /// building a prefix as we drill down the logic tree, 
    /// this prefix is added to the names of parameters to ensure that each parameter has a unique name
    /// this is only used with parameters that have duplicate names
    /// </param>
    /// <param name="duplicates">
    /// keep track of all of the user supplied parameter names that are duplicates
    /// this will be use in the leaf parameter node to determine if a prefix is needed or not.
    /// </param>
    /// <returns>Returns all the parameters associated with the logic, as key value pairs.</returns>
    internal override IEnumerable<KeyValuePair<string, object>> GetParameters(string prefix, IEnumerable<string> duplicates) =>
        _left.GetParameters($"{prefix}_L", duplicates).Concat(_right.GetParameters($"{prefix}_R", duplicates));
}