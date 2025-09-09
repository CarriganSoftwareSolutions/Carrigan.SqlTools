namespace Carrigan.SqlTools.JoinTypes;
/// <summary>
/// Represents an SQL join. This class is basically just an alias for the Left Join class.
/// </summary>
/// <typeparam name="T">A data model representing the main table, left table or base table. This is the table you are selecting from, updating or deleting.</typeparam>
/// <typeparam name="J">A data model representing the right table or joined table. This is the table being joined to the main table.</typeparam>
public class Join<T, J> : LeftJoin<T, J>
{
    /// <summary>
    /// Constructor for the join class.
    /// </summary>
    /// <param name="predicate">Represents the "on" part of the join clause.</param>
    public Join(Predicates.PredicatesBase predicate) : base(predicate)
    { }
}
