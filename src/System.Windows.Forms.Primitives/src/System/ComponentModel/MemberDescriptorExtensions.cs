// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
}
