// Licensed to the .NET Foundation under one or more agreements.
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

    public static bool TryGetAttribute
        <[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicFields)] T>(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type componentType,
        [NotNullWhen(true)] out T? attribute) where T : Attribute
    {
        attribute = TypeDescriptor.GetAttributes(componentType)[typeof(T)] as T;
        return attribute is not null;
    }

    public static T? GetEditor<T>([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type)
    {
        return (T?)TypeDescriptor.GetEditor(type, typeof(T));
    }

    public static bool TryGetEditor<T>(object component, [NotNullWhen(true)] out T? editor) where T : class
    {
        editor = TypeDescriptor.GetEditor(component, typeof(T)) as T;
        return editor is not null;
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
