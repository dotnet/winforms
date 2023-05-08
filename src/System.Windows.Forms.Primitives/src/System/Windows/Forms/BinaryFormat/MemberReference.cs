﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.BinaryFormat;

/// <summary>
///  The <see cref="MemberReference"/> record contains a reference to another record that contains the actual value.
/// </summary>
/// <remarks>
///  <para>
///   <see href="https://learn.microsoft.com/openspecs/windows_protocols/ms-nrbf/eef0aa32-ab03-4b6a-a506-bcdfc10583fd">
///    [MS-NRBF] 2.5.3
///   </see>
///  </para>
/// </remarks>
internal sealed class MemberReference : IRecord<MemberReference>
{
    public Id IdRef { get; }

    public MemberReference(Id idRef) => IdRef = idRef;

    public static RecordType RecordType => RecordType.MemberReference;

    static MemberReference IBinaryFormatParseable<MemberReference>.Parse(
        BinaryReader reader,
        RecordMap recordMap) => new(reader.ReadInt32());

    public void Write(BinaryWriter writer)
    {
        writer.Write((byte)RecordType);
        writer.Write(IdRef);
    }

    // The following implicit conversions are to facilitate lookup of related records
    // using the correct identifier.

    public static implicit operator Id(MemberReference value) => value.IdRef;
}
