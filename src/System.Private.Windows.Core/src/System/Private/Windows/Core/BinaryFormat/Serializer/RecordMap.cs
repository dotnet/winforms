// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Private.Windows.Core.BinaryFormat.Serializer;

/// <summary>
///  Map of records that ensures that IDs are only entered once.
/// </summary>
internal class RecordMap : IReadOnlyRecordMap
{
    private readonly Dictionary<int, IRecord> _recordMap = [];

    public IRecord this[Id id] => _recordMap[id];

    public void AddRecord(IRecord record)
    {
        Id id = record.Id;
        if (id.IsNull)
        {
            return;
        }

        if ((int)id < 0)
        {
            // Negative record Ids should never be referenced. Duplicate negative ids can be
            // exported by the writer. The root object Id can be negative.
            _recordMap.TryAdd(id, record);
            return;
        }

        _recordMap.Add(id, record);
    }
}
