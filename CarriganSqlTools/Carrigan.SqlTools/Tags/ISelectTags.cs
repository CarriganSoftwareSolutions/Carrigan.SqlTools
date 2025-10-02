using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Tags;
//Document and unit test.
public interface ISelectTags
{
    public string GetSelects();
    public IEnumerable<TableTag> GetTableTags(); //TODO: can we get rid of Table Tag now?
    public bool Any();
}
