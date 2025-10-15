using Carrigan.Core.DataTypes;
using Carrigan.Core.Extensions;

namespace Carrigan.SqlTools.IdentifierTypes;

//TODO: Proof read documentation 
/// <summary>
/// Strongly typed string wrapper for Procedure names
/// </summary>
public class ProcedureName : StringWrapper
{
    public ProcedureName(string? name) : base(name) { }

    /// <summary>
    /// If <param name="name"> is not null or empty, it creates a new instance,
    /// otherwise it returns a null object.
    /// </summary>
    /// <param name="name">Procedure Name</param>
    /// <returns>
    /// If <param name="name"> is not null or empty, it creates a new instance,
    /// otherwise it returns a null object.
    /// </returns>
    public static ProcedureName? New(string? name)
    {
        if (name.IsNullOrEmpty())
            return null;
        else
            return new ProcedureName(name);
    }
}