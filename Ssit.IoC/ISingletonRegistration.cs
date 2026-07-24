using System;

namespace Ssit.IoC;

public interface ISingletonRegistration<out TImplementation>: IIoCContainerBuilder where TImplementation : class
{
    ISingletonRegistration<TImplementation> OnCreated(Action<TImplementation> action);
    ISingletonRegistration<TImplementation> OnCreated(Action<TImplementation, IIoCContainer> action);
    ISingletonRegistration<TImplementation> AsImplementedInterfaces();
}