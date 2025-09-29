using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Carrigan.SqlTools.IdentifierTypes;

//TODO: Proof read documentation
/// <summary>
/// Strongly typed string wrapper for Schema names
/// </summary>
/// <param name="Value">Schema name</param>
internal readonly record struct SchemaName(string Value) : IWhiteSpace
{
    /// <summary>
    /// implicit operator
    /// </summary>
    /// <param name="value">Schema name</param>
    public static implicit operator string(SchemaName name) => 
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

