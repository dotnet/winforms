// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel;

internal static class PropertyDescriptorCollectionExtensions
{
    internal static bool TryGetPropertyDescriptorValue<T>(
        this PropertyDescriptorCollection propertyDescriptors,
        string name,
        IComponent component,
        ref T value)
    {
        PropertyDescriptor? propertyDescriptor = propertyDescriptors[name];
        if (propertyDescriptor is not null)
        {
            value = (T)propertyDescriptor.GetValue(component)!;

            return true;
        }

        return false;
    }
}
