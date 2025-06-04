// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Reflection;
using System.Resources;

namespace System.ComponentModel.Design.Serialization;

public sealed partial class CodeDomComponentSerializationService
{
    private sealed partial class CodeDomSerializationStore
    {
        /// <summary>
        ///  This is a serialization manager that can load assemblies and search for types
        ///  and provide a resource manager from our serialization store.
        /// </summary>
        private class LocalDesignerSerializationManager : DesignerSerializationManager
        {
            private readonly CodeDomSerializationStore _store;
            private bool? _typeSvcAvailable;

            /// <summary>
            ///  Creates a new serialization manager.
            /// </summary>
            internal LocalDesignerSerializationManager(CodeDomSerializationStore store, IServiceProvider provider) : base(provider)
            {
                _store = store;
            }

            /// <summary>
            ///  We override CreateInstance here to provide a hook to our resource manager.
            /// </summary>
            protected override object CreateInstance(Type type, ICollection? arguments, string? name, bool addToContainer)
            {
                if (typeof(ResourceManager).IsAssignableFrom(type))
                {
                    return _store.Resources;
                }

                return base.CreateInstance(type, arguments, name, addToContainer);
            }

            private bool TypeResolutionAvailable => _typeSvcAvailable ??= GetService(typeof(ITypeResolutionService)) is not null;

            /// <summary>
            ///  Override of GetType. We favor the base implementation first,
            ///  which uses the type resolution service if it is available.
            ///  If that fails, we will try to load assemblies from the given array of assembly names.
            /// </summary>
            protected override Type? GetType(string? name)
            {
                Type? t = base.GetType(name);
                if (t is not null || TypeResolutionAvailable)
                {
                    return t;
                }

                AssemblyName[] names = _store.AssemblyNames!;
                // First try the assembly names directly.
                foreach (AssemblyName n in names)
                {
                    Assembly a = Assembly.Load(n);
                    t = a?.GetType(name!);
                    if (t is not null)
                    {
                        return t;
                    }
                }

                // Failing that go after their dependencies.
                foreach (AssemblyName n in names)
                {
                    Assembly a = Assembly.Load(n);
                    if (a is not null)
                    {
                        foreach (AssemblyName dep in a.GetReferencedAssemblies())
                        {
                            Assembly aDep = Assembly.Load(dep);
                            t = aDep?.GetType(name!);
                            if (t is not null)
                            {
                                return t;
                            }
                        }
                    }
                }

                return t;
            }
        }
    }
}
