using System;
using System.Collections.Generic;

namespace Ssit.IoC;

/// <summary>
/// Interface representing an Inversion of Control (IoC) Container.
/// Provides methods for resolving dependencies and constructing objects using injected dependencies.
/// </summary>
public interface IIoCContainer: IDisposable
{
    /// Retrieves an instance of the requested type from the IoC container.
    /// <typeparam name="TType">The type of the instance to retrieve.</typeparam>
    /// <returns>An instance of the requested type.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the requested type is not registered in the container and cannot be resolved.</exception>
    TType Get<TType>();

    /// <summary>
    /// Resolves an instance of the specified type from the IoC container.
    /// Throws a KeyNotFoundException if the type is not registered in the container.
    /// </summary>
    /// <param name="type">The type of the instance to resolve.</param>
    /// <returns>An instance of the specified type.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the specified type is not found in the container.</exception>
    object Get(Type type);

    /// <summary>
    /// Tries to retrieve an instance of the specified type from the IoC container.
    /// </summary>
    /// <typeparam name="TType">The type of the instance to retrieve.</typeparam>
    /// <param name="instance">When this method returns, contains the object from the container if the type was found; otherwise, the default value for the type.</param>
    /// <returns>true if the type was found in the container; otherwise, false.</returns>
    bool TryGet<TType>(out TType instance);


    /// <summary>
    /// Tries to retrieve an instance of the specified type from the IoC container.
    /// </summary>
    /// <param name="type">The type of the instance to retrieve.</param>
    /// <param name="instance">When this method returns, contains the object from the container if the type was found; otherwise, the default value for the type.</param>
    /// <returns>true if the type was found in the container; otherwise, false.</returns>
    bool TryGet(Type type, out object instance);

    /// <summary>
    /// Constructs an object of the specified type using IoC (Inversion of Control) container with optional parameters.
    /// </summary>
    /// <typeparam name="TType">The type of object to construct.</typeparam>
    /// <param name="parameters">Optional parameters to be used for the construction of the object.</param>
    /// <returns>An instance of the specified type.</returns>
    TType IoCConstruct<TType>(object parameters = null);

    /// <summary>
    /// Constructs an object of the specified type using the Inversion of Control (IoC) pattern,
    /// optionally passing provided parameters to the constructor.
    /// </summary>
    /// <param name="type">The type of the object to construct.</param>
    /// <param name="parameters">Optional parameters to pass to the constructor.</param>
    /// <returns>An object of the specified type.</returns>
    object IoCConstruct(Type type, object parameters = null);
}