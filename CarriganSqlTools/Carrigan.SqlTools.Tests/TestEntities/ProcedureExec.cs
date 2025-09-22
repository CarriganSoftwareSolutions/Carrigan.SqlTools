using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Tests.TestEntities;

[Identifier("UpdateThing", "schema")]
public class ProcedureExec
{
    [Parameter ("SomeValue")]
    public string? ValueColumn { get; set; }
}