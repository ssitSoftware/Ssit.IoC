using System;

namespace Ssit.IoC;

/// <summary>
/// Interface for building an Inversion of Control (IoC) container.
/// </summary>
public interface IIoCContainerBuilder
{
    /// Adds a parent container to the current IoC container builder.
    /// <param name="container">The parent container to be associated.</param>
    /// <returns>The current IoC container builder with the parent container set.</returns>
    IIoCContainerBuilder WithParent(IIoCContainer container);


    /// Registers an instance of the specified type to the IoC container.
    /// <param name="instance">The instance to be registered in the IoC container.</param>
    /// <typeparam name="TType">The type of the instance being registered. Must be a reference type.</typeparam>
    /// <returns>The current IoC container builder with the instance registered.</returns>
    IIoCContainerBuilder WithInstance<TType>(TType instance) where TType : class;

    /// Specifies the type this instance should be registered as in the IoC container.
    /// <typeparam name="TAbstract">The abstract type to register the instance as.</typeparam>
    /// <returns>The current IoC container builder with the specified type registration applied.</returns>
    IIoCContainerBuilder As<TAbstract>() where TAbstract : class;
    
    /// <summary>
    /// Registers a singleton implementation for a given abstract type in the IoC container.
    /// </summary>
    /// <typeparam name="TAbstract">The abstract type to be registered.</typeparam>
    /// <typeparam name="TImplementation">The concrete type implementing the abstract type.</typeparam>
    /// <param name="parameter">Optional parameter to be passed to the singleton instance.</param>
    /// <returns>The IoC container builder with the singleton registration.</returns>
    IIoCContainerBuilder WithSingleton<TAbstract, TImplementation>(object parameter = null)
        where TAbstract : class where TImplementation : class, TAbstract;

    /// <summary>
    /// Registers a specific implementation type for a given abstract type.
    /// </summary>
    /// <param name="abstract">The abstract type or interface to register.</param>
    /// <param name="implementation">The concrete implementation type to use for the given abstract type.</param>
    /// <returns>An instance of the <see cref="IIoCContainerBuilder"/>.</returns>
    IIoCContainerBuilder WithImplementation(Type @abstract, Type implementation);

    /// <summary>
    /// Registers a concrete type (implementation) to be associated with an abstract type (interface or base class).
    /// </summary>
    /// <typeparam name="TAbstract">The abstract type, such as an interface or base class.</typeparam>
    /// <typeparam name="TImplementation">The concrete type that implements or extends the abstract type.</typeparam>
    /// <returns>The <see cref="IIoCContainerBuilder"/> instance to allow for method chaining.</returns>
    IIoCContainerBuilder WithImplementation<TAbstract, TImplementation>()
        where TAbstract : class where TImplementation : class, TAbstract;

    IIoCContainerBuilder WithImplementation<TAbstract, TImplementation>(string key)
        where TAbstract : class where TImplementation : class, TAbstract;
    
    /// <summary>
    /// Checks if the specified type is registered in the IoC container.
    /// </summary>
    /// <param name="type">The type to check for registration.</param>
    /// <returns>True if the type is registered, false otherwise.</returns>
    bool IsRegistered(Type type);

    /// <summary>
    /// Constructs and configures an IoC container based on the current builder configuration.
    /// </summary>
    /// <returns>A fully configured instance of an IoC container.</returns>
    IIoCContainer Build();

    /// <summary>
    /// Gets the implementation mapper associated with the IoC container builder.
    /// </summary>
    /// <remarks>
    /// The implementation mapper is responsible for resolving the implementation types
    /// associated with abstract types within the IoC container.
    /// </remarks>
    IImplementationMapper ImplementationMapper { get; }
}