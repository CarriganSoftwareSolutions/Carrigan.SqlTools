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

internal class SchemaName : StringWrapper
{
    internal SchemaName(string? name) : base(name) { }

    /// <summary>
    /// If <param name="name"> is not null or empty, it creates a new instance,
    /// otherwise it returns a null object.
    /// </summary>
    /// <param name="name">Schema Name</param>
    /// <returns>
    /// If <param name="name"> is not null or empty, it creates a new instance,
    /// otherwise it returns a null object.
    /// </returns>
    internal static SchemaName? New(string? name)
    {
        if (name.IsNullOrEmpty())
            return null;
        else
            return new SchemaName(name);
    }
}

