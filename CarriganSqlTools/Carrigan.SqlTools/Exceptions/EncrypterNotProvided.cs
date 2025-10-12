using Carrigan.Core.Extensions;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Exceptions;
//TODO: Proof Read Documentation
/// <summary>
/// Thrown when an encrypter is not provided
/// </summary>
public class EncrypterNotProvided<T> : Exception
{
    /// <summary>
    /// Constructor for EncrypterNotProvided
    /// Thrown when an encrypter is not provided
    /// </summary>
    /// <param name="propertyNames">Invalid property names to include in exception message.</param>
    /// 
    internal EncrypterNotProvided() :
        base($"No encrypter provided for Sql Generator<{nameof(T)}>, and {nameof(T)} has encrypted properties.")   
    {
    }
}
