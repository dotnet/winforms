// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;

namespace System;

public static class ReflectionHelper
{
    public static IEnumerable<Type> GetPublicNotAbstractClasses<T>()
    {
        var types = typeof(T).Assembly.GetTypes().Where(IsPublicNonAbstract<T>);
        foreach (var type in types)
        {
            if (type.GetConstructor(
                bindingAttr: BindingFlags.Public | BindingFlags.Instance,
                binder: null,
                types: [],
                modifiers: null) is null)
            {
                continue;
            }

            yield return type;
        }
    }

    public static T? InvokePublicConstructor<T>(Type type)
    {
        var ctor = type.GetConstructor(
            bindingAttr: BindingFlags.Public | BindingFlags.Instance,
            binder: null,
            types: [],
            modifiers: null);

        if (ctor is null)
        {
            return default;
        }

        T obj = (T)ctor.Invoke([]);
        Assert.NotNull(obj);
        return obj;
    }

    private static bool IsPublicNonAbstract<T>(Type type)
    {
        return !type.IsAbstract && type.IsPublic && typeof(T).IsAssignableFrom(type);
    }
}
