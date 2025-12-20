namespace Carrigan.SqlTools.Exceptions;

/// <summary>
/// Thrown when a model type <typeparamref name="T"/> does not define a primary key property.
/// </summary>
/// <typeparam name="T">
/// The model type that requires a primary key property.
/// </typeparam>
/// <remarks>
/// This exception is evaluated and may be thrown when a <c>SqlGenerator&lt;T&gt;</c> is constructed.
/// A primary key property is required when SQL generation needs to uniquely identify a row (for example,
/// for <c>SelectById</c>, <c>Update</c>, or <c>Delete</c> operations).
/// </remarks>
public class NoPrimaryKeyPropertyException<T> : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NoPrimaryKeyPropertyException{T}"/> class.
    /// </summary>
    internal NoPrimaryKeyPropertyException()
        : base($"{typeof(T).Name} has no primary key property.")
    {
    }
}