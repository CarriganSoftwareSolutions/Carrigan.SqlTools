using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.PredicatesLogic;

/// <summary>
/// Predicates control the boolean logic for join and where clauses.
/// This class is essentially an alias for Column = Value
/// The intent is to reduce the amount of code needed to perform a routine task.
/// </summary>
[Obsolete("Use ColumnValues<T> instead.")]
public class ByColumnValues<T> : ColumnValue<T>
{
    public ByColumnValues(PropertyName propertyName, object value) : base(propertyName, value)
    {
    }
}
