using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces;

namespace Carrigan.SqlTools.IdentifierTypes;

//TODO: Proof read documentation
/// <summary>
/// Strongly typed string wrapper for Alias names
/// </summary>
/// <param name="Value">Alias name</param>
internal readonly record struct AliasName(string Value) : IWhiteSpace
{
    /// <summary>
    /// implicit operator
    /// </summary>
    /// <param name="value">Alias name</param>
    public static implicit operator string(AliasName name) =>
        name.ToString();

    /// <summary>
    /// To string method
    /// </summary>
    /// <returns></returns>
    public override string ToString() =>
        Value;
    //TODO: Document Code
    public bool IsWhiteSpace() =>
        Value.IsWhiteSpace();
    //TODO: Document Code
    public bool IsNotWhiteSpace() =>
        IsWhiteSpace() is false;
    //TODO: Document Code
    public bool IsEmpty() =>
        Value.IsEmpty();
    //TODO: Document Code
    public bool IsNotEmpty() =>
        IsEmpty() is false;
}