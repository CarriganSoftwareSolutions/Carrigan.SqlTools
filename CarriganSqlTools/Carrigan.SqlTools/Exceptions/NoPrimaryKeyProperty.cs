using Carrigan.Core.Extensions;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Exceptions;
//TODO: Proof Read Documentation
/// <summary>
/// Throw when when no PrimaryKey or Key attribute have been applied to a class and one of the "By Id" methods are called.
/// </summary>
public class NoPrimaryKeyProperty<T> : Exception
{
    /// <summary>
    /// Constructor for NoPrimaryKeyProperty
    /// Throw when when no PrimaryKey or Key attribute have been applied to a class and one of the "By Id" methods are called.
    /// </summary>
    internal NoPrimaryKeyProperty() :
        base($"No Primary Key property has been specified for the {nameof(T)} class.")
    {
    }
}
