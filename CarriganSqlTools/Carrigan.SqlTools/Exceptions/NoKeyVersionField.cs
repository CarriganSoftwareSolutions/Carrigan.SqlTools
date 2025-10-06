using Carrigan.Core.Extensions;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Exceptions;
//TODO: Proof Read Documentation
/// <summary>
/// Thrown when an encrypter is not provided
/// </summary>
internal class NoKeyVersionField<T> : Exception
{
    /// <summary>
    /// Constructor for EncrypterNotProvided
    /// Thrown when an encrypter is not provided
    /// </summary>
    /// <param name="propertyNames">Invalid property names to include in exception message.</param>
    /// 
    internal NoKeyVersionField() :
        base($"No Key Version field has been provided in class {nameof(T)}, and {nameof(T)} has encrypted properties.")   
    {
    }
}
