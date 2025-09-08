using Carrigan.Core.Extensions;
using Carrigan.SqlTools.JoinTypes;
using System.Diagnostics.CodeAnalysis;

namespace Carrigan.SqlTools.Extensions;

public static class IJoinsExtensions
{
    public static bool IsNullOrEmpty(this IJoins? joins) =>
        joins?.Joints?.None() ?? true;
    public static bool IsNotNullOrEmpty([NotNullWhen(true)] this IJoins? joins) =>
        joins.IsNullOrEmpty() == false;
}
