using Carrigan.Core.Extensions;
using Carrigan.SqlTools.IdentifierTypes;

namespace Carrigan.SqlTools.Exceptions;
/// <summary>
/// Thrown when no property in the model class <typeparamref name="T"/> is marked with
/// either a <c>[PrimaryKey]</c> or <c>[Key]</c> attribute and a “By Id” operation is invoked.
/// </summary>
/// <typeparam name="T">
/// The model type for which no primary key property was found.
/// </typeparam>
/// <remarks>
/// This exception is typically thrown during SQL generation when methods such as
/// <c>UpdateById()</c>, <c>DeleteById()</c>, or similar "By Id" operations are invoked,
/// but the target entity type does not define a primary key through either the
/// <see cref="Carrigan.SqlTools.Attributes.PrimaryKeyAttribute"/> or the standard
/// <see cref="System.ComponentModel.DataAnnotations.KeyAttribute"/>.
/// </remarks>
public class NoPrimaryKeyProperty<T> : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NoPrimaryKeyProperty{T}"/> class.
    /// </summary>
    /// <remarks>
    /// This exception is raised when a “By Id” SQL operation is executed on a model
    /// class that lacks a defined primary key property.
    /// </remarks>
    internal NoPrimaryKeyProperty() :
        base($"No Primary Key property has been specified for the {nameof(T)} class.")
    {
    }
}
