using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.Paging;

/// <summary>
/// Represents a paging strategy that uses the LIMIT and OFFSET clauses, commonly supported by databases like PostgreSQL and MySQL.
/// </summary>
public class LimitOffset : PagingBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LimitOffset"/> class.
    /// </summary>
    /// <param name="limit">The maximum number of rows to return.</param>
    /// <param name="offset">The number of rows to skip before starting to return rows.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="limit"/> is <c>0</c>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="offset"/> is <c>0</c>.
    /// </exception>
    public LimitOffset(uint limit, uint offset) : base(offset, limit)
    { }
}
