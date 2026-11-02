using Carrigan.Core.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carrigan.SqlTools.IdentifierTypes;

/// <summary>
/// Represents a strongly typed wrapper for an SQL join type identifier,
/// </summary>
internal class JoinType : TextWrapper
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JoinType"/> class with the specified join type string.
    /// </summary>
    /// <param name="joinType">
    /// The join type string value (e.g., "INNER JOIN", "LEFT JOIN", etc.). This value is compared in a case-insensitive manner.
    /// </param>
    public JoinType(string joinType) : base(joinType, StringComparison.OrdinalIgnoreCase)
    {
    }
}
