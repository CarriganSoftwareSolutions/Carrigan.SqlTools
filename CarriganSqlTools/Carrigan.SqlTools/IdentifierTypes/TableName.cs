using Carrigan.Core.Extensions;

namespace Carrigan.SqlTools.IdentifierTypes;

//TODO: Proof read documentation
/// <summary>
/// Strongly typed string wrapper for Table names
/// </summary>
public class TableName : StringWrapper
{
    public TableName(string? name) : base(name) { }

    /// <summary>
    /// If <param name="name"> is not null or empty, it creates a new instance,
    /// otherwise it returns a null object.
    /// </summary>
    /// <param name="name">Table Name</param>
    /// <returns>
    /// If <param name="name"> is not null or empty, it creates a new instance,
    /// otherwise it returns a null object.
    /// </returns>
    public static TableName? New(string? name)
    {
        if (name.IsNullOrEmpty())
            return null;
        else
            return new TableName(name);
    }
}

