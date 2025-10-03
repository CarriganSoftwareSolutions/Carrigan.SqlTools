using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Tags;
//TODO: Proof read Documentation and unit test.
public interface ISelectTags
{
    /// <summary>
    /// Get all SelectTags associated with the instance, as a string.
    /// For SelectTag this will just be itself.
    /// For SelectTags this will be a comma separated list.
    /// </summary>
    /// <returns>
    /// All SelectTags associated with the instance, as a string. 
    /// For SelectTag this will just be itself.
    /// For SelectTags this will be a comma separated list.
    /// </returns>
    public string GetSelects();
    /// <summary>
    /// Get all TableTags associated with the instance.
    /// For SelectTag this will just it's TableTag as an Enumerable.
    /// For SelectTags this will be multiple TableTags.
    /// </summary>
    /// <returns>
    /// All TableTags associated with the instance.
    /// For SelectTag this will just it's TableTag as an Enumerable.
    /// For SelectTags this will be multiple TableTags.
    /// </returns>
    public IEnumerable<TableTag> GetTableTags();

    /// <summary>
    /// Determines if this instance contains any actual SelectTags
    /// For SelectTag, this should always be true.
    /// For SelectTags, this will be true if the underlying Enumeration is not empty.
    /// </summary>
    /// <returns>
    /// True if this instance contains any actual SelectTags
    /// For SelectTag, this should always be true.
    /// For SelectTags, this will be true if the underlying Enumeration is not empty.
    /// </returns>
    public bool Any();
}
