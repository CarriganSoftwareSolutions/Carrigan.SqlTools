using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces;

namespace Carrigan.SqlTools.IdentifierTypes;

//TODO: Proof read documentation 
/// <summary>
/// Strongly typed string wrapper for Member names
/// </summary>
internal class MemberName : StringWrapper
{
    internal MemberName(string? name) : base(name) { }

    /// <summary>
    /// If <param name="name"> is not null or empty, it creates a new instance,
    /// otherwise it returns a null object.
    /// </summary>
    /// <param name="name">Member Name</param>
    /// <returns>
    /// If <param name="name"> is not null or empty, it creates a new instance,
    /// otherwise it returns a null object.
    /// </returns>
    internal static MemberName? New(string? name)
    {
        if (name.IsNullOrEmpty())
            return null;
        else
            return new MemberName(name);
    }
}