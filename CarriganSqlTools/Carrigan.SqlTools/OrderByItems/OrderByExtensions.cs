using System.Diagnostics.CodeAnalysis;

namespace Carrigan.SqlTools.OrderByItems;

public static class OrderByExtensions
{
    public static bool IsNullOrEmpty(this IOrderByClause? value) =>
        value?.IsEmpty() ?? true;
    public static bool IsNotNullOrEmpty([NotNullWhen(true)]this IOrderByClause? value) =>
        value.IsNullOrEmpty() ==  false;
}
