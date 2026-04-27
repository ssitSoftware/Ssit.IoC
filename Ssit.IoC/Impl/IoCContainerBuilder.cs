using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Ssit.IoC.Impl;

internal class IoCContainerBuilder: IIoCContainerBuilder
{
    private class SingletonInfo
    {
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
        public Type Type;
        public object Parameter;
        public List<Type> Interfaces;
    }
        
    private readonly IoCContainer _container = new();

    public IImplementationMapper ImplementationMapper => _container;
    private readonly Dictionary<Type, SingletonInfo> _singletonTypes = new();

    private object _lastInstance;
    private SingletonInfo _lastSingletonInfo;

    public IoCContainerBuilder()
    {
        _container.RegisterImplementation(typeof(IIoCContainerBuilder), typeof(IoCContainerBuilder));
        _container.Register(typeof(IIoCContainer), _container);
        _container.Register(typeof(IImplementationMapper), _container);
    }

    public IIoCContainer Parent => _container.Parent;

    public IIoCContainerBuilder WithParent(IIoCContainer container)
    {
        _container.Parent = container;
        _container.ParentImplementationMapper = container.Get<IImplementationMapper>();
        
        return this;
    }

    public IIoCContainerBuilder As<TAbstract>() where TAbstract : class
    {
        if (_lastSingletonInfo is not null)
        {
            _lastSingletonInfo.Interfaces.Add(typeof(TAbstract));
            _singletonTypes.Add(typeof(TAbstract), _lastSingletonInfo);
            return this;
        }
        
        if (_lastInstance is null)
        {
            throw new InvalidOperationException("Cannot register type as it has not been instantiated.");
        }

        if (!typeof(TAbstract).IsAssignableFrom(_lastInstance.GetType()))
        {
            throw new InvalidOperationException("Cannot register type as it is not assignable to the abstract type.");
        }
        
        _container.Register(typeof(TAbstract), _lastInstance);
        return this;
    }

    public IIoCContainerBuilder WithInstance<TType>(TType instance) where TType : class
    {
        _container.Register(typeof(TType), instance);
        _lastInstance = instance;
        _lastSingletonInfo = null;
        return this;
    }

    public IIoCContainerBuilder WithSingleton<TAbstract, TImplementation>(object parameter = null)
        where TAbstract : class where TImplementation : class, TAbstract
    {
        _lastSingletonInfo = new SingletonInfo
        {
            Type = typeof(TImplementation),
            Parameter = parameter,
            Interfaces = [typeof(TAbstract)]
        };

        _singletonTypes.Add(typeof(TAbstract), _lastSingletonInfo); 
        _lastInstance = null;
        return this;
    }
        
    public IIoCContainerBuilder WithImplementation(Type @abstract, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementation)
    {
        _container.RegisterImplementation(@abstract, implementation);
        _lastInstance = null;
        _lastSingletonInfo = null;
        return this;
    }

    public IIoCContainerBuilder WithImplementation<TAbstract, TImplementation>()
        where TAbstract : class where TImplementation : class, TAbstract
    {
        _container.RegisterImplementation(typeof(TAbstract), typeof(TImplementation));
        _lastInstance = null;
        _lastSingletonInfo = null;
        return this;
    }
    
    public IIoCContainerBuilder WithImplementation<TAbstract, TImplementation>(string key)
        where TAbstract : class where TImplementation : class, TAbstract
    {
        _container.RegisterImplementation(typeof(TAbstract), typeof(TImplementation), key);
        
        _lastInstance = null;
        _lastSingletonInfo = null;
        return this;
    }

    public bool IsRegistered(Type type)
    {
        if (_singletonTypes.TryGetValue(type, out _))
        {
            return true;
        }

        if (_container.TryGet(type, out _))
        {
            return true;
        }

        try
        {
            _container.ResolveImplementation(type);
            return true;
        }
        catch (KeyNotFoundException)
        {
            return false;
        }
    }

    public IIoCContainer Build()
    {
        _lastInstance = null;
        _lastSingletonInfo = null;
        
        while (_singletonTypes.Count > 0)
        {
            var pair = _singletonTypes.First();
            
            if (!TryGet(pair.Key, out _))
            {
                var name = pair.Value.Type.Name;
                throw new InvalidOperationException(
                    $"Cannot create instance of type {name} because its constructor requires not registered services.");
            }
        }
        return _container;
    }
        
    private bool TryGet(Type type, out object instance)
    {
        if (_container.TryGet(type, out instance))
        {
            _singletonTypes.Remove(type);
            return true;
        }
        
        if (!_singletonTypes.TryGetValue(type, out var implType) && type.IsAbstract)
        {
            throw new KeyNotFoundException($"Implementation for {type.FullName} could not be found.");
        }

        _singletonTypes.Remove(type);
        if ( type.IsAbstract && implType is null) throw new InvalidOperationException($"Circular dependency detected!");

        implType ??= new SingletonInfo
        {
            Type = type,
            Parameter = null,
            Interfaces = [type]
        };

        try
        {
            instance = ObjectCreationHelper.CreateObject(implType.Type, implType.Parameter, TryGet);

            foreach (var @interface in implType.Interfaces)
            {
                _container.Register(@interface, instance); 
                _singletonTypes.Remove(@interface);
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
}