// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class Control
{
    /// <summary>
    ///  Gets the service object of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the service object to get.</typeparam>
    /// <returns>A service object of type <typeparamref name="T"/> or <see langword="null"/> if there is no such service.</returns>
    internal T? GetService<T>()
        where T : class
        => GetService(typeof(T)) as T;

    /// <summary>
    ///  Gets the service object of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the service object to get.</typeparam>
    /// <param name="service">
    ///  If found, contains the service object when this method returns; otherwise, <see langword="null"/>.
    /// </param>
    /// <returns>
    ///  A service object of type <typeparamref name="T"/> or <see langword="null"/> if there is no such service.
    /// </returns>
    internal bool TryGetService<T>(
        [NotNullWhen(true)] out T? service)
        where T : class
        => (service = GetService(typeof(T)) as T) is not null;
}
