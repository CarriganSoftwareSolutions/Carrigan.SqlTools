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
/// Strongly typed string wrapper for Property names
/// </summary>
public class PropertyName : StringWrapper
{
    /// <summary>
    /// If <param name="name"> is not null or empty, it creates a new instance,
    /// otherwise it returns a null object.
    /// </summary>
    /// <param name="name">Procedure Name</param>
    /// <returns>
    /// If <param name="name"> is not null or empty, it creates a new instance,
    /// otherwise it returns a null object.
    /// </returns>

    public PropertyName(string? name) : base(name) { }

    public static PropertyName? New(string? name)
    {
        if (name.IsNullOrEmpty())
            return null;
        else
            return new PropertyName(name);
    }
}
