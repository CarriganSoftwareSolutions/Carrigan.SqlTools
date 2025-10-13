using Carrigan.Core.DataTypes;
using Carrigan.Core.Extensions;

namespace Carrigan.SqlTools.IdentifierTypes;

//TODO: Proof read documentation
/// <summary>
/// Strongly typed string wrapper for Table names
/// </summary>
internal class TableName : StringWrapper
{
    internal TableName(string? name) : base(name) { }

    /// <summary>
    /// If <param name="name"> is not null or empty, it creates a new instance,
    /// otherwise it returns a null object.
    /// </summary>
    /// <param name="name">Table Name</param>
    /// <returns>
    /// If <param name="name"> is not null or empty, it creates a new instance,
    /// otherwise it returns a null object.
    /// </returns>
    internal static TableName? New(string? name)
    {
        if (name.IsNullOrEmpty())
            return null;
        else
            return new TableName(name);
    }
}

