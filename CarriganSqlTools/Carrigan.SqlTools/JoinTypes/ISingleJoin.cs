namespace Carrigan.SqlTools.JoinTypes;

/// <summary>
/// An interface for classes representing a singular SQL join.
/// Admittedly having an Interface for a single join, that inherits from the interface to represent multiple joins seems convoluted.
/// However, doing this allows the Join base class to stand in for an IJoins object. 
/// This allows methods calls to accept a single join or Joins that represents multiple joins.
/// This was more a naming failure than an actual failure to represent a real world problem.
/// </summary>
public interface ISingleJoin : IJoins
{
}
