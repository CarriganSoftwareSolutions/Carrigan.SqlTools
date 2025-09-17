using Carrigan.Core.Extensions;
using Carrigan.SqlTools.Sets;
using System.Diagnostics.CodeAnalysis;

namespace Carrigan.SqlTools.Sets;

public static class SetColumnsExtensions
{
    public static bool IsNullOrEmpty<T>(this SetColumns<T>? setColumns) =>
        setColumns?.ColumnTags.IsNullOrEmpty() ?? true;
    public static bool IsNotNullOrEmpty<T>([NotNullWhen(true)] this SetColumns<T>? setColumns) =>
        setColumns.IsNullOrEmpty() == false;
}
