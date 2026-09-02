using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Expressions;

/// <summary>
/// Defines the contract for a parameter in SQL expressions, encapsulating its name, value, and optional field properties.
/// </summary>
public interface IParameter
{
    /// <summary>
    /// Gets the optional field properties associated with the parameter, which may include metadata such as data type, length, and nullability.
    /// </summary>
    FieldProperties? FieldProperties { get; init; }
    /// <summary>
    /// Gets the name of the parameter, represented as a <see cref="ParameterTag"/>. This name is used to identify the parameter in SQL expressions.
    /// </summary>
    ParameterTag Name { get; init; }
    /// <summary>
    /// Gets the value of the parameter, which can be of any type. This value is used in SQL expressions where the parameter is referenced.
    /// </summary>
    object? Value { get; init; }
}