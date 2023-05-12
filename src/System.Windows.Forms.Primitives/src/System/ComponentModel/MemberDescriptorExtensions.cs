// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ComponentModel;

internal static class MemberDescriptorExtensions
{
    public static bool TryGetAttribute
        <[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicFields)] T>(
        this MemberDescriptor descriptor,
        [NotNullWhen(true)] out T? attribute) where T : Attribute
    {
        attribute = descriptor?.Attributes[typeof(T)] as T;
        return attribute is not null;
    }

    public static T? GetAttribute
        <[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicFields)] T>(
        this MemberDescriptor descriptor) where T : Attribute
        => descriptor?.Attributes[typeof(T)] as T;

    public static bool TryGetValue<T>(this PropertyDescriptor descriptor, object? component, out T? value)
    {
        if (descriptor.PropertyType == typeof(T))
        {
            value = (T?)descriptor.GetValue(component);
            return true;
        }

        value = default;
        return false;
    }

    public static T? GetValue<T>(this PropertyDescriptor descriptor, object? component) where T : class
    {
        if (descriptor.PropertyType == typeof(T))
        {
            return (T?)descriptor.GetValue(component);
        }

        return null;
    }
}
