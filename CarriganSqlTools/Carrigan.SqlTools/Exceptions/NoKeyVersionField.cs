using Carrigan.Core.Extensions;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Exceptions;
//TODO: Proof Read Documentation
/// <summary>
/// Thrown when an encrypter is required and a key version field has not been specified.
/// </summary>
public class NoKeyVersionField<T> : Exception
{
    /// <summary>
    /// Constructor for NoKeyVersionField
    /// Thrown when an encrypter is required and a key version field has not been specified.
    /// </summary>
    internal NoKeyVersionField() :
        base($"No Key Version field has been provided for the{nameof(T)} class, and {nameof(T)} has encrypted properties.")   
    {
    }
}
