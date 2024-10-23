// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Private.Windows.Core.BinaryFormat.Serializer;

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
internal sealed partial class ArraySingleString : ArrayRecord<string?>, IRecord<ArraySingleString>
{
    public static RecordType RecordType => RecordType.ArraySingleString;

    private readonly IReadOnlyList<object?> _records;

    internal ArraySingleString(Id objectId, IReadOnlyList<object?> arrayObjects, IReadOnlyRecordMap recordMap)
        : base(new ArrayInfo(objectId, arrayObjects.Count), new StringListAdapter(arrayObjects, recordMap))
    {
        _records = arrayObjects;
    }

    public override BinaryType ElementType => BinaryType.String;

    private protected override void Write(BinaryWriter writer)
    {
        writer.Write((byte)RecordType);
        _arrayInfo.Write(writer);
        WriteRecords(writer, _records, coalesceNulls: true);
    }
}
