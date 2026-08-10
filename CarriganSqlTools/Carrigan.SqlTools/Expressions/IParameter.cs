using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Expressions;

public interface IParameter
{
    FieldProperties? FieldProperties { get; init; }
    ParameterTag Name { get; init; }
    object? Value { get; init; }
}