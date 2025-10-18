using Carrigan.Core.Extensions;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Exceptions;
//TODO: Proof Read Documentation
/// <summary>
/// Thrown when an encrypter is required and a key version property has not been specified.
/// </summary>
public class NoKeyVersionProperty<T> : Exception
{
    /// <summary>
    /// Constructor for NoKeyVersionProperty
    /// Thrown when an encrypter is required and a key version property has not been specified.
    /// </summary>
    public NoKeyVersionProperty() :
        base($"No Key Version property has been provided for the{nameof(T)} class, and {nameof(T)} has encrypted properties.")   
    {
    }
}
