using Carrigan.SqlTools.OrderByItems;
using System.Diagnostics.CodeAnalysis;

namespace Carrigan.SqlTools.Extensions;

public static class OrderByExtensions
{
    public static bool IsNullOrEmpty(this OrderBy? value) =>
        value?.IsEmpty() ?? true;
    public static bool IsNotNullOrEmpty([NotNullWhen(true)]this OrderBy? value) =>
        value.IsNullOrEmpty() ==  false;
}
