﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel;

internal static class TypeDescriptorHelper
{
    public static bool TryGetAttribute
        <[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicFields)] T>(
        object component,
        [NotNullWhen(true)] out T? attribute) where T : Attribute
    {
        attribute = TypeDescriptor.GetAttributes(component)[typeof(T)] as T;
        return attribute is not null;
    }

    public static bool TryGetPropertyValue<T>(
        object component,
        string name,
        out T? value)
    {
        PropertyDescriptor? property = TypeDescriptor.GetProperties(component)[name];
        if (property is not null && property.TryGetValue(component, out value))
        {
            return true;
        }

        value = default;
        return false;
    }
}
