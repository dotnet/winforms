// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.BinaryFormat;

/// <summary>
///  Map of records that ensures that IDs are only entered once.
/// </summary>
internal class RecordMap : IReadOnlyRecordMap
{
    private readonly Dictionary<int, IRecord> _records = [];

    public IRecord this[Id id]
    {
        get => _records[id];
        set
        {
            if ((int)id < 0)
            {
                // Negative record Ids should never be referenced. Duplicate negative ids can be
                // exported by the writer. The root object Id can be negative.
                _records.TryAdd(id, value);
                return;
            }

            _records.Add(id, value);
        }
    }
}
