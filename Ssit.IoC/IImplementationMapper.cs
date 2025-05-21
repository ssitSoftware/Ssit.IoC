using System;

namespace Ssit.IoC;

public interface IImplementationMapper
{
    /// <summary>
    /// Resolves the implementation type for a given abstract type TType.
    /// </summary>
    /// <typeparam name="TType">The abstract type whose implementation needs to be resolved.</typeparam>
    /// <returns>The implementation type associated with the abstract type TType.</returns>
    Type ResolveImplementation<TType>(string key = null);

    /// <summary>
    /// Resolves the concrete implementation type for a given abstract type or interface.
    /// </summary>
    /// <param name="type">The abstract type or interface for which to resolve the implementation.</param>
    /// <param name="key">Additional key for different implementation than default one.</param>
    /// <returns>The concrete implementation type if found; otherwise, throws a KeyNotFoundException.</returns>
    Type ResolveImplementation(Type type, string key = null);
}