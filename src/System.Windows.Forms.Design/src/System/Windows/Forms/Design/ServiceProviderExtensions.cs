// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design;

internal static class ServiceProviderExtensions
{
    public static T? GetService<T>(this IServiceProvider serviceProvider) where T : class
        => serviceProvider?.GetService(typeof(T)) as T;
}
