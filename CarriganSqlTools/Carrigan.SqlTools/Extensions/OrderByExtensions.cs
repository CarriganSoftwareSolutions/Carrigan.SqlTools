using SqlTools.OrderByItems;
using System.Diagnostics.CodeAnalysis;

namespace SqlTools.Extensions;

public static class OrderByExtensions
{
    public static bool IsNullOrEmpty(this OrderBy? value) =>
        value?.IsEmpty() ?? true;
    public static bool IsNotNullOrEmpty([NotNullWhen(true)]this OrderBy? value) =>
        value.IsNullOrEmpty() ==  false;
}
