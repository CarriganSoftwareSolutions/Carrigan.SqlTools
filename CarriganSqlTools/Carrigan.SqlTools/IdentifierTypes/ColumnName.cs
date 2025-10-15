using Carrigan.Core.DataTypes;
using Carrigan.Core.Extensions;

namespace Carrigan.SqlTools.IdentifierTypes;

//TODO: Proof read documentation 
/// <summary>
/// Strongly typed string wrapper for Column names
/// </summary>
public class ColumnName : StringWrapper
{
    public ColumnName(string? name) : base(name) { }

    /// <summary>
    /// If <param name="name"> is not null or empty, it creates a new instance,
    /// otherwise it returns a null object.
    /// </summary>
    /// <param name="name">Column Name</param>
    /// <returns>
    /// If <param name="name"> is not null or empty, it creates a new instance,
    /// otherwise it returns a null object.
    /// </returns>
    public static ColumnName? New(string? name)
    {
        if (name.IsNullOrEmpty())
            return null;
        else
            return new ColumnName(name);
    }
}