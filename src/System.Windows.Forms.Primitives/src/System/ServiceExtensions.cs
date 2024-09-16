// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Primitives.Resources;

namespace System;

internal static class ServiceExtensions
{
    /// <summary>
    ///  Gets the service object of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the service object to get.</typeparam>
    /// <param name="provider">The <see cref="IServiceProvider"/> to retrieve the service object from.</param>
    /// <returns>A service object of type <typeparamref name="T"/> or <see langword="null"/> if there is no such service.</returns>
    public static T? GetService<T>(this IServiceProvider provider)
        where T : class
        => provider.GetService(typeof(T)) as T;

    /// <summary>
    ///  Gets the service object of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the service object to get.</typeparam>
    /// <param name="provider">The <see cref="IServiceProvider"/> to retrieve the service object from.</param>
    /// <returns>A service object of type <typeparamref name="T"/>.</returns>
    /// <exception cref="InvalidOperationException"/>
    public static T GetRequiredService<T>(this IServiceProvider provider)
        where T : class
        => provider.GetRequiredService<T, T>();

    /// <summary>
    ///  Gets the service object of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the service object to get.</typeparam>
    /// <param name="provider">The <see cref="IServiceProvider"/> to retrieve the service object from.</param>
    /// <param name="service">
    ///  If found, contains the service object when this method returns; otherwise, <see langword="null"/>.
    /// </param>
    /// <returns>A service object of type <typeparamref name="T"/> or <see langword="null"/> if there is no such service.</returns>
    public static bool TryGetService<T>(
        [NotNullWhen(true)] this IServiceProvider? provider,
        [NotNullWhen(true)] out T? service)
        where T : class
        => (service = provider?.GetService(typeof(T)) as T) is not null;

    /// <summary>
    ///  Gets the service object of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the service object to get.</typeparam>
    /// <param name="designerHost">The <see cref="IDesignerHost"/> to retrieve the service object from.</param>
    /// <param name="service">
    ///  If found, contains the service object when this method returns; otherwise, <see langword="null"/>.
    /// </param>
    /// <returns>A service object of type <typeparamref name="T"/> or <see langword="null"/> if there is no such service.</returns>
    public static bool TryGetService<T>(
        [NotNullWhen(true)] this IDesignerHost? designerHost,
        [NotNullWhen(true)] out T? service)
        where T : class
        => (service = designerHost?.GetService(typeof(T)) as T) is not null;

    /// <summary>
    ///  Gets the service object of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the service object to get.</typeparam>
    /// <param name="context">The <see cref="ITypeDescriptorContext"/> to retrieve the service object from.</param>
    /// <param name="service">
    ///  If found, contains the service object when this method returns; otherwise, <see langword="null"/>.
    /// </param>
    /// <returns>A service object of type <typeparamref name="T"/> or <see langword="null"/> if there is no such service.</returns>
    public static bool TryGetService<T>(
        [NotNullWhen(true)] this ITypeDescriptorContext? context,
        [NotNullWhen(true)] out T? service)
        where T : class
        => (service = context?.GetService(typeof(T)) as T) is not null;

    private static TInterface? GetService<TService, TInterface>(this IServiceProvider provider)
        where TService : class
        where TInterface : class
        => provider.GetService(typeof(TService)) as TInterface;

    private static TInterface GetRequiredService<TService, TInterface>(this IServiceProvider provider)
        where TService : class
        where TInterface : class
    {
        TInterface service = provider.GetService<TService, TInterface>()
            ?? throw new InvalidOperationException(string.Format(SR.General_MissingService, typeof(TInterface).FullName))
            {
                HelpLink = SR.General_MissingService
            };

        return service;
    }
}
