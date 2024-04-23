﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.BinaryFormat;

/// <summary>
///  Single dimensional array of objects.
/// </summary>
/// <remarks>
///  <para>
///   <see href="https://learn.microsoft.com/openspecs/windows_protocols/ms-nrbf/982b2f50-6367-402a-aaf2-44ee96e2a5e0">
///    [MS-NRBF] 2.4.3.2
///   </see>
///  </para>
/// </remarks>
internal sealed class ArraySingleObject :
    ArrayRecord<object?>,
    IRecord<ArraySingleObject>,
    IBinaryFormatParseable<ArraySingleObject>
{
    public static RecordType RecordType => RecordType.ArraySingleObject;

    public ArraySingleObject(Id objectId, IReadOnlyList<object?> arrayObjects)
        : base(new ArrayInfo(objectId, arrayObjects.Count), arrayObjects)
    { }

    static ArraySingleObject IBinaryFormatParseable<ArraySingleObject>.Parse(
        BinaryFormattedObject.ParseState state)
    {
        ArraySingleObject record = new(
            ArrayInfo.Parse(state.Reader, out Count length),
            ReadRecords(state, length));

        state.RecordMap[record.ObjectId] = record;
        return record;
    }

    public override void Write(BinaryWriter writer)
    {
        writer.Write((byte)RecordType);
        ArrayInfo.Write(writer);
        WriteRecords(writer, ArrayObjects);
    }
}
