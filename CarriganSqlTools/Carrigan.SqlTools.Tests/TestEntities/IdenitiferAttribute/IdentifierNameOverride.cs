using Carrigan.SqlTools.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Tests.TestEntities.IdenitiferAttribute;
[Identifier("IdentifierNameOverrideTable")]
[Table("WtfMate")]
public class IdentifierNameOverride
{
    public Guid Id { get; set; }
    public string? Text { get; set; }
}

