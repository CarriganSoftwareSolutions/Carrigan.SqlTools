using Carrigan.SqlTools.Attributes;

namespace Carrigan.SqlTools.Base.Tests.TestEntities;

[Identifier("UpdateThing", "schema")]
internal class ProcedureExec
{
    [Parameter ("SomeValue")]
    public string? ValueColumn { get; set; }
}