// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel;

internal static class PropertyDescriptorExtensions
{
    public static bool TryGetValue<T>(this PropertyDescriptor descriptor, object? component, out T? value)
    {
        if (typeof(T).IsAssignableFrom(descriptor.PropertyType))
        {
            value = (T?)descriptor.GetValue(component);

            return true;
        }

        value = default;

        return false;
    }

    public static T? GetValue<T>(this PropertyDescriptor descriptor, object? component) where T : class
    {
        if (typeof(T).IsAssignableFrom(descriptor.PropertyType))
        {
            return (T?)descriptor.GetValue(component);
        }

        return null;
    }

    public static T? GetEditor<T>(this PropertyDescriptor descriptor) => (T?)descriptor.GetEditor(typeof(T));

    public static bool TryGetEditor<T>(this PropertyDescriptor descriptor, [NotNullWhen(true)] out T? value)
    {
        value = (T?)descriptor.GetEditor(typeof(T));

        return value is not null;
    }
}
