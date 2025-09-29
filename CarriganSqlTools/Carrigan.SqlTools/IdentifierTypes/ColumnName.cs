using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces;

namespace Carrigan.SqlTools.IdentifierTypes;
/// <summary>
/// Strongly typed string wrapper for Column names
/// </summary>
/// <param name="Value">Column name</param>
//TODO: Proof read documentation
internal readonly record struct ColumnName(string Value) : IWhiteSpace
{
    /// <summary>
    /// implicit operator
    /// </summary>
    /// <param name="value">Column name</param>
    public static implicit operator string(ColumnName name) =>
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