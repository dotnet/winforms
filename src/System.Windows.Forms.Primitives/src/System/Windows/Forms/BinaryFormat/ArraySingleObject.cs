// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.BinaryFormat;

/// <summary>
///  Single dimensional array of objects.
/// </summary>
/// <remarks>
///  <para>
///   <see href="https://learn.microsoft.com/openspecs/windows_protocols/ms-nrbf/aa509b5a-620a-4592-a5d8-7e9613e0a03e">
///    [MS-NRBF] 2.3.1.2
///   </see>
///  </para>
/// </remarks>
internal sealed class ArraySingleObject : ArrayRecord, IRecord<ArraySingleObject>
{
    public static RecordType RecordType => RecordType.ArraySingleObject;

    public ArraySingleObject(ArrayInfo arrayInfo, IReadOnlyList<object> arrayObjects)
        : base(arrayInfo, arrayObjects)
    { }

    static ArraySingleObject IBinaryFormatParseable<ArraySingleObject>.Parse(
        BinaryReader reader,
        RecordMap recordMap)
    {
        ArraySingleObject record = new(
            ArrayInfo.Parse(reader, out Count length),
            ReadRecords(reader, recordMap, length));

        recordMap[record.ObjectId] = record;
        return record;
    }

    public override void Write(BinaryWriter writer)
    {
        writer.Write((byte)RecordType);
        ArrayInfo.Write(writer);
        WriteRecords(writer, ArrayObjects);
    }
}
