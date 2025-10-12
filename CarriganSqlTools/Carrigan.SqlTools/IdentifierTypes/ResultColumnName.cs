using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces;

namespace Carrigan.SqlTools.IdentifierTypes;

//TODO: Proof read documentation 
/// <summary>
/// Strongly typed string wrapper for ResultColumn names
/// </summary>
internal class ResultColumnName : StringWrapper
{
    internal ResultColumnName(string? name) : base(name) { }

    /// <summary>
    /// If <param name="name"> is not null or empty, it creates a new instance,
    /// otherwise it returns a null object.
    /// </summary>
    /// <param name="name">ResultColumn Name</param>
    /// <returns>
    /// If <param name="name"> is not null or empty, it creates a new instance,
    /// otherwise it returns a null object.
    /// </returns>
    internal static ResultColumnName? New(string? name)
    {
        if (name.IsNullOrEmpty())
            return null;
        else
            return new ResultColumnName(name);
    }
}