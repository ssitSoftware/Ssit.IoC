using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ssit.IoC.Impl;

internal class SingletonRegistration<TType>: ISingletonRegistration<TType> where TType: class
{
    private readonly IoCContainerBuilder _ioCContainerBuilder;
    private readonly IoCContainerBuilder.SingletonInfo _lastSingletonInfo;

    public SingletonRegistration(IoCContainerBuilder ioCContainerBuilder, IoCContainerBuilder.SingletonInfo lastSingletonInfo)
    {
        _ioCContainerBuilder = ioCContainerBuilder;
        _lastSingletonInfo = lastSingletonInfo;
    }

    public ISingletonRegistration<TType> OnCreated(Action<TType> action)
    {
        _lastSingletonInfo.OnCreated ??= new List<Action<object, IIoCContainer>>();
        _lastSingletonInfo.OnCreated.Add( (instance, _) => action((TType) instance));
        return this;
    }

    public ISingletonRegistration<TType> OnCreated(Action<TType, IIoCContainer> action)
    {
        _lastSingletonInfo.OnCreated ??= new List<Action<object, IIoCContainer>>();
        _lastSingletonInfo.OnCreated.Add( (instance, container) => action((TType) instance, container));
        return this;
    }

    public ISingletonRegistration<TType> AsImplementedInterfaces()
    {
        foreach(var type in typeof(TType).GetTypeInfo().ImplementedInterfaces)
        {
            _lastSingletonInfo.Interfaces.Add(type);
        }
        return this;
    }

    public IIoCContainer Parent => _ioCContainerBuilder.Parent;

    public IIoCContainerBuilder WithParent(IIoCContainer container) => _ioCContainerBuilder.WithParent(container);

    public IIoCContainerBuilder WithInstance<TImplementation>(TImplementation instance) where TImplementation : class => _ioCContainerBuilder.WithInstance(instance);

    public IIoCContainerBuilder As<TAbstract>() where TAbstract : class => _ioCContainerBuilder.As<TAbstract>();

    public ISingletonRegistration<TImplementation> WithSingleton<TAbstract, TImplementation>(object parameter = null) where TAbstract : class where TImplementation : class, TAbstract => _ioCContainerBuilder.WithSingleton<TAbstract, TImplementation>(parameter);

    public IIoCContainerBuilder WithImplementation(Type @abstract, Type implementation) => _ioCContainerBuilder.WithImplementation(@abstract, implementation);

    public IIoCContainerBuilder WithImplementation<TAbstract, TImplementation>() where TAbstract : class where TImplementation : class, TAbstract => _ioCContainerBuilder.WithImplementation<TAbstract, TImplementation>();

    public IIoCContainerBuilder WithImplementation<TAbstract, TImplementation>(string key) where TAbstract : class where TImplementation : class, TAbstract => _ioCContainerBuilder.WithImplementation<TAbstract, TImplementation>(key);
    public IIoCContainerBuilder WithPostBuildDelegate<TAbstract>(Action<TAbstract> postBuildDelegate) where TAbstract : class => _ioCContainerBuilder.WithPostBuildDelegate(postBuildDelegate);
    public IIoCContainerBuilder WithPostBuildDelegate<TAbstract>(Action<TAbstract, IIoCContainer> postBuildDelegate) where TAbstract : class => _ioCContainerBuilder.WithPostBuildDelegate(postBuildDelegate);

    public IIoCContainerBuilder WithPostBuildDelegate(Action<IIoCContainer> postBuildDelegate) => _ioCContainerBuilder.WithPostBuildDelegate(postBuildDelegate);
    public bool IsRegistered(Type type) => _ioCContainerBuilder.IsRegistered(type);

    public IIoCContainer Build() => _ioCContainerBuilder.Build();

    public IImplementationMapper ImplementationMapper => _ioCContainerBuilder.ImplementationMapper;
}