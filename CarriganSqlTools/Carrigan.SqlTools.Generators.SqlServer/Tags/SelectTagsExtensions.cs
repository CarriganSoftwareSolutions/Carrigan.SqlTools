using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Tags;

/// <summary>
/// Provides extension methods for working with collections of SelectTag objects.
/// </summary>
public static class SelectTagsExtensions
{
    /// <summary>
    /// Creates a SelectTags collection from an IEnumerable of SelectTag objects.
    /// </summary>
    /// <param name="selectTags">
    /// The IEnumerable of SelectTag objects to convert into a SelectTags collection.
    /// </param>
    /// <returns>
    /// A new SelectTags collection containing the provided SelectTag objects.
    /// </returns>
    public static SelectTags AsSelectTags(this IEnumerable<SelectTag> selectTags) =>
        new(selectTags);
}
