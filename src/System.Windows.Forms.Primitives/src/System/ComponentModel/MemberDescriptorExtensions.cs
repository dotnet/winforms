// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace System.ComponentModel
{
    internal static class MemberDescriptorExtensions
    {
        public static bool TryGetAttribute<T>(
            this MemberDescriptor descriptor,
            [NotNullWhen(true)] out T? attribute) where T : Attribute
        {
            attribute = descriptor?.Attributes[typeof(T)] as T;
            return attribute is not null;
        }

        public static T? GetAttribute<T>(this MemberDescriptor descriptor) where T : Attribute
            => descriptor?.Attributes[typeof(T)] as T;
    }
}
