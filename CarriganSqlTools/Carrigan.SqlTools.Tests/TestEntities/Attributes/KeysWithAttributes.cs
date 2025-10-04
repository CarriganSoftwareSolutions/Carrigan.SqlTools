using Carrigan.SqlTools.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Tests.TestEntities.Attributes;
internal class PrimaryKeysWithAttributes
{
    [PrimaryKey]
    public int Id1 { get; set; }
    [PrimaryKey]
    [Column("IdTwo")]
    public int Id2 { get; set; }
    [PrimaryKey]
    [Identifier("IdThree")]
    [Column("Id3")]
    public int Id3 { get; set; }
}
