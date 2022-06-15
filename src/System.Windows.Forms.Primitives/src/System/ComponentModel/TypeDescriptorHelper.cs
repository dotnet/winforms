// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace System.ComponentModel
{
    internal static class TypeDescriptorHelper
    {
        public static bool TryGetAttribute<T>(
            object component,
            [NotNullWhen(true)] out T? attribute) where T : Attribute
        {
            attribute = TypeDescriptor.GetAttributes(component)[typeof(T)] as T;
            return attribute is not null;
        }
    }
}
