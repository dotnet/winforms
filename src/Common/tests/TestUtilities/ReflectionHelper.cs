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

    /// <summary>
    ///  Gets a nested type, and if it's a generic type definition, makes it a full type using the parent type's generic arguments.
    /// </summary>
    /// <param name="parentType">The parent type.</param>
    /// <param name="nestedTypeName">The name of the nested type.</param>
    /// <param name="genericTypes">Additional nested type parameters, if any.</param>
    /// <returns>Nested types.</returns>
    /// <exception cref="ArgumentException">Could not find the <paramref name="nestedTypeName"/>.</exception>
    /// <exception cref="NotImplementedException">An additional case still needs implemented.</exception>
    public static Type GetFullNestedType(
        this Type parentType,
        string nestedTypeName,
        params Span<Type> genericTypes)
    {
        Type nestedType = parentType.GetNestedType(nestedTypeName, BindingFlags.Public | BindingFlags.NonPublic)
            ?? throw new ArgumentException($"Could not find {nestedTypeName} in {parentType.Name}");

        if (!nestedType.IsTypeDefinition)
        {
            return nestedType;
        }

        if (parentType.IsGenericType)
        {
            Type[] parentTypes = parentType.GenericTypeArguments;
            Type[] nestedTypes = nestedType.GenericTypeArguments;

            if (nestedTypes.Length == 0)
            {
                // Only the parent types are needed.
                Type fullType = nestedType.MakeGenericType(parentTypes);
                return fullType;
            }
        }

        // Implementing the other cases is relatively trivial, leaving them until we have concrete usage.
        throw new NotImplementedException("Implement other cases as they occur");
    }
}
