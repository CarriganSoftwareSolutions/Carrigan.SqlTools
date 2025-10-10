using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Tags;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Predicates control the boolean logic for join and where clauses.
/// This class is a base class to represent SQL's logical AND and OR operator for logical operations on one more predicate values.
/// </summary>
public abstract class LogicalOperator : Predicates
{
    private readonly string _operator;
    private readonly IEnumerable<Predicates> _predicates;

    /// <summary>
    /// Base constructor for the logical boolean operator "AND" and "OR".
    /// If no predicate values are passed in, then a <see cref="ArgumentNullException"/> is thrown.
    /// If only one predicate value is provided, then this class is deigned to use just that predicate in place of the logical operator.
    /// If two or more are provided then each predicate is chained together with the AND or OR logical operator.
    /// </summary>
    /// <param name="op">One or more boolean predicates.</param>
    /// <param name="predicates">One or more boolean predicates.</param>
    /// <exception cref="ArgumentNullException">thrown if no predicates are provided</exception>
    public LogicalOperator(string op, params IEnumerable<Predicates> predicates)
    {
        if (predicates.IsNullOrEmpty())
            throw new ArgumentNullException(nameof(predicates), $"{nameof(predicates)} must contain at least one value.");
        _operator = op;
        _predicates = predicates;
    }

    /// <summary>
    /// Recursively get all the parameters associated with the logic.
    /// </summary>
    internal override IEnumerable<Parameter> Parameters =>
        _predicates.SelectMany(predicate => predicate.Parameters);

    /// <summary>
    ///  Recursively get all the columns associated with the logic.
    /// </summary>
    internal override IEnumerable<IColumn> Columns =>
        _predicates.SelectMany(predicate => predicate.Columns);

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
    internal override string ToSql(string prefix, IEnumerable<ParameterTag> duplicates)
    {
        if (_predicates.Count() == 1)
            return _predicates.Single().ToSql(prefix, duplicates);
        else
            return $"({string.Join($" {_operator} ", _predicates.Select((predicate,index) => predicate.ToSql($"{prefix}_{index}", duplicates)))})";
    }


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
    internal override IEnumerable<KeyValuePair<ParameterTag, object>> GetParameters(string prefix, IEnumerable<ParameterTag> duplicates)
    {
        if (_predicates.Count() == 1)
            return _predicates.Single().GetParameters(prefix, duplicates);
        else
            return _predicates.SelectMany((predicate, index) => predicate.GetParameters($"{prefix}_{index}", duplicates));
    }
}
