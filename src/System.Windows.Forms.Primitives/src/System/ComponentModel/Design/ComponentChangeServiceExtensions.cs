// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design;

internal static class ComponentChangeServiceExtensions
{
    public static void OnComponentChanged(
        this IComponentChangeService changeService,
        object component,
        MemberDescriptor? member = null,
        object? oldValue = null,
        object? newValue = null)
    {
        changeService.OnComponentChanged(component, member, oldValue, newValue);
    }

    public static void OnComponentChanging(
        this IComponentChangeService changeService,
        object component,
        MemberDescriptor? member = null)
    {
        changeService.OnComponentChanging(component, member);
    }
}
