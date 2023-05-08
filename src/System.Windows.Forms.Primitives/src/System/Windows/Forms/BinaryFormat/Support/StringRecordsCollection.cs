// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.BinaryFormat;

/// <summary>
///  Helper to create and track records for <see cref="BinaryObjectString"/> and <see cref="MemberReference"/>
///  when duplicates are found.
/// </summary>
internal class StringRecordsCollection
{
    private readonly Dictionary<string, int> _strings = new();
    private readonly Dictionary<int, MemberReference> _memberReferences = new();

    /// <summary>
    ///  Returns the appropriate record for the given string.
    /// </summary>
    /// <param name="nextId">The id to use if needed, will be incremented if used.</param>
    public IRecord GetStringRecord(string value, ref int nextId)
    {
        if (_strings.TryGetValue(value, out int id))
        {
            // The record with the data has already been retrieved, only a reference is needed now
            if (_memberReferences.TryGetValue(id, out MemberReference? memberReference))
            {
                return memberReference;
            }

            MemberReference reference = new(id);
            _memberReferences.Add(id, reference);
            return reference;
        }

        _strings[value] = nextId;
        IRecord record = new BinaryObjectString(nextId, value);
        nextId++;
        return record;
    }
}
