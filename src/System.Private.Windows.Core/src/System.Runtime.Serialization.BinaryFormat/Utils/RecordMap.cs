﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO.Hashing;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization.BinaryFormat;

internal sealed class RecordMap
{
    private readonly List<SerializationRecord> _records = []; // TODO: verify whether we actually need that
    private readonly Dictionary<int, SerializationRecord> _map = new(CollisionResistantInt32Comparer.Instance);

    internal void Add(SerializationRecord record)
    {
        _records.Add(record);

        // From https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-nrbf/0a192be0-58a1-41d0-8a54-9c91db0ab7bf:
        // "If the ObjectId is not referenced by any MemberReference in the serialization stream,
        // then the ObjectId SHOULD be positive, but MAY be negative."
        if (record.ObjectId != SerializationRecord.NoId)
        {
            // use Add on purpose, so in case of duplicate Ids we get an exception
            _map.Add(record.ObjectId, record);
        }
    }

    internal SerializationRecord this[int objectId] => _map[objectId];

    // keys (32-bit integer ids) are adversary-provided so we need a collision-resistant comparer
    private sealed class CollisionResistantInt32Comparer : IEqualityComparer<int>
    {
        internal static CollisionResistantInt32Comparer Instance { get; } = new();

        private CollisionResistantInt32Comparer() { }

        public bool Equals(int x, int y) => x == y;

        public int GetHashCode(int obj)
        {
#if NETCOREAPP
            Span<int> integers = new(ref obj);
#else
            Span<int> integers = stackalloc int[1] { obj };
#endif
            return (int)XxHash32.HashToUInt32(MemoryMarshal.AsBytes(integers));
        }
    }
}
