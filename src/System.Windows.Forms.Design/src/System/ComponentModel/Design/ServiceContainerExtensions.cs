// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design;

internal static class ServiceContainerExtensions
{
    public static void AddService<T>(this IServiceContainer serviceContainer, T serviceInstance) where T : class =>
        serviceContainer.AddService(typeof(T), serviceInstance);

    public static void AddService<T>(this IServiceContainer serviceContainer, ServiceCreatorCallback callback) =>
        serviceContainer.AddService(typeof(T), callback);

    public static void RemoveService<T>(this IServiceContainer serviceContainer) => serviceContainer.RemoveService(typeof(T));
}
