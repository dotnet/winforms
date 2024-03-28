// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32;
using Windows.Win32.System.Com;

namespace System.IO;

internal static class StreamExtensions
{
    /// <summary>
    ///  Get a <see cref="IStream"/> wrapper around the given <paramref name="stream"/>. Use the return value
    ///  in a <see langword="using"/> scope.
    /// </summary>
    internal static ComScope<IStream> ToIStream(this Stream stream, bool makeSeekable = false)
    {
        ArgumentNullException.ThrowIfNull(stream);
        return ComHelpers.GetComScope<IStream>(new ComManagedStream(stream, makeSeekable));
    }
}
