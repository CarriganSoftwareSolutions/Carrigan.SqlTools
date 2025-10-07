using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces;

namespace Carrigan.SqlTools.IdentifierTypes;

//TODO: Proof read documentation and unit test
/// <summary>
/// Strongly typed string wrapper for Alias names
/// </summary>
public class AliasName : StringWrapper
{
    internal AliasName(string? name) : base(name) { }

    /// <summary>
    /// If <param name="name"> is not null or empty, it creates a new instance,
    /// otherwise it returns a null object.
    /// </summary>
    /// <param name="name">Alias name</param>
    /// <returns>
    /// If <param name="name"> is not null or empty, it creates a new instance,
    /// otherwise it returns a null object.
    /// </returns>
    public static AliasName? New(string? name)
    {
        if (name.IsNullOrEmpty())
            return null;
        else
            return new AliasName(name);
    }
}