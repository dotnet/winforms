// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.BinaryFormat;

/// <summary>
///  Single dimensional array of strings.
/// </summary>
/// <remarks>
///  <para>
///   <see href="https://learn.microsoft.com/openspecs/windows_protocols/ms-nrbf/3d98fd60-d2b4-448a-ac0b-3cd8dea41f9d">
///    [MS-NRBF] 2.4.3.4
///   </see>
///  </para>
/// </remarks>
internal sealed class ArraySingleString : ArrayRecord<object?>, IRecord<ArraySingleString>, IBinaryFormatParseable<ArraySingleString>
{
    public static RecordType RecordType => RecordType.ArraySingleString;

    public ArraySingleString(Id objectId, IReadOnlyList<object?> arrayObjects)
        : base(new ArrayInfo(objectId, arrayObjects.Count), arrayObjects)
    { }

    static ArraySingleString IBinaryFormatParseable<ArraySingleString>.Parse(
        BinaryFormattedObject.IParseState state)
    {
        ArraySingleString record = new(
            ArrayInfo.Parse(state.Reader, out Count length),
            ReadObjectArrayValues(state, length));

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
