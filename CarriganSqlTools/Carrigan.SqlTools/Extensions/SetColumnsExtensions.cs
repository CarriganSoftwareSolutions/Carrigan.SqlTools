using Carrigan.Core.Extensions;
using SqlTools.Sets;
using System.Diagnostics.CodeAnalysis;

namespace SqlTools.Extensions;

public static class SetColumnsExtensions
{
    public static bool IsNullOrEmpty<T>(this SetColumns<T>? setColumns) =>
        setColumns?.ColumnNames.IsNullOrEmpty() ?? true;
    public static bool IsNotNullOrEmpty<T>([NotNullWhen(true)] this SetColumns<T>? setColumns) =>
        setColumns.IsNullOrEmpty() == false;
}
