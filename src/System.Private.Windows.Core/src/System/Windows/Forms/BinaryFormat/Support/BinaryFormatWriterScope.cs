// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization.BinaryFormat;
using System.Text;

namespace System.Windows.Forms.BinaryFormat;

internal readonly ref struct BinaryFormatWriterScope
{
    internal BinaryWriter Writer { get; }

    public BinaryFormatWriterScope(Stream stream)
    {
        Writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);
        // SerializationHeader
        Writer.Write((byte)RecordType.SerializedStreamHeader);
        Writer.Write(1); // root ID
        Writer.Write(1); // header ID
        Writer.Write(1); // major version
        Writer.Write(0); // minor version
    }

    public static implicit operator BinaryWriter(in BinaryFormatWriterScope scope) => scope.Writer;

    public void Dispose()
    {
        Writer.Write((byte)RecordType.MessageEnd);
        Writer.Dispose();
    }
}
