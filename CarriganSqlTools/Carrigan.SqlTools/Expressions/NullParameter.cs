using Carrigan.SqlTools.Attributes;
using Carrigan.SqlTools.Dialects;
using Carrigan.SqlTools.Fragments;
using Carrigan.SqlTools.Tags;
using Carrigan.SqlTools.Types;

namespace Carrigan.SqlTools.Expressions;

public class NullParameter<T> : Parameter
{
    /// <summary>
    /// Initializes a new instance of <see cref="NullParameter{T}"/> with an auto-generated name if not provided.
    /// </summary>
    /// <param name="sqlDialect">
    /// The SQL dialect to use for determining default field properties based on the CLR type <typeparamref name="T"/>.
    /// </param>
    /// <param name="parameterTag">
    /// The tag (name + metadata) for the parameter. If null, an auto-generated name will be used.
    /// </param>
    public NullParameter(ISqlDialects sqlDialect, ParameterTag? parameterTag = null) : 
        base(null, parameterTag ?? new ParameterTag("Parameter"), sqlDialect.GetDefaultFieldPropertiesByClrType(typeof(T)))
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="NullParameter{T}"/> with a specified parameter name.
    /// </summary>
    /// <param name="sqlDialect">
    /// The SQL dialect to use for determining default field properties based on the CLR type <typeparamref name="T"/>.
    /// </param>
    /// <param name="parameterName">
    /// The name of the parameter. If null, an auto-generated name will be used.
    /// </param>
    [ExternalOnly]
    public NullParameter(ISqlDialects sqlDialect, string parameterName) : this(sqlDialect, parameterName is null ? null : new ParameterTag(parameterName))
    {
    }
}
