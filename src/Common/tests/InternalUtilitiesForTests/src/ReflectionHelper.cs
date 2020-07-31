﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System
{
    public static class ReflectionHelper
    {
        public static IEnumerable<object[]> GetPublicNotAbstractClasses<T>()
        {
            var types = typeof(T).Assembly.GetTypes().Where(type => IsPublicNonAbstract<T>(type));
            foreach (var type in types)
            {
                if (type.GetConstructor(
                    bindingAttr: BindingFlags.Public | BindingFlags.Instance,
                    binder: null,
                    types: Array.Empty<Type>(),
                    modifiers: null) == null)
                {
                    continue;
                }
                
                yield return new object[] { type };
            }
        }

        public static T InvokePublicConstructor<T>(Type type)
        {
            var ctor = type.GetConstructor(
                bindingAttr: BindingFlags.Public | BindingFlags.Instance,
                binder: null,
                types: Array.Empty<Type>(),
                modifiers: null);

            if (ctor == null)
            {
                return default;
            }

            return (T)ctor.Invoke(Array.Empty<object>());
        }

        public static bool IsPublicNonAbstract<T>(Type type)
        {
            return !type.IsAbstract && type.IsPublic && typeof(T).IsAssignableFrom(type);
        }
    }
}
