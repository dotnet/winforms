// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization.Formatters.Binary;

namespace System.Windows.Forms.BinaryFormat.Tests;

internal static class TestExtensions
{
    /// <summary>
    ///  Serializes the object using the <see cref="BinaryFormatter"/> and reads it into a <see cref="BinaryFormattedObject"/>.
    /// </summary>
    public static BinaryFormattedObject SerializeAndParse(this object source) => new(source.Serialize());

    /// <summary>
    ///  Serializes the object using the <see cref="BinaryFormatter"/>.
    /// </summary>
    public static Stream Serialize(this object source)
    {
        MemoryStream stream = new();
        using var formatterScope = new BinaryFormatterScope(enable: true);
#pragma warning disable SYSLIB0011 // Type or member is obsolete
        BinaryFormatter formatter = new();
#pragma warning restore SYSLIB0011
        formatter.Serialize(stream, source);
        stream.Position = 0;
        return stream;
    }
}
