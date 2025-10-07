using Carrigan.Core.Extensions;
using Carrigan.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Carrigan.SqlTools.IdentifierTypes;


//TODO: Proof read documentation and unit test
/// <summary>
/// Strongly typed string wrapper for Schema names
/// </summary>

public class SchemaName : StringWrapper
{
    public SchemaName(string? name) : base(name) { }

    /// <summary>
    /// If <param name="name"> is not null or empty, it creates a new instance,
    /// otherwise it returns a null object.
    /// </summary>
    /// <param name="name">Schema Name</param>
    /// <returns>
    /// If <param name="name"> is not null or empty, it creates a new instance,
    /// otherwise it returns a null object.
    /// </returns>
    public static SchemaName? New(string? name)
    {
        if (name.IsNullOrEmpty())
            return null;
        else
            return new SchemaName(name);
    }
}

