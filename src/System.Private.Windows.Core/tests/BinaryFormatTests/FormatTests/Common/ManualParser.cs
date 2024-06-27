// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization.Formatters.Binary;
using System.Private.Windows.Core.BinaryFormat;
using Record = System.Private.Windows.Core.BinaryFormat.Record;

namespace FormatTests.Common;

/// <summary>
///  Helper for manually looking through records.
/// </summary>
public sealed class ManualParser : BinaryFormattedObject.IParseState, IDisposable
{
    private readonly RecordMap _recordMap;
    private readonly BinaryFormattedObject.Options _options = new();

    /// <summary>
    ///  Creates a parse
    /// </summary>
    /// <param name="object"></param>
    public ManualParser(object @object)
    {
        BinaryFormatter formatter = new();
        MemoryStream stream = new();
        formatter.Serialize(stream, @object);
        stream.Position = 0;
        Reader = new(stream);

        _recordMap = new();
    }

    public BinaryReader Reader { get; }

    RecordMap BinaryFormattedObject.IParseState.RecordMap => _recordMap;

    BinaryFormattedObject.Options BinaryFormattedObject.IParseState.Options => _options;

    BinaryFormattedObject.ITypeResolver BinaryFormattedObject.IParseState.TypeResolver => throw new NotSupportedException();

    internal IRecord ReadRecord() => Record.ReadBinaryFormatRecord(this);

    public void Dispose() => Reader.Dispose();
}
