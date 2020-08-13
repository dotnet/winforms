// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace System
{
    public static class ReflectionHelper
    {
        public static IEnumerable<Type> GetPublicNotAbstractClasses<T>()
        {
            var types = typeof(T).Assembly.GetTypes().Where(type => IsPublicNonAbstract<T>(type));
            foreach (var type in types)
            {
                if (type.GetConstructor(
                    bindingAttr: BindingFlags.Public | BindingFlags.Instance,
                    binder: null,
                    types: Array.Empty<Type>(),
                    modifiers: null) is null)
                {
                    continue;
                }

                yield return type;
            }
        }

        public static T InvokePublicConstructor<T>(Type type)
        {
            var ctor = type.GetConstructor(
                bindingAttr: BindingFlags.Public | BindingFlags.Instance,
                binder: null,
                types: Array.Empty<Type>(),
                modifiers: null);

            if (ctor is null)
            {
                return default;
            }

            T obj = (T)ctor.Invoke(Array.Empty<object>());
            Assert.NotNull(obj);
            return obj;
        }

        private static bool IsPublicNonAbstract<T>(Type type)
        {
            return !type.IsAbstract && type.IsPublic && typeof(T).IsAssignableFrom(type);
        }
    }
}
