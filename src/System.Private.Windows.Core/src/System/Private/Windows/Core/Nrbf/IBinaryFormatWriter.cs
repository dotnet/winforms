// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Private.Windows.Core.Nrbf;

/// <summary>
///  An interface identifying an object that handles serializing supported types.
/// </summary>
internal interface IBinaryFormatWriter
{
    /// <summary>
    ///  Tries to binary format <paramref name="value"/> and write into <paramref name="stream"/> if the type is supported.
    /// </summary>
    /// <param name="stream">The stream to write the binary formatted data into</param>
    /// <param name="value">object to binary format</param>
    /// <returns>
    ///  <see langword="true"/> if <paramref name="value"/> is supported and can be written to the stream. Otherwise, <see langword="false"/>.
    /// </returns>
    bool TryWriteCommonObject(Stream stream, object value);
}
