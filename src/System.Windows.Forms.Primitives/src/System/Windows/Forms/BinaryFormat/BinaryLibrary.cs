// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.BinaryFormat;

/// <summary>
///  Library full name information.
/// </summary>
/// <remarks>
///  <para>
///   <see href="https://learn.microsoft.com/openspecs/windows_protocols/ms-nrbf/7fcf30e1-4ad4-4410-8f1a-901a4a1ea832">
///    [MS-NRBF] 2.6.2
///   </see>
///  </para>
/// </remarks>
internal sealed class BinaryLibrary : IRecord<BinaryLibrary>
{
    public Id LibraryId;
    public string LibraryName = string.Empty;

    public static RecordType RecordType => RecordType.BinaryLibrary;

    static BinaryLibrary IBinaryFormatParseable<BinaryLibrary>.Parse(
        BinaryReader reader,
        RecordMap recordMap)
    {
        BinaryLibrary record = new()
        {
            LibraryId = reader.ReadInt32(),
            LibraryName = reader.ReadString()
        };

        recordMap[record.LibraryId] = record;
        return record;
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write((byte)RecordType);
        writer.Write(LibraryId);
        writer.Write(LibraryName);
    }
}
