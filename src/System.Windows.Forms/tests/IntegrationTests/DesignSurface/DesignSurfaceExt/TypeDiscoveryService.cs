// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Reflection;

namespace System.ComponentModel.Design;

/// <summary>
///  This service is requested by <see cref="TypeDescriptor" /> when asking for type information for a component.
///  This is a sample implementation suitable for this sample application.
/// </summary>
internal sealed class TypeDiscoveryService : ITypeDiscoveryService
{
    public TypeDiscoveryService() { }

    private readonly ConcurrentDictionary<Type, ImmutableArray<Type>> _discoveredTypesCache = new();

    public ICollection GetTypes(Type baseType, bool excludeGlobalTypes)
    {
        return baseType is null
            ? throw new ArgumentNullException(nameof(baseType))
            : (ICollection)_discoveredTypesCache.GetOrAdd(baseType, type => FindTypes(type, AppDomain.CurrentDomain.GetAssemblies()));

        static ImmutableArray<Type> FindTypes(Type baseType, Assembly[] assemblies)
        {
            var builder = ImmutableArray.CreateBuilder<Type>();

            foreach (var assembly in assemblies)
            {
                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException exception)
                {
                    types = exception.Types!;
                }

                foreach (var type in types)
                {
                    if (baseType.IsAssignableFrom(type))
                    {
                        builder.Add(type);
                    }
                }
            }

            return builder.ToImmutable();
        }
    }
}
