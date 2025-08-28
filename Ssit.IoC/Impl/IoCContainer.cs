using System;
using System.Collections.Generic;

namespace Ssit.IoC.Impl;

internal class IoCContainer : IIoCContainer, IImplementationMapper
{
    public IIoCContainer Parent { get; set; }
    public IImplementationMapper ParentImplementationMapper { get; set; }

    private readonly List<IDisposable> _disposables = new();
    
    private readonly Dictionary<Type, object> _instances = new();

    private readonly Dictionary<Type, Type> _implementations = new();
    private readonly Dictionary<(string, Type), Type> _keyedImplementations = new();

    public void Register(Type type, object instance)
    {
        _instances.Add(type, instance);

        if (instance is IDisposable disposable && !ReferenceEquals(disposable, this))
        {
            if (!_disposables.Contains(disposable))
            {
                _disposables.Add(disposable);
            }
        }
    }

    public void RegisterImplementation(Type type, Type impl, string key = null)
    {
        if (key == null)
        {
            _implementations.Add(type, impl);
        }
        else
        {
            _keyedImplementations.Add((key, type), impl);
        }
    }
        
    public void Dispose()
    {
        for (var idx = _disposables.Count - 1; idx >= 0; idx--)
        {
            _disposables[idx].Dispose();
        }
        _disposables.Clear();
        _instances.Clear();

        GC.Collect();
    }

    public TType Get<TType>()
    {
        if (!TryGet<TType>(out var instance))
        {
            throw new KeyNotFoundException();
        }

        return instance;
    }

    public object Get(Type type)
    {
        if (!TryGet(type, out var instance))
        {
            throw new KeyNotFoundException();
        }

        return instance;
    }

    public bool TryGet<TType>(out TType instance)
    {
        if (TryGet(typeof(TType), out var @object))
        {
            instance = (TType) @object;
            return true;
        }

        instance = default;
        return false;
    }

    public bool TryGet(Type type, out object instance)
    {
        if (_instances.TryGetValue(type, out instance))
        {
            return true;
        }

        return Parent?.TryGet(type, out instance) ?? false;
    }

    public TType IoCConstruct<TType>(object parameters = null) => (TType) IoCConstruct(typeof(TType), parameters);

    public object IoCConstruct(Type type, object parameters = null)
    {
        if (type.IsAbstract)
        {
            type = ResolveImplementation(type, null);
        }

        return ObjectCreationHelper.CreateObject(type, parameters, TryGet);
    }

    private bool TryGetImplementation(Type abstractType, string key, out Type type)
    {
        if (key is null)
        {
            return _implementations.TryGetValue(abstractType, out type);
        }
        return _keyedImplementations.TryGetValue((key, abstractType), out type);
    }
    
    public Type ResolveImplementation(Type abstractType, string key = null)
    {
        if (TryGetImplementation(abstractType, key, out var type))
        {
            return type;
        }

        if (abstractType.IsGenericTypeDefinition && TryGetImplementation(abstractType.GetGenericTypeDefinition(), key, out type))
        {
            return type.MakeGenericType(abstractType.GetGenericArguments());
        }

        if (Parent is null)
        {
            throw new KeyNotFoundException();
        }

        return ParentImplementationMapper.ResolveImplementation(abstractType, key);
    }

    public Type ResolveImplementation<TType>(string key = null) => ResolveImplementation(typeof(TType), key);
}