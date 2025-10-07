using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces;

namespace Carrigan.SqlTools.IdentifierTypes;

//TODO: Proof read documentation and unit test
/// <summary>
/// Strongly typed string wrapper for Member names
/// </summary>
public class MemberName : StringWrapper
{
    public MemberName(string? name) : base(name) { }

    /// <summary>
    /// If <param name="name"> is not null or empty, it creates a new instance,
    /// otherwise it returns a null object.
    /// </summary>
    /// <param name="name">Member Name</param>
    /// <returns>
    /// If <param name="name"> is not null or empty, it creates a new instance,
    /// otherwise it returns a null object.
    /// </returns>
    public static MemberName? New(string? name)
    {
        if (name.IsNullOrEmpty())
            return null;
        else
            return new MemberName(name);
    }
}