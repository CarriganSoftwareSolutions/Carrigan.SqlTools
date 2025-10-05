using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces;

namespace Carrigan.SqlTools.IdentifierTypes;
/// <summary>
/// Strongly typed string wrapper for Class names
/// </summary>
/// <param name="Value">Class name</param>
//TODO: Proof read documentation and unit test
internal readonly record struct ClassName(string Value) : IWhiteSpace
{
    /// <summary>
    /// implicit operator
    /// </summary>
    /// <param name="value">Class name</param>
    public static implicit operator string(ClassName name) =>
        name.ToString();

    /// <summary>
    /// To string method
    /// </summary>
    /// <returns></returns>
    public override string ToString() =>
        Value;
    /// <summary>
    /// Determines if the underlying string is empty or whitespace.
    /// </summary>
    /// <returns>true is the underlying string is empty or whitespace, else false</returns>
    public bool IsWhiteSpace() =>
        Value.IsWhiteSpace();
    /// <summary>
    /// Determines if the underlying string is not empty and not whitespace.
    /// </summary>
    /// <returns>false is the underlying string is empty or whitespace, else true</returns>
    public bool IsNotWhiteSpace() =>
        IsWhiteSpace() is false;
    /// Determines if the underlying string is empty.
    /// </summary>
    /// <returns>true is the underlying string is empty, else false</returns>
    public bool IsEmpty() =>
        Value.IsEmpty();
    /// <summary>
    /// Determines if the underlying string is not empty.
    /// </summary>
    /// <returns>false is the underlying string is empty, else true</returns>
    public bool IsNotEmpty() =>
        IsEmpty() is false;
}