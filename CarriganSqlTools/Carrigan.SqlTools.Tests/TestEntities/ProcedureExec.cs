using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Tests.TestEntities;

[Identifier("UpdateThing", "schema")]
internal class ProcedureExec
{
    [Parameter ("SomeValue")]
    public string? ValueColumn { get; set; }
}