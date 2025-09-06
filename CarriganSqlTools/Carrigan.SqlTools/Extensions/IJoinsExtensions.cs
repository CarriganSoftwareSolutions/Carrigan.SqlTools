using Carrigan.Core.Extensions;
using SqlTools.Joins;
using System.Diagnostics.CodeAnalysis;

namespace SqlTools.Extensions;

public static class IJoinsExtensions
{
    public static bool IsNullOrEmpty(this IJoins? joins) =>
        joins?.Joints?.None() ?? true;
    public static bool IsNotNullOrEmpty([NotNullWhen(true)] this IJoins? joins) =>
        joins.IsNullOrEmpty() == false;
}
