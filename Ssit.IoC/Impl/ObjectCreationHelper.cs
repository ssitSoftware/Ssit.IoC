using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ssit.IoC.Impl;

internal static class ObjectCreationHelper
{
    public delegate bool ResolveDelegate(Type type, out object instance);
        
    public static object CreateObject(Type type, object creationParameters, ResolveDelegate resolveDelegate)
    {
        if (type.IsAbstract)
        {
            throw new InvalidOperationException($"Cannot instantiate abstract class {type.FullName}.");
        }

        var constructors = type.GetConstructors();
        var parametersList = new List<object>();

        foreach (var constructor in constructors)
        {
            if (MatchConstructor(constructor, resolveDelegate, creationParameters, parametersList))
            {
                return constructor.Invoke(parametersList.ToArray());
            }
        }

        throw new InvalidOperationException($"Cannot match constructor for {type.FullName}.");
    }

    private static bool MatchConstructor(ConstructorInfo constructor, ResolveDelegate resolveDelegate, object creationParameters, List<object> parametersList)
    {            

        parametersList.Clear();

        var parameters = constructor.GetParameters();
        if (creationParameters != null && !parameters.Any(o => o.ParameterType.IsInstanceOfType(creationParameters)))
        {
            return false;
        }

        foreach (var parameter in parameters)
        {
            if (!MatchParameter(parameter.ParameterType, resolveDelegate, creationParameters, out var instance) && !parameter.HasDefaultValue)
            {
                return false;
            }
            parametersList.Add(instance);
        }

        return true;
    }

    private static bool MatchParameter(Type type, ResolveDelegate resolveDelegate, object creationParameters, out object instance)
    {
        if (creationParameters != null)
        {
            if (type.IsInstanceOfType(creationParameters))
            {
                instance = creationParameters;
                return true;
            }
        }

        return resolveDelegate(type, out instance);
    }
}