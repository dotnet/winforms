// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Private.Windows.Core.BinaryFormat.Serializer;

internal sealed partial class ArraySingleString
{
    private sealed class StringListAdapter : IReadOnlyList<string?>
    {
        private readonly IReadOnlyList<object?> _recordList;
        private readonly IReadOnlyRecordMap _recordMap;

        internal StringListAdapter(IReadOnlyList<object?> recordList, IReadOnlyRecordMap recordMap)
        {
            _recordList = recordList;
            _recordMap = recordMap;
        }

        public string? this[int index] => _recordList[index] switch
        {
            null => null,
            IRecord record => _recordMap.Dereference(record) is BinaryObjectString stringRecord
                ? stringRecord.Value
                : throw new InvalidOperationException(),
            _ => throw new InvalidOperationException()
        };

        int IReadOnlyCollection<string?>.Count => _recordList.Count;

        public IEnumerator<string?> GetEnumerator()
        {
            return GetEnumerable().GetEnumerator();

            IEnumerable<string?> GetEnumerable()
            {
                for (int i = 0; i < _recordList.Count; i++)
                {
                    yield return this[i];
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
