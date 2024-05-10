﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;

#pragma warning disable SYSLIB0050 // Type or member is obsolete

namespace System.Windows.Forms.BinaryFormat;

internal sealed partial class BinaryFormattedObject
{
    internal sealed class DefaultTypeResolver : ITypeResolver
    {
        private readonly FormatterAssemblyStyle _assemblyMatching;
        private readonly SerializationBinder? _binder;

        private readonly Dictionary<string, Assembly> _assemblies = [];
        private readonly Dictionary<(string TypeName, string LibraryId), Type> _types = [];

        internal DefaultTypeResolver(Options options)
        {
            _assemblyMatching = options.AssemblyMatching;
            _binder = options.Binder;
        }

        /// <summary>
        ///  Resolves the given type name against the specified library.
        /// </summary>
        /// <param name="libraryName">The library id, or <see cref="Id.Null"/> for the "system" assembly.</param>
        [RequiresUnreferencedCode("Calls System.Reflection.Assembly.GetType(String)")]
        [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type ITypeResolver.GetType(TypeName typeName, AssemblyNameInfo libraryName)
        {
            if (_types.TryGetValue((typeName.FullName, libraryName.FullName), out Type? cachedType))
            {
                return cachedType;
            }

            if (_binder?.BindToType(libraryName.FullName, typeName.FullName) is Type binderType)
            {
                // BinaryFormatter is inconsistent about what caching behavior you get with binders.
                // It would always cache the last item from the binder, but wouldn't put the result
                // in the type cache. This could lead to inconsistent results if the binder didn't
                // always return the same result for a given set of strings. Choosing to always cache
                // for performance.

                _types[(typeName.FullName, libraryName.FullName)] = binderType;
                return binderType;
            }

            if (!_assemblies.TryGetValue(libraryName.FullName, out Assembly? assembly))
            {
                Debug.Assert(libraryName.FullName != typeof(object).Assembly.FullName);

                AssemblyName assemblyName = libraryName.ToAssemblyName();
                try
                {
                    assembly = Assembly.Load(assemblyName);
                }
                catch
                {
                    if (_assemblyMatching != FormatterAssemblyStyle.Simple)
                    {
                        throw;
                    }

                    assembly = Assembly.Load(assemblyName.Name!);
                }

                _assemblies.Add(libraryName.FullName, assembly);
            }

            Type? type = _assemblyMatching != FormatterAssemblyStyle.Simple
                ? assembly.GetType(typeName.FullName)
                : GetSimplyNamedTypeFromAssembly(assembly, typeName);

            _types[(typeName.FullName, libraryName.FullName)] = type ?? throw new SerializationException($"Could not find type '{typeName}'.");
            return type;
        }

        [RequiresUnreferencedCode("Calls System.Reflection.Assembly.GetType(String, Boolean, Boolean)")]
        private static Type? GetSimplyNamedTypeFromAssembly(Assembly assembly, TypeName typeName)
        {
            // Catching any exceptions that could be thrown from a failure on assembly load
            // This is necessary, for example, if there are generic parameters that are qualified
            // with a version of the assembly that predates the one available.

            try
            {
                return assembly.GetType(typeName.FullName, throwOnError: false, ignoreCase: false);
            }
            catch (TypeLoadException) { }
            catch (FileNotFoundException) { }
            catch (FileLoadException) { }
            catch (BadImageFormatException) { }

            return Type.GetType(typeName.FullName, ResolveSimpleAssemblyName, new TopLevelAssemblyTypeResolver(assembly).ResolveType, throwOnError: false);

            static Assembly? ResolveSimpleAssemblyName(AssemblyName assemblyName)
            {
                try
                {
                    return Assembly.Load(assemblyName);
                }
                catch { }

                try
                {
                    return Assembly.Load(assemblyName.Name!);
                }
                catch { }

                return null;
            }
        }

        private sealed class TopLevelAssemblyTypeResolver
        {
            private readonly Assembly _topLevelAssembly;

            public TopLevelAssemblyTypeResolver(Assembly topLevelAssembly) => _topLevelAssembly = topLevelAssembly;

            [RequiresUnreferencedCode("Calls System.Reflection.Assembly.GetType(String, Boolean, Boolean)")]
            public Type? ResolveType(Assembly? assembly, string simpleTypeName, bool ignoreCase)
            {
                assembly ??= _topLevelAssembly;
                return assembly.GetType(simpleTypeName, throwOnError: false, ignoreCase);
            }
        }
    }
}

#pragma warning restore SYSLIB0050 // Type or member is obsolete
